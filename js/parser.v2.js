//#region Databases
var formatstringDB, itemDB, npcDB, mapDB, skillDB;
$.getJSON("./js/db/formatstring.json").done((json) => {
    formatstringDB = json;
});
$.getJSON("./js/db/Item.json").done((json) => {
    itemDB = json;
});
$.getJSON("./js/db/NPC.json").done((json) => {
    npcDB = json;
});
$.getJSON("./js/db/Map.json").done((json) => {
    mapDB = json;
});
$.getJSON("./js/db/Skill.json").done((json) => {
    skillDB = json;
});
//#endregion

// init packets database
const parserVersion = 5;
const targetFile = "_pkts.v5.ignore.json";
let url = new URL(window.location);
let pktSource = `https://raw.githubusercontent.com/blackphreak/Feather_Server/gh-pages/_pkts.v5.json`;
if (window.location.hostname == "127.0.0.1" || window.location.hostname.indexOf("feather") > -1) pktSource = `./`;
if (url.searchParams.has("file")) pktSource = url.searchParams.get("file");

if (pktSource.indexOf(".json") == -1)
    pktSource += targetFile;

var pktList = {};
$.getJSON(pktSource)
    .done((_pktList) => {
        $(`#pkt_ver`).html("Packet Source Version: " + new Date(_pktList._timestamp).toString());

        var _ver = parseInt(_pktList._builder[2]) - parserVersion;
        if (_ver > 0)
            $(`#pkt_ver`).append(` | Parser is outdated. Curr[${parserVersion}] New[${_pktList._builder[2]}]`);
        else if (_ver < 0)
            $(`#pkt_ver`).append(` | Builder is outdated. Parser[${parserVersion}] Builder[${_pktList._builder[2]}]`);
        else $(`#pkt_ver`).append(` | Up-to-date :)`);

        delete _pktList._timestamp;
        delete _pktList._builder;

        // re-ordering the packet header before actually use
        let keyLst = Object.keys(_pktList).sort(sortFn);
        keyLst.forEach((keyName) => {
            let lowerKeyName = keyName.toLowerCase();
            pktList[lowerKeyName] = { ..._pktList[keyName] }; // duplicate _pktList
            pktList[lowerKeyName].signature = {};

            // re-ordering the signature before actually use
            // pktList[lowerKeyName].signature[sign.replace(/  /g, " ")] = _pktList[keyName].signature[sign];
            let signs = Object.entries(_pktList[keyName].signature);
            Object.keys(
                Object.fromEntries(
                    signs.sort(sortSignature)
                )
            ).map((k) => {
                pktList[lowerKeyName].signature[k.replace(/  /g, " ")] = _pktList[keyName].signature[k];
            });
        });

        log("Packet List Load Succeed.", LOG);
        $("#btn_do_parse").attr("disabled", false);
        $("#btn_do_parse").on("click", doParse);
    })
    .fail((_) => {
        $(`#pkt_ver`).html(
            `Failed to load packet list.<br>Please check for the existance of ${targetFile} on github.`
        );
        if (false) alert("Failed to load packet list.");
        log("Failed to load packet list.", ERR);
    });

var doParse = (_) => {
    $('[role="tooltip"]').tooltip("dispose");

    // parse them one by one
    $(`[role=item]`).map((_, ele) => singleParser($(ele)));

    // global knownID highlight (do highlight for each match)
    $(`[role=editable]`).map((_, e) => {
        let $e = $(e);
        let pkt = $e.text(); // spaced pkt bytes
        $.each(window._knownID, (k, v) => {
            if (v.n <= 1)
            {
                return true; // (continue) skip color for ID that occur once only
            }
            if (pkt.indexOf(k) > -1)
            {
                let [fg, bg] = window._highlightColor[v.i];
                pkt = pkt.replaceAll(k, ` <span style="color:${fg};background:${bg};">${k}</span> `);
            }
        });
        $e.html(pkt.replace(/\s{2,}/gi, ' '));
    })

    $(`[role=item][data-original-title]`).on("click", (e) => {
        $(e.target)
            .toggleClass("active");
    });

    $(`[role=item][data-original-title] *`).on("click", (e) => {
        e.stopPropagation();
        $(e.target)
            .parents(`[role=item]`)
            .toggleClass("active");
    });
};

$("#tooltip_killer").on("click", (_) => {
    $('[role="tooltip"]').tooltip("dispose");
});

$("#btn_reset").on("click", (_) => {
    document.getElementById("io").innerHTML = ``;
    $(`[role='tooltip']`).tooltip("dispose");
});

