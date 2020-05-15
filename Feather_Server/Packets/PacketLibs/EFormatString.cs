using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.PacketLibs
{
    public enum EFormatString : int
    {
        #region Cate: Warning
        ///<summary>Incorrect Username / Password!</summary>
        INCORRECT_USERNAME_PASSWORD = 0x6AD876,
        #endregion

        #region Cate: Template
        ///<summary>$1`N|2</summary>
        TEMPLATE_LOGIN_NAME = 0x0F455E,
        #endregion
    }
}
