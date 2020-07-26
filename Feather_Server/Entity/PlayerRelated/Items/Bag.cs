using System.Linq;
using System.Collections.Generic;
using System.Text;
using Feather_Server.Database;
using System.Data.SQLite;
using Newtonsoft.Json;
using Feather_Server.ServerRelated;
using System;

namespace Feather_Server.PlayerRelated
{
    public class Bag
    {
        public byte size = 24; // extend this when extra-bag is used
        public List<uint> items = new List<uint>();

        /// <summary>
        /// Extra Bag avalible slots
        /// </summary>
        public byte[] extraBag = { 00, 00, 00 };

        /// <summary>
        /// Used to save the bag in memory.
        /// </summary>
        [JsonIgnore]
        private Dictionary</*itemUID*/uint, Item> itemDict = new Dictionary<uint, Item>();

        /// <summary>
        /// Used to save the item position in memory.
        /// </summary>
        [JsonIgnore]
        private Dictionary</*pos*/byte, /*itemUID*/uint> itemPos = new Dictionary<byte, uint>();

        public void loadItemsFromDB()
        {
            Console.WriteLine("[!] Bag: " + string.Join(",", items));
            // TODO: load items from db
            if (items.Count == 0)
                return;

            var res = DB2.GetInstance().Select(
                "itemUID, itemInfo",
                "UniqueItem",
                $"itemUID IN ({string.Join(",", items)});"
            );

            if (res == null)
                return;

            // load items to memory
            Item item;
            foreach (var db_item in res)
            {
                item = Item.fromJson(db_item[1]);
                itemDict.Add(uint.Parse(db_item[0]), item);
                itemPos.Add(item.position, item.itemUID);
            }
        }

        public bool addItem(Hero p, Item item)
        {
            // TODO: pick up auto stack
            // TODO: send add-item to backpack packet
            byte pos;
            for (pos = 1; pos < size; pos++)
            {
                // looking for empty pos
                if (!itemPos.ContainsKey(pos))
                    break;
            }
            if (pos < size)
            {
                item.position = pos;
                this.items.Add(item.itemUID);
                this.itemPos.Add(pos, item.itemUID);
                this.itemDict.Add(item.itemUID, item);

                DB2.GetInstance().Insert(
                    "UniqueItem",
                    new Dictionary<string, object>()
                    {
                        { "itemUID", item.itemUID },
                        { "itemInfo", item.toJson() },
                        { "belongsTo", p.heroID },
                    }
                );

                return true;
            }
            else
            {
                // backpack is full.
                // TODO: send cannot pick up packet
                return false;
            }
        }

        public Item getItem(uint itemUID)
        {
            return this.itemDict.GetValueOrDefault(itemUID, null);
        }

        public bool removeItem(uint itemUID)
        {
            // TODO: send reomve-item in backpack packet
            if (!this.itemDict.ContainsKey(itemUID))
                return false;

            this.items.Remove(itemUID);
            var item = this.itemDict[itemUID];
            this.itemPos.Remove(item.position);
            this.itemDict.Remove(item.itemUID);
            return true;
        }

        public List<Item> getAllItems() => this.itemDict.Values.ToList();
    }
}