$("#btn_tocb").on("click", (e) => {
    var txt = "";
    $io.find(`> [role=item]${e.shiftKey ? "" : ":not(.bg-err):not(.bg-ignored):not(.bg-warn)"} > [role=editable]`).map((_, e) => {
        if (e) txt += e.textContent + "\n";
    });
    navigator.clipboard
        .writeText(txt)
        .then((_) => alert("Copied to your clipboard!"))
        .catch((e) => log(e.message, ERR));
});

var $io = $(`#io`);
window.$io = $io;
window.itemid = 0;

const template =
    `<div role="item" data-id="{{itemID}}">` +
    `<div role="header"></div>` +
    `<div role="editable" contenteditable="plaintext-only" spellcheck="false">{{content}}</div>` +
    `<div role="control"></div>` +
    `</div>`;
$(function() {
    $io.addItem = function(content, focus = false, appendAfterThisEle = undefined) {
        let html = template
            .replace("{{itemID}}", window.itemid++)
            .replace("{{content}}", (content && content.replace(/\n/g, "")) || "");
        if (!appendAfterThisEle) $io.append(html);
        else $(html).insertAfter($(appendAfterThisEle));

        if (focus) $(`[data-id="${window.itemid - 1}"] > [role="editable"]`).focus();
    };
});

var pasteHandler = function (evt) {
    evt.stopPropagation();

    if (document.activeElement.getAttribute(`role`) == "editable") {
        // dont paste it as new item, but in activeElement only.
        //let data = (evt.clipboardData || window.clipboardData).getData("Text");
        return false;
    }
    evt.preventDefault();

    // unbind all events
    $(`[role=item]`).unbind();

    let data = (evt.clipboardData || window.clipboardData).getData("Text");
    
    let isPasteOnLastItem = evt.srcElement == (this.lastElementChild && this.lastElementChild.children && this.lastElementChild.children[1] || undefined);

    data.split("\n").forEach((ele) => {
        if (!ele || ele == "\n" || ele.charCodeAt(0) == 0x0d) return; // ignore empty line

        if (isPasteOnLastItem) {
            evt.srcElement.textContent = ele;
            isPasteOnLastItem = false;
        }
        else
            $io.addItem(ele, false, document.activeElement.tagName == "BODY" ? undefined : document.activeElement.parentElement);
    });

    // add one empty line after all if there is not empty line
    if (this.lastElementChild.children[1].textContent)
        $io.addItem();
};

document.body.addEventListener("paste", pasteHandler)
document.getElementById("io").addEventListener("paste", pasteHandler);

document.getElementById("io").addEventListener("keydown", function(evt) {
    if (evt.keyCode == 13) {
        // enter
        evt.preventDefault();

        if (
            document.activeElement.parentElement.attributes.getNamedItem("data-id").value ==
            $io
                .children()
                .last()
                .attr("data-id")
        ) {
            // last item
            let txt = document.activeElement.textContent;
            if (txt == "" || txt == "\n")
                // if empty line, add in previous
                $io.addItem("", true, document.activeElement.parentElement.previousSibling);
            else $io.addItem("", true);
        } else $io.addItem("", true, document.activeElement.parentElement);
    } else if (evt.keyCode == 8) {
        // backspace
        if (evt.shiftKey) {
            // remove previous
            if ($io.children().length <= 2) return;
            if (document.activeElement.parentElement.previousSibling)
                document.activeElement.parentElement.previousSibling.remove();
            document.activeElement.focus();
        } else {
            // remove self
            if ($io.children().length <= 2) return;
            if (
                document.activeElement.parentElement.attributes.getNamedItem("data-id").value ==
                $io
                    .children()
                    .last()
                    .attr("data-id")
            )
                return;
            if (document.activeElement.textContent.length <= 0) {
                let ele = $(document.activeElement.parentElement.previousSibling).find(`[role=editable]`);
                document.activeElement.parentElement.remove();
                ele.focus();
            }
        }
    } else if (evt.keyCode == 46 && evt.shiftKey) {
        // shift + delete
        // remove self
        if ($io.children().length <= 2) return;

        // if is last item
        if (
            document.activeElement.parentElement.attributes.getNamedItem("data-id").value ==
            $io
                .children()
                .last()
                .attr("data-id")
        )
            // remove previous
            document.activeElement.parentElement.previousSibling.remove();

        document.activeElement.parentElement.remove();
    } else if (evt.keyCode == 38) {
        // arrow up
        if (
            document.activeElement.parentElement.previousSibling &&
            document.activeElement.parentElement.previousSibling.childNodes[1]
        )
            document.activeElement.parentElement.previousSibling.childNodes[1].focus();
    } else if (evt.keyCode == 40) {
        // arrow down
        if (
            document.activeElement.parentElement.nextSibling &&
            document.activeElement.parentElement.nextSibling.childNodes[1]
        )
            document.activeElement.parentElement.nextSibling.childNodes[1].focus();
    }
    // else if (evt.keyCode == 89 && evt.ctrlKey)
    // {
    //     // ctrl + y
    //     console.log("^y", evt);
    // }
    // else if (evt.keyCode == 90 && evt.ctrlKey)
    // {
    //     // ctrl + z
    //     console.log("^z", evt);
    // }
});

