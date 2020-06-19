using System;
using System.Collections.Generic;
using System.Text;

namespace FeatherServer
{
    public enum Map : ushort
    {
        #region Map 101 ~ 112 (Lv.0 ~ Lv.19)
        /// <summary>
        /// 鲤鱼村
        /// </summary>
        CarpVillage = 101,
        /// <summary>
        /// 青石滩
        /// </summary>
        BluestoneBeach = 102,
        /// <summary>
        /// 杏林
        /// </summary>
        ApricotForest = 103,
        /// <summary>
        /// 天羽城
        /// </summary>
        GoldenPlumeCity = 104,
        /// <summary>
        /// 桑林
        /// </summary>
        MulberryForest = 105,
        /// <summary>
        /// 焦木村
        /// </summary>
        CharredVillage = 106,
        /// <summary>
        /// 白露坡
        /// </summary>
        WhiteDewSlope = 107,
        /// <summary>
        /// 细雨林
        /// </summary>
        DrizzleForest = 108,
        /// <summary>
        /// 白羊坡
        /// </summary>
        AriesSlope = 109,
        /// <summary>
        /// 清水溪
        /// </summary>
        SpringBrook = 110,
        /// <summary>
        /// 仙羽湖 &lt;-- TBC
        /// </summary>
        LakeOfFairyFeather = 111,
        /// <summary>
        /// 选手休息区 &lt;-- TBC
        /// </summary>
        RestArea = 112,
        #endregion

        #region Map 201 ~ 206 (Lv.20 ~ 29)
        EagleRidge = 201,
        SteleForest = 202,
        /// <summary>
        /// 苇林庄
        /// </summary>
        ReedVillage = 203,
        RedrockHill = 204,
        CloudRidge = 205,
        CougarRidge = 206,
        #endregion

        #region Map 301 ~ 306 (Lv.30 ~ 39)
        /// <summary>
        /// 常殷村
        /// </summary>
        SunshineVillage = 301,
        /// <summary>
        /// 残阳古道
        /// </summary>
        SunsetPath = 302,
        /// <summary>
        /// 
        /// </summary>
        PebbleBeach = 303,
        /// <summary>
        /// 琅琊驿道
        /// </summary>
        PathofPearl = 304,
        /// <summary>
        /// 枯木林
        /// </summary>
        WitheredForest = 305,
        /// <summary>
        /// 戈壁滩
        /// </summary>
        GobiBeach = 306,
        #endregion

        #region Map 401 ~ 407 (Lv.40 ~ 49)
        /// <summary>
        /// 流沙古城
        /// </summary>
        QuicksandCastle = 401,
        /// <summary>
        /// 一线丘
        /// </summary>
        SeamDune = 402,
        /// <summary>
        /// 幻洲
        /// </summary>
        OasisofIllusion = 403,
        /// <summary>
        /// 浪沙丘
        /// </summary>
        SandWave = 404,
        /// <summary>
        /// 困龙关
        /// </summary>
        DragonPass = 405,
        /// <summary>
        /// 淘石岗
        /// </summary>
        StoneHill = 406,
        /// <summary>
        /// 凄凉荒漠
        /// </summary>
        ChillDesert = 407,
        #endregion

        #region Map 501 ~ 513 (Lv.50 ~ 69)
        /// <summary>
        /// 寻龙涧
        /// </summary>
        DragonStream = 501,
        /// <summary>
        /// 鹰林
        /// </summary>
        HawkForest = 502,
        /// <summary>
        /// 断魂崖
        /// </summary>
        LostsoulCliff = 503,
        /// <summary>
        /// 凌皇古墓一层
        /// </summary>
        RoyalMausoleum_L1 = 504,
        /// <summary>
        /// 凌皇古墓二层
        /// </summary>
        RoyalMausoleum_L2 = 505,
        /// <summary>
        /// 凌皇古墓三层
        /// </summary>
        RoyalMausoleum_L3 = 506,
        /// <summary>
        /// 还魂洞一层
        /// </summary>
        RevivalCave_L1 = 507,
        /// <summary>
        /// 还魂洞二层
        /// </summary>
        RevivalCave_L2 = 508,
        /// <summary>
        /// 还魂洞三层
        /// </summary>
        RevivalCave_L3 = 509,
        /// <summary>
        /// 苍桐谷
        /// </summary>
        DarkTungValley = 510,
        /// <summary>
        /// 望月崖
        /// </summary>
        PleniluneCliff = 511,
        /// <summary>
        /// 斩龙峡
        /// </summary>
        DeadDragonGorge = 512,
        /// <summary>
        /// 落日城
        /// </summary>
        SunsetCity = 513,
        #endregion

