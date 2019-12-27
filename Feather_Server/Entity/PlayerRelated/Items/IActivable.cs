using Feather_Server.PlayerRelated.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity.PlayerRelated.Items
{
    public interface IActivable
    {
        public bool isActive { get; }
    }
}
