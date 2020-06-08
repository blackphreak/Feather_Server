import json, glob, re, traceback, os, sys, time

# bit mask
LOG = 1 << 0
WARN = 1 << 1
ERR = 1 << 2
DEBUG = 1 << 3

logMode = LOG | WARN | ERR | DEBUG

# global vars
_log_indent = -1
delimeters = {}
backTrace = []
pkt_outputs = {}
longestHeader = -1
"""
format: pkt_outputs
{
    <pkt_header>: {
        "delimeter": <pkt_delimeter>, # for pkt_header collision checking
        "desc": <desc>,
        "signature": {
            <signature>: {
                "desc": <desc for this signature>,
                "params": [
                    {
                        "name": <name for param[0]>
                    },
                    {
                        "name": <name for param[1]>
                        "func": <function for parsing param[1]>
                    }
                ]
            }
        }
    }
}
"""

# functions
def hasFlag(src, targetFlag = LOG):
    return (src & targetFlag) > 0

def getLogSign(logLv):
    if hasFlag(logMode, DEBUG) and hasFlag(logLv, DEBUG):
        return "*"
    if hasFlag(logLv, LOG):
        return "+"
    if hasFlag(logLv, WARN):
        return "!"
    if hasFlag(logLv, ERR):
        return "-"

def log(msg, logLv = LOG, indentOffset = 0):
    if not hasFlag(logMode, logLv):
        return

    indent = (_log_indent + indentOffset) * "    "
    if type(msg) is str:
        print(f"{indent}[{getLogSign(logLv)}] {msg}")
    else:
        print(f"{indent}[{getLogSign(logLv)}] {msg.pop(0)}")
        for message in msg:
            print(f"{indent}    {message}")
        print()
    if hasFlag(logLv, ERR):
        print("\n---------------------------------------------")
        print("BackTrace (Most Recent Called Last):")
        for i in reversed(range(0, len(backTrace))):
            func, file, line = backTrace[i]["func"], backTrace[i]["file"], backTrace[i]["line"]
            print(f"    {i}: {func}()\n          Processing File[{file}] @ Line[{line}]")
        print("\n---------------------------------------------")
        # print exception
        traceback.print_exc()
        print()
        exit()

def initBT(func, line = "--init--", file = None):
    global backTrace
    if not file and len(backTrace) != 0:
        file = backTrace[-1]["file"]

    backTrace.append({"func": func, "file": file, "line": line})
    global _log_indent
    _log_indent += 1

def rmBT():
    global backTrace
    backTrace.pop()
    global _log_indent
    _log_indent -= (1 if _log_indent > -1 else 0)

def updateBT(line, file = None):
    backTrace[-1]["line"] = line
    if file:
        backTrace[-1]["file"] = file

# actual parser function
def doesKeywordAppear(line, keywordList):
    for keyword in keywordList:
        if keyword in line:
            return True
    return False

def readFragment(path, tag):
    initBT("readFragment")
    codeList = []
    with open(basePath + path, "r") as f:
        lines = f.readlines()

    i = 0
    indent = 0
    while f"JS_F: Here[{tag}]" not in lines[i]:
        if i >= len(lines):
            log(["Failed to locate fragment tag while reading fragment. [E-RF-0]", f"Tag: {tag}", f"Path: {path}"], ERR)
        i += 1
    
    indent = lines[i].find("/* JS_F: Here")
    i += 1 # skip the "JS_F" line
    # find next valid line
    while f"/* JS" not in lines[i]:
        if i >= len(lines):
            log(["Failed to locate next valid line while reading fragment. [E-RF-1]", f"Tag: {tag}", f"Path: {path}"], ERR)
        i += 1

    i -= 1 # read the last line that contains "/* JS" mark
    while i < len(lines) - 1:
        i += 1
        line = lines[i].rstrip().replace("\r\n", "")
        updateBT(line)
        
        if not line: # empty lines
            continue

        if not line.startswith((indent * " ")):
            break # end of fragment function

        if "/* JS_F: To" in line:
            m = re.match(".*\/\* JS_F: To\[(\w+)\@(.+\.cs)\] \*\/.*", line)
            if not m:
                log(["Malform JS_F. [E-RF-PT]", f"Line[{i + 1}] {line}"], ERR)

            tag = m[1]
            path = m[2]
            
            log(f"Reading Sub-Fragment. Tag[{tag}] At[{path}]", LOG, indentOffset=1)

            # merge fragment
            codeList += readFragment(path, tag)

            # search for line after .writeFragment()
            while ".writeFragment(" not in lines[i]:
                i += 1
            
            i += 1 # skip the .writeFragment() line
        else:
            codeList.append({"line": i + 1, "code": line.lstrip(), "from": path.split("/")[-1]})
    
    rmBT()
    return codeList

