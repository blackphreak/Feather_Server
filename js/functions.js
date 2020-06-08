// region: Enums
var eQualityColor = (q) => {
    if (q == 1) return "#00FFFF";
    if (q == 2) return "#FFFF00";
    if (q == 3) return "#F775FF";
    if (q == 4) return "#00FF00";
    if (q == 5) return "#";
    if (q == 6) return "#";
    if (q == 7) return "#";
    if (q == 8) return "#";
    if (q == 9) return "#";
};

var eEquipPos = (i) => {
    // TODO: YBshop Equip Pos
    // WTF? YBshop different from equipPos?!?!
    // HELMET = 101,     // 0x65
    // NECKLACE = 102,   // 0x66
    // WEAPON = 103,     // 0x67
    // CHESTPLATE = 104, // 0x68
    // BODY = 105,       // 0x69 - style
    // BOOTS = 106,      // 0x6A
    // ----------------------------------------
    // LRING = 107,      // 0x6B
    // RRING = 108,      // 0x6C
    // WINGS = 109,      // 0x6D
    // HAT = 110,        // 0x6E - style
    // MASK = 111,       // 0x6F - style
    // TAIL = 112,       // 0x70 - style
    if (i == 0x69) return "Head";
    if (i == 0x6a) return "";
    if (i == 0x6b) return "Body";
    if (i == 0x6c) return "";
    if (i == 0x6d) return "";
    if (i == 0x6e) return "Tail";
    if (i == 0x6f) return "?";
};

var ePlayerAct = (actCodeInPureHex) => {
    if (actCodeInPureHex <= "01") return "Normal (Stand)";
    if (actCodeInPureHex == "02") return "Walking";
    if (actCodeInPureHex == "03") return "";
    if (actCodeInPureHex == "04") return "Sit";
    if (actCodeInPureHex == "06") return "Attack (Skill)";
    if (actCodeInPureHex == "09") return "Self-Buff (Skill)";
    if (actCodeInPureHex == "0B") return "Fight";
    // TODO: find, confirm & fill all
};

var eChatChannel = (hex) => {
    //return "Marquee (Top)";
    if (hex == "02") return "Rumor";
    if (hex == "04") return "Tribe";
    if (hex == "07") return "Team";
    if (hex == "11") return "Annonce (Top)";
    if (hex == "0a") return "System";
};

var eMap = (mapCode) => {
    let retn = l2b_hex(mapCode);
    retn = `0x${retn}[${parseInt(retn, 16)}] (${mapDB[parseInt(retn, 16)].name || "-- Unk --"})`;
    return retn + ")";
};

var eStuffbag = (inp) => {
    var out = [];
    (parseInt(l2b_hex(inp), 16) + "")
        .padStart(8, "0")
        .match(/.{2}/g)
        .reverse() // turning RightMost to the last
        .forEach((bag) => {
            if (bag == "01") out.push("❌ 06");
            else if (bag == "02") out.push("❌ 12");
            else if (bag == "03") out.push("❌ 24");
            else if (bag == "06") out.push("✅ 06");
            else if (bag == "12") out.push("✅ 12");
            else if (bag == "24") out.push("✅ 24");
            else if (bag == "25") out.push("♾️ 24");
            else out.push("🆕 --");
        });

    return `[${out.join(" | ")}] (Default ---> Right Most)`;
};

var eAnimationDuration = (inp) => parseInt(inp, 16) + " seconds";
var eAnimationLayer = (inp) => (inp <= "01" ? "Front" : "Back");
var eAnimationLoop = (inp) => (inp <= "01" ? "Once" : "Repeated");

var db_FormatString = (inp) => (formatstringDB[inp] || "-- FSDB Fail --");
var db_Item = (inp) => (itemDB[inp] || "-- ItemDB Fail --");
var db_NPC = (inp) => (npcDB[inp] || "-- NPCDB Fail --");
var db_Map = (inp) => (mapDB[inp] || "-- MapDB Fail --");
var db_Skill = (inp) => (skillDB[inp] || "-- SkillDB Fail --");

