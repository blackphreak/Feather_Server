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
        /// <summary>You disconnected from &lt;[$2]IP&gt; at &lt;[$1]DateTime&gt;. (您上次是 $1 从 $2 离线。`N)</summary>
        LAST_LOGIN_MESSAGE = 0x0F26,

        #endregion

        #region Cate: Template
        ///<summary>$1`N|2</summary>
        TEMPLATE_LOGIN_NAME = 0x0F455E,
        #endregion
    }
}