def buildSignature(codes):
    global _log_indent
    initBT("buildSignature")

    global pkt_outputs
    pl = False
    header = None
    info = {} # delimeter, desc, signature
    sign = [] # sign item (01, $2, $2, $1, ...)
    sign_info = {} # desc, params
    i = -1
    while True:
        i += 1

        if i >= len(codes):
            break
        
        line = codes[i]["line"]
        code = codes[i]["code"]
        updateBT(line)

        if pl:
            print(line, ": ", code)

        if "/* JS_D:" in code:
            m = re.search("setDelimeter\(Delimeters\.(\S+)\)", codes[i + 1]["code"])
            if not m:
                log("Failed to get Delimeter (Regex Failed).", ERR)

            deli = m.group(1) # packet delimeter name
            if deli not in delimeters:
                log([f"Unable to identify Delimeter[{deli}]", f"Line: {line}", f"Code: {code}"], ERR)
            
            header = delimeters[deli].upper() # packet header bytes
            if header in pkt_outputs:
                if "delimeter" not in pkt_outputs[header]:
                    log(f"de mo ar {header}", ERR)

                if pkt_outputs[header]["delimeter"] != deli:
                    log(f"Collision Detected for Delimeter[{deli}] 0x{header}", ERR)
                
                # init empty dict or append previous
                info["signature"] = pkt_outputs[header]["signature"] if "signature" in pkt_outputs[header] else {}
            else:
                log(f"Building signature for Delimeter[{deli}] 0x{header}", LOG)
                info = {
                    "delimeter": deli,
                    "signature": {}
                }

            # for debug the specific packet only:
            # if header == "A91A":
            #     log([f"{line}: {code}"], DEBUG, indentOffset=-2)
            #     pl = True

            # reset
            sign = [] # sign item (01, $2, $2, $1, ...)
            sign_info = {} # desc, params
            sign_info["params"] = []
            isInFormat = False
            
            # read info
            matches = re.findall("(Desc|Mark)\[(.+?)\]", code)
            if not matches:
                log("Failed to read info for Delimeter. [E-BS-RI]", ERR)
            
            for m in matches:
                _type = m[0]
                _val = m[1]

                if _type == "Desc":
                    info["desc"] = _val
                elif _type == "Mark":
                    if _val == "EOP":
                        info["flag"] = "FLAG_EOP"
                    else:
                        log(["Invalid Mark tag value found. [W-BS-IMV1]", f"Captured: {_val}", f"Line: {line}", f"Code: {code}"], WARN)

            if "desc" not in info:
                log("Missing Desc tag. [E-BS-MD]", ERR)

            i += 1 # skip "setDelimeter(...)" line

        elif "/* JS_SC:" in code:
            # read info
            subcate = re.match(".*JS_SC: ([A-Fa-f0-9PT]{2}).*", code)
            if not subcate:
                log(["Invalid Subcate. [E-BS-SC]", f"Line: {line}", f"Code: {code}"], ERR)

            subcate = subcate[1].upper()
            if "PT" not in subcate:
                sign.append(subcate)
                sign_info["params"].append({"name": f"SubCate[{subcate}]"})

            matches = re.findall("((Desc|Mark)\[(.+?)\])", code)
            if not matches:
                log("Failed to read info for Delimeter. [E-BS-RI]", ERR)

            for m in matches:
                _type = m[1]
                _val = m[2]

                if _type == "Desc":
                    sign_info["desc"] = _val
                elif _type == "Mark":
                    if _val == "EOP":
                        sign_info["flag"] = "FLAG_EOP"
                    else:
                        log(["Invalid Mark tag value found. [W-BS-IMV2]", f"Captured: {_val}", f"Line: {line}", f"Code: {code}"], WARN)
            
            log(f"Building signature for SubCate[{subcate}]: {sign_info['desc']}" + (" Flag[" + sign_info["flag"] + "]" if "flag" in sign_info else ""), LOG, indentOffset=1)

        elif "/* JS:" in code:
            paramInfo = {}
            deferMark = None

            matches = re.findall("([TR]|Fn|Desc|Mark)\[(.*?)\]", code)
            for m in matches:
                _type = m[0]
                v = m[1]

                if (_type == "Desc"):
                    paramInfo["name"] = v
                elif (_type == "T"):
                    paramInfo["type"] = v
                elif (_type == "R"):
                    cols = []
                    v = v.split(",")
                    db = v.pop(0)

                    if (db == "MAP"):
                        cols.append("db_MAP")
                    elif (db == "NPC"):
                        cols.append("db_NPC")
                    elif (db == "ITEM"):
                        cols.append("db_Item")
                    elif (db == "SKILL"):
                        cols.append("db_Skill")
                    elif (db == "FS"):
                        cols.append("db_FormatString")
                    else:
                        log([f"Unknown Reference Type[{v}].", f"Line: {line}", f"Code: {code}"], WARN)
                    
                    paramInfo["func"] = cols + v
                elif (_type == "Fn"):
                    paramInfo["func"] = v.split(",")
                elif (_type == "Mark"):
                    if (v == "REPEAT_START" or v == "REPS"):
                        sign.append("[REPS]")
                    elif (v == "REPEAT_END" or v == "REPE"):
                        deferMark = "[REPE]" # let this mark to be appended later
                    else:
                        log([f"Invalid Mark Value[{v}].", f"Line: {line}", f"Code: {code}"], ERR)

            # read next line, the actual code
            nextI = i + 1
            next_line = codes[nextI]["line"]
            next_code = codes[nextI]["code"]
            updateBT(next_line)

            if ".writeByte" in next_code:
                _tmpSign = "$1"
            elif ".writeWord" in next_code:
                _tmpSign = "$2"
            elif ".writeDWord" in next_code:
                _tmpSign = "$4"
            elif ".writePadding" in next_code:
                var = re.search("\(([A-Fa-f0-9x]+)\)", next_code)
                if not var:
                    log([f"Failed to capture the value enclosed.", f"Line: {next_line}", f"Code: {next_code}"], ERR)

                var = var.group(1)
                if "0x" in var:
                    # hex
                    sz = int(var, 16)
                else:
                    # dec
                    sz = int(var)
                _tmpSign = "00" * sz
            elif ".writeFormat" in next_code:
                _tmpSign = "$4"
                isInFormat = True
                if "func" not in paramInfo: # default use db_FormatString
                    paramInfo["func"] = ["db_FormatString"]
            elif ".writeParam" in next_code:
                _tmpSign = "64$4"
                isInFormat = True
            elif ".writeString" in next_code:
                _tmpSign = "73$4$gbk" if isInFormat else " $gbk"

            sign.append(_tmpSign)
            sign_info["params"].append(paramInfo)

            # if there is deferMark to be appended to "sign" array
            if deferMark:
                sign.append(deferMark)

            i = nextI # skip the handled line

        if ".pack()" in code or ".nextPacket" in code or ("flag" in sign_info and sign_info["flag"] == "FLAG_EOP"):
            # merge the last signature with other signatures in same packet header
            info["signature"] = {**info["signature"], **{' '.join(sign): sign_info}}
            if header not in pkt_outputs:
                pkt_outputs[header] = {}
            pkt_outputs[header] = {**pkt_outputs[header], **info}

            # reset
            isInFormat = False
            pl = False

    rmBT()

