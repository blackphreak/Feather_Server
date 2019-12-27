using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity.PlayerRelated.Items.ItemAttributes
{
    public enum EItemParts : uint
    {
        NONE = 0x00097918,

        #region Parts
        /// <summary>
        /// `N[帽子]
        /// </summary>
        parts_hat = 620561,
        /// <summary>
        /// `N[头饰]
        /// </summary>
        parts_head = 620562,
        /// <summary>
        /// `N[饰物]
        /// </summary>
        parts_decoration = 620563,
        /// <summary>
        /// `N[衣服]
        /// </summary>
        parts_cloth = 620564,
        /// <summary>
        /// `N[时装]
        /// </summary>
        parts_style = 620565,
        /// <summary>
        /// `N[戒指]
        /// </summary>
        parts_rings = 620566,
        /// <summary>
        /// `N[武器]
        /// </summary>
        parts_weapon = 620568,
        /// <summary>
        /// `N[鞋子]
        /// </summary>
        parts_boots = 620569,
        #endregion
    }
}
