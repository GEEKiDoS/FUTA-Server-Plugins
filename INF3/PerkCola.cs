﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using InfinityScript;

namespace INF3
{
    /// <summary>
    /// 指定Perk-a-Cola的类型
    /// </summary>
    public enum PerkColaType
    {
        /// <summary>
        /// 快速复活苏打水
        /// </summary>
        QUICK_REVIVE = 0,
        /// <summary>
        /// 快手可乐
        /// </summary>
        SPEED_COLA = 1,
        /// <summary>
        /// 重装蛋奶酒
        /// </summary>
        JUGGERNOG = 2,
        /// <summary>
        /// 加速剂
        /// </summary>
        STAMIN_UP = 3,
        /// <summary>
        /// 三枪汽水
        /// </summary>
        MULE_KICK = 4,
        /// <summary>
        /// 子弹分裂原浆啤酒II
        /// </summary>
        DOUBLE_TAP = 5,
        /// <summary>
        /// 死亡射手鸡尾酒
        /// </summary>
        DEAD_SHOT = 6,
        /// <summary>
        /// 防爆泡泡水
        /// </summary>
        PHD = 7,
        /// <summary>
        /// 电力樱桃汽水
        /// </summary>
        ELECTRIC_CHERRY = 8,
        /// <summary>
        /// 黑寡妇红酒
        /// </summary>
        WIDOW_S_WINE = 9,
        /// <summary>
        /// 秃鹫辅助药水
        /// </summary>
        VULTURE_AID = 10,
    }

    public class PerkCola
    {
        public PerkColaType Type { get; private set; }
        public string FullName
        {
            get
            {
                switch (Type)
                {
                    case PerkColaType.QUICK_REVIVE:
                        return "Quick Revive";
                    case PerkColaType.SPEED_COLA:
                        return "Speed Cola";
                    case PerkColaType.JUGGERNOG:
                        return "Juggernog";
                    case PerkColaType.STAMIN_UP:
                        return "Stamin-Up";
                    case PerkColaType.MULE_KICK:
                        return "Mule Kick";
                    case PerkColaType.DOUBLE_TAP:
                        return "Double Tap Root Beer II";
                    case PerkColaType.DEAD_SHOT:
                        return "Deadshot Daiquiri";
                    case PerkColaType.PHD:
                        return "PhD Flopper";
                    case PerkColaType.ELECTRIC_CHERRY:
                        return "Electric Cherry";
                    case PerkColaType.WIDOW_S_WINE:
                        return "Widow's Wine";
                    case PerkColaType.VULTURE_AID:
                        return "Vulture Aid Elixir";
                    default:
                        return null;
                }
            }
        }
        public string HudName
        {
            get
            {
                switch (Type)
                {
                    case PerkColaType.QUICK_REVIVE:
                        return "Quick Revive";
                    case PerkColaType.SPEED_COLA:
                        return "Speed Cola";
                    case PerkColaType.JUGGERNOG:
                        return "Juggernog";
                    case PerkColaType.STAMIN_UP:
                        return "Stamin-Up";
                    case PerkColaType.MULE_KICK:
                        return "Mule Kick";
                    case PerkColaType.DOUBLE_TAP:
                        return "Double Tap II";
                    case PerkColaType.DEAD_SHOT:
                        return "Deadshot";
                    case PerkColaType.PHD:
                        return "PhD";
                    case PerkColaType.ELECTRIC_CHERRY:
                        return "Electric Cherry";
                    case PerkColaType.WIDOW_S_WINE:
                        return "Widow's Wine";
                    case PerkColaType.VULTURE_AID:
                        return "Vulture Aid";
                    default:
                        return null;
                }
            }
        }
        public Vector3 HudColor
        {
            get
            {
                switch (Type)
                {
                    case PerkColaType.QUICK_REVIVE:
                        return new Vector3(0, 0.5f, 0.5f);
                    case PerkColaType.SPEED_COLA:
                        return new Vector3(0.3f, 0.9f, 0.3f);
                    case PerkColaType.JUGGERNOG:
                        return new Vector3(1, 0.3f, 0.3f);
                    case PerkColaType.STAMIN_UP:
                        return new Vector3(0.9f, 0.9f, 0.3f);
                    case PerkColaType.MULE_KICK:
                        return new Vector3(0.9f, 0.3f, 0.3f);
                    case PerkColaType.DOUBLE_TAP:
                        return new Vector3(0.9f, 0.5f, 0.2f);
                    case PerkColaType.DEAD_SHOT:
                        return new Vector3(0.1f, 0.1f, 0.1f);
                    case PerkColaType.PHD:
                        return new Vector3(0.7f, 0.5f, 1f);
                    case PerkColaType.ELECTRIC_CHERRY:
                        return new Vector3(1, 0.3f, 0.3f);
                    case PerkColaType.WIDOW_S_WINE:
                        return new Vector3(0.5f, 0.5f, 0.5f);
                    case PerkColaType.VULTURE_AID:
                        return new Vector3(0.9f, 0.5f, 0.2f);
                    default:
                        return new Vector3();
                }
            }
        }
        public string Icon
        {
            get
            {
                switch (Type)
                {
                    case PerkColaType.QUICK_REVIVE:
                        return "specialty_finalstand";
                    case PerkColaType.SPEED_COLA:
                        return "specialty_fastreload";
                    case PerkColaType.JUGGERNOG:
                        return "cardicon_juggernaut_1";
                    case PerkColaType.STAMIN_UP:
                        return "specialty_longersprint";
                    case PerkColaType.MULE_KICK:
                        return "specialty_twoprimaries";
                    case PerkColaType.DOUBLE_TAP:
                        return "specialty_moredamage";
                    case PerkColaType.DEAD_SHOT:
                        return "cardicon_headshot";
                    case PerkColaType.PHD:
                        return "specialty_blastshield";
                    case PerkColaType.ELECTRIC_CHERRY:
                        return "cardicon_cod4";
                    case PerkColaType.WIDOW_S_WINE:
                        return "cardicon_soap_bar";
                    case PerkColaType.VULTURE_AID:
                        return "specialty_scavenger";
                    default:
                        return null;
                }
            }
        }
        public int Pay
        {
            get
            {
                switch (Type)
                {
                    case PerkColaType.QUICK_REVIVE:
                        return 600;
                    case PerkColaType.SPEED_COLA:
                        return 700;
                    case PerkColaType.JUGGERNOG:
                        return 1000;
                    case PerkColaType.STAMIN_UP:
                        return 800;
                    case PerkColaType.MULE_KICK:
                        return 700;
                    case PerkColaType.DOUBLE_TAP:
                        return 1000;
                    case PerkColaType.DEAD_SHOT:
                        return 600;
                    case PerkColaType.PHD:
                        return 800;
                    case PerkColaType.ELECTRIC_CHERRY:
                        return 600;
                    case PerkColaType.WIDOW_S_WINE:
                        return 1000;
                    case PerkColaType.VULTURE_AID:
                        return 600;
                    default:
                        return 0;
                }
            }
        }
        public string Dvar
        {
            get
            {
                switch (Type)
                {
                    case PerkColaType.QUICK_REVIVE:
                        return "perk_revive";
                    case PerkColaType.SPEED_COLA:
                        return "perk_speedcola";
                    case PerkColaType.JUGGERNOG:
                        return "perk_juggernog";
                    case PerkColaType.STAMIN_UP:
                        return "perk_staminup";
                    case PerkColaType.MULE_KICK:
                        return "perk_mulekick";
                    case PerkColaType.DOUBLE_TAP:
                        return "perk_doubletap";
                    case PerkColaType.DEAD_SHOT:
                        return "perk_deadshot";
                    case PerkColaType.PHD:
                        return "perk_phd";
                    case PerkColaType.ELECTRIC_CHERRY:
                        return "perk_cherry";
                    case PerkColaType.WIDOW_S_WINE:
                        return "perk_widow";
                    case PerkColaType.VULTURE_AID:
                        return "perk_vultrue";
                    default:
                        return null;
                }
            }
        }
        public Action<Entity> GiveFunction
        {
            get
            {
                return InitGetFunction();
            }
        }
        public Action<Entity> TakeFunction
        {
            get
            {
                return InitTakeFunction();
            }
        }

