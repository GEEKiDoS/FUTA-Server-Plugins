using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class GobbleGum : BaseScript
    {
        private struct GobbleInfo
        {
            public string Name;
            public string Info;

            public GobbleInfo(string name, string info)
            {
                Name = name;
                Info = info;
            }
        }

        private static readonly Dictionary<GobbleType, GobbleInfo> gobblegums = new Dictionary<GobbleType, GobbleInfo>();
        private static readonly List<KeyValuePair<int, GobbleType>> rolls = new List<KeyValuePair<int, GobbleType>>
        {
            new KeyValuePair<int, GobbleType>(10,GobbleType.AlwaysDoneSwiftly),
            new KeyValuePair<int, GobbleType>(10,GobbleType.NoGamble),
            new KeyValuePair<int, GobbleType>(10,GobbleType.Coagulant),
            new KeyValuePair<int, GobbleType>(10,GobbleType.InPlainSight),
            new KeyValuePair<int, GobbleType>(10,GobbleType.StockOption),
            new KeyValuePair<int, GobbleType>(7,GobbleType.Wiper),
            new KeyValuePair<int, GobbleType>(7,GobbleType.SwordFlay),
            new KeyValuePair<int, GobbleType>(7,GobbleType.DangerClosest),
            new KeyValuePair<int, GobbleType>(7,GobbleType.Antidote),
            new KeyValuePair<int, GobbleType>(7,GobbleType.LuckyCrit),
            new KeyValuePair<int, GobbleType>(7,GobbleType.Recon),
            new KeyValuePair<int, GobbleType>(7,GobbleType.Strongbox),
            new KeyValuePair<int, GobbleType>(4,GobbleType.Aftertaste),
            new KeyValuePair<int, GobbleType>(4,GobbleType.BurnedOut),
            new KeyValuePair<int, GobbleType>(4,GobbleType.DeadOfNuclearWinter),
            new KeyValuePair<int, GobbleType>(4,GobbleType.IAmFeelingLucky),
            new KeyValuePair<int, GobbleType>(4,GobbleType.ImmolationLiquidation),
            new KeyValuePair<int, GobbleType>(4,GobbleType.LicensedContractor),
            new KeyValuePair<int, GobbleType>(4,GobbleType.PhoenixUp),
            new KeyValuePair<int, GobbleType>(4,GobbleType.PopShocks),
            new KeyValuePair<int, GobbleType>(4,GobbleType.RespinCycle),
            new KeyValuePair<int, GobbleType>(4,GobbleType.Unquenchable),
            new KeyValuePair<int, GobbleType>(4,GobbleType.WhosKeepingScore),
            new KeyValuePair<int, GobbleType>(4,GobbleType.CacheBack),
            new KeyValuePair<int, GobbleType>(4,GobbleType.KillJoy),
            new KeyValuePair<int, GobbleType>(4,GobbleType.WalkingDeath),
            new KeyValuePair<int, GobbleType>(4,GobbleType.TemporalGift),
            new KeyValuePair<int, GobbleType>(1,GobbleType.KillingTime),
            new KeyValuePair<int, GobbleType>(1,GobbleType.Perkaholic),
            new KeyValuePair<int, GobbleType>(1,GobbleType.HeadDrama),
        };

        public enum GobbleType
        {
            None,
            // 最常见
            AlwaysDoneSwiftly, //自动激活，瞄准时移动更快
            NoGamble, //当自己赌博抽到You live or die或You infected，自己或其他人抽到Other humans die时自动激活，可以保护自己不被赌博机击杀，持续1次
            Coagulant, //自动激活，缩短赌博机倒计时时间
            InPlainSight, //激活后，10秒内无法被僵尸看到
            StockOption, //自动激活，1分钟内，射击消耗备弹而不是弹匣
            // 较常见
            Wiper, //被Boomer效果影响后自动激活，消除Boomer的影响，持续1次
            SwordFlay, //自动激活，1分钟内，近战一击必杀
            DangerClosest, //激活后，10秒内免疫所有僵尸的伤害，包括近战和爆炸伤害
            Antidote, //被Spider效果影响时自动激活，消除Spider的伤害，持续1次
            LuckyCrit, //激活后，重新运行一次赌博机，只能在进行赌博后的10秒内激活
            Recon, //自动激活，1分钟内所有僵尸都会有红框标记
            Strongbox, //当有人从赌博机中抽到Surprise和Robbed all money时激活，保护自己的现金和Bonus Points不会损失，持续1次
            // 稀有
            Aftertaste, //拥有Quick Revive后激活，被僵尸干掉复活不会丢失Perk-a-Cola
            BurnedOut, //激活后，1分钟内所有僵尸重生只能Roll到减弱而不能Roll到增强
            DeadOfNuclearWinter, //激活后立刻获得一个Nuke Power-Up
            IAmFeelingLucky, //激活后立刻获得一个随机Power-Up
            ImmolationLiquidation, //激活后立刻获得一个Fire Sale Power-Up
            LicensedContractor, //激活后立刻获得一个Carpenter Power-Up
            PhoenixUp, //激活后，随机将一个僵尸重生为人类
            PopShocks, //被僵尸近战攻击自动激活，被僵尸近战攻击立即将周围大范围内僵尸冻住并免疫本次攻击造成的伤害，持续1次
            RespinCycle, //激活后，立刻更新所有人的武器
            Unquenchable, //当Perk-a-Cola达到5个时候自动激活，可以额外购买一个Perk-a-Cola
            WhosKeepingScore, //激活后立刻获得一个Double Points Power-Up
            CacheBack, //激活后立刻获得一个Max Ammo Power-Up
            KillJoy, //激活后立刻获得一个Insta-Kill Power-Up
            WalkingDeath, //激活后立刻减慢所有存活僵尸的移动速度
            TemporalGift, //激活后，延长Power-Up的保留时间
            // 超级稀有
            KillingTime, //激活后，15秒内所有被命中的僵尸都会被冻住，如果没有被击杀则会在10秒后自爆
            Perkaholic, //当幸存人类数少于3个时激活，立即给与幸存人类所有Perk-a-Cola
            HeadDrama, //激活后，所有对僵尸的伤害都会附加损伤僵尸的头，持续1分钟
        }

        public GobbleGum()
        {
            gobblegums.Add(GobbleType.AlwaysDoneSwiftly, new GobbleInfo("Always Done Swiftly", "Walk faster when aiming. Activates immediately."));
            gobblegums.Add(GobbleType.NoGamble, new GobbleInfo("No Gamble", "Protect you not killed by Gambler. For once."));
            gobblegums.Add(GobbleType.Coagulant, new GobbleInfo("Coagulant", "Shorted gamble time. Activates immediately."));
            gobblegums.Add(GobbleType.InPlainSight, new GobbleInfo("In Plain Sight", "The player is ignored by zombies for 10 seconds. Activates manual."));
            gobblegums.Add(GobbleType.StockOption, new GobbleInfo("Stock Option", "Ammo is taken from the player's stockpile instead of their weapon's magazine. For 1 min. Activates immediately."));
            gobblegums.Add(GobbleType.Wiper, new GobbleInfo("Wiper", "Eliminate the effects of Boomer. For once."));
            gobblegums.Add(GobbleType.SwordFlay, new GobbleInfo("Sword Flay", "Melee attacks and any melee weapon will one hit kill on zombies. Activates immediately."));
            gobblegums.Add(GobbleType.DangerClosest, new GobbleInfo("Danger Closest", "Zero any damage from zombies for 10 second. Activates manual."));
            gobblegums.Add(GobbleType.Antidote, new GobbleInfo("Danger Closest", "Eliminate the damage of Spider. For once."));
            gobblegums.Add(GobbleType.LuckyCrit, new GobbleInfo("Lucky Crit", "Restart the Gambler. Only can active for gambled 10 second. Activates manual."));
            gobblegums.Add(GobbleType.Recon, new GobbleInfo("Recon", "Mark all zombies for 1 min. Activates immediately."));
            gobblegums.Add(GobbleType.Strongbox, new GobbleInfo("Strongbox", "Protect you cash and Bouns Point not take by otherone. For once."));
            gobblegums.Add(GobbleType.Aftertaste, new GobbleInfo("Aftertaste", "Not lose all Perk-a-Cola if you killed by zombie. Activates immediately if have Quick Revive."));
            gobblegums.Add(GobbleType.BurnedOut, new GobbleInfo("Burned Out", "Limit zombies only can roll bad item for 1 min. Activates immediately."));
            gobblegums.Add(GobbleType.DeadOfNuclearWinter, new GobbleInfo("Dead of Nuclear Winter", "Spawns a Nuke Power-Up. Activates manual."));
            gobblegums.Add(GobbleType.IAmFeelingLucky, new GobbleInfo("I am Feeling Lucky", "Spawns a random Power-Up. Activates manual."));
            gobblegums.Add(GobbleType.ImmolationLiquidation, new GobbleInfo("Immolation Liquidation", "Spawns a Fire Sale Power-Up. Activates manual."));
            gobblegums.Add(GobbleType.LicensedContractor, new GobbleInfo("Licensed Contractor", "Spawns a Carpenter Power-Up. Activates manual."));
            gobblegums.Add(GobbleType.PhoenixUp, new GobbleInfo("Phoenix Up", "Change a random zombie to human. Activates manual."));
            gobblegums.Add(GobbleType.PopShocks, new GobbleInfo("Pop Shocks", "Freeze nearby zombies if zombie hit you. And you immunity this attack damage. For once."));
            gobblegums.Add(GobbleType.RespinCycle, new GobbleInfo("Respin Cycle", "Cycle all humans weapon. Activates manual."));
            gobblegums.Add(GobbleType.Unquenchable, new GobbleInfo("Unquenchable", "Can buy an extra Perk-a-Cola.  Activates immediately."));
            gobblegums.Add(GobbleType.WhosKeepingScore, new GobbleInfo("Who's Keeping Score", "Spawns a Double Points Power-Up. Activates manual."));
            gobblegums.Add(GobbleType.CacheBack, new GobbleInfo("Cache Back", "Spawns a Max Ammo Power-Up. Activates manual."));
            gobblegums.Add(GobbleType.KillJoy, new GobbleInfo("Kill Joy", "Spawns a Insta-Kill Power-Up. Activates manual."));
            gobblegums.Add(GobbleType.WalkingDeath, new GobbleInfo("Walking Death", "Slow down all zombies move speed. Activates immediately."));
            gobblegums.Add(GobbleType.TemporalGift, new GobbleInfo("Temporal Gift", "Power-Ups last longer. Activates immediately."));
            gobblegums.Add(GobbleType.KillingTime, new GobbleInfo("Killing Time", "For 15 second. Freeze zombies if they hit by you. If they not killed for 10 second. Then they will exploed. Activates immediately."));
            gobblegums.Add(GobbleType.Perkaholic, new GobbleInfo("Perkaholic", "Give all survivor all Perk-a-Colas. Activates if only have 2 survivor."));
            gobblegums.Add(GobbleType.HeadDrama, new GobbleInfo("Head Drama", "Any bullet which hits a zombie will damage its head for 1 min. Activates immediately."));

            PlayerConnected += player =>
            {
                player.SetField("gobbletype", new Parameter(GobbleType.None));
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

        private static GobbleType RandomItem()
        {
            KeyValuePair<int, GobbleType> pair2;
            int maxvalue = rolls.Count;

            var list = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < rolls.Count; i++)
            {
                pair2 = rolls[i];
                var num = pair2.Key + Utility.Rng.Next(0, maxvalue);
                list.Add(new KeyValuePair<int, int>(i, num));
            }

            list.Sort((kvp1, kvp2) => (kvp2.Value - kvp1.Value));
            var pair = rolls[list[0].Key];

            return pair.Value;
        }

        private static void GetRandomGobbleGum(Entity player)
        {
            if (player.GetField<GobbleType>("gobbletype") != GobbleType.None)
                return;

            var type = RandomItem();
            while (
                (player.GetField<int>("gobble_coagulant") == 1 && type == GobbleType.Coagulant) ||
                (player.GetField<int>("gobble_inplainsight") == 1 && type == GobbleType.InPlainSight) ||
                (player.GetField<int>("gobble_stockoption") == 1 && type == GobbleType.StockOption) ||
                (player.GetField<int>("gobble_swordflay") == 1 && type == GobbleType.SwordFlay) ||
                (player.GetField<int>("gobble_dangerclosest") == 1 && type == GobbleType.DangerClosest) ||
                (player.GetField<int>("gobble_recon") == 1 && type == GobbleType.Recon) ||
                (player.GetField<int>("gobble_aftertaste") == 1 && type == GobbleType.Aftertaste) ||
                (player.GetField<int>("gobble_unquenchable") == 1 && type == GobbleType.Unquenchable) ||
                (player.GetField<int>("gobble_killingtime") == 1 && type == GobbleType.KillingTime) ||
                (player.GetField<int>("gobble_headdrama") == 1 && type == GobbleType.KillingTime) ||
                (Function.Call<int>("getdvarint", "global_gobble_temporalgift") == 1 && type == GobbleType.TemporalGift)
                )
            {
                type = RandomItem();
            }
        }

        private static void PrintGobbleGumInfo()
        {

        }

        public void Activation(Entity player, GobbleType type)
        {

        }
    }
}
