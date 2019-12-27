using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity
{
    public interface IEntity
    {
        public int entityID { get; }

        public ushort locX { get; }
        public ushort locY { get; }

        public void updateLoc(ushort x, ushort y);

        public ushort map { get; }

        public void updateMap(ushort newMap);

        public byte facing { get; }

        public void updateFacing(byte facing);
    }
}
