using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.PacketLibs
{
    public enum EFormatString : int
    {
        #region Cate: Warning
        /// <summary>Incorrect Username / Password! (账号密码错误)</summary>
        INCORRECT_USERNAME_PASSWORD = 0x6AD876,
        /// <summary>Same Charactor Name Exists! (已经有相同的角色名。)</summary>
        EXACT_NAME_EXISTS = 0x0F2B,
        /// <summary>
        /// Format_CHS: 您上次是 $1 从 $2 离线。`N
        /// <br />
        /// Format_ENG: You disconnected from $2 at $1
        /// <para>
        /// 1: DataTime, 2: IP
        /// </para>
        /// </summary>
        LAST_LOGIN_MESSAGE = 0x0F26,

        #endregion

        #region Cate: Template
        /// <summary>
        /// Only for FormatString with direct GBK
        /// <br />
        /// Not exists in FormatStringDB.
        /// </summary>
        TEMPLATE_DIRECT_GBK = -1,

        /// <summary>
        /// Format: $1`N|2
        /// <para>
        /// $1=gbk, |2=ID
        /// </para>
        /// </summary>
        TEMPLATE_LOGIN_NAME = 0x0F455E,

        /// <summary>
        /// Format: ^1
        /// <para>
        /// 1=ID
        /// </para>
        /// </summary>
        TEMPLATE_ENTITY_NAME_ID_NAME1 = 0x10CB6A,

        /// <summary>
        /// Format: @1||||^1
        /// <para>
        /// 1: ID
        /// </para>
        /// <example>
        /// Example_CHS: 居民丁 李四 <br />
        /// Example_ENG: 
        /// </example>
        /// </summary>
        TEMPLATE_ENTITY_NAME_ID_NAME2 = 0x0F4266,

        /// <summary>
        /// Format: ^1($2)
        /// <para>
        /// 1: NPCID, 2: gbk
        /// </para>
        /// <example>
        /// Example_CHS: 寵物 ID 名(寵物主人名) <br />
        /// Example_ENG: PetID Name(Pet Owner Name)
        /// </example>
        /// </summary>
        TEMPLATE_ENTITY_NAME_ID_GBK = 0x0AAEB5,

        /// <summary>
        /// Format: $1($2)
        /// <para>
        /// 1: gbk, 2: gbk
        /// </para>
        /// </summary>
        TEMPLATE_ENTITY_NAME_GBK_GBK = 0x0AAEB6,

        /// <summary>
        /// Format_CHS: ^1($2级)
        /// <br />
        /// Format_ENG: ^1(Lv.$2)
        /// <para>
        /// 1: NPCID, 2: int
        /// </para>
        /// </summary>
        TEMPLATE_ENTITY_NAME_ID_LV = 0x0AB091,

        /// <summary>
        /// Format: ^1|2
        /// <para>
        /// 1: NPCID, 2: FSID
        /// </para>
        /// </summary>
        TEMPLATE_ENTITY_NAME_ID_FSID = 0x0F426C,
        #endregion
    }
}