# filter out the actual codes from "JS_D" to "pack()" for each functions (for JS_D) / variants (for subcate)
# replicate codes for different "JS_SC" before building signature
# (this function will filter all the JS_J_#, JS_F before pass to build signature)
def funcsHandler(funcsList):
    initBT("funcsHandler")
    global _log_indent
    _log_indent -= 1

    try:
        # stores all filtered code, and wait for build signature
        codeList = []

        for func in funcsList:
            i = -1

            # loop all lines within that function
            while i + 1 < len(func):
                i += 1
                lineNum, line = func[i]["line"], func[i]["code"]

                updateBT(lineNum)

                # for debug the specific packet only:
                # if lineNum > 138:
                #     log([f"{lineNum}: {line}"], DEBUG, indentOffset=-2)

                if "JS_D: " in line:
                    # packet begins
                    currCodeBeforeSubcate = [] # {"line": <lineNum>, "code": <code>}, {"line": <lineNum>, "code": <code>}, ...

                    # find the next valid code for "JS_D": "setDelimeter(...)"
                    nextI = i + 1
                    if "setDelimeter" not in func[nextI]["code"]:
                        log("setDelimeter not found after JS_D declaration. [E-JS_D-0]", ERR)
                    
                    currCodeBeforeSubcate.append(func[i]) # push curr code (JS_D)
                    currCodeBeforeSubcate.append(func[nextI]) # push curr code (setDelimeter)
                    i = nextI # skip the nextI

                    # init/reset subcate
                    subcateVariants = []
                    currSubcateIdx = -1

                elif "JS_SC: " in line:
                    m = re.search("(Jump|At|Mark)\[(.*?)(,(.*?))?\]", line)
                    if not m:
                        log(["Invalid JS_SC tag found. [E-JS_SC-0]", f"Line[{lineNum}] {line}"], ERR)

                    _type = m.group(1)
                    currSubcateIdx += 1
                    handledSubcateIdx = i # in variable "func"'s index
                    subcateVariants.append([]) # [{"line": <line>, "code": <code>}, ...]

                    if _type == "Jump":
                        if len(m.groups()) < 4:
                            log(["Malform Jump tag encountered. [E-JS_SC-1]", f"Line[{lineNum}] {line}"], ERR)

                        # Subcate is using "Jump" tag
                        _begins = m.group(2)
                        _cont = m.group(4)

                        flag_subcateJumpFound = False
                        flag_continueFound = False
                        subcateVariants[currSubcateIdx].append(func[i]) # push current line (JS_SC)

                        # search for next line which contains _begins
                        nextI = i + 1
                        while True:
                            if nextI >= len(func):
                                if len(subcateVariants[currSubcateIdx]) == 0:
                                    log(["Failed to locate " + ("begins" if flag_subcateJumpFound else "continue" if flag_continueFound else "end") + " mark for JS_SC before EOF", f"Target: {_begins}", f"Curr i: {i}", f"Line[{lineNum}] {code}"], ERR)
                                break
                            
                            updateBT(func[nextI]["line"])
                            code = func[nextI]["code"]
                            if f"/* {_begins}_END */" in code:
                                flag_subcateJumpFound = False
                            elif f"/* {_begins} */" in code:
                                flag_subcateJumpFound = True
                            elif "/* JS_J_" not in code: # make sure to skip all JS_J_# tag enclosed within the start, end tag
                                if flag_subcateJumpFound or flag_continueFound:
                                    # push everything enclosed
                                    subcateVariants[currSubcateIdx].append(func[nextI])
                                elif _cont is not None:
                                        # continue mark exists, search for continue line
                                        if f"/* {_cont} */" in code:
                                            flag_continueFound = True
                            
                            nextI += 1

                    elif _type == "Mark":
                        # Subcate is using "Mark[EOP]" tag
                        val = m.group(2)
                        if val != "EOP":
                            log(["Invalid Mark tag for JS_SC. Only Mark[EOP] is allowed. [E-JS_SC-2]", f"Captured: {val}"], ERR)
                        subcateVariants[currSubcateIdx].append(func[i])
                    else:
                        log(["Invalid tag for JS_SC. [E-JS_SC-EX]", f"Captured: {m.groups()}"], ERR)

                    i = handledSubcateIdx # skip handled

                elif "JS_F: " in line:
                    # !REVIEW! can we use JS_F in subcate? will it causes a problem?
                    
                    m = re.match(".*\/\* JS_F: To\[(\w+)\@(.+\.cs)\] \*\/.*", line)
                    if not m:
                        log(["Malform JS_F. [E-JS_F-PT", f"Line[{lineNum}] {line}"], ERR)

                    tag = m[1]
                    path = m[2]
                    print("")
                    log(f"Reading Fragment for next Delimeter. Tag[{tag}] At[{path}]", LOG, indentOffset=1)

                    currCodeBeforeSubcate += readFragment(path, tag)

                    log(f"Whole Fragment Read Completed.", LOG, indentOffset=1)
                elif "JS: " in line and currSubcateIdx == -1:
                    # outside subcate
                    currCodeBeforeSubcate.append(func[i]) # push curr code (JS)

                    nextI = i + 1
                    if not doesKeywordAppear(func[nextI]["code"], ["writeByte", "writeWord", "writeDWord", "writeFormat", "writeString", "writePadding", "writeParam"]):
                        log(["Missing valid write function after JS declaration. [E-JS-0]", f"Previous Line: {func[i]}"], ERR)
                    
                    currCodeBeforeSubcate.append(func[nextI]) # push curr code (write...)
                    i = nextI # skip nextI
                elif doesKeywordAppear(line, [".pack()", ".nextPacket()"]):
                    if currSubcateIdx == -1:
                        # no subcate
                        currCodeBeforeSubcate.append(func[i])
                        buildSignature(currCodeBeforeSubcate)
                    else:
                        # have subcate
                        subcateVariants[currSubcateIdx].append(func[i])
                        for subcate in subcateVariants:
                            # merge & build
                            buildSignature(currCodeBeforeSubcate + subcate)

                        # reset
                        subcateVariants = []
                        currSubcateIdx = -1

    except Exception as e:
        log(f"Exception occurred: {e}", ERR)

    _log_indent += 1
    rmBT()


