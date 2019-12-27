
using System.Drawing;

namespace Feather_Server.PlayerRelated.Items
{
    public class WeaponItem : EquippableItem
    {
        public new Color color;

        public WeaponItem(ushort modelID, Color color) : base(modelID, 0x0)
        {
            this.color = color;
        }
    }
}
