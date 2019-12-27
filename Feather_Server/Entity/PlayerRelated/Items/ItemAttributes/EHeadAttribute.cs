using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity.PlayerRelated.Items.ItemAttributes
{
    /// <summary>
    /// Only use for Binding and Appliable-For attributes.
    /// </summary>
    public enum EHeadAttribute : uint
    {
        #region Binding
        /// <summary>
        /// `N`+C0x00ffff`-C[购买后绑定]`=C
        /// </summary>
        buy_then_bind = 620866,
        /// <summary>
        /// `N`+C0x00ffff`-C[装备后绑定]`=C
        /// </summary>
        equip_then_bind = 620546,
        /// <summary>
        /// `N`+C0x00ffff`-C[使用后绑定]`=C
        /// </summary>
        use_then_bind = 620547,
        /// <summary>
        /// `N`+C0x00ffff`-C[装备已绑定]`=C
        /// </summary>
        equip_binded = 620548,
        /// <summary>
        /// `N`+C0x00ffff`-C[使用已绑定]`=C
        /// </summary>
        use_binded = 620549, // wtf is this? --------------------------------------------------------------
        /// <summary>
        /// `N`+C0x00ffff`-C[获得后绑定]`=C
        /// </summary>
        gain_then_bind = 620550,
        /// <summary>
        /// `N`+C0x00ffff`-C[已绑定]`=C
        /// </summary>
        binded = 620551,
        /// <summary>
        /// `N`+C0xff0000`-C [被封印]`=C
        /// </summary>
        locked = 620552,
        /// <summary>
        /// `N`+C0xff0000`-C [永久绑定]`=C
        /// </summary>
        forever_binded = 620553,
        #endregion

        #region Appliable For
        /// <summary>
        /// 适用於: |1男性`N
        /// </summary>
        appliableFor_boy = 620597,
        /// <summary>
        /// 适用於: |1女性`N
        /// </summary>
        appliableFor_girl = 620598,
        /// <summary>
        /// |1剑客`=C可使用`N
        /// </summary>
        appliableFor_JK = 620599,
        /// <summary>
        /// |1武士`=C可使用`N
        /// </summary>
        appliableFor_WS = 620600,
        /// <summary>
        /// |1天师`=C可使用`N
        /// </summary>
        appliableFor_TS = 620601,
        /// <summary>
        /// |1术士`=C可使用`N
        /// </summary>
        appliableFor_SS = 620602,
        /// <summary>
        /// |1祭师`=C可使用`N
        /// </summary>
        appliableFor_JS = 620603,
        /// <summary>
        /// |1天师 剑客`=C可使用`N
        /// </summary>
        appliableFor_TS_JK = 620604,
        /// <summary>
        /// |1术士 祭师`=C可使用`N
        /// </summary>
        appliableFor_SS_JS = 620605,
        /// <summary>
        /// |1剑客 武士 天师 术士 祭师`=C可使用`N
        /// </summary>
        appliableFor_JK_WS_TS_SS_JS = 620606,
        #endregion
    }
}
