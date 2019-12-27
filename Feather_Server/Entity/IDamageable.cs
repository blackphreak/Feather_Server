using Feather_Server.MobRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity
{
    public interface IDamageable
    {
        public void makeDamage(ILivingEntity damagedBy, bool ignoreDefense);
    }
}