        #region Map 601 ~ 609 (Lv.70 ~ 89)
        /// <summary>
        /// 黄藤泽
        /// </summary>
        WalnutSwamp = 601,
        /// <summary>
        /// 青龙泽
        /// </summary>
        BlueDragonSwamp = 602,
        /// <summary>
        /// 天坑
        /// </summary>
        SkyPit = 603,
        /// <summary>
        /// 无水涧
        /// </summary>
        DryedGully = 604,
        /// <summary>
        /// 平湖滩
        /// </summary>
        PeaceLakeBeach = 605,
        /// <summary>
        /// 祭天湖
        /// </summary>
        SkyLake = 606,
        /// <summary>
        /// 泪湖
        /// </summary>
        LakeOfTears = 607,
        /// <summary>
        /// 断情滩
        /// </summary>
        LoveBeach = 608,
        /// <summary>
        /// 沼泽矿脉
        /// </summary>
        TBC = 609,
        #endregion

        #region Map 701 ~ 708 (Lv.90 ~ 109)
        /// <summary>
        /// 望日崖
        /// </summary>
        SunviewCliff = 701,
        /// <summary>
        /// 射日崖
        /// </summary>
        SundownCliff = 702,
        /// <summary>
        /// 九羽峰
        /// </summary>
        NinePlumePeak = 703,
        /// <summary>
        /// 千羽谷
        /// </summary>
        PlumeValley = 704,
        /// <summary>
        /// 藏仙洞一层
        /// </summary>
        ImmortalCave_L1 = 705,
        /// <summary>
        /// 藏仙洞二层
        /// </summary>
        ImmortalCave_L2 = 706,
        /// <summary>
        /// 藏弓洞一层
        /// </summary>
        ArcherCave_L1 = 707,
        /// <summary>
        /// 藏弓洞二层
        /// </summary>
        ArcherCave_L2 = 708,
        #endregion

        #region Map 801 ~ 808 (Lv.110 ~ 129)
        /// <summary>
        /// 上古战场
        /// </summary>
        AncientBattlefield = 801,
        /// <summary>
        /// 远古废墟
        /// </summary>
        AncientWasteland = 802,
        /// <summary>
        /// 洪荒遗迹
        /// </summary>
        VestigeofFloods = 803,
        /// <summary>
        /// 天鹰海岸
        /// </summary>
        EagleBeach = 804,
        /// <summary>
        /// 乌金秘道一层
        /// </summary>
        DarkGoldPath_L1 = 805,
        /// <summary>
        /// 乌金秘道二层
        /// </summary>
        DarkGoldPath_L2 = 806,
        /// <summary>
        /// 乌金矿洞一层
        /// </summary>
        DarkGoldMine_L1 = 807,
        /// <summary>
        /// 乌金矿洞二层
        /// </summary>
        DarkGoldMine_L2 = 808,
        #endregion

        #region Map 901 ~ 912 (Lv.130 ~ 150)
        /// <summary>
        /// 仙霞宫一层
        /// </summary>
        FairyPalace_L1 = 901,
        /// <summary>
        /// 仙霞宫二层
        /// </summary>
        FairyPalace_L2 = 902,
        /// <summary>
        /// 玉宇阁一层
        /// </summary>
        JadePavilion_L1 = 903,
        /// <summary>
        /// 玉宇阁二层
        /// </summary>
        JadePavilion_L2 = 904,
        /// <summary>
        /// 寒川山一层
        /// </summary>
        ChillPlain_L1 = 905,
        /// <summary>
        /// 寒川山二层
        /// </summary>
        ChillPlain_L2 = 906,
        /// <summary>
        /// 冰原殿一层
        /// </summary>
        IceHall_L1 = 907,
        /// <summary>
        /// 冰原殿二层
        /// </summary>
        IceHall_L2 = 908,
        /// <summary>
        /// 雪恩岛
        /// </summary>
        SnowflakeIsland = 909,
        /// <summary>
        /// 波比雪林
        /// </summary>
        BobiForest = 910,
        /// <summary>
        /// 好运岛
        /// </summary>
        FortuneIsland = 911,
        /// <summary>
        /// 圣诞村
        /// </summary>
        ChristmasTown = 912,
        #endregion

        #region Special Map
        /// <summary>
        /// 天羽比武场 &lt;-- TBC
        /// </summary>
        FightMap = 990,
        /// <summary>
        /// 王者热身赛场 &lt;-- TBC
        /// </summary>
        WarmUpMap = 991,
        #endregion
    }
}
