using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.PlayerRelated.Model
{
    public class Effect
    {
        public byte effectID = 0x70;
        public byte animationDuration = 0x04;
        public byte frontOrBack = 0x02;
        public byte onceOrLoop = 0x02;

        public Effect(byte effectID, byte duration, byte frontOrBack, byte onceOrLoop)
        {
            this.effectID = effectID;
            this.animationDuration = duration;
            this.frontOrBack = frontOrBack;
            this.onceOrLoop = onceOrLoop;
        }

        public string toHex()
        {
            return Lib.toHex(effectID)
                + "d10c00"
                + Lib.toHex(animationDuration)
                + Lib.toHex(frontOrBack)
                + Lib.toHex(onceOrLoop);
        }
    }
}
