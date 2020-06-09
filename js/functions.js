//#region Enum
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

var ePlayerRole = (hex) => {
    if (hex == 0x11fe) return "Warrior";
    if (hex == 0x11fd) return "Swordman";
    if (hex == 0x1200) return "Mage";
    if (hex == 0x11ff) return "Taoist";
    if (hex == 0x1201) return "Priest";
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

var db_FormatString = (inp, cols = ["title"]) => {
    let db = formatstringDB[inp] || false;
    if (!db)
        return "-- FSDB Fail --";
    
    let data = [];
    cols.forEach(col => {
        data.push(db[col] || "ERR-COL");
    })
    return data;
}
var db_Item = (inp, cols = ["name"]) => {
    let db = itemDB[inp] || false;
    if (!db)
        return "-- ItemDB Fail --";
    
    let data = [];
    cols.forEach(col => {
        data.push(db[col] || "ERR-COL");
    })
    return data;
}
var db_NPC = (inp, cols = ["name", "name2"]) => {
    let db = npcDB[inp] || false;
    if (!db)
        return "-- NPCDB Fail --";
    
    let data = [];
    cols.forEach(col => {
        data.push(db[col] || "ERR-COL");
    })
    return data;
}
var db_Map = (inp, cols = ["name"]) => {
    let db = mapDB[inp] || false;
    if (!db)
        return "-- MapDB Fail --";
    
    let data = [];
    cols.forEach(col => {
        data.push(db[col] || "ERR-COL");
    })
    return data;
}

var db_Skill = (inp, cols = ["name"]) => {
    let db = skillDB[inp] || false;
    if (!db)
        return "-- SkillDB Fail --";
    
    let data = [];
    cols.forEach(col => {
        data.push(db[col] || "ERR-COL");
    })
    return data;
}

//#endregion Enums

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
    DEBUG = 1 << 3;
var logMode = LOG | WARN | ERR | DEBUG;
var terminateFlag = false;

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
    else if (hasFlag(logLevel, ERR)) {
        fn = console.error;
        terminateFlag = true;
    } else fn = console.log;

    if (typeof messages == "object") messages.forEach((msg) => whole.push(`[${logSign}] ${msg}`));
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

    /**
     * Convert Little-Endianess to Big-Endianess
     */
    le2be: (endian) => {
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

    /**
     * Parse Big-Endian hex value to padded/signed number & hex
     * @param {string} val Big-Endian Hex
     * @param {string/number} type Type of convertion. If number, then unit is (n * 2) byte
     * @returns {array} [un-/signed number, padded number, padded hex]
     */
    parseToNum: function(val, type = "uint") {
        val = parseInt(val, 16);
        let hex = val.toString(16);

        if (type == "byte" || type == -2)
        {
            if (val & 0x80)
            {
                val <<= 1;
                val >>= 1;
                val *= -1;
            }
            return [val, val.toString(), hex.padStart(2, "0").toUpperCase()];
        }
        if (type == "ubyte" || type == 2)
        {
            return [val, val.toString().padStart(2, "0"), hex.padStart(2, "0").toUpperCase()];
        }
        if (type == "short" || type == -4)
        {
            if (val & 0x8000)
            {
                val <<= 1;
                val >>= 1;
                val *= -1;
            }
            return [val, val.toString(), hex.padStart(4, "0").toUpperCase()];
        }
        if (type == "ushort" || type == 4)
        {
            return [val, val.toString().padStart(4, "0"), hex.padStart(4, "0").toUpperCase()];
        }
        if (type == "int" || type == -8)
        {
            if (val & 0x80000000)
            {
                val <<= 1;
                val >>= 1;
                val *= -1;
            }
            return [val, val.toString(), hex.padStart(8, "0").toUpperCase()];
        }
        if (type == "uint" || type == 8)
        {
            return [val, val.toString(), hex.padStart(8, "0").toUpperCase()];
        }
        log(["Failed to parseToNum", `Input: ${val}`], ERR);
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

//#region sorting function
var sortFn = (a, b) => (a.length > b.length ? +1 : a.length < b.length ? -1 : a - b);
var sortFnLongerFirst = (a, b) => (a.length > b.length ? -1 : a.length < b.length ? +1 : b - a);
//#endregion

//#region Parser
/**
 * Warning Flag: expected more bytes, but already reached the end.
 */
const WFLAG_MORE_EXPECTED = 1 << 0;
var $io = $(`#io`);

var buildOutput = (info) => {
    let title = info.title || "default title",
        packet = info.packet || "default packet";

    if (info.log) log([title, `Packet: ${packet}`], info.log);

    let tooltipInfo = "";
    if (info.tooltip) {
        // parsed packet
        /* example of info.tooltip:
        [
            { "LHS": ["packet bytes", "" ] },
            ...
        ]
        */
        info.tooltip.forEach((ele) => {
            if (typeof ele == "number") {
                // is flag
                let flag = parseInt(ele);
                if (hasFlag(flag, WFLAG_MORE_EXPECTED))
                    tooltipInfo += `<tr class="fg-err"><td colspan=100></td></tr>`;
            } else {
                $.each(ele, (desc, actual) => {
                    tooltipInfo += `<tr><td>${desc}</td>`;
                    let last = actual.pop();
                    actual.forEach(e => {
                        tooltipInfo += `<td>${e}</td>`;
                    });
                    tooltipInfo += `<td colspan=100>${last}</td>`;
                    tooltipInfo += "</tr>";
                });
            }
        });
        tooltipInfo = `<table><tbody>` + tooltipInfo + `</tbody></table><br/><br/>`;
        (info.extraTooltip || []).forEach((ele) => {
            tooltipInfo += `<div>${ele}</div>`;
        });
        tooltipInfo = tooltipInfo.replace(/\"/gm, "&quot;"); // replace double-qoute (") to HTML charCode
        tooltipInfo = ` data-toggle="tooltip" data-original-title="${tooltipInfo}"`;
    }

    $io.append(`<div class="${info.class || ""}"${tooltipInfo}>// ${title}<br />${packet}</div>`);
};

var matchHeader = (pkt) => {
    let typeA = pkt.substr(0, 4) + "",
        typeB = pkt.substr(0, 2) + "";

    if (pktList.hasOwnProperty(typeA)) return typeA;
    if (pktList.hasOwnProperty(typeB)) return typeB;

    return false;
};

/**
 * To validate the size of the packet,
 *   only applicable for packet starting with size and end with 00.
 * @param {string} pkt packet that without underscore(_), spaces, and time header
 */
var sizeCheck = (pkt) => {
    let sz = parseInt(pkt.substr(0, 2), 16),
        body = pkt.slice(2).length; // exclude size

    // skip size check for 0xFE and 0xFF packet
    if (sz >= 0xfe) return true;

    return sz * 2 == body;
};

var _info;
var isMatchSign = (signature, signInfo, original_pkt) => {
    // clone pkt, this variable will be used to store the unprocessed bytes.
    var pkt = original_pkt;
    var allParamInfo = signInfo["params"];
    var deferfunc = [], // = [ [<func 1 name>, <selfValue>, <Param1 / @Name>] ]
        funcParam = {};
    var repeatPattern = false; // [<start param>, <index of REPE>]
    
    let arr_signature = signature.split(" ");
    for (let i = 0; i < arr_signature.length; i++) {
        const param = arr_signature[i];
        
        if (pkt.length == 0 && param != "[REPE]" && !repeatPattern)
        {
            _info.tooltip.push(WFLAG_MORE_EXPECTED);
            return false; // expacted more but end of packet
        }
        
        var LHS, byte = "", endIndex = 0, paramInfo = [];
        if (param == "[REPS]") {
            repeatPattern = [i, -1];
            continue;
        }
        else if (param == "[REPE]") {
            repeatPattern[1] = i;
            if (arr_signature.length - 1 == i)
            {
                // end of params.

                if (pkt.length == 0)
                {
                    // parse completed.
                    break;
                }
                else
                {
                    // repeat the repeat_start param
                    i = repeatPattern[0];
                    continue;
                }
            }
            else
            {
                // have next param
                if (pkt.length > 0)
                {
                    if (!repeatPattern)
                        continue;
                    else
                    {
                        // repeat the repeat_start param again
                        i = repeatPattern[0];
                        continue;
                    }
                }
                else
                    break; // parse completed.
            }
        }
        else if (param == "73$4$gbk") {
            if (pkt.slice(0, 2) != "73" || pkt.length < 12)
            {
                if (!repeatPattern)
                    return false; // signature not match
                else
                {
                    // not match pattern
                    // remove the last-N parsed param
                    for (let diff = i - repeatPattern[0]; diff > 0; diff--) {
                        paramInfo.pop();
                    }

                    i = repeatPattern[1];
                    repeatPattern = false; // no pattern again
                    continue; // go next
                }
            }
            
            LHS = allParamInfo[i].name;
            let gbkByteLength = parseInt(lib.le2be(pkt.slice(2, 10)), 16) * 2;
            endIndex = 10 + gbkByteLength;
            byte = pkt.slice(0, endIndex)
            let value = pkt.slice(10, endIndex);
            paramInfo.push(lib.parseGBK(value)); // push value

            if (allParamInfo[i].param || false)
                funcParam[allParamInfo[i].param] = value;
            
            pkt = pkt.slice(endIndex);
        }
        else if (param == "64$4") {
            if (pkt.slice(0, 2) != "64" || pkt.length < 10)
            {
                if (!repeatPattern)
                    return false; // signature not match
                else
                {
                    // not match pattern
                    // remove the last-N parsed param
                    for (let diff = i - repeatPattern[0]; diff > 0; diff--) {
                        paramInfo.pop();
                    }

                    i = repeatPattern[1];
                    repeatPattern = false; // no pattern again
                    continue; // go next
                }
            }
            
            endIndex = 10;
            LHS = allParamInfo[i].name;
            byte = pkt.slice(0, endIndex);
            let val = pkt.slice(2, endIndex);

            let [value, num, hex] = lib.parseToNum(lib.le2be(val), allParamInfo[i].type || 8);
            paramInfo.push(`${num} [0x${hex}]`); // push value

            if (allParamInfo[i].param || false)
                funcParam[allParamInfo[i].param] = value;
            
            if (allParamInfo[i].func || false)
            {
                let fn = [...allParamInfo[i].func]; // duplicate array
                if (window[fn[0]] || false)
                {
                    if (fn.length == 1)
                        paramInfo.push(window[fn[0]](value)); // push formatted value
                    else if (fn.toString().indexOf("@") > -1)
                        deferfunc.push([fn.shift(), value, fn]);
                    else
                        paramInfo.push(window[fn.shift()](value, fn)); // push formatted value
                }
                else
                {
                    log(["Function not found!", "Value: " + value, "Infos: ", fn], WARN);
                }
            }
            
            pkt = pkt.slice(endIndex);
        }
        else if (param.startsWith("$")) {
            let sz = parseInt(param.substr(1)) * 2;
            if (pkt.length < sz)
            {
                if (!repeatPattern)
                    return false; // signature not match
                else
                {
                    // not match pattern
                    // remove the last-N parsed param
                    for (let diff = i - repeatPattern[0]; diff > 0; diff--) {
                        paramInfo.pop();
                    }

                    i = repeatPattern[1];
                    repeatPattern = false; // no pattern again
                    continue; // go next
                }
            }
            
            endIndex = sz;
            LHS = allParamInfo[i].name;
            byte = pkt.slice(0, endIndex);

            let [value, num, hex] = lib.parseToNum(lib.le2be(byte), allParamInfo[i].type || sz);
            paramInfo.push(`${num} [0x${hex}]`); // push value
            
            if (allParamInfo[i].func || false)
            {
                let fn = [...allParamInfo[i].func]; // duplicate array
                if (window[fn[0]] || false)
                {
                    if (fn.length == 1)
                        paramInfo.push(window[fn[0]](value)); // push formatted value
                    else if (fn.toString().indexOf("@") > -1)
                        deferfunc.push([fn.shift(), value, fn]);
                    else
                        paramInfo.push(window[fn.shift()](value, fn)); // push formatted value
                }
                else
                {
                    log(["Function not found!", "Value: " + value, "Infos: ", fn], WARN);
                }
            }
            
            pkt = pkt.slice(endIndex);
        }
        else {
            // direct byte matching
            if (pkt.slice(0, param.length) != param)
            {
                if (!repeatPattern)
                    return false; // signature not match
                else
                {
                    // not match pattern
                    // remove the last-N parsed param
                    for (let diff = i - repeatPattern[0]; diff > 0; diff--) {
                        paramInfo.pop();
                    }

                    i = repeatPattern[1];
                    repeatPattern = false; // no pattern again
                    continue; // go next
                }
            }

            endIndex = param.length;
            pkt = pkt.slice(param.length);
            byte = param;

            if (param.match(/^(00)+$/))
            {
                // this is padding
                LHS = allParamInfo[i].name;
                paramInfo.push(param);
            }
            else
            {
                // this is subcate byte
                LHS = allParamInfo[i].name;
                paramInfo.push(signInfo["desc"]);
                _info.title += " (" + signInfo["desc"] + ")";
            }
        }
        let map = {};
        map[LHS] = paramInfo;
        _info.tooltip.push(map);
        _info.packet += " " + byte;
    };

    _info.extraTooltip = [];
    deferfunc.some(funcInfo => {
        let fnName = funcInfo[0];
        if (!(window[fnName] || false))
        {
            console.warn("[!] Function not found!", "\nFunc Name:", fnName, "\nInfo:", funcInfo);
            return false; // skip this function
        }

        if (funcInfo.toString().indexOf("@") > -1)
        {
            for (let i = 2; i < funcInfo.length; i++) {
                const item = funcInfo[i];
                if (typeof(item) == "string" && item.startsWith("@"))
                    funcInfo[i] = funcParam[item];
                
            }
        }
        _info.extraTooltip.push(window[fnName](funcInfo));
    })
    return true;
};

var parseWorker = (header, pkt) => {
    _info = {
        title: "", // title with subcate (if any), extra mark (if applicable)
        packet: "", // space seperated
        tooltip: [], // parsed data
        class: false // css class name (string). "false" only if using default css class
    };

    let target = pktList[header];
    _info.title = `[${target.delimeter}] ${target.desc}`;

    var signature = undefined;
    $.each(target.signature, (_sign, v) => {
        if (isMatchSign(_sign, v, pkt)) {
            signature = _sign;
            return false;
        }
        else
        {
            _info.packet = "";
            _info.tooltip = [];
            _info.class = false;
        }
    });

    if (!signature)
    {
        return buildOutput({
            class: "bg-warn",
            title: _info.title + ` -- Failed to match any signature`,
            packet: `__ ${header} ${pkt} 00`,
            log: WARN
        });
    }

    _info.packet = `__ ${header}${_info.packet} 00`;
    buildOutput(_info);
};

var singleParser = (pkt) => {
    if (pkt == "") {
        log("Empty Packet Byte.", WARN);
        return;
    }
    if (pkt.indexOf("//") > -1) {
        log(["Ignoring Commented Line.", "Line:", pkt], WARN);
        return;
    }

    // try to parse with JSON
    var json = undefined;
    try {
        json = JSON.parse(pkt);
    } catch (e) {}

    if (json != undefined) {
        // packet is using JSON format
        // TODO: ...
    }

    // remove all spaces
    pkt = pkt.replace(/\s/g, "").toLowerCase();
    if (pkt.match(/([a-f0-9]{8})([a-f0-9]{2}_)+/i)) {
        // official record format
        // format: <time (4-bytes)><size (2-bytes)>_<data ... (split by _)>_00_
        // raw   : 2d217c01 03_ff_02_00_
        // trimmed: 2d217c0103_ff_02_00_
        // output: ff02

        pkt = pkt.replace(/_/g, "");

        if (!sizeCheck(pkt.slice(8))) {
            return buildOutput({
                class: "bg-warn",
                title: "Mismatch Packet Size. [Packet Size Check Failed - F1]",
                packet: `${pkt}`,
                log: WARN
            });
        }
        pkt = pkt.slice(10, -2);
    } else if (pkt.match(/([_-]{2})([a-f0-9]{2})+00/)) {
        // replaced packet size with underscore(__) or dash(--), and without time.
        // formatA: __ <data ...>
        // raw    : __ ff0200
        // trimmed: __ff0200
        // output : ff02

        // formatB: -- <data ...>
        // raw    : -- ff0200
        // trimmed: --ff0200
        // output : ff02

        pkt = pkt.slice(2, -2);
    } else if (pkt.match(/([a-f0-9]{2})+/)) {
        // sized raw packet
        // format : <whole packet>
        // raw    : 03ff0200
        // trimmed: 03ff0200
        // output : ff02

        if (!sizeCheck(pkt)) {
            return buildOutput({
                class: "bg-warn",
                title: "Mismatch Packet Size. [Packet Size Check Failed - F2]",
                packet: `${pkt}`,
                log: WARN
            });
        }
        pkt = pkt.slice(2, -2);
    } else {
        // failed to match any
        return buildOutput({
            class: "bg-err",
            title: "Invalid Format",
            packet: `${pkt}`,
            log: ERR
        });
    }

    let header = matchHeader(pkt);
    if (!header) {
        // not in known-packet database
        return buildOutput({
            class: "",
            title: "-- Not Known Yet --",
            packet: `__ ${pkt} 00`,
            log: ERR
        });
    }

    parseWorker(header, pkt.substr(header.length));
};

//#endregion

const parserVersion = 4;