function setCaret(target, row = -1, pos = -1) {
    var el = target;
    var range = document.createRange();
    var sel = window.getSelection();

    if (row === undefined || row == -1) row = document.activeElement.lastChild;
    else row = el.childNodes[row];

    if (pos == -1) pos = row.length;

    range.setStart(row, pos);
    range.collapse(pos != -1);
    sel.removeAllRanges();
    sel.addRange(range);
    el.focus();
}

//#region High Light Colors
window._highlightColor = [
    ["#000", "#C2D954"],
    ["#000", "#AC7D95"],
    ["#000", "#63FE5E"],
    ["#000", "#15BEAD"],
    ["#000", "#C89999"],
    ["#000", "#7F8DAC"],
    ["#fff", "#338040"],
    ["#fff", "#3D20E1"],
    ["#000", "#FE9D32"],
    ["#fff", "#D31288"],
    ["#fff", "#358FA4"],
    ["#fff", "#985974"],
    ["#fff", "#1361CD"],
    ["#fff", "#080E78"],
    ["#000", "#D2C151"],
    ["#000", "#FB4FBF"],
    ["#fff", "#BE3D0B"],
    ["#fff", "#4D19E4"],
    ["#000", "#FBE73F"],
    ["#fff", "#E75F0F"],
    ["#000", "#D96075"],
    ["#000", "#AAEDAC"],
    ["#fff", "#D02290"],
    ["#fff", "#076F9A"],
    ["#000", "#C6C7FB"],
    ["#000", "#C29809"],
    ["#000", "#30F7B1"],
    ["#000", "#6DB923"],
    ["#fff", "#FC0769"],
    ["#fff", "#648534"],
    ["#000", "#E39BF8"],
    ["#000", "#56A642"],
    ["#000", "#2CEB9A"],
    ["#fff", "#817F48"],
    ["#000", "#968BD2"],
    ["#000", "#E5F19E"],
    ["#fff", "#EA0DBE"],
    ["#000", "#33E56A"],
    ["#000", "#D9AB25"],
    ["#fff", "#7961B9"],
    ["#000", "#B05FD5"],
    ["#000", "#AFB151"],
    ["#fff", "#9B307F"],
    ["#000", "#A4C82B"],
    ["#fff", "#5142EB"],
    ["#000", "#54C365"],
    ["#000", "#31FA56"],
    ["#000", "#6FEB19"],
    ["#fff", "#72000A"],
    ["#fff", "#9C038F"],
    ["#fff", "#956D28"],
    ["#fff", "#87900D"],
    ["#fff", "#9736EB"],
    ["#fff", "#2509E1"],
    ["#000", "#E7A75D"],
    ["#fff", "#493445"],
    ["#fff", "#8B2378"],
    ["#000", "#ADB044"],
    ["#000", "#29A5E7"],
    ["#000", "#CE95DA"],
    ["#000", "#87EDAE"],
    ["#000", "#79D4C3"],
    ["#fff", "#085B75"],
    ["#000", "#D6B280"],
    ["#fff", "#452A2A"],
    ["#000", "#E9876A"],
    ["#fff", "#38738C"],
    ["#fff", "#323D36"],
    ["#fff", "#197C00"],
    ["#000", "#56E376"],
    ["#000", "#A19265"],
    ["#000", "#B29502"],
    ["#fff", "#638C5C"],
    ["#000", "#5ED812"],
    ["#000", "#B9BE04"],
    ["#fff", "#754A41"],
    ["#000", "#8CFE51"],
    ["#000", "#94E72F"],
    ["#fff", "#9A2C02"],
    ["#000", "#38F1DC"],
    ["#000", "#AA7E40"],
    ["#000", "#68A723"],
    ["#fff", "#8D1133"],
    ["#fff", "#2D86AC"],
    ["#fff", "#A14B67"],
    ["#000", "#E9FF6F"],
    ["#fff", "#617021"],
    ["#000", "#0EE65B"],
    ["#fff", "#0E20F2"],
    ["#fff", "#A222C1"],
    ["#000", "#DA7887"],
    ["#000", "#FF8BBB"],
    ["#fff", "#F513AB"],
    ["#000", "#08EFA3"],
    ["#000", "#C0F13D"],
    ["#fff", "#0F7398"],
    ["#000", "#6AAD54"],
    ["#fff", "#9851CF"],
    ["#000", "#E455D8"],
    ["#000", "#D8C28B"],
];
//#endregion