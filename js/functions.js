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
    if (!hasFlag(logMode, DEBUG) && hasFlag(logLevel, DEBUG)) return;

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
    else whole.push(`\n[${logSign}] ${messages}`);

    fn(...whole);
};
//#endregion

//#region Misc Functions
var colOrders = {
    ITEM: ["name", "dec", "buffdec"],
    MAP: ["name", "block"],
    NPC: ["name", "name2", "phyle", "attribute"],
    SKILL: ["name", "dec", "dec2", "buffdec"],
    FS: ["title", "content"]
};
const mapRange      = [1, 991];         // map
const itemRange     = [100000, 162630]; // item
const npcRange      = [311010, 492009]; // npc
const skillRange    = [510000, 840033]; // skill
// formatStringDB & itemDB collision range (inclusive): 100000 ~ 101356
const re_fs = /`\+C0x([0-9a-fA-F]{6})`-C([^`]+)(`=C)?/g;
const re_fs_param = /(\@|\||\^|\$)(\d{1,2})/g;
const re_fs_param_link = /\\t\#\d+,(.+?)\=\d+,(\d+),(\d+)\\t\$(\(\d+,\d+\))?/g; // `+C0xff0000`-C\t#16515072,食魂鸟=504,90,160\t$`=C
var lib = {
    parseGBK: (hex) => GBK.decode(lib.hex2bytes(hex)),

    _determineDB: (id, exceptFSDB = false) => {
        if (id >= skillRange[0] && id <= skillRange[1])
            return ["SKILL", skillDB];
        if (id >= npcRange[0] && id <= npcRange[1])
            return ["NPC", npcDB];
        if (id >= itemRange[0] && id <= itemRange[1])
            return ["ITEM", itemDB];
        if (id >= mapRange[0] && id <= mapRange[1])
            return ["MAP", mapDB];
        
        if (exceptFSDB)
        {
            log(["Failed to query anything from DBs (except FSDB). -- Query with option exceptFSDB = true", `ID used in DB query: ${id}`], ERR | DEBUG);
            return [undefined, undefined];
        }
        else
            return ["FS", formatstringDB];
    },

    /**
     * Parse format string into HTML preview
     * @param {number/string} fs formatString ID in number/hex-byte (big-endian)
     * @param {array} params parameters for the format string
     * @return {string} parsed format string in HTML
     */
    parseFS: (fs, ...params) =>
    {
        if (typeof(fs) == "number")
            fs = formatstringDB[fs] || false;
        else
            fs = formatstringDB[parseInt(fs, 16)] || fs;
        
        // if is found in FSDB
        if (fs.title || false)
            fs = fs.title;

        while ((m = re_fs_param.exec(fs)) !== null) {
            if (m.index === re_fs_param.lastIndex)
                re_fs_param.lastIndex++;
            
            let val = params[m[2] - 1];
            if (m[1] == "^")
            {
                // get data from db col[1] only. dont query in FSDB.
                let [dbName, db] = lib._determineDB(val, true);
                val = db[val][colOrders[dbName][0]];
            }
            else if (m[1] == "@")
            {
                // get data from db col[2] only. dont query in FSDB.
                let [dbName, db] = lib._determineDB(val, true);
                val = db[val][colOrders[dbName][1]];
            }
            else if (m[1]== "|")
            {
                // force query in FSDB only
                val = formatstringDB[val] && formatstringDB[val].title || "- FSDB-NotFound -";
            }
            else if (m[1] == "$")
            {
                // direct value
                if (typeof (val) == "string")
                {
                    // gbk string
                    val = lib.parseGBK(val);
                }
                // else: (implicit) $ is number
            }
            else if (m[1] == "&")
            {
                // direct hex value (lower case, without padding)
                val = parseInt(val).toString(16);
            }
            else
            {
                log.warn("Unknown FormatString placeholder:", m[1], "\nObject:", m);
            }

            fs = fs.replace(m[0], _ => val);
        }

        return lib.parseStyling(fs);
    },

    parseStyling: (fs) => {
        // parse color
        while ((m = re_fs.exec(fs)) !== null) {
            if (m.index === re_fs.lastIndex)
                re_fs.lastIndex++;
            
            fs = fs.replace(m[0], `<span style='color:#${m[1]};'>${m[2]}</span>`);
        }

        // remove link text
        while ((m = re_fs_param_link.exec(fs)) !== null) {
            if (m.index === re_fs_param_link.lastIndex)
                re_fs_param_link.lastIndex++;
            
            fs = fs.replace(m[0], `<span style='text-decoration:underline;cursor:pointer'>${m[1] + ((m[4] && " "+m[4]) || "")}</span>`);
        }

        fs = fs.replace(/(`N|\n|\|\|\|\|)/g, "<br>"); // replace newline

        return fs;
    },

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

        if (typeof (endian) == "number")
            endian = endian.toString(16);
        
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
    parseToNum: function (val, type = "uint") {
        if (type == "raw" || type == "")
        {
            // convert back from endian-ed to original
            return [lib.le2be(val), "-Raw-", "-Raw-"]
        }

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
var parseFS = lib.parseFS;
var parseGBK = lib.parseGBK;
var parseStyling = lib.parseStyling;
var autoDB = (id) => {
    [name, _] = lib._determineDB(id);
    if (name == "SKILL")
    {
        // try FSDB if SkillDB failed
        let res = db_Skill(id);
        return res == "-- SkillDB Fail --" ? db_FormatString(id) : res;
    }
    if (name == "NPC")
        return db_NPC(id);
    if (name == "ITEM")
        return db_Item(id);
    if (name == "MAP")
        return db_Map(id);
    else
        return db_FormatString(id);
}
//#endregion

//#region sorting function
const _regex_zeros = /[a-f0-9]{2}/g;
var sortFn = (a, b) => (a.length > b.length ? +1 : a.length < b.length ? -1 : a - b);
var sortSignature = ([sa, a], [sb, b]) => {
    // more fix data goes first
    // sa: sign A, sb: sign B < 1st
    // a: param A, b: param B < 2nd
    // -1: move up, 1: move down

    // 1st: more fix data more explicit
    let za = sa.match(_regex_zeros),
        zb = sb.match(_regex_zeros);
    
    if (!za)
        return 1;
    
    if (!zb)
        return -1;
    
    if (za.length > zb.length)
        return -1;

    // 2nd: sort by explicity (less $, more explicit)
    let ea = sa.split("$").length,
        eb = sb.split("$").length;
    
    if (ea > eb)
        return 1;
    else if (ea < eb)
        return -1;
    
    // then: sort by longer param length 1st
    if (!a.params)
        return 1; // a has no param, move a downward
    if (!b.params)
        return -1; // b has no param, move a upward

    if (a.params.length > b.params.length)
        return -1; // a has more params, move a upward
    else if (a.params.length < b.params.length)
        return 1;
    
    return 0; // else keep curr pos
};
//#endregion

//#region Parser
/**
 * Warning Flag: expected more bytes, but already reached the end.
 */
const WFLAG_MORE_EXPECTED = 1 << 0;
/**
 * Warning Flag: expected a signature during parse, but the current signature block is "" (empty string)
 */
const WFLAG_UNEXPECTED_NULL_SIGNATURE = 1 << 1;
var $io = $(`#io`);

var buildOutput = (info) => {
    let title = info.title || ["default title"],
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
                    tooltipInfo += `<tr class="fg-err"><td colspan="100"></td></tr>`;
                else if (hasFlag(flag, WFLAG_UNEXPECTED_NULL_SIGNATURE))
                    tooltipInfo += `<tr class="fg-err"><td colspan="100">Unexpceted NULL signature!</td></tr>`;
            } else {
                $.each(ele, (desc, actual) => {
                    // actual[<1st>], actual[<2nd>] are packet byte offset
                    tooltipInfo += `<tr><td>[0x${actual.shift().toString(16).padStart(2, '0').toUpperCase()} - 0x${actual.shift().toString(16).padStart(2, '0').toUpperCase()}]</td><td>${desc}</td>`;
                    let last = actual.pop();
                    actual.forEach(e => {
                        tooltipInfo += `<td>${e}</td>`;
                    });
                    tooltipInfo += `<td colspan="100">${last}</td>`;
                    tooltipInfo += "</tr>";
                });
            }
        });
        tooltipInfo = `<table><tbody>` + tooltipInfo + `</tbody></table>`;
        // tooltipInfo = tooltipInfo.replace(/\"/gm, "&quot;"); // replace double-qoute (") to HTML charCode
        (info.extraTooltip || []).forEach((ele) => {
            tooltipInfo += `<div class="extra">${ele.replace(/\"/gm, "&quot;")}</div>`;
        });

        if (window.currItem.tooltip)
            window.currItem.tooltip("dispose");
        
        window.currItem.tooltip({
            html: true,
            animation: false,
            placement: "right",
            trigger: "hover click",
            boundary: "window",
            title: tooltipInfo,
        });
    }
    window.currItem.addClass(`${info.class || ""}`);
    window.currItem.find(" > [role=header]").html(`// ${title.join(' ')}`);
    window.currItem.find(" > [role=editable]").html(`${packet}`);

    $(window.currItem).children().on('click', e => {
        let $parent = $(e.target).parents(`[role="item"]`);
        $parent.toggleClass("active");
        $parent.click();
    });
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
window._knownID = {}; // { id: {n: occurrenceCount, i: highlightColorIndex} }
var isMatchSign = (signature, signInfo, original_pkt) => {
    // clone pkt, this variable will be used to store the unprocessed bytes.
    var pkt = original_pkt;
    var allParamInfo = signInfo["params"];
    var deferfunc = [], // = [ [<func 1 name>, <selfValue>, <Param1 / @Name>] ]
        funcParam = {};
    var repeatPattern = false; // [<start param>, <index of REPE>]
    
    let arr_signature = signature.split(" ");
    let isFormatParam = false;
    let params = [];

    // push sub-desc to title
    if (signInfo["desc"] && signInfo["desc"] != "")
        _info.title.push("(" + (signInfo["desc"] || " -- No Desc --") + ")");
    
    var pktByteOffset = (_info.pkt_header.length / 2) + 1; // +1 is for sizeByte
    var paramIndex = 1;
    for (let i = 0; i < arr_signature.length; i++) {
        const param = arr_signature[i];

        if (param == "")
        {
            if (i > 0)
            {
                // empty signature after few previous signatures.
                break; // skip and continue to do the next task after this for-i-loop
            }
            else
            {
                _info.tooltip.push(WFLAG_UNEXPECTED_NULL_SIGNATURE);
                return false;
            }
        }
        
        if (pkt.length == 0 && param != "[REPE]" && !repeatPattern)
        {
            if (pkt.length == 0 && param == "[REPS]")
            {
                // [REPS] is optional data, and the packet has no any.
                // go to match the data after [REPE]
                i = arr_signature.indexOf("[REPE]", i);
                continue;
            }
            
            _info.tooltip.push(WFLAG_MORE_EXPECTED);
            return false; // expacted more but end of packet
        }
        
        var LHS, byte = "", endIndex = 0;
        /**
         * Every values pushed in paramInfo are parsed to Big-Endian
         * For strings, push the bytes to this list without parsing to GBK.
         */
        var paramInfo = [];
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
        else if (param == "param") {
            if (pkt.slice(0, 2) == "73" && pkt.length >= 12)
            {
                // push starting offset
                paramInfo.push(pktByteOffset);
                
                LHS = allParamInfo[i].name.replace(/\<index\>/i, paramIndex++);
                let gbkByteLength = parseInt(lib.le2be(pkt.slice(2, 10)), 16) * 2;
                endIndex = 10 + gbkByteLength;
                byte = pkt.slice(0, endIndex);
    
                // push ending offset
                pktByteOffset += endIndex / 2;
                paramInfo.push(pktByteOffset - 1);
    
                let value = pkt.slice(10, endIndex);
                paramInfo.push(lib.parseGBK(value)); // push value
    
                if (allParamInfo[i].param || false)
                    funcParam[allParamInfo[i].param] = value;
    
                if (isFormatParam)
                    params.push(value);
                
                pkt = pkt.slice(endIndex);
            }
            else if (pkt.slice(0, 2) == "64" && pkt.length >= 10)
            {
                // push starting offset
                paramInfo.push(pktByteOffset);
                
                endIndex = 10;
                LHS = allParamInfo[i].name.replace(/\<index\>/i, paramIndex++);
                byte = pkt.slice(0, endIndex);
                let val = pkt.slice(2, endIndex);

                // push ending offset
                pktByteOffset += endIndex / 2;
                paramInfo.push(pktByteOffset - 1);

                let [value, num, hex] = lib.parseToNum(lib.le2be(val), allParamInfo[i].type || 8);
                paramInfo.push(`${num} [0x${hex}]`); // push value

                if (allParamInfo[i].param || false)
                    funcParam[allParamInfo[i].param] = value;
                
                paramInfo.push(autoDB(value)); // push formatted value (autoDB)

                if (isFormatParam)
                    params.push(value);
                
                pkt = pkt.slice(endIndex);
            }
            else
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

            // push starting offset
            paramInfo.push(pktByteOffset);
            
            LHS = allParamInfo[i].name.replace(/\<index\>/i, paramIndex++);
            let gbkByteLength = parseInt(lib.le2be(pkt.slice(2, 10)), 16) * 2;
            endIndex = 10 + gbkByteLength;
            byte = pkt.slice(0, endIndex);

            // push ending offset
            pktByteOffset += endIndex / 2;
            paramInfo.push(pktByteOffset - 1);

            let value = pkt.slice(10, endIndex);
            paramInfo.push(lib.parseGBK(value)); // push value

            if (allParamInfo[i].param || false)
                funcParam[allParamInfo[i].param] = value;

            if (isFormatParam)
                params.push(value);
            
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

            // push starting offset
            paramInfo.push(pktByteOffset);
            
            endIndex = 10;
            LHS = allParamInfo[i].name.replace(/\<index\>/i, paramIndex++);
            byte = pkt.slice(0, endIndex);
            let val = pkt.slice(2, endIndex);

            // push ending offset
            pktByteOffset += endIndex / 2;
            paramInfo.push(pktByteOffset - 1);

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

            if (isFormatParam)
                params.push(value);
            
            pkt = pkt.slice(endIndex);
        }
        else if (param == "$gbk") {
            if (pkt.length <= 0)
                return false; // signature not match
            
            // push starting offset
            paramInfo.push(pktByteOffset);
            
            LHS = allParamInfo[i].name;
            byte = value = pkt;

            // push ending offset
            pktByteOffset += pkt.length / 2;
            paramInfo.push(pktByteOffset - 1);

            paramInfo.push(`${lib.parseGBK(value)}`); // push value
            
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
            
            pkt = "";
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
            
            // push starting offset
            paramInfo.push(pktByteOffset);
            
            endIndex = sz;
            LHS = allParamInfo[i].name;
            byte = pkt.slice(0, endIndex);

            // push ending offset
            pktByteOffset += endIndex / 2;
            paramInfo.push(pktByteOffset - 1);

            let [value, num, hex] = lib.parseToNum(lib.le2be(byte), allParamInfo[i].type);
            paramInfo.push(`${num} [0x${hex}]`); // push value
            
            if (allParamInfo[i].func || false)
            {
                let fn = [...allParamInfo[i].func]; // duplicate array
                if (fn[0] == "parseFS")
                {
                    isFormatParam = true;
                    deferfunc.push([fn.shift(), value, fn]);
                    paramInfo.push(db_FormatString(value)); // push formatstring db value
                }
                else if (window[fn[0]] || false)
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
            
            // push starting offset
            paramInfo.push(pktByteOffset);

            endIndex = param.length;
            pkt = pkt.slice(param.length);
            byte = param;

            // push ending offset
            pktByteOffset += endIndex / 2;
            paramInfo.push(pktByteOffset - 1);

            if (param.match(/^(00)+$/))
            {
                // this is padding
                LHS = allParamInfo[i].name || "Padding";
                paramInfo.push(param);
            }
            else
            {
                // this is subcate byte
                LHS = allParamInfo[i].name;

                if (allParamInfo[i].func || false)
                {
                    let fn = [...allParamInfo[i].func]; // duplicate array
                    let value = parseInt(lib.le2be(param), 16);
                    paramInfo.push(value);
                    
                    if (fn[0] == "parseFS")
                    {
                        isFormatParam = true;
                        deferfunc.push([fn.shift(), value, fn]);
                        paramInfo.push(db_FormatString(value)); // push formatstring db value
                    }
                    else if (window[fn[0]] || false)
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
                else
                {
                    paramInfo.push(signInfo["desc"]);
                }
            }
        }
        let map = {};
        map[LHS] = paramInfo;
        _info.tooltip.push(map);
        _info.packet += " " + byte;

        // global knownID highlight (build knownID list)
        if (LHS.indexOf("ID") > -1 && byte.length == 8) { // len 8 is $4
            if (window._knownID[byte])
                window._knownID[byte].n++;
            else
                window._knownID[byte] = { n: 1, i: Object.keys(window._knownID).length };
        }
    };

    if (pkt.length > 0)
    {
        // the signature not fully match
        return false;
    }

    _info.extraTooltip = [];
    deferfunc.some(funcInfo => {
        let fnName = funcInfo[0];
        if (!(window[fnName] || false))
        {
            console.warn("[!] Function not found!", "\nFunc Name:", fnName, "\nInfo:", funcInfo);
            return false; // skip this function
        }

        if (fnName == "parseFS")
        {
            let val = funcInfo[1];
            let opt = funcInfo[2];
            if (opt && opt[0])
            {
                let db;
                let col;

                if (opt[0].startsWith("!@")) {
                    // col name only, auto db
                    db = lib._determineDB(val, true)[1];
                    col = opt[0].slice(2);
                }
                else if (opt[0].startsWith("!"))
                {
                    // db with col name
                    let _tmp = opt[0].slice(1).split("@");
                    col = _tmp[1]; // set col name

                    let dbName = _tmp[0];
                    if (dbName == "SKILL")
                        db = skillDB;
                    else if (dbName == "NPC")
                        db = npcDB;
                    else if (dbName == "ITEM")
                        db = itemDB;
                    else if (dbName == "MAP")
                        db = mapDB;
                    else
                        db = undefined;
                }
                
                if (!db)
                {
                    _info.extraTooltip.push("Failed to determine database of FormatString");
                    console.warn("[!] Failed to determine database of FormatString.\n\tValue:", val);
                    return false; // continue for next func
                }
                
                if (!col)
                {
                    console.warn("[!] Invalid column name.\n\tValue:", col,"\n\tCurrent Pkt:", _info.packet);
                    return false; // continue for next func
                }
                val = db[val][col];
            }
            _info.extraTooltip.push("Parsed FormatString:<br/>" + parseFS(val, ...params));
            return true; // end defer func loop
        }
        else if (funcInfo.toString().indexOf("@") > -1)
        {
            for (let i = 2; i < funcInfo.length; i++) {
                const item = funcInfo[i];
                if (typeof(item) == "string" && item.startsWith("@"))
                    funcInfo.push(funcParam[item]); // get value from defer func param (@sth)
            }
            funcInfo.shift(); // remove fnName
            _info.extraTooltip.push(window[fnName](...funcInfo.flat()));
            return true;
        }
        _info.extraTooltip.push(window[fnName](funcInfo));
    })
    return true;
};

var parseWorker = (header, pkt) => {
    _info = {
        title: [], // title with subcate (if any), extra mark (if applicable)
        packet: "", // space seperated
        tooltip: [], // parsed data
        class: false, // css class name (string). "false" only if using default css class
        pkt_header: header // packet header bytes
    };

    let target = pktList[header];
    _info.title.push(`[${header.toUpperCase()}] ${target.desc}`);

    var signature = undefined;
    $.each(target.signature, (_sign, v) => {
        if (isMatchSign(_sign, v, pkt)) {
            signature = _sign;
            return false;
        }
        else
        {
            _info.title.pop();
            _info.packet = "";
            _info.tooltip = [];
            _info.class = false;
        }
    });

    if (!signature)
    {
        return buildOutput({
            class: "bg-warn",
            title: [_info.title.join(' '), ` -- Failed to match any signature`],
            packet: `__ ${header} ${pkt} 00`,
            log: WARN
        });
    }

    _info.packet = `__ ${header}${_info.packet} 00`;
    buildOutput(_info);
};

window.color = [];
var singleParser = ($ele) => {
    var pkt = $ele.find(` > [role="editable"]`).text();
    window.currItem = $ele;

    if (pkt == "")
        return;

    pkt = pkt.replace(/\/\*.+\*\//ig, "");

    if (pkt.replace(/\s+/g, "").startsWith("//")) {
        if (pkt.indexOf("// Command") > -1)
        {
            // do not repeat parse command line.
            return;
        }

        log(["Ignoring Comment Line.", "Line:", pkt], WARN);
        buildOutput({
            class: "bg-ignored",
            title: ["Comment Line [Ignored]"],
            packet: pkt.replace(/\s{2,}/g, ""),
            log: WARN
        });
        return;
    }

    if (pkt.startsWith("00"))
    {
        log(["Ignoring Filtered Packet.", "Line:", pkt], WARN);
        buildOutput({
            class: "bg-ignored",
            title: ["Filtered Packet [Ignored]"],
            packet: pkt.replace(/\s{2,}/g, ""),
            log: WARN,
        });
        return;
    }

    // try to parse with JSON
    var json = undefined;
    try {
        json = JSON.parse(pkt);
    } catch (e) {}

    if (json != undefined) {
        // packet is using JSON format
        window.currItem.remove(); // remove self
        $.each(json, (timestamp, dict) => {
            dict.data.forEach(pkt => {
                $io.addItem(pkt);

                // assign color to distinguish different client ports
                let colorIdx = color.indexOf(dict.port);
                if (colorIdx == -1)
                {
                    color.push(dict.port);
                    colorIdx = color.length - 1;
                }
                
                window.currItem = $(`[data-id="${window.itemid - 1}"]`);
                window.currItem.prepend(
                    `<div class="header_extra${dict.srv ? " srv" : ""} c${colorIdx}">I[${
                        dict.idx
                    }] Time[${timestamp}] [${dict.srv ? "cli <- srv" : "cli -> srv"} @ ${dict.port}]</div>`
                );

                if (!dict.srv)
                {
                    // from client (string command)
                    let cmd;
                    if (pkt.indexOf("<00>") == -1)
                    {
                        // not yet parsed from gbk, perform parsing
                        cmd = lib.parseGBK(pkt.replace(/0d0a(00)+/ig, "").replace(/(00)+/ig, "00"));
                    }
                    else
                    {
                        // alreaddy parsed from gbk, only need to remove the repeated "00" paddings
                        cmd = pkt.replace(/(<00>)+/g, "");
                    }
                    window.currItem.find(` > [role="editable"]`).html("// Command: " + cmd);
                    window.currItem.find(` > [role="header"]`).html("// Command[" + cmd + "]");
                    return true; // loop next
                }

                // ignore the filtered packet
                if (pkt.startsWith("00")) {
                    log(["Ignoring Filtered Packet.", "Line:", pkt], WARN);
                    buildOutput({
                        class: "bg-ignored",
                        title: ["Filtered Packet [Ignored]"],
                        packet: pkt.replace(/\s{2,}/g, ""),
                        log: WARN,
                    });
                    return;
                }

                pkt = pkt.slice(2, -2); // remove size and ending byte
                let header = matchHeader(pkt);
                if (!header) {
                    // not in known-packet database
                    buildOutput({
                        class: "",
                        title: ["-- Not Known Yet --"],
                        packet: `__ ${pkt} 00`,
                        log: ERR
                    });
                    return;
                }

                parseWorker(header, pkt.substr(header.length));
            })
        });
        $io.addItem();
        return true;
    }

    let _regex_pkt_cappy = pkt.match(/\[\+\]\s\[(\d+\.\d+)\]\s\[(\d{1,5})([\<\>])\]\sData(\[[^\n\r]{2,}\])\s\I\[(\d+)\]/i); // 1: time, 2: port, 3: direction, 4: data, 5: index
    if (_regex_pkt_cappy) {
        // sized & AES padded raw packet(s) -- from pkt_cap.py
        // format : [+] [<17 char long timestamp>] [client tcp port <pkt direction>]
        // raw    : [+] [1606907939.964399] [12345<] Data['06786e8 ... omitted (multiple unparsed packets) ... 00']
        // remarks[pkt direction]: "<" means incoming, ">" means outgoing
        /**
         * Sample:
         *  [+] [1607106820.879367] [62591>] Data['path 22360 233,173 210 1 0'] I[1]
         *  [+] [1607106821.059525] [62591>] Data['go2 232,174 2 22360'] I[2]
         *  [+] [1607106821.205270] [62591<] Data['0f67395f0101a1751012e800ae003200'] I[3]
         *  [+] [1607106822.594286] [62591>] Data['go >1'] I[4]
         *  [+] [1607106822.774222] [62591>] Data['act 11'] I[5]
         *  [+] [1607106822.775935] [62591<] Data['075e395f01010100'] I[6]
         *  [+] [1607106822.926760] [62591<] Data['0b40395f0101b37510120b00', '043d090100'] I[7]
         *  [+] [1607106826.731505] [62591>] Data['path 22418 234,172 210 1 0'] I[8]
         *  [+] [1607106826.912422] [62591>] Data['go2 233,173 2 22418'] I[9]
         *  [+] [1607106827.067038] [62591<] Data['0f67395f0101df751012e900ad003200'] I[10]
         */

        let [_, timestamp, port, fromSrv, data, idx] = _regex_pkt_cappy;
        fromSrv = fromSrv == "<";

        window.currItem.remove(); // remove self is a must :)
        if (data == "-- szPkt >= 1460 --")
        {
            // remove this pkt block as the packet seperated into two or more packets. (large packet)
            return true; // continue the outer-loop (single parser)
        }
        else
        {
            // seperate packet blocks into parsed packet blocks.
            data = JSON.parse(data.replace(/'/g, '"')); // to array
            data.forEach(singlePkt => {
                $io.addItem(singlePkt);

                // assign color to distinguish different client ports
                let colorIdx = color.indexOf(port);
                if (colorIdx == -1)
                {
                    color.push(port);
                    colorIdx = color.length - 1;
                }
                
                window.currItem = $(`[data-id="${window.itemid - 1}"]`);
                window.currItem.prepend(
                    `<div class="header_extra${fromSrv ? " srv" : ""} c${colorIdx}">I[${
                        idx
                    }] Time[${timestamp}] [${fromSrv ? "cli <- srv" : "cli -> srv"} @ ${port}]</div>`
                );

                if (!fromSrv)
                {
                    // string command
                    window.currItem.find(` > [role="editable"]`).html("// Command: " + singlePkt);
                    window.currItem.find(` > [role="header"]`).html("// Command[" + singlePkt + "]");
                    return true; // loop next
                }

                let pkt = singlePkt.slice(2, -2); // remove size and ending byte
                let header = matchHeader(pkt);
                if (!header) {
                    // not in known-packet database
                    return buildOutput({
                        class: "",
                        title: ["-- Not Known Yet --"],
                        packet: `__ ${pkt} 00`,
                        log: ERR
                    });
                }

                parseWorker(header, pkt.substr(header.length));
            });
        }
        
        return true;
    }

    // take only the bytes before comment, remove all spaces
    pkt = pkt.split("//")[0].replace(/\s/g, "").toLowerCase();
    let _regex_pkt_official = pkt.match(/([a-f0-9]{3,8})([a-f0-9]{2}_)+/i);
    if (_regex_pkt_official) {
        // official record format
        // format : <time (3~8 char long)><size (2-bytes)>_<data ... (split by _)>_00_
        // raw    : 2d217c01 03_ff_02_00_
        // trimmed: 2d217c0103_ff_02_00_
        // output : ff02

        pkt = pkt.slice(_regex_pkt_official[1].length); // remove time
        pkt = pkt.replace(/_/g, "");

        if (!sizeCheck(pkt)) {
            return buildOutput({
                class: "bg-warn",
                title: ["Mismatch Packet Size. [Packet Size Check Failed - F1]"],
                packet: `${pkt}`,
                log: WARN,
            });
        }
        pkt = pkt.slice(2, -2); // remove pkt size
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
                title: ["Mismatch Packet Size. [Packet Size Check Failed - F2]"],
                packet: `${pkt}`,
                log: WARN,
            });
        }
        pkt = pkt.slice(2, -2);
    } else {
        // failed to match any
        return buildOutput({
            class: "bg-err",
            title: ["Invalid Format"],
            packet: `${pkt}`,
            log: ERR,
        });
    }

    let header = matchHeader(pkt);
    if (!header) {
        // not in known-packet database
        return buildOutput({
            class: "",
            title: ["-- Not Known Yet --"],
            packet: `__ ${pkt} 00`,
            log: ERR
        });
    }

    parseWorker(header, pkt.substr(header.length));
};

//#endregion