//#endregion Enums

//#region Common Functions
//#region Extension Functions
String.prototype.cPadStart = function(sz = 20) {
    return this.padStart(sz).replace(/\s/g, " ");
};

String.prototype.cPadEnd = function(sz = 14) {
    return this.padEnd(sz).replace(/\s/g, " ");
};
//#endregion

//#region Log Related
var LOG = 1 << 0,
    WARN = 1 << 1,
    ERR = 1 << 2,
    DEBUG = 1 << 3
;

var logMode = LOG | WARN | ERR | DEBUG;

var hasFlag = (myLv, targetLv) => (myLv & targetLv) == targetLv;
var _logSign = (logLevel) => {
    if (hasFlag(logLevel, DEBUG)) return "*";
    if (hasFlag(logLevel, LOG)) return "+";
    if (hasFlag(logLevel, WARN)) return "!";
    if (hasFlag(logLevel, ERR)) return "-";
};

var log = (messages, logLevel) => {
    if (hasFlag(logMode, DEBUG) && !hasFlag(logLevel, DEBUG)) return;

    let fn,
        logSign = _logSign(logLevel),
        whole = [];
    if (hasFlag(logLevel, DEBUG)) fn = console.trace;
    else if (hasFlag(logLevel, WARN)) fn = console.warn;
    else if (hasFlag(logLevel, ERR)) fn = console.error;
    else fn = console.log;

    if (typeof messages == "object") messages.forEach(msg => whole.push(`[${logSign}] ${msg}`));
    else whole.push(`[${logSign}] ${messages}`);

    fn(...whole);
};
//#endregion

//#region Misc Functions
var lib = {
    parseGBK: (hex) => GBK.decode(lib.hex2bytes(hex)),

    hex2bytes: (str) => {
        // src: https://gist.github.com/tauzen/3d18825ae41ff3fc8981
        if (!str) return new Uint8Array();

        var a = [];
        for (var i = 0, len = str.length; i < len; i += 2) a.push(parseInt(str.substr(i, 2), 16));

        return new Uint8Array(a);
    },

    le2be: (endian) => {
        // Convert Little-Endianess to Big-Endianess
        // src: https://stackoverflow.com/a/44288059
        if (endian == undefined) {
            log("Empty input.", ERR | DEBUG);
            return "00";
        }
        try {
            return endian
                .match(/[a-fA-F0-9]{2}/g)
                .reverse()
                .join("");
        } catch (Exception) {
            log(["Error while parse input to big-end.", `Input: ${endian}`], ERR | DEBUG);
            return "00";
        }
    },

    _extractTextWithWhitespaceWorker: function(elems, lineBreakNodeName) {
        // src: https://stackoverflow.com/a/4140071
        var ret = "";

        for (var i = 0; elems[i]; i++)
        {
            elem = elems[i];
            if (
                elem.nodeType === 3 || // text node
                elem.nodeType === 4
            )
                // CDATA node
                ret += elem.nodeValue;

            if (elem.nodeName === lineBreakNodeName) ret += "\n";

            if (elem.nodeType !== 8)
                // comment node
                ret += lib._extractTextWithWhitespaceWorker(elem.childNodes, lineBreakNodeName);
        };

        return ret;
    }
};
//#endregion
//#endregion

// sorting function
var sortFn = (a, b) => (a.length > b.length ? +1 : a.length < b.length ? -1 : a - b);
var sortFnReverse = (a, b) => (a.length > b.length ? -1 : a.length < b.length ? +1 : b - a);

//#region Trace Function
window.backTrace = []
var initBT = (_name, _data) => {
    window.backTrace.push({
        fn: _name, data: _data
    })
}
var rmBT = window.backTrace.pop();
//#endregion

//#region Parser

var singleParser = (bytes) => {
    if (bytes == "")
        return "";
    
    // identify packet format

}
//#endregion