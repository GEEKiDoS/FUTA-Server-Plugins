using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    /// <summary>
    /// 指定泡泡糖的作用
    /// </summary>
    public enum GobbleGumType
    {
        /// <summary>
        /// 无任何作用，此项用于表示玩家并没有任何泡泡糖
        /// </summary>
        None,

        // 最常见

        /// <summary>
        /// 自动激活，瞄准时移动更快
        /// </summary>
        AlwaysDoneSwiftly,
        /// <summary>
        /// 当自己赌博抽到You live or die或You infected，自己或其他人抽到Other humans die时自动激活，可以保护自己不被赌博机击杀，持续1次
        /// </summary>
        NoGamble,
        /// <summary>
        /// 自动激活，缩短赌博机倒计时时间
        /// </summary>
        Coagulant,
        /// <summary>
        /// 激活后，10秒内无法被僵尸看到
        /// </summary>
        InPlainSight,
        /// <summary>
        /// 自动激活，1分钟内，射击消耗备弹而不是弹匣
        /// </summary>
        StockOption,

        // 较常见

        /// <summary>
        /// 被Boomer效果影响后自动激活，消除Boomer的影响，持续1次
        /// </summary>
        Wiper,
        /// <summary>
        /// 自动激活，1分钟内，近战一击必杀
        /// </summary>
        SwordFlay,
        /// <summary>
        /// 激活后，10秒内免疫所有僵尸的伤害，包括近战和爆炸伤害
        /// </summary>
        DangerClosest,
        /// <summary>
        /// 被Spider效果影响时自动激活，消除Spider的伤害，持续1次
        /// </summary>
        Antidote,
        /// <summary>
        /// 激活后，重新运行一次赌博机，只能在进行赌博后的10秒内激活
        /// </summary>
        LuckyCrit,
        /// <summary>
        /// 自动激活，1分钟内所有僵尸都会有红框标记
        /// </summary>
        Recon,
        /// <summary>
        /// 当有人从赌博机中抽到Surprise和Robbed all money时激活，保护自己的现金和Bonus Points不会损失，持续1次
        /// </summary>
        Strongbox,

        // 稀有

        /// <summary>
        /// 拥有Quick Revive后激活，被僵尸干掉复活不会丢失Perk-a-Cola
        /// </summary>
        Aftertaste,
        /// <summary>
        /// 激活后，1分钟内所有僵尸重生只能Roll到减弱而不能Roll到增强
        /// </summary>
        BurnedOut,
        /// <summary>
        /// 激活后立刻获得一个Nuke Power-Up
        /// </summary>
        DeadOfNuclearWinter,
        /// <summary>
        /// 激活后立刻获得一个随机Power-Up
        /// </summary>
        IAmFeelingLucky,
        /// <summary>
        /// 激活后立刻获得一个Fire Sale Power-Up
        /// </summary>
        ImmolationLiquidation,
        /// <summary>
        /// 激活后立刻获得一个Carpenter Power-Up
        /// </summary>
        LicensedContractor,
        /// <summary>
        /// 激活后，随机将一个僵尸重生为人类
        /// </summary>
        PhoenixUp,
        /// <summary>
        /// 被僵尸近战攻击自动激活，被僵尸近战攻击立即将周围大范围内僵尸冻住并免疫本次攻击造成的伤害，持续1次
        /// </summary>
        PopShocks,
        /// <summary>
        /// 激活后，立刻更新所有人的武器
        /// </summary>
        RespinCycle,
        /// <summary>
        /// 当Perk-a-Cola达到5个时候自动激活，可以额外购买一个Perk-a-Cola
        /// </summary>
        Unquenchable,
        /// <summary>
        /// 激活后立刻获得一个Double Points Power-Up
        /// </summary>
        WhosKeepingScore,
        /// <summary>
        /// 激活后立刻获得一个Max Ammo Power-Up
        /// </summary>
        CacheBack,
        /// <summary>
        /// 激活后立刻获得一个Insta-Kill Power-Up
        /// </summary>
        KillJoy,
        /// <summary>
        /// 激活后立刻减慢所有存活僵尸的移动速度
        /// </summary>
        WalkingDeath,
        /// <summary>
        /// 自动激活，延长Power-Up的保留时间
        /// </summary>
        TemporalGift,

        // 超级稀有

        /// <summary>
        /// 激活后，15秒内所有被命中的僵尸都会被冻住，如果没有被击杀则会在10秒后自爆
        /// </summary>
        KillingTime,
        /// <summary>
        /// 当幸存人类数少于3个时激活，立即给与所有幸存人类所有Perk-a-Cola
        /// </summary>
        Perkaholic,
        /// <summary>
        /// 激活后，所有对僵尸的伤害都会附加损伤僵尸的头，持续1分钟
        /// </summary>
        HeadDrama,
    }

    public class GobbleGum
    {
        public delegate void GobbleFunc(Entity player);

        public GobbleGumType Type { get; set; }
        public string Name
        {
            get
            {
                switch (Type)
                {
                    case GobbleGumType.AlwaysDoneSwiftly:
                        return "Always Done Swiftly";
                    case GobbleGumType.NoGamble:
                        return "No Gamble";
                    case GobbleGumType.Coagulant:
                        return "Coagulant";
                    case GobbleGumType.InPlainSight:
                        return "In Plain Sight";
                    case GobbleGumType.StockOption:
                        return "Stock Option";
                    case GobbleGumType.Wiper:
                        return "Wiper";
                    case GobbleGumType.SwordFlay:
                        return "Sword Flay";
                    case GobbleGumType.DangerClosest:
                        return "Danger Closest";
                    case GobbleGumType.Antidote:
                        return "Antidote";
                    case GobbleGumType.LuckyCrit:
                        return "Lucky Crit";
                    case GobbleGumType.Recon:
                        return "Recon";
                    case GobbleGumType.Strongbox:
                        return "Strongbox";
                    case GobbleGumType.Aftertaste:
                        return "Aftertaste";
                    case GobbleGumType.BurnedOut:
                        return "Burned Out";
                    case GobbleGumType.DeadOfNuclearWinter:
                        return "Dead Of Nuclear Winter";
                    case GobbleGumType.IAmFeelingLucky:
                        return "I'm Feeling Lucky";
                    case GobbleGumType.ImmolationLiquidation:
                        return "Immolation Liquidation";
                    case GobbleGumType.LicensedContractor:
                        return "Licensed Contractor";
                    case GobbleGumType.PhoenixUp:
                        return "Phoenix Up";
                    case GobbleGumType.PopShocks:
                        return "Pop Shocks";
                    case GobbleGumType.RespinCycle:
                        return "Respin Cycle";
                    case GobbleGumType.Unquenchable:
                        return "Unquenchable";
                    case GobbleGumType.WhosKeepingScore:
                        return "Who's Keeping Score";
                    case GobbleGumType.CacheBack:
                        return "Cache Back";
                    case GobbleGumType.KillJoy:
                        return "Kill Joy";
                    case GobbleGumType.WalkingDeath:
                        return "Walking Death";
                    case GobbleGumType.TemporalGift:
                        return "Temporal Gift";
                    case GobbleGumType.KillingTime:
                        return "Killing Time";
                    case GobbleGumType.Perkaholic:
                        return "Perkaholic";
                    case GobbleGumType.HeadDrama:
                        return "Head Drama";
                    default:
                        return "";
                }
            }
        }
        public string Info
        {
            get
            {
                switch (Type)
                {
                    case GobbleGumType.AlwaysDoneSwiftly:
                        return "Walk faster when aiming. Activates immediately.";
                    case GobbleGumType.NoGamble:
                        return "Protect you not killed by Gambler. For once.";
                    case GobbleGumType.Coagulant:
                        return "Shorted gamble time. Activates immediately.";
                    case GobbleGumType.InPlainSight:
                        return "The player is ignored by zombies for 10 seconds. Activates manual.";
                    case GobbleGumType.StockOption:
                        return "Ammo is taken from the player's stockpile instead of their weapon's magazine. For 1 min. Activates immediately.";
                    case GobbleGumType.Wiper:
                        return "Eliminate the effects of Boomer. For once.";
                    case GobbleGumType.SwordFlay:
                        return "Melee attacks and any melee weapon will one hit kill on zombies. Activates immediately.";
                    case GobbleGumType.DangerClosest:
                        return "Zero any damage from zombies for 10 second. Activates manual.";
                    case GobbleGumType.Antidote:
                        return "Eliminate the damage of Spider. For once.";
                    case GobbleGumType.LuckyCrit:
                        return "Restart the Gambler. Only can active for gambled 10 second. Activates manual.";
                    case GobbleGumType.Recon:
                        return "Mark all zombies for 1 min. Activates immediately.";
                    case GobbleGumType.Strongbox:
                        return "Protect you cash and Bonus Point not take by otherone. For once.";
                    case GobbleGumType.Aftertaste:
                        return "Not lose all Perk-a-Cola if you killed by zombie. Activates immediately if have Quick Revive.";
                    case GobbleGumType.BurnedOut:
                        return "Limit zombies only can roll bad item for 1 min. Activates immediately.";
                    case GobbleGumType.DeadOfNuclearWinter:
                        return "Spawns a Nuke Power-Up. Activates manual.";
                    case GobbleGumType.IAmFeelingLucky:
                        return "Spawns a random Power-Up. Activates manual.";
                    case GobbleGumType.ImmolationLiquidation:
                        return "Spawns a Fire Sale Power-Up. Activates manual.";
                    case GobbleGumType.LicensedContractor:
                        return "Spawns a Carpenter Power-Up. Activates manual.";
                    case GobbleGumType.PhoenixUp:
                        return "Change a random zombie to human. Activates manual.";
                    case GobbleGumType.PopShocks:
                        return "Freeze nearby zombies if zombie hit you. And you immunity this attack damage. For once.";
                    case GobbleGumType.RespinCycle:
                        return "Cycle all humans weapon. Activates manual.";
                    case GobbleGumType.Unquenchable:
                        return "Can buy an extra Perk-a-Cola.  Activates immediately.";
                    case GobbleGumType.WhosKeepingScore:
                        return "Spawns a Double Points Power-Up. Activates manual.";
                    case GobbleGumType.CacheBack:
                        return "Spawns a Max Ammo Power-Up. Activates manual.";
                    case GobbleGumType.KillJoy:
                        return "Spawns a Insta-Kill Power-Up. Activates manual.";
                    case GobbleGumType.WalkingDeath:
                        return "Slow down all zombies move speed. Activates immediately.";
                    case GobbleGumType.TemporalGift:
                        return "Power-Ups last longer. Activates immediately.";
                    case GobbleGumType.KillingTime:
                        return "For 15 second. Freeze zombies if they hit by you. If they not killed for 10 second. Then they will exploed. Activates immediately.";
                    case GobbleGumType.Perkaholic:
                        return "Give all survivor all Perk-a-Colas. Activates if only have 2 survivor.";
                    case GobbleGumType.HeadDrama:
                        return "Any bullet which hits a zombie will damage its head for 1 min. Activates immediately.";
                    default:
                        return "";
                }
            }
        }

        public int Weight
        {
            get
            {
                switch (Type)
                {
                    case GobbleGumType.AlwaysDoneSwiftly:
                    case GobbleGumType.NoGamble:
                    case GobbleGumType.Coagulant:
                    case GobbleGumType.InPlainSight:
                    case GobbleGumType.StockOption:
                        return 10;
                    case GobbleGumType.Wiper:
                    case GobbleGumType.SwordFlay:
                    case GobbleGumType.DangerClosest:
                    case GobbleGumType.Antidote:
                    case GobbleGumType.LuckyCrit:
                    case GobbleGumType.Recon:
                    case GobbleGumType.Strongbox:
                        return 7;
                    case GobbleGumType.Aftertaste:
                    case GobbleGumType.BurnedOut:
                    case GobbleGumType.DeadOfNuclearWinter:
                    case GobbleGumType.IAmFeelingLucky:
                    case GobbleGumType.ImmolationLiquidation:
                    case GobbleGumType.LicensedContractor:
                    case GobbleGumType.PhoenixUp:
                    case GobbleGumType.PopShocks:
                    case GobbleGumType.RespinCycle:
                    case GobbleGumType.Unquenchable:
                    case GobbleGumType.WhosKeepingScore:
                    case GobbleGumType.CacheBack:
                    case GobbleGumType.KillJoy:
                    case GobbleGumType.WalkingDeath:
                    case GobbleGumType.TemporalGift:
                        return 4;
                    case GobbleGumType.KillingTime:
                    case GobbleGumType.Perkaholic:
                    case GobbleGumType.HeadDrama:
                        return 1;
                    default:
                        return 0;
                }
            }
        }
        public bool IsManual
        {
            get
            {
                switch (Type)
                {
                    case GobbleGumType.AlwaysDoneSwiftly:
                    case GobbleGumType.NoGamble:
                    case GobbleGumType.Coagulant:
                    case GobbleGumType.StockOption:
                    case GobbleGumType.Wiper:
                    case GobbleGumType.SwordFlay:
                    case GobbleGumType.Antidote:
                    case GobbleGumType.Recon:
                    case GobbleGumType.Strongbox:
                    case GobbleGumType.Aftertaste:
                    case GobbleGumType.PopShocks:
                    case GobbleGumType.Unquenchable:
                    case GobbleGumType.TemporalGift:
                    case GobbleGumType.Perkaholic:
                        return false;
                    case GobbleGumType.InPlainSight:
                    case GobbleGumType.DangerClosest:
                    case GobbleGumType.LuckyCrit:
                    case GobbleGumType.BurnedOut:
                    case GobbleGumType.DeadOfNuclearWinter:
                    case GobbleGumType.IAmFeelingLucky:
                    case GobbleGumType.ImmolationLiquidation:
                    case GobbleGumType.LicensedContractor:
                    case GobbleGumType.PhoenixUp:
                    case GobbleGumType.RespinCycle:
                    case GobbleGumType.WhosKeepingScore:
                    case GobbleGumType.CacheBack:
                    case GobbleGumType.KillJoy:
                    case GobbleGumType.WalkingDeath:
                    case GobbleGumType.KillingTime:
                    case GobbleGumType.HeadDrama:
                        return true;
                    default:
                        return false;
                }
            }
        }
        public GobbleFunc Activation { get; }

        public GobbleGum(GobbleGumType type)
        {
            Type = type;
            Activation = InitActiveAction();
        }

        private GobbleFunc InitActiveAction()
        {
            switch (Type)
            {
                case GobbleGumType.AlwaysDoneSwiftly:
                    return player =>
                    {
                        player.SetPerk("specialty_autospot", true, false);
                    };
                case GobbleGumType.NoGamble:
                    return player =>
                    {
                        player.GamblerText("You live! (Gobble Gum Protect)", new Vector3(1, 1, 1), new Vector3(0, 0, 1), 1, 0.85f);
                    };
                case GobbleGumType.Coagulant:
                    return player =>
                    {
                        player.SetField("gobble_coagulant", 1);
                    };
                case GobbleGumType.InPlainSight:
                    return player =>
                    {
                        player.SetField("gobble_inplainsight", 1);
                        player.Call("hide");
                        player.AfterDelay(10000, e =>
                        {
                            player.SetField("gobble_inplainsight", 0);
                            player.Call("show");
                        });
                    };
                case GobbleGumType.StockOption:
                    return player =>
                    {
                        player.SetField("gobble_stockoption", 1);
                        player.AfterDelay(60000, e =>
                        {
                            player.SetField("gobble_stockoption", 0);
                        });
                    };
                case GobbleGumType.Wiper:
                    return player =>
                    {
                        player.GamblerText("Cleaned View! (Gobble Gum Protect)", new Vector3(1, 1, 1), new Vector3(0, 0, 1), 1, 0.85f);
                    };
                case GobbleGumType.SwordFlay:
                    return player =>
                    {
                        player.SetField("gobble_swordflay", 1);
                        player.AfterDelay(60000, e =>
                        {
                            player.SetField("gobble_swordflay", 0);
                        });
                    };
                case GobbleGumType.DangerClosest:
                    return player =>
                    {
                        player.SetField("gobble_dangerclosest", 1);
                        player.AfterDelay(10000, e =>
                        {
                            player.SetField("gobble_dangerclosest", 0);
                        });
                    };
                case GobbleGumType.Antidote:
                    return player =>
                    {
                        player.GamblerText("Acid Protect! (Gobble Gum Protect)", new Vector3(1, 1, 1), new Vector3(0, 0, 1), 1, 0.85f);
                    };
                case GobbleGumType.LuckyCrit:
                    return player =>
                    {
                        player.GamblerText("Gambler Restarted!", new Vector3(1, 1, 1), new Vector3(0, 0, 1), 1, 0.85f);
                    };
                case GobbleGumType.Recon:
                    return player =>
                    {
                        player.SetField("gobble_recon", 1);
                        player.Call("thermalvisionon");
                        player.AfterDelay(60000, e =>
                        {
                            player.SetField("gobble_recon", 0);
                            player.Call("thermalvisionoff");
                        });
                    };
                case GobbleGumType.Strongbox:
                    return player =>
                    {
                        player.GamblerText("You cash and Bouns Points was protected!", new Vector3(1, 1, 1), new Vector3(0, 0, 1), 1, 0.85f);
                    };
                case GobbleGumType.Aftertaste:
                    return player =>
                    {
                        player.SetField("gobble_aftertaste", 1);
                    };
                case GobbleGumType.BurnedOut:
                    return player =>
                    {
                        Utility.SetDvar("global_gobble_burnedout", 1);
                        player.AfterDelay(60000, e =>
                        {
                            Utility.SetDvar("global_gobble_burnedout", 0);
                        });
                    };
                case GobbleGumType.DeadOfNuclearWinter:
                    return player =>
                    {
                        PowerUp.Nuke(player);
                    };
                case GobbleGumType.IAmFeelingLucky:
                    return player =>
                    {
                        var p = (PowerUpType)Utility.Random.Next(Enum.GetNames(typeof(PowerUpType)).Length);
                        switch (p)
                        {
                            case PowerUpType.MaxAmmo:
                                PowerUp.MaxAmmo(player);
                                break;
                            case PowerUpType.DoublePoints:
                                PowerUp.DoublePoints(player);
                                break;
                            case PowerUpType.InstaKill:
                                PowerUp.InstaKill(player);
                                break;
                            case PowerUpType.Nuke:
                                PowerUp.Nuke(player);
                                break;
                            case PowerUpType.FireSale:
                                PowerUp.FireSale(player);
                                break;
                            case PowerUpType.BonusPoints:
                                PowerUp.BonusPoints(player);
                                break;
                            case PowerUpType.Carpenter:
                                PowerUp.Carpenter(player);
                                break;
                        }
                    };
                case GobbleGumType.ImmolationLiquidation:
                    return player =>
                    {
                        PowerUp.FireSale(player);
                    };
                case GobbleGumType.LicensedContractor:
                    return player =>
                    {
                        PowerUp.Carpenter(player);
                    };
                case GobbleGumType.PhoenixUp:
                    return player =>
                    {
                        foreach (var item in Utility.Players)
                        {
                            if (item.IsAlive && item.GetTeam() == "axis")
                            {
                                item.Suicide();
                                item.SetTeam("allies");
                                item.GamblerText("You has be revive to human by " + player.Name, new Vector3(1, 1, 1), new Vector3(1, 1, 0), 1, 0.85f);
                                break;
                            }
                        }
                    };
                case GobbleGumType.PopShocks:
                    return player =>
                    {
                        player.GamblerText("Pop Shocks!", new Vector3(1, 1, 1), new Vector3(0, 0, 1), 1, 0.85f);
                    };
                case GobbleGumType.RespinCycle:
                    return player =>
                    {
                        Sharpshooter._cycleRemaining = 0;
                    };
                case GobbleGumType.Unquenchable:
                    return player =>
                    {
                        player.SetField("gobble_unquenchable", 1);
                    };
                case GobbleGumType.WhosKeepingScore:
                    return player =>
                    {
                        PowerUp.DoublePoints(player);
                    };
                case GobbleGumType.CacheBack:
                    return player =>
                    {
                        PowerUp.MaxAmmo(player);
                    };
                case GobbleGumType.KillJoy:
                    return player =>
                    {
                        PowerUp.InstaKill(player);
                    };
                case GobbleGumType.WalkingDeath:
                    return player =>
                    {
                        foreach (var item in Utility.Players)
                        {
                            if (item.IsAlive && item.GetTeam() == "axis")
                            {
                                item.SetSpeed(0.5f);
                            }
                        }
                    };
                case GobbleGumType.TemporalGift:
                    return player =>
                    {
                        Utility.SetDvar("global_gobble_temporalgift", 1);
                    };
                case GobbleGumType.KillingTime:
                    return player =>
                    {
                        player.SetField("gobble_killingtime", 1);
                        player.AfterDelay(15000, e =>
                         {
                             player.SetField("gobble_killingtime", 0);
                         });
                    };
                case GobbleGumType.Perkaholic:
                    return player =>
                    {
                        foreach (var item in Utility.Players)
                        {
                            if (item.IsAlive && item.GetTeam() == "allies")
                            {
                                item.GiveAllPerkCola();
                            }
                        }
                    };
                case GobbleGumType.HeadDrama:
                    return player =>
                    {
                        player.SetField("gobble_headdrama", 1);
                        player.AfterDelay(60000, e =>
                        {
                            player.SetField("gobble_headdrama", 0);
                        });

                    };
                default:
                    return player =>
                    {

                    };
            }
        }
    }

    public class GobbleGumFunction : BaseScript
    {
        public static readonly GobbleGum[] _currentGobblegum = new GobbleGum[18];

        private static List<GobbleGum> _allGobblegum = new List<GobbleGum>();

        public GobbleGumFunction()
        {
            for (int i = 0; i < Enum.GetNames(typeof(GobbleGumType)).Length; i++)
            {
                _allGobblegum.Add(new GobbleGum((GobbleGumType)i));
            }
            _allGobblegum = (from a in _allGobblegum where a.Type != GobbleGumType.None orderby a.Weight select a).ToList();

            PlayerConnected += player =>
            {
                player.SetCurrentGobbleGum(new GobbleGum(GobbleGumType.None));

                player.SetField("gobble_coagulant", 0);
                player.SetField("gobble_inplainsight", 0);
                player.SetField("gobble_stockoption", 0);
                player.SetField("gobble_swordflay", 0);
                player.SetField("gobble_dangerclosest", 0);
                player.SetField("gobble_recon", 0);
                player.SetField("gobble_aftertaste", 0);
                player.SetField("gobble_unquenchable", 0);
                player.SetField("gobble_killingtime", 0);
                player.SetField("gobble_headdrama", 0);
            };
        }

        private static GobbleGumType RandomItem()
        {
            int count = _allGobblegum.Count;

            var dic = new Dictionary<KeyValuePair<int, int>, GobbleGumType>();
            int index = 1;
            foreach (var item in _allGobblegum)
            {
                int start = index;
                int end = index + item.Weight - 1;
                index += end + 1;
                dic.Add(new KeyValuePair<int, int>(start, end), item.Type);
            }

            var rng = Utility.Random.Next(1, index);
            var tar = (
                from r in dic
                where rng >= r.Key.Key || rng <= r.Key.Value
                select r.Value).ToList()[0];

            return tar;
        }

        public static void GetRandomGobbleGum(Entity player)
        {
            if (player.GetField<GobbleGumType>("gobbletype") != GobbleGumType.None)
                return;

            var type = RandomItem();
            if (
                (player.GetField<int>("gobble_coagulant") == 1 && type == GobbleGumType.Coagulant) ||
                (player.GetField<int>("gobble_inplainsight") == 1 && type == GobbleGumType.InPlainSight) ||
                (player.GetField<int>("gobble_stockoption") == 1 && type == GobbleGumType.StockOption) ||
                (player.GetField<int>("gobble_swordflay") == 1 && type == GobbleGumType.SwordFlay) ||
                (player.GetField<int>("gobble_dangerclosest") == 1 && type == GobbleGumType.DangerClosest) ||
                (player.GetField<int>("gobble_recon") == 1 && type == GobbleGumType.Recon) ||
                (player.GetField<int>("gobble_aftertaste") == 1 && type == GobbleGumType.Aftertaste) ||
                (player.GetField<int>("gobble_unquenchable") == 1 && type == GobbleGumType.Unquenchable) ||
                (player.GetField<int>("gobble_killingtime") == 1 && type == GobbleGumType.KillingTime) ||
                (player.GetField<int>("gobble_headdrama") == 1 && type == GobbleGumType.KillingTime) ||
                (Utility.GetDvar<int>("global_gobble_burnedout") == 1 && type == GobbleGumType.BurnedOut) ||
                (Utility.GetDvar<int>("global_gobble_temporalgift") == 1 && type == GobbleGumType.TemporalGift)
                )
            {
                GetRandomGobbleGum(player);
                return;
            }
        }

        public static void GiveGobbleGum(Entity player, GobbleGum gobblegum)
        {
            player.SetCurrentGobbleGum(gobblegum);
        }

        public static void ActiveGobbleGum(Entity player)
        {
            var gobble = player.GetCurrentGobbleGum();
            gobble.Activation(player);

            player.AfterDelay(100, e => player.SetCurrentGobbleGum(new GobbleGum(GobbleGumType.None)));
        }

        private static void PrintGobbleGumInfo(Entity player, GobbleGum gobblegum)
        {
            string alias;
            if (gobblegum.Weight == 10)
            {
                alias = "^2";
            }
            else if (gobblegum.Weight == 7)
            {
                alias = "^5";
            }
            else if (gobblegum.Weight == 4)
            {
                alias = "^6";
            }
            else
            {
                alias = "^3";
            }

            player.GobbleGumHud(gobblegum.Name, gobblegum.Info);
            Utility.Println(player.Name + " get a Gobble Gum - " + alias + gobblegum.Name);

            AIZDebug.DebugLog(typeof(GobbleGumFunction), player.Name + " get a Gobble Gum - " + alias + gobblegum.Name);
        }
    }
}
