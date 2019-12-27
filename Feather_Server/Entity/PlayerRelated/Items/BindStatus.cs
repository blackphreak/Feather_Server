namespace Feather_Server.PlayerRelated
{
    public enum BindStatus : byte
    {
        /* not sure yet
         * TODO: make sure the status codes are correct.
        EMPTY = 0x0,           // 無綁定狀態
        EQUIP_THEN_BIND = 0x1, // 裝備後綁定
        */

        BINDED = 0x8,   // 已綁定
        FOREVER = 0x9,  // 永久綁定
    }
}