# if not eitherOne:
#     log(["Either one constraint violation.", "Applied: " + eitherOne, " Trying:" + _type], ERR)
# eitherOne = _type

# seperates different functions by indentation
def srcHandler(lines, file):
    initBT("srcHandler", "init", file)

    funcsList = [] # stores: [{"line": <lineNum>, "code": <lineCode>}]
    tmpFuncInfo = []
    currFuncIndent = 0
    flag_funcStart = False
    lineNum = 0 # starting from 1

    while lineNum < len(lines) - 1:
        lineNum += 1
        line = lines[lineNum].rstrip().replace("\r\n", "")
        updateBT(lineNum)

        if "public static PacketStreamData" in line:
            # start of a packet function
            currFuncIndent = line.find("public")
            tmpFuncInfo = []
            tmpFuncInfo.append({"line": lineNum + 1, "code": line.lstrip()})
            flag_funcStart = True
            continue
        elif line.startswith((currFuncIndent * " ") + "}"):
            # end of function
            if flag_funcStart: # filter out empty function
                funcsList.append(tmpFuncInfo)
            flag_funcStart = False
            continue # loop for next function

        if flag_funcStart:
            tmpFuncInfo.append({"line": lineNum + 1, "code": line.lstrip()})

    funcsHandler(funcsList)
    rmBT()

