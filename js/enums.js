//#region Enum
var eAniLayer = (num) => {
    if (num == 0) return "Over Title";
    if (num == 1) return "Under Foot";
    if (num == 2) return "Over Body";
    if (num == 3) return "Top-Most";
    return "-Not Yet Defined-";
}

var eAniMode = (num) => {
    if (num == 0) return "Stop (Hide)";
    if (num == 1) return "Once (Timed)";
    if (num == 2) return "Loop (Infinitely)";
}

var eQualityColor = (q) => {
    let color = "#FFFFFF"; // q == 0 || default
    if (q == 1) color = "#00FFFF";
    if (q == 2) color = "#FFFF00";
    if (q == 3) color = "#F775FF";
    if (q == 4) color = "#00FF00";
    if (q == 5) color = "--";
    if (q == 6) color = "--";
    if (q == 7) color = "--";
    if (q == 8) color = "--";
    if (q == 9) color = "--";

    return `<span style="color:${color};">${color}</span>`;
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
    if (hex == 0x01) return "World"; // 世界
    if (hex == 0x02) return "Rumor"; // 謠言
    if (hex == 0x03) return "City"; // 城市
    if (hex == 0x04) return "Tribe"; // 部落
    if (hex == 0x05) return "Role"; // 職業
    if (hex == 0x06) return "Near"; // 當前
    if (hex == 0x07) return "Team"; // 隊伍
    if (hex == 0x08) return "School"; // 校園
    if (hex == 0x0a) return "System"; // 系統
    if (hex == 0x0b) return "- Headerless -"; // - 沒有 header -
    if (hex == 0x10) return "Near"; // near (special)
    if (hex == 0x11) return "Annoncement (Top)";
};

var eMap = (mapCode) => (mapDB[mapCode] && mapDB[mapCode].name) || "-- Unk --";

