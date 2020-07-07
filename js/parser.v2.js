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
let url = new URL(window.location);
let pktSource =
    "https://raw.githubusercontent.com/blackphreak/Feather_Server/develop/Feather_Server/Packets/_pkts.v2.json";
if (window.location.hostname == "127.0.0.1") pktSource = "./_pkts.v2.ignore.json";
else if (url.searchParams.has("file")) pktSource = url.searchParams.get("file");

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
            Object.keys(_pktList[keyName].signature)
                .sort(sortFnLongerFirst)
                .forEach((sign) => {
                    pktList[lowerKeyName].signature[sign.replace(/  /g, " ")] = _pktList[keyName].signature[sign];
                });
        });

        log("Packet List Load Succeed.", LOG);
        $("#btn_do_parse").attr("disabled", false);
        $("#btn_do_parse").on("click", doParse);
    })
    .fail((_) => {
        $(`#pkt_ver`).html(
            "Failed to load packet list.<br>Please check for the existance of _pkts.v2.json on the github develop branch."
        );
        if (false) alert("Failed to load packet list.");
        log("Failed to load packet list.", ERR);
    });

var doParse = (_) => {
    $('[role="tooltip"]').tooltip("dispose");

    // parse them one by one
    $(`[role=item]`).map((_, ele) => singleParser($(ele)));

    $(`[role=item][data-original-title]`).on("click", (e) => {
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

$("#btn_tocb").on("click", (_) => {
    var txt = "";
    $io.find(`> [role=item]:not(.bg-err):not(.bg-ignored):not(.bg-warn) > [role=editable]`).map((_, e) => {
        if (e) txt += e.textContent + "\n";
    });
    navigator.clipboard
        .writeText(txt)
        .then((_) => alert("Copied to your clipboard!"))
        .catch((e) => log(e.message, ERR));
});

var $io = $(`#io`);
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

document.getElementById("io").addEventListener("paste", function(evt) {
    evt.stopPropagation();
    evt.preventDefault();

    // unbind all events
    $(`[role=item]`).unbind();

    let data = (evt.clipboardData || window.clipboardData).getData("Text");

    data.split("\n").forEach((ele) => {
        if (!ele || ele == "\n" || ele.charCodeAt(0) == 0x0d) return; // ignore empty line

        $io.addItem(ele);
    });

    // add one empty line after all
    $io.addItem();
});

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
