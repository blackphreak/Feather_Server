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
$.getJSON("https://raw.githubusercontent.com/blackphreak/Feather_Server/develop/Feather_Server/Packets/_pkts.v2.json").done(_pktList => {
    $(`#pkt_ver`).html("Packet Source Version: " + new Date(_pktList.timestamp).toString())
    // re-ordering the packet header before actually use
    let keyLst = Object.keys(_pktList).sort(sortFn);
    keyLst.forEach(keyName => {
        pktList[keyName] = {..._pktList[keyName]}; // duplicate _pktList
        pktList[keyName].signature = {};

        // re-ordering the signature before actually use
        Object.keys(_pktList[keyName].signature).sort(sortFnReverse).forEach(sign => pktList[keyName].signature[sign] = _pktList[keyName].signature[sign]);
    });

    log("Packet List Load Succeed.", LOG);
    $("#btn_do_parse").attr("disabled", false);
    $("#btn_do_parse").on("click", doParse);
}).fail(_ => {
    alert("Failed to load packet list.\nPlease disable cache & try to reload.");
    log("Failed to load packet list.", ERR);
});

var doParse = _ => {
    $('[role="tooltip"]').tooltip("dispose");

    var out = "";
    lib._extractTextWithWhitespaceWorker($io, "div")
        .split("\n")
        .map((pkt) => {
            let retn = singleParser(pkt);
            if (retn) out += retn + "<br>";
        });

    document.getElementById("io").innerHTML = out;

    // rebind tooltip
    $('[data-toggle="tooltip"]').tooltip({
        html: true,
        animation: true,
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