if __name__ == "__main__":
    versionInfo = ["2020-06-09", "build 50148", "3"]
    selfName = __file__.replace('\\', '/').split("/")[-1]
    print(f'------------ [Destiny Online Project] ------------\nFile: {selfName}\nVersion: {" ".join(versionInfo)}')

    global basePath
    basePath = "./"
    if len(sys.argv) == 2:
        basePath = sys.argv[1]

    basePath = (os.path.abspath(basePath) + "/").replace("\\", "/")
    print(f"Base Path (Server Root Path): {basePath}")

    print('--------------------------------------------------\n\n')

    with open(basePath + "Feather_Server/Packets/Delimeters.cs", "r") as f:
        log('Importing Delimeter List ....')
        for line in f:
            if "public static readonly byte[]" not in line:
                continue
            line = line.replace("public static readonly byte[] ", "").split("=")
            k = line[0].replace(" ", "")
            v = "".join(line[1].replace("0x", "").replace(" ", "").split("}")[0][1:].split(","))
            delimeters[k] = v
        
    if len(delimeters) <= 1:
        log('Failed to load delimeters.', ERR)

    sourceCodes = [f for f in glob.glob(basePath + "Feather_Server/Packets/Actual/**/*.cs", recursive=True)] + [f for f in glob.glob(basePath + "Feather_Server/Packets/Utils/**/*.cs", recursive=True)]
    if len(sourceCodes) <= 0:
        log('Failed to file any source code file. (files that ended with ".cs")', ERR)
    
    log('Reading Source Codes ....')
    for file in sourceCodes:
        with open(file, "r", newline='\r\n') as f:
            file = file.replace("\\", "/")[file.find("Feather_Server/Packets/"):]
            log(f"Processing Source [{file}] ...")
            srcHandler(f.readlines(), file)
    
    print()
    log(f"All Packet Codes Processed.")

    pkt_outputs["_builder"] = versionInfo
    pkt_outputs["_timestamp"] = int(time.time()) * 1000 # from sec to m.sec
    jsonPath = basePath + "Feather_Server/Packets/_pkts.v2.json"
    with open(jsonPath, "w") as f:
        json.dump(pkt_outputs, f)

    log([f"The packet file is named \"_pkts.v2.json\" at the Feather_Server/Packets directory.", " ! Full Path: " + jsonPath.replace('\\', '/')])