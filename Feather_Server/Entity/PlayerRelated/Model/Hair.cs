namespace Feather_Server.ServerRelated
{
    public class Hair
    {
        public ushort icon  = 0x0003; // girls: > 1000
        public ushort model = 0x0001; // girls: > 1000
        public ushort color = 0x80e7;

        public Hair(ushort icon, ushort model, ushort color)
        {
            this.icon = icon;
            this.model = model;
            this.color = color;
        }
    }
}