var eBagSlot = (inp) => {
    var out = [];
    (inp + "")
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
var eAnimationLayer = (inp) => (inp <= 1 ? "Front" : "Back - " + inp);
var eAnimationLoop = (inp) => (inp <= 1 ? "Once" : "Repeat - " + inp);

var eFacing = (gbk) => {
    let hex = parseInt(gbk, 16);
    if ((hex >= 0) & (hex <= 8)) return `<div style="transform: rotate(${hex * 45}deg);display: inline-block;">↙</div>`;

    if (hex < 0x30 || hex > 0x38)
        // 0 ~ 8
        return `<div class="bg-err">Invalid Facing</div>`;

    return `<div style="transform: rotate(${parseInt(lib.parseGBK(gbk)) * 45}deg);display: inline-block;">↙</div>`;
};

var eDropNameColor = (hex) => {
    if (hex > 10)
        return eRGB565(hex);
    
    if (hex == 0) return "white";
    if (hex == 1) return "blue";
    if (hex == 2) return "yellow";
    if (hex == 3) return "purple";
    if (hex == 4) return "green";
    if (hex == 5) return "red?"; // TODO: confirm

    return "--dont know yet--";
}

var eRGB565 = (hex) => {
    // TODO: color matching :'(
    let r = (hex & 0xf800) << 8,
        g = (hex & 0x7e0) << 5,
        b = (hex & 0x1f) << 3,
        c = (r + g + b).toString(16).padStart(6, "0");

    if (c == "000000") return "- None -";

    return `<div style="height: 19.4px; width: 60px; background: #${c};"></div>`;
};

var eRGB = (hex) => {
    return `<div style="height: 19.4px; width: 60px; background: #${hex};"></div>`;
}

var eHPBar = (int) => {
    return int * 2 + "%";
};

var eAnimation = (hex) => {
    if (hex == 0x0c5c10) desc = `On Fire Effect (Skill Victim)`;
    else if (hex == 0x0c5c11) desc = `Slow Down Effect (Skill Victim / Prop Victim)`;
    else if (hex == 0x0c5c12) desc = `?`;
    else if (hex == 0x0c5c13) desc = `Posioning (Eat / De-Buff)`;
    else if (hex == 0x0c5c14) desc = `Weak (Reduced PD) (Skill Victim)`;
    else if (hex == 0x0c5c15) desc = `?`;
    else if (hex == 0x0c5c16) desc = `Ice Frozen (Skill Victim)`;
    else if (hex == 0x0c5c17) desc = `Disable All Skills (Skill Victim / Boss Skill Victim)`;
    else if (hex == 0x0c5c18) desc = `DizzyA (Skill?)`;
    else if (hex == 0x0c5c19) desc = `DizzyB (Skill?)`;
    else if (hex == 0x0c5c1a) desc = `Spawn Animation`;
    else if (hex == 0x0c5c1b) desc = `?`;
    else if (hex == 0x0c5c1c) desc = `Star Effect (When failed in star_make?)`;
    else if (hex == 0x0c5c1d) desc = `?`;
    else if (hex == 0x0c5c1e) desc = `?`;
    else if (hex == 0x0c5c1f) desc = `?`;
    else if (hex == 0x0c5c20) desc = `Healing (Hero Group Heal Skill Target [Player & Pet] / Heal Skill Target)`;
    else if (hex == 0x0c5c21) desc = `Level Up Animation`;
    else if (hex == 0x0c5c22) desc = `Health Recovery Effect (Prop: HP Drug / HP Recovery Buff)`;
    else if (hex == 0x0c5c23) desc = `Magic Point Recovery Effect (Prop: MP Drug / MP Recovery Buff)`;
    else if (hex == 0x0c5c24) desc = `?`;
    else if (hex == 0x0c5c28) desc = `製作裝備鑑定道具A (Small)`;
    else if (hex == 0x0c5c29) desc = `製作裝備鑑定道具B (Small)`;
    else if (hex == 0x0c5c2a) desc = `製作裝備鑑定道具A (Large)`;
    else if (hex == 0x0c5c2b) desc = `製作裝備鑑定道具B (Large)`;
    else if (hex == 0x0c5c2c) desc = `Invincible (Hero Skill - Self Only / Boss Skill - Self Only)`;
    else if (hex == 0x0c5c2d) desc = `Magicial Shield (Hero Skill - Self Only)`;
    else if (hex == 0x0c5c2e) desc = `?`;
    else if (hex == 0x0c5c2f) desc = `Pre-Fire Ball Skill Effect (Hero Skill - During Skill Pending)`;
    else if (hex == 0x0c5c30) desc = `Hearts (Prop Self - Love Fireworks / Ride Spawn - Self Only)`;
    else if (hex == 0x0c5c31) desc = `Lucky Star (Prop Self - Use a lucky star item)`;
    else if (hex == 0x0ca6b7) desc = `Team Leader`;
    else return db_Skill(hex) || `- Not Exists / Unknown -`;

    return `<img src="../img/ani_0x${hex.toString(16).padStart(6, "0")}.png" title="${desc}" />`;
};

var eRideModel = (hex, color) => {
    let ride = undefined;

    if (hex == 0x0001 && !color)
        ride = "雪灵鹿初代 contract: 101224";
    else if (hex == 0x0001)
        ride = "蓝灵初代 contract: 101288";
    else if (hex == 0x0002 && !color)
        ride = " contract: 101226";
    else if (hex == 0x0002)
        ride = "紫电初代 contract: 101289";
    else if (hex == 0x0003)
        ride = " contract: 101230";
    else if (hex == 0x0004 && !color)
        ride = " contract: 101227";
    else if (hex == 0x0004)
        ride = "翠鳞初代 contract: 101287";
    else if (hex == 0x0005 && !color)
        ride = " contract: 101225";
    else if (hex == 0x0005)
        ride = "赤焰初代 contract: 101285";
    else if (hex == 0x0006)
        ride = " contract: 101229";
    else if (hex == 0x0007 && !color)
        ride = " contract: 101228";
    else if (hex == 0x0007)
        ride = "金刚初代 contract: 101286";
    else if (hex == 0x000D)
        ride = "骆马契约 contract: 101231";
    else if (hex == 0x000E)
        ride = "爬爬虫召唤石 contract: 101234";
    else if (hex == 0x000F)
        ride = "轻风契约 contract: 101235";
    else if (hex == 0x0011)
        ride = "欢乐圣诞座驾 / 欢乐圣诞座驾（3天）contract: 101201/101300";
    else if (hex == 0x0012)
        ride = "年兽契约（1天） contract: 101202";
    else if (hex == 0x0013)
        ride = "爱之犬契约（1天） contract: 101203";
    else if (hex == 0x0014)
        ride = "元宵车契约（1天） contract: 101204";
    else if (hex == 0x0015)
        ride = "旋风神驱契约 contract: 101200";
    else if (hex == 0x002C)
        ride = "潜水鲸鱼初代 contract: 101223";
    else if (hex == 0x001A && !color)
        ride = "凤尾狐一代 contract: 101205";
    else if (hex == 0x001A)
        ride = "紫电一代 contract: 101294";
    else if (hex == 0x001C && !color)
        ride = "焰之契约 contract: 101207";
    else if (hex == 0x001C)
        ride = "赤焰之魂 contract: 101290";
    else if (hex == 0x001E && !color)
        ride = "力之契约 contract: 101209";
    else if (hex == 0x001E)
        ride = "金刚之魂 contract: 101291";
    else if (hex == 0x0020 && !color)
        ride = "地之契约 contract: 101211";
    else if (hex == 0x0020)
        ride = "翠鳞之魂 contract: 101292";
    else if (hex == 0x0022)
        ride = " contract: 101213";
    else if (hex == 0x0024)
        ride = " contract: 101215";
    else if (hex == 0x0026 && !color)
        ride = "雪灵鹿一代 contract: 101217";
    else if (hex == 0x0026)
        ride = "蓝灵一代 contract: 101293";
    else if (hex == 0x0028)
        ride = "轻风飞毯一代 contract: 101219";
    else if (hex == 0x002A)
        ride = "爬爬虫一代 contract: 101221";
    else if (hex == 0x0010)
        ride = "潜水鲸鱼一代 contract: 101236";
    else if (hex == 0x001B && !color)
        ride = "岚之契约 contract: 101206";
    else if (hex == 0x001B)
        ride = "紫电之魂 contract: 101299";
    else if (hex == 0x001D && !color)
        ride = "焰之契约 contract: 101208";
    else if (hex == 0x001D)
        ride = "赤焰之魂 contract: 101295";
    else if (hex == 0x001F && !color)
        ride = "力之契约 contract: 101210";
    else if (hex == 0x001F)
        ride = "金刚之魂 contract: 101296";
    else if (hex == 0x0021 && !color)
        ride = "地之契约 contract: 101212";
    else if (hex == 0x0021)
        ride = "翠鳞之魂 contract: 101297";
    else if (hex == 0x0023)
        ride = " contract: 101214";
    else if (hex == 0x0025)
        ride = " contract: 101216";
    else if (hex == 0x0027 && !color)
        ride = "雪灵鹿二代 contract: 101218";
    else if (hex == 0x0027)
        ride = "蓝灵二代 contract: 101298";
    else if (hex == 0x0029)
        ride = "轻风飞毯二代 contract: 101220";
    else if (hex == 0x002B)
        ride = "爬爬虫二代 contract: 101222";
    else if (hex == 0x0037)
        ride = "潜水鲸鱼二代 contract: 101237";
    else
        return "-- Unknwon Ride Model --";
    
    return ride;
}

var eBool = (int, [t, f]) => {
    return int == 1 ? t : f;
};

var eTri = (int, o) => {
    return o[int] || o[o.length - 1];
};

var eIf = (int, param) => {
    var i = 0;
    for (let j = 0; j < param.length / 2; i += 2, j++) {
        // loop for each condition group, last of param is always the "else" value.
        let condition = param[i],
            val = param[i + 1];

        if (eval(`${int}` + condition)) return val;
    }
    return param[param.length - 1]; // default value
};

var eItemType = (int) => {
    // TODO: item type
};

var eCHRate = (int) => {
    return int.toString().slice(0, -2) + "." + int.toString().slice(-2) + "%";
};

var db_FormatString = (inp, cols = ["title"]) => {
    let db = formatstringDB[inp] || false;
    if (!db) return ""; // "-- FSDB Fail --";

    let data = [];
    cols.forEach((col) => {
        data.push(db[col] || "ERR-COL");
    });
    return data.join(" - ");
};
var db_Item = (inp, cols = ["name"]) => {
    let db = itemDB[inp] || false;
    if (!db) return ""; // "-- ItemDB Fail --";

    let data = [];
    cols.forEach((col) => {
        data.push(db[col] || "ERR-COL");
    });
    return data.join(" - ");
};
var db_NPC = (inp, cols = ["name"]) => {
    let db = npcDB[inp] || false;
    if (!db) return ""; // "-- NPCDB Fail --";

    let data = [];
    cols.forEach((col) => {
        data.push(db[col] || "ERR-COL");
    });
    return data.join(" - ");
};
var db_Map = (inp, cols = ["name"]) => {
    let db = mapDB[inp] || false;
    if (!db) return ""; // "-- MapDB Fail --";

    let data = [];
    cols.forEach((col) => {
        data.push(db[col] || "ERR-COL");
    });
    return data.join(" - ");
};

var db_Skill = (inp, cols = ["name"]) => {
    let db = skillDB[inp] || false;
    if (!db) return ""; // "-- SkillDB Fail --";

    let data = [];
    cols.forEach((col) => {
        data.push(db[col] || "ERR-COL");
    });
    return data.join(" - ");
};

//#endregion Enums
