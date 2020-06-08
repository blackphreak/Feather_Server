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
var pktList = {};
let pktSource = (window.location.hostname == "127.0.0.1") ? "/js/_pkts.v2.ignore.json" : "https://raw.githubusercontent.com/blackphreak/Feather_Server/develop/Feather_Server/Packets/_pkts.v2.json";
$.getJSON(pktSource)
    .done((_pktList) => {
        $(`#pkt_ver`).html("Packet Source Version: " + new Date(_pktList._timestamp).toString());
        
        var _ver = parseInt(_pktList._builder[2]) - parserVersion;
        if (_ver > 0)
            $(`#pkt_ver`).append(` | Parser is outdated. Curr[${parserVersion}] New[${_pktList._builder[2]}]`);
        else if (_ver < 0)
            $(`#pkt_ver`).append(` | Builder is outdated. Parser[${parserVersion}] Builder[${_pktList._builder[2]}]`);
        else
            $(`#pkt_ver`).append(` | Up-to-date :)`);
        
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
                .forEach((sign) => (pktList[lowerKeyName].signature[sign] = _pktList[keyName].signature[sign]));
        });

        log("Packet List Load Succeed.", LOG);
        $("#btn_do_parse").attr("disabled", false);
        $("#btn_do_parse").on("click", doParse);
    })
    .fail((_) => {
        $(`#pkt_ver`).html("Failed to load packet list.<br>Please check for the existance of _pkts.v2.json on the github develop branch.");
        if (false) alert("Failed to load packet list.");
        log("Failed to load packet list.", ERR);
    });

var doParse = (_) => {
    $('[role="tooltip"]').tooltip("dispose");

    lib._extractTextWithWhitespaceWorker($io, "div")
        .split("\n")
        .map((pkt) => singleParser(pkt));

    // rebind tooltip
    $('[data-toggle="tooltip"]').tooltip({
        html: true,
        animation: false,
        placement: "right",
        trigger: "click hover",
        boundary: "window"
    });
    $('[data-toggle="tooltip"]').on("click", (e) => {
        $(e.target).toggleClass("active");
    });
};

$("#tooltip_killer").on("click", (_) => {
    $('[role="tooltip"]').tooltip("dispose");
});

$("#btn_reset").on("click", (_) => {
    document.getElementById("io").innerHTML = ``;
    $(`[role='tooltip']`).tooltip("dispose");
});
