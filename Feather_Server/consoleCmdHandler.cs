using Feather_Server.Database;
using Feather_Server.Entity.PlayerRelated.Items;
using Feather_Server.Entity.PlayerRelated.Items.Activable;
using Feather_Server.Entity.PlayerRelated.Items.ItemAttributes;
using Feather_Server.PlayerRelated;
using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Feather_Server
{
    public class consoleCmdHandler
    {
        public static void handle(string cmd)
        {
            var inp = cmd.Split(" ");

            switch (inp[0].ToLower())
            {
                case "e":
                case "end":
                case "exit":
                    Lib.endSrv = true;
                    Console.WriteLine("[!] Killing Server ...");
                    break;
                case "snpc":
                    //Lib.broadcast(PacketEncoder.spawnNPC(int.Parse(inp[1]), new Location(45, 45, Map.TYC)));
                    break;
                case "send_raw_pkt":
                case "srp":
                    Console.WriteLine("[.] Input Raw Packet Mode:");
                    Console.WriteLine("    -- Type \"erp\" to exit this mode --");
                    Console.WriteLine("    -- Type \"x\" to clear the current send buffer --");
                    Console.WriteLine("    -- Type \"show\" to show the current send buffer --");
                    Console.WriteLine("    -- Press ENTER or \"s\" after packet to send the packets --");
                    Console.Write("\t[+] Please type the HeroID to send to. (-1 is broadcast)\n\tHeroID: ");

                    int heroID;
                    if (!int.TryParse(Console.ReadLine(), out heroID))
                    {
                        Console.WriteLine($"\t[!] Failed to parse heroID to integer. Fallback to broadcast mode.");
                        heroID = -1;
                    }

                    if (!Lib.clientList.ContainsKey(heroID))
                    {
                        Console.WriteLine($"\t[!] HeroID[{heroID}] is not connected. Fallback to broadcast mode.");
                        heroID = -1;
                    }

                    Console.WriteLine("\t[.] Enter the pkt.\n");
                    string pkt;
                    List<byte[]> fullPkt = new List<byte[]>();
                    do
                    {
                        Console.Write("\t +-> ");
                        pkt = Console.ReadLine();
                        if (pkt == "" || pkt == "s")
                        {
                            if (heroID == -1)
                                Lib.broadcast(fullPkt.SelectMany(x => x).ToArray());
                            else
                            {
                                Client c;
                                if (Lib.clientList.TryGetValue(heroID, out c))
                                    c.send(fullPkt.SelectMany(x => x).ToArray());
                                else
                                    Console.WriteLine($"\t[-] HeroID[{heroID}] not found. No packet sent.");
                            }

                        }
                        else if (pkt[0..1] == "r")
                        {
                            if (pkt.Length < 3)
                            {
                                continue;
                            }
                            byte idx = byte.Parse(pkt.Split(" ")[1]);
                            fullPkt.RemoveAt(idx);
                        }
                        else if (pkt == "x")
                        {
                            fullPkt = new List<byte[]>();
                            Console.WriteLine($"\t[!] Cleared.\n");
                        }
                        else if (pkt == "show" || pkt == "sh" || pkt == "q")
                        {
                            Console.WriteLine(Lib.pktParser(
                                Lib.toHex(fullPkt.SelectMany(x => x).ToArray())
                            ));
                        }
                        else if (pkt == "erp")
                            break;
                        else if (pkt.Contains("x"))
                        {
                            Console.WriteLine("\t[!] Failed to add pkt. Since it contains \"x\"\n");
                            continue;
                        }
                        else if (pkt != "")
                        {
                            byte[] tmpPkt = new byte[0];
                            try
                            {
                                PacketEncoder.concatPacket(Lib.hexToBytes(
                                    pkt.Replace("{hid}", Lib.toHex(heroID == -1 ? 0x78563412 : heroID)).Replace("_", "")
                                ), ref tmpPkt);
                                fullPkt.Add(tmpPkt);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"\t[!] Failed to parse pkt. Error[{e.Message}]");
                            }
                        }

                    } while (true);

                    Console.WriteLine("[.] Exited Raw Packet Mode.");
                    break;
                case "give":
                    if (inp[1] == "item")
                    {
                        int targetID = int.Parse(inp[2]);
                        uint itemID = uint.Parse(inp[3]);
                        uint baseID = 0;

                        if (inp.Length == 5)
                            baseID = uint.Parse(inp[4]);

                        var cli = Lib.clientList.GetValueOrDefault(targetID, null);

                        if (cli == null)
                        {
                            Console.WriteLine($"[X] Failed to find target[{targetID}]");
                            return;
                        }

                        Item item = null;
                        if (itemID >= 124000 && itemID <= 124019)
                        {
                            item = new RideWing();
                            ((RideWing)item).wingsID = 0x0841;
                            ((RideWing)item).wingsLv = 0x000b;
                        }
                        else if (baseID >= 101035 && baseID <= 101039 || baseID == 101057)
                        {
                            item = new RideContract();
                        }

                        item ??= new Item();
                        item.itemUID = Lib.lastItemUID++;
                        item.baseID = baseID == 0 ? itemID : baseID;
                        item.itemID = itemID;
                        item.stack = 1;
                        item.quality = 4;

                        item.headAttributes.Add(EHeadAttribute.forever_binded);

                        item.itemAttributes.Add(new ItemAttribute(
                            EItemAttribute.durability,
                            Lib.hexToBytes(
                                "64 78000000 64 ff000000 00"
                            ),
                            new object[] { 0x78, 0xFF }
                        ));
                        item.itemAttributes.Add(new ItemAttribute(
                            EItemAttribute.hole_skill_desc,
                            Lib.hexToBytes(
                                "64 91E10800 64 03000000 00"
                            ),
                            new object[] { /*skillID*/582033, 3 }
                        ));
                        item.itemAttributes.Add(new ItemAttribute(
                            EItemAttribute.hole_skill_amount,
                            Lib.hexToBytes(
                                "64 0B000000 64 03000000 00"
                            ),
                            new object[] { 11, 3 }
                        ));

                        if (cli.hero.bag.addItem(cli.hero, item))
                        {
                            cli.send(PacketEncoder.addBagItem(item));
                        }
                        else
                            Console.WriteLine($"[-] Failed to give item to Hero[{cli.hero.heroName} (ID: {targetID})]. [More: Bag is full]");
                    }
                    break;
                case "help":
                    Console.WriteLine("----- Help -----");
                    Console.WriteLine("  exit\t Kill the server.");
                    Console.WriteLine("  srp\t Enter \"send raw packet\" mode.");
                    Console.WriteLine("  give\t Give item to player.");
                    Console.WriteLine("--- End Help ---");
                    break;
                default:
                    Console.WriteLine("[-] Unknown Command. Type \"help\" to print command list.");
                    break;
                    
            }
        }
    }
}