        public PerkCola(PerkColaType type)
        {
            Type = type;
        }

        private Action<Entity> InitGetFunction()
        {
            switch (Type)
            {
                case PerkColaType.QUICK_REVIVE:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_revive", 1);
                    });
                case PerkColaType.SPEED_COLA:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_speedcola", 1);
                        player.SetPerk("specialty_fastreload", true, false);
                        player.SetPerk("specialty_quickswap", true, false);
                        player.SetPerk("specialty_quickdraw", true, false);
                        player.SetPerk("specialty_armorpiercing", true, false);
                    });
                case PerkColaType.JUGGERNOG:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_juggernog", 1);
                        player.SetField("oldmodel", player.GetField<string>("model"));
                        player.Call("setmodel", "mp_fullbody_ally_juggernaut");
                        player.Call("setviewmodel", "viewhands_juggernaut_ally");
                        player.SetField("maxhealth", 300);
                        player.Health = 300;
                    });
                case PerkColaType.STAMIN_UP:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_staminup", 1);
                        player.SetField("speed", 1.3f);
                        player.SetPerk("specialty_marathon", true, false);
                        player.SetPerk("specialty_lightweight", true, false);
                        player.SetPerk("specialty_fastmantle", true, false);
                        player.SetPerk("specialty_fastsprintrecovery", true, false);
                    });
                case PerkColaType.MULE_KICK:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_mulekick", 1);
                        player.GiveWeapon(Sharpshooter._mulekickWeapon.Code);
                        player.Call("givemaxammo", Sharpshooter._mulekickWeapon.Code);
                    });
                case PerkColaType.DOUBLE_TAP:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_doubletap", 1);
                    });
                case PerkColaType.DEAD_SHOT:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_deadshot", 1);
                        player.OnInterval(100, e =>
                        {
                            player.Call("recoilscaleon", 0);
                            return player.IsPlayer && player.IsAlive && player.HasPerkCola(this);
                        });
                        player.SetPerk("specialty_reducedsway", true, false);
                        player.SetPerk("specialty_bulletaccuracy", true, false);
                    });
                case PerkColaType.PHD:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_phd", 1);
                        player.SetPerk("_specialty_blastshield", true, false);
                    });
                case PerkColaType.ELECTRIC_CHERRY:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_cherry", 1);
                    });
                case PerkColaType.WIDOW_S_WINE:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_widow", 1);
                    });
                case PerkColaType.VULTURE_AID:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_vultrue", 1);
                        Ammo.MaxAmmo(player);
                        player.SetPerk("specialty_scavenger", true, false);
                    });
                default:
                    return null;
            }
        }

        private Action<Entity> InitTakeFunction()
        {
            switch (Type)
            {
                case PerkColaType.QUICK_REVIVE:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_revive", 0);
                    });
                case PerkColaType.SPEED_COLA:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_speedcola", 0);
                        player.DeletePerk("specialty_fastreload");
                        player.DeletePerk("specialty_quickswap");
                        player.DeletePerk("specialty_quickdraw");
                    });
                case PerkColaType.JUGGERNOG:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_juggernog", 0);
                        player.Call("setmodel", player.GetField<string>("oldmodel"));
                        player.Call("setviewmodel", "viewmodel_base_viewhands");
                        player.SetField("maxhealth", 100);
                        player.Health = 100;
                    });
                case PerkColaType.STAMIN_UP:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_staminup", 0);
                        player.SetSpeed(1f);
                        player.DeletePerk("specialty_marathon");
                        player.DeletePerk("specialty_lightweight");
                        player.DeletePerk("specialty_fastsprintrecovery");
                    });
                case PerkColaType.MULE_KICK:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_mulekick", 0);
                        player.GiveMaxAmmoWeapon(Sharpshooter._mulekickWeapon.Code);
                    });
                case PerkColaType.DOUBLE_TAP:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_doubletap", 0);
                    });
                case PerkColaType.DEAD_SHOT:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_deadshot", 0);
                        player.DeletePerk("specialty_reducedsway");
                        player.DeletePerk("specialty_bulletaccuracy");
                    });
                case PerkColaType.PHD:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_phd", 0);
                        player.DeletePerk("_specialty_blastshield");
                    });
                case PerkColaType.ELECTRIC_CHERRY:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_cherry", 0);
                    });
                case PerkColaType.WIDOW_S_WINE:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_widow", 0);
                    });
                case PerkColaType.VULTURE_AID:
                    return new Action<Entity>(player =>
                    {
                        player.SetField("perk_vultrue", 0);
                        player.DeletePerk("specialty_scavenger");
                    });
                default:
                    return null;
            }
        }

        private static readonly List<KeyValuePair<int, PerkColaType>> Rolls = new List<KeyValuePair<int, PerkColaType>>
        {
            new KeyValuePair<int, PerkColaType>(4,PerkColaType.QUICK_REVIVE),
            new KeyValuePair<int, PerkColaType>(1,PerkColaType.SPEED_COLA),
            new KeyValuePair<int, PerkColaType>(1,PerkColaType.JUGGERNOG),
            new KeyValuePair<int, PerkColaType>(1,PerkColaType.STAMIN_UP),
            new KeyValuePair<int, PerkColaType>(4,PerkColaType.MULE_KICK),
            new KeyValuePair<int, PerkColaType>(1,PerkColaType.DOUBLE_TAP),
            new KeyValuePair<int, PerkColaType>(7,PerkColaType.DEAD_SHOT),
            new KeyValuePair<int, PerkColaType>(4,PerkColaType.PHD),
            new KeyValuePair<int, PerkColaType>(7,PerkColaType.ELECTRIC_CHERRY),
            new KeyValuePair<int, PerkColaType>(1,PerkColaType.WIDOW_S_WINE),
            new KeyValuePair<int, PerkColaType>(7,PerkColaType.VULTURE_AID),
        };

        private static PerkColaType RandomItem()
        {
            int count = Rolls.Count;
            var rolls = (from r in Rolls orderby r.Key select r).ToList();

            var dic = new Dictionary<KeyValuePair<int, int>, PerkColaType>();
            int index = 1;
            foreach (var item in rolls)
            {
                int start = index;
                int end = index + item.Key - 1;
                index += end + 1;
                dic.Add(new KeyValuePair<int, int>(start, end), item.Value);
            }

            var rng = Utility.Random.Next(1, index);
            var tar = (
                from r in dic
                where rng >= r.Key.Key || rng <= r.Key.Value
                select r.Value).ToList()[0];

            return tar;
        }

        public static PerkCola GetRandomPerkCola()
        {
            return new PerkCola(RandomItem());
        }

        public void GiveToPlayer(Entity player, bool haseffect)
        {
            player.SetPerkColaCount(player.PerkColasCount() + 1);
            player.AddPerkHud(haseffect ? player.PerkHud(Icon, HudColor, HudName) : player.PerkHudNoEffect(Icon));
            GiveFunction.DynamicInvoke(player);
        }

        public void TakePerkCola(Entity player)
        {
            TakeFunction.DynamicInvoke();
        }
    }
}
