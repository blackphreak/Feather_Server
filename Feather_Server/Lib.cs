using Feather_Server.Database;
using Feather_Server.Entity;
using Feather_Server.Entity.NPC_Related;
using Feather_Server.ServerRelated;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Feather_Server
{
    public static class Lib
    {
        public static string dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static int lastUID = 10000;
        public static uint lastItemUID = 0;
        public static bool endSrv = false;
        public static Encoding textEncoder;
        public static Dictionary</*player-id*/int, Client> clientList = new Dictionary<int, Client>();
        public static Dictionary</*map*/ushort, List<IEntity>> entityListByMap = new Dictionary<ushort, List<IEntity>>();
        public static readonly JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        // put the current Lv as index to get the lv up exp requirement: rideLvUpExp[lv]
        // 3590
        public static readonly int[] rideLvUpExp =
        {
            /* exp req to lv001 - lv010 */ 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000,
            /* exp req to lv011 - lv020 */ 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000,
            /* exp req to lv021 - lv030 */ 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000,
            /* exp req to lv031 - lv040 */ 19540, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv041 - lv050 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv051 - lv060 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50, // gen1 max: 50, share 25%
            /* exp req to lv061 - lv070 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50, // gen2 max: 65, share 50%
            /* exp req to lv071 - lv080 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv081 - lv090 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv091 - lv100 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv101 - lv110 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv111 - lv120 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv121 - lv130 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv131 - lv140 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv141 - lv150 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50, // gen3 max: 150, share 100%
        };

        // put the current Lv as index to get the lv up exp requirement: lvUpExp[lv]
        public static readonly int[] lvUpExp =
        {
            /* exp req to lv001 - lv010 */ 20, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv011 - lv020 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv021 - lv030 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv031 - lv040 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv041 - lv050 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv051 - lv060 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv061 - lv070 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv071 - lv080 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv081 - lv090 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv091 - lv100 */ 102558123, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv101 - lv110 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv111 - lv120 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv121 - lv130 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv131 - lv140 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv141 - lv150 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            /* exp req to lv151 - lv160 */ 10, 20, 30, 40, 50, 50, 50, 50, 50, 50,
            // not yet open
            /* exp req to lv161 - lv170 */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        public static readonly Dictionary<Role, int[]> roleDefault = new Dictionary<Role, int[]>(){
            {
                // order: hp, mp, meleeAtk, meleeDef, magicAtk, magicDef, hit, dodge, CHR (100 = 1.00%)
                Role.Warrior, new int[]{
                    306, 98, 36, 53, 5, 40, 10, 4, 100
                }
            },
            {
                Role.Mage, new int[]{
                    172, 211, 11, 27, 28, 59, 10, 4, 100
                }
            },
            {
                Role.Swordman, new int[]{
                    208, 135, 27, 53, 16, 64, 10, 4, 100
                }
            },
            {
                Role.Taoist, new int[]{
                    263, 200, 11, 27, 26, 80, 10, 4, 100
                }
            },
            {
                Role.Priest, new int[]{
                    181, 249, 11, 28, 41, 66, 10, 4, 100
                }
            }
        };

        public static byte[] rsaEncrypt(byte[] asn1_pubKey, byte[] msg)
        {
            var pubKey = (DerSequence)Asn1Object.FromByteArray(asn1_pubKey);
            var keyParameters = new RsaKeyParameters(
                false,
                ((DerInteger)pubKey[0]).PositiveValue,
                ((DerInteger)pubKey[1]).PositiveValue
            );

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(DotNetUtilities.ToRSAParameters(keyParameters));

            return rsa.Encrypt(msg, RSAEncryptionPadding.Pkcs1);
        }

        public static bool isStartWith(byte[] src, byte[] target)
        {
            if (src.Length < target.Length)
                return false;

            byte[] cmp = new byte[target.Length];
            Buffer.BlockCopy(src, 0, cmp, 0, target.Length);
            return StructuralComparisons.StructuralEqualityComparer.Equals(cmp, target);
        }

        public static byte[] hexToBytes(string hex)
        {
            hex = hex.Replace("\r\n", "").Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                throw new Exception("Hex Length is not multiple of 2.");
            }
            return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
        }

        public static string toHex(byte src)
        {
            return toHex(BitConverter.GetBytes(src)).Substring(0, 2);
        }
        public static string toHex(short src)
        {
            return toHex(BitConverter.GetBytes(src)).Substring(0, 4);
        }
        public static string toHex(ushort src)
        {
            return toHex(BitConverter.GetBytes(src)).Substring(0, 4);
        }
        public static string toHex(int src)
        {
            return toHex(BitConverter.GetBytes(src));
        }
        public static string toHex(uint src)
        {
            return toHex(BitConverter.GetBytes(src));
        }
        public static string toHex(long src)
        {
            return toHex(BitConverter.GetBytes(src));
        }
        public static string toHex(byte[] src)
        {
            return BitConverter.ToString(src).Replace("-", "");
        }

        public static byte[] gbkToBytes(string inp)
        {
            return textEncoder.GetBytes(inp);
        }

        public static string padWithString(string inp, string pad, int maxLength)
        {
            StringBuilder sb = new StringBuilder(inp);
            for (int i = (maxLength - inp.Length) / (pad.Length); i > 0; i--)
                sb.Append(pad);

            return sb.ToString();
        }

        /// <summary>
        /// Trim the packet by reading its init. byte (size byte)
        /// </summary>
        /// <param name="src">Original Packet</param>
        /// <returns>Trimed Packet</returns>
        public static byte[] trimPkt(byte[] src)
        {
            byte[] newPkt = new byte[src[0]];
            Buffer.BlockCopy(src, 0, newPkt, 0, src[0]);
            return newPkt;
        }

        public static List<byte[]> splitBlock(byte[] src)
        {
            var list = new List<byte[]>();
            int blockCount = (int)(src.Length / 1460) + 1;
            int mod = src.Length % 1460;
            int szFirstBlock = src.Length < 1460 ? mod : 1460;
            byte[] tmp_bytes;
            int sz;
            int offset = 0;

            for (int i = 0; i < blockCount; i++)
            {
                if (i == 0)
                    sz = szFirstBlock;
                else if (i == blockCount - 1)
                    sz = mod;
                else
                    sz = 1460;

                tmp_bytes = new byte[sz];
                Buffer.BlockCopy(src, offset, tmp_bytes, 0, sz);
                list.Add(tmp_bytes);

                //Console.WriteLine($"[*] Block sz {sz}, Bc {blockCount}, mod {mod}, szFb {szFirstBlock}");

                offset += sz;
            }

            return list;
        }

        /// <summary>
        /// Search for bytes in an byte array. Return first index.
        /// </summary>
        /// <returns>First index of found</returns>
        public static int indexOfBytes(byte[] input, byte[] search)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == search[0])
                {
                    for (int j = 1; j < search.Length; j++)
                    {
                        if (input[i + j] != search[j])
                            break;
                    }
                    return i;
                }
            }
            return -1;
        }

        public static List<int> broadcast(byte[] plain_packet)
        {
            List<int> failed = new List<int>();
            clientList.Values.ToList().ForEach(p =>
            {
                try
                {
                    p.send(plain_packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[-] Failed to broadcast data [{e.Message.ToString()}]");
                    //TODO: Logger.log(LOGLEVEL.ERROR | LOGLEVEL.FILE , /*PlayerObject*/p, /*Expection*/e);
                    failed.Add(((IEntity)p).entityID);
                    return; // continue
                }
            });

            return failed;
        }

        public static void sendToNearby(IEntity entity, byte[] plain_packet, bool includeSelf = false)
        {
            if (plain_packet == null)
                return;

            var nearbys = searchForNearby(entity, 16, includeSelf);
            if (nearbys == null)
                return;

            (from nearby in nearbys
             where includeSelf ? true : nearby.entityID != entity.entityID
             select nearby.entityID).ToList()
            .ForEach(id =>
            {
                clientList.GetValueOrDefault(id, null)?.send(plain_packet);
            });
        }

        public static List<IEntity> searchForNearby(IEntity entity, int distance = 16, bool includeSelf = true)
        {
            // TODO: optimization (searchForNearby), use temp list to store the known nearbys

            var X = entity.locX;
            var Y = entity.locY;

            List<IEntity> entities;
            entityListByMap.TryGetValue(entity.map, out entities);

            if (entities is null)
                return null;

            lock (entityListByMap)
                return (from tmp_entity in entities
                   where
                   (!includeSelf ? tmp_entity.entityID != entity.entityID : true)
                   &&
                    (Math.Pow(tmp_entity.locX - X, 2)
                    + Math.Pow(tmp_entity.locY - Y, 2))
                    < (distance * distance)
                   select tmp_entity).ToList();
        }

        public static void spawnNearbys(Client client, IEntity baseEntity, int distance = 16, bool includeSelf = true)
        {
            Lib.searchForNearby(baseEntity, distance, includeSelf)?.ForEach(nearby =>
            {
                if (nearby is Hero)
                    client.send(
                        PacketEncoder.spawnHero(nearby as Hero, false)
                    );
                else if (nearby is NPC)
                    client.send(
                        PacketEncoder.spawnNPC(nearby as NPC)
                    );
            });
        }

        public static HeroBasicInfo[] getBasicInfos(List<int> heroIDs)
        {
            var lst = new List<HeroBasicInfo>();
            if (heroIDs.Count == 0)
                return lst.ToArray();

            var res = DB2.GetInstance().Select(
                "heroID, playerName, basicInfo",
                "Hero",
                $"heroID IN ({string.Join(",", heroIDs)})"
            );

            HeroBasicInfo tmp;

            if (res != null)
                foreach (var row in res)
                {
                    tmp = HeroBasicInfo.fromJson(row[2]);
                    tmp.heroID = int.Parse(row[0]);
                    tmp.heroName = row[1];
                    lst.Add(tmp);
                }

            return lst.ToArray();
        }

        public static void sendPktByHeroID(int heroID, byte[] pkts)
        {
            Client cli;
            if (!Lib.clientList.TryGetValue(heroID, out cli))
                return;

            cli.send(pkts);
        }

        public static Hero getHero(string username, byte position)
        {
            var res = DB2.GetInstance().Select(
                $"h.heroID as h_heroID, l.hero{position} as l_heroID, h.fullInfo as fullInfo, h.playerName as playerName",
                "Login l, Hero h",
                $"l.hero{position} = h.heroID AND username = @uname LIMIT 1",
                new Dictionary<string, object>()
                {
                    { "uname", username }
                }
            );

            if (res.Count != 1)
                return null;

            var tmp = Hero.fromJson(res[0][2]);
            tmp.heroID = int.Parse(res[0][0]);
            tmp.heroName = res[0][3];
            return tmp;
        }

        public static string pktParser(string hexs)
        {
            StringBuilder sb = new StringBuilder("\n\t[+] Start of Packets\n");
            if (hexs.Length == 0)
            {
                sb.Append("\t[+] End of Packets\n");
                return sb.ToString();
            }
            byte index = 0;
            do
            {
                byte sz = Lib.hexToBytes(hexs[0..2])[0];
                if (sz != 0x00)
                {
                    if (sz == 0xFF)
                    {
                        sb.Append($"\t[{index++}] [RAW] {hexs}\n");
                        hexs = "";
                    }
                    else
                    {
                        int spliter = hexs.IndexOf("0d0a00");
                        if (spliter > -1)
                        {
                            // is text
                            sb.Append($"\t[{index++}] [Msg] {textEncoder.GetString(Lib.hexToBytes(hexs[0..spliter]))}\n");
                            hexs = hexs[spliter..];
                        }
                        else
                        {
                            sb.Append($"\t[{index++}] sz[0x{hexs[0..2]}] __ {hexs[2..4]} {hexs[4..(sz*2)]} 00\n");
                            hexs = hexs[(sz*2 + 2)..];
                        }

                    }
                }
            } while (hexs.Length != 0);
            sb.Append("\t[+] End of Packets\n");
            return sb.ToString();
        }
    }
}
