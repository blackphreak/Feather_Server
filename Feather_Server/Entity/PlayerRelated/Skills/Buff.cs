using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.PlayerRelated.Skills
{
    public class Buff
    {
        public int buffID = 0x000cd140;
        public ushort duration = 0x60; // unit: seconds

        public Buff(int id, ushort duration)
        {
            this.buffID = id;
            this.duration = duration;
        }
    }
}
