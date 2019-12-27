using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity.PlayerRelated.Items
{
    public class ItemAttribute
    {
        public EItemAttribute attr;
        public byte[] descPkt;
        public object[] value;

        public ItemAttribute(EItemAttribute attr, byte[] descPkt, object[] value = null)
        {
            this.attr = attr;
            this.descPkt = descPkt;
            this.value = value;
        }
    }

    public enum EItemAttribute : uint
    {
        // remark:
        // `N: newline
        // ^?: number (int)
        // $?: string ([0] int: string length, [1] string: GBK encoded string)
        // |?: color start
        // `+C0x??????`-C: color start, RGB (web hex color notation)
        // `=C: color end

        /// <summary>
        /// `+C0xffff00`-C已染色:`+C0x00ffff`-C^1`=C`N
        /// </summary>
        dye = 14207,

        #region Formating
        /// ItemID, Enhancement, color1, color2
        /// |3^1 `=C|4+$2`=C
        /// </summary>
        format_1 = 620555,
        /// <summary>
        /// color, ItemID
        /// |1^2`=C
        /// </summary>
        format_2 = 620671,
        #endregion

        #region Item Basic Attribute
        /// <summary>
        /// |1耐力: $2`=C`N
        /// </summary>
        basic_cps = 620611,
        /// <summary>
        /// |1精神: $2`=C`N
        /// </summary>
        basic_spi = 620612,
        /// <summary>
        /// |1力量: $2`=C`N
        /// </summary>
        basic_str = 620613,
        /// <summary>
        /// |1体质: $2`=C`N
        /// </summary>
        basic_con = 620614,
        /// <summary>
        /// |1命中: $2`=C`N
        /// </summary>
        basic_hit = 620615,
        /// <summary>
        /// |1智力: $2`=C`N
        /// </summary>
        basic_dex = 620616,
        /// <summary>
        /// |1回避: $2`=C`N
        /// </summary>
        basic_dodge = 620617,
        /// <summary>
        /// |1物理攻击: $2`=C`N
        /// </summary>
        basic_ap = 620618,
        /// <summary>
        /// |1暴击率: $2%`=C`N
        /// </summary>
        basic_double = 620622,
        /// <summary>
        /// |1法术攻击: $2`=C`N
        /// </summary>
        basic_cp = 620626,
        /// <summary>
        /// |1生命: $2`=C`N
        /// </summary>
        basic_hp = 620638,
        /// <summary>
        /// |1法力: $2`=C`N
        /// </summary>
        basic_mp = 620642,
        /// <summary>
        /// |1附加法攻: $2`=C`N
        /// </summary>
        cpp = 620627,
        /// <summary>
        /// |1附加生命: $2%`=C`N
        /// </summary>
        hprate = 620663,
        /// <summary>
        /// |1附加法力: $2%`=C`N
        /// </summary>
        mprate = 620664,
        /// <summary>
        /// |1附加物理攻击: $2%`=C`N
        /// </summary>
        aprate = 620665,
        /// <summary>
        /// |1附加法术攻击: $2%`=C`N
        /// </summary>
        cprate = 620666,
        /// <summary>
        /// |1附加物理防御: $2%`=C`N
        /// </summary>
        dprate = 620667,
        /// <summary>
        /// |1附加法术防御: $2%`=C`N
        /// </summary>
        pprate = 620668,
        /// <summary>
        /// |1物理防御: $2%`=C`N
        /// </summary>
        dp_percent = 620907,
        /// <summary>
        /// |1法术防御: $2%`=C`N
        /// </summary>
        pp_percent = 620908,
        /// <summary>
        /// 装备要求: |1$2 级`=C`N
        /// </summary>
        level_req = 620872,
        #endregion

        #region Time Limit
        /// <summary>
        /// `N`+C0xffff00`-C[有效时间]：2小时`=C
        /// </summary>
        valid_time = 620869,
        /// <summary>
        /// `N`+C0xff0000`-C[剩余时间]:$1小时$2分$3秒`=C
        /// </summary>
        time_left = 620870,
        /// <summary>
        /// `N`+C0xff0000`-C已过时..`=C
        /// </summary>
        expired = 620871,
        #endregion

        #region Special Effect
        /// <summary>
        /// `+C0xf076fc`-C装备后生命、法力以及物理和`=C`N`+C0xf076fc`-C法术攻击、防御全部提高3％`=C`N
        /// </summary>
        spe_hp_mp_ap_cp_dp_pp_all3_percent = 620575,
        #endregion

        /// <summary>
        /// `+C0xff0000`-C未鑑定`=C`N
        /// </summary>
        //not_ = 620608, // TODO
        /// <summary>
        /// 耐久: $1 / $2`N
        /// </summary>
        durability = 620609,
        /// <summary>
        /// 使用次数: $1 / $2`N
        /// </summary>
        used = 620610,

        /// <summary>
        /// `+C0x00ffff`-C^1 Lv.$2`=C`N
        /// </summary>
        hole_skill_desc = 620669,
        /// <summary>
        /// [4:H$1:U$2]`N
        /// </summary>
        hole_skill_amount = 620670,

        /// <summary>
        /// `+C0x00ff00`-C每5秒回$1点生命`=C`N
        /// </summary>
        every_5sec_re_N_hp = 620672,
        /// <summary>
        /// `+C0x00ff00`-C每5秒回$1点法力`=C`N
        /// </summary>
        every_5sec_re_N_mp = 620673,

        /// <summary>
        /// `+C0xffff00`-C[有效时间]：$1天`=C`N
        /// </summary>
        valid_for_1day = 620687,
        /// <summary>
        /// `+C0xff0000`-C[剩余时间]:$1天$2小时$3分`=C`N
        /// </summary>
        time_left_for = 620688,
        /// <summary>
        /// `+C0xffff00`-C[有效时间]:游戏时间$1小时`=C`N
        /// </summary>
        valid_for_gameplay_time = 620880,
        /// <summary>
        /// `+C0xff0000`-C已过时..`=C`N
        /// </summary>
        //expired2 = 620689,  // duplicated
        /// <summary>
        /// `+C0xffff00`-C[一品装备]`=C`N
        /// </summary>
        quanlity_1p = 620690,
        /// <summary>
        /// `+C0xffff00`-C[二品装备]`=C`N
        /// </summary>
        quanlity_2p = 620691,
        /// <summary>
        /// `+C0xffff00`-C[三品装备]`=C`N
        /// </summary>
        quanlity_3p = 620692,
        /// <summary>
        /// `+C0xffff00`-C[四品装备]`=C`N
        /// </summary>
        quanlity_4p = 620693,
        /// <summary>
        /// `+C0xffff00`-C[五品装备]`=C`N
        /// </summary>
        quanlity_5p = 620694,
        /// <summary>
        /// `+C0xffff00`-C[六品装备]`=C`N
        /// </summary>
        quanlity_6p = 620695,
        /// <summary>
        /// `+C0xffff00`-C[七品装备]`=C`N
        /// </summary>
        quanlity_7p = 620696,
        /// <summary>
        /// `+C0xffff00`-C[八品装备]`=C`N
        /// </summary>
        quanlity_8p = 620697,
        /// <summary>
        /// `+C0xffff00`-C[九品装备]`=C`N
        /// </summary>
        quanlity_9p = 620698,
        /// <summary>
        /// `+C0xffff00`-C[製作者：$1]`=C`N
        /// </summary>
        maker_byID = 620699,
        /// <summary>
        /// `+C0xff0000`-C|1已超过使用时限。`=C`N
        /// </summary>
        already_expired = 620749,
        /// <summary>
        /// |1还有 $2 天的使用期限。`N
        /// </summary>
        time_left_for_2days = 620750,

        /// <summary>
        /// `+C0x00ff00`-C暴击伤害：+$1%`=C`N
        /// </summary>
        double_damage_rate_percent = 620792,

        /// <summary>
        /// `+C0x00ff00`-C法术伤害效果+$1`=C`N
        /// </summary>
        cp_attack_buff = 620674,
        /// <summary>
        /// `+C0x00ff00`-C法术治疗效果+$1`=C`N
        /// </summary>
        cp_heal_buff = 620675,
        /// <summary>
        /// |1输出法術傷害:$2`N
        /// </summary>
        cp_damage = 620794,
        /// <summary>
        /// |1输出傷害:$2%`N
        /// </summary>
        damage_percent = 620795,
        /// <summary>
        /// |1输出物理傷害:$2%`N
        /// </summary>
        ap_damage_percent = 620796,
        /// <summary>
        /// |1输出法術傷害:$2%`N
        /// </summary>
        cp_damage_percent = 620797,
        /// <summary>
        /// |1受到傷害:$2%`N
        /// </summary>
        damage_gain_percent = 620798,
        /// <summary>
        /// |1法术治疗效果:$2%`N
        /// </summary>
        cp_heal_buff_percent = 620799,
        /// <summary>
        /// 装备要求: |1$2 级`=C`N
        /// </summary>
        //level_requ = 620800, // duplicated

        /// <summary>
        /// `+C0xffff00`-C[有效时间]:$1天`=C`N
        /// </summary>
        valid_for_N_days = 620801,

        /// <summary>
        /// `N`+C0xffff00`-C[製作者：无名氏]`=C`N
        /// </summary>
        maker_unknown = 620844,
        /// <summary>
        /// `N`+C0xffff00`-C[製作者：^1]`=C`N
        /// </summary>
        maker_byName = 620845,

        #region Enhancement
        /// <summary>
        /// [精炼] 生命`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_hp = 620848,
        /// <summary>
        /// [精炼]  生命+$1`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_hp_plus = 620849,
        /// <summary>
        /// [精炼] 物防`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_dp = 620850,
        /// <summary>
        /// [精炼]  物防+$1`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_dp_plus = 620851,
        /// <summary>
        /// [精炼] 法力`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_mp = 620852,
        /// <summary>
        /// [精炼]  法力+$1`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_mp_plus = 620853,
        /// <summary>
        /// [精炼] 物攻`N[适用装备] 武器、项链
        /// </summary>
        enhancement_ap = 620854,
        /// <summary>
        /// [精炼]  物攻+$1`N[适用装备] 武器、项链
        /// </summary>
        enhancement_ap_plus = 620855,
        /// <summary>
        /// [精炼] 法攻`N[适用装备] 武器、项链
        /// </summary>
        enhancement_cp = 620856,
        /// <summary>
        /// [精炼]  法攻+$1`N[适用装备] 武器、项链
        /// </summary>
        enhancement_cp_plus = 620857,
        /// <summary>
        /// [精炼] 法防`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_pp = 620858,
        /// <summary>
        /// [精炼]  法防+$1`N[适用装备] 防具、项链、鞋子、`N头饰、背饰、面具、尾巴
        /// </summary>
        enhancement_pp_plus = 620859,
        /// <summary>
        /// [精炼] 回避`N[适用装备] 项链、鞋子、背饰、尾巴
        /// </summary>
        enhancement_dodge = 620860,
        /// <summary>
        /// [精炼]  回避+$1`N[适用装备] 项链、鞋子、背饰、尾巴
        /// </summary>
        enhancement_dodge_plus = 620861,
        /// <summary>
        /// [精炼] 命中`N[适用装备] 武器、项链、戒指、背饰、尾巴
        /// </summary>
        enhancement_hit = 620862,
        /// <summary>
        /// [精炼]  命中+$1`N[适用装备] 武器、项链、戒指、`N背饰、尾巴
        /// </summary>
        enhancement_hit_plus = 620863,
        /// <summary>
        /// [精炼] 爆击`N[适用装备] 武器、背饰、尾巴
        /// </summary>
        enhancement_double = 620864,
        /// <summary>
        /// [精炼]  爆击+$1.$2％`N[适用装备] 武器、背饰、尾巴
        /// </summary>
        enhancement_double_plus = 620865,
        #endregion

        /// <summary>
        /// `N当前还可以传送`+C0xffff00`-C$1次`=C
        /// </summary>
        teleport_N_times = 620868,

        /// <summary>
        /// `N`+C0x00ff00`-C目前已捕捉了$1个^2。`N`+C0x00ff00`-C该宠物每次可生产$3个|4。
        /// </summary>
        captured_N_produce_N = 620874,
        /// <summary>
        /// 水元素
        /// </summary>
        element_water = 620875,
        /// <summary>
        /// 火元素
        /// </summary>
        element_fire = 620876,
        /// <summary>
        /// 土元素
        /// </summary>
        element_dirt = 620877,
        /// <summary>
        /// 金元素
        /// </summary>
        element_gold = 620878,
        /// <summary>
        /// 风元素
        /// </summary>
        element_winds = 620879,
    }
}
