using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class ZombieRollTheDice : BaseScript
    {
        private bool hasDiscoZombie = false;
        private Entity curretDiscoZombie = null;

        private enum RollType
        {
            Nothing,
            FlagZombie,
            StaminUp,
            OneHitKill,
            Juggernaut,
            SMAW,
            RPG,
            Stinger,
            AA12,
            Spas,
            Turtle,
            Javelin,
            Riotshield,
            KingOfJuggernaut,
            DesertEagle,
            ThrowingKnife,
            M320,
            XM25,
            MK14,
            SVD,
            ISIS,
            ISISJuggernaut,
            ZombieIncantation,
            Tombstone,
            Tank,
            Boomer,
            Spider,
            ExoZombie,
            ExoJuggernaut,
            ExoISISZombie,
            DiscoZombie,
        }

        private enum RollGoodType
        {
            Bad,
            Good,
            Excellent
        }

        private static readonly List<KeyValuePair<int, RollType>> Rolls = new List<KeyValuePair<int, RollType>>
        {
            new KeyValuePair<int, RollType>(8, RollType.Nothing),
            new KeyValuePair<int, RollType>(8, RollType.FlagZombie),
            new KeyValuePair<int, RollType>(4, RollType.StaminUp),
            new KeyValuePair<int, RollType>(2, RollType.OneHitKill),
            new KeyValuePair<int, RollType>(4, RollType.Juggernaut),
            new KeyValuePair<int, RollType>(1, RollType.SMAW),
            new KeyValuePair<int, RollType>(1, RollType.RPG),
            new KeyValuePair<int, RollType>(3, RollType.Stinger),
            new KeyValuePair<int, RollType>(1, RollType.AA12),
            new KeyValuePair<int, RollType>(1, RollType.Spas),
            new KeyValuePair<int, RollType>(2, RollType.Turtle),
            new KeyValuePair<int, RollType>(1, RollType.Javelin),
            new KeyValuePair<int, RollType>(4, RollType.Riotshield),
            new KeyValuePair<int, RollType>(2, RollType.KingOfJuggernaut),
            new KeyValuePair<int, RollType>(1, RollType.DesertEagle),
            new KeyValuePair<int, RollType>(3, RollType.ThrowingKnife),
            new KeyValuePair<int, RollType>(1, RollType.M320),
            new KeyValuePair<int, RollType>(1, RollType.XM25),
            new KeyValuePair<int, RollType>(1, RollType.MK14),
            new KeyValuePair<int, RollType>(1, RollType.SVD),
            new KeyValuePair<int, RollType>(4, RollType.ISIS),
            new KeyValuePair<int, RollType>(1, RollType.ISISJuggernaut),
            new KeyValuePair<int, RollType>(4, RollType.ZombieIncantation),
            new KeyValuePair<int, RollType>(4, RollType.Tombstone),
            new KeyValuePair<int, RollType>(2, RollType.Tank),
            new KeyValuePair<int, RollType>(4, RollType.Boomer),
            new KeyValuePair<int, RollType>(4, RollType.Spider),
            new KeyValuePair<int, RollType>(4, RollType.ExoZombie),
            new KeyValuePair<int, RollType>(2, RollType.ExoJuggernaut),
            new KeyValuePair<int, RollType>(2, RollType.ExoISISZombie),
            new KeyValuePair<int, RollType>(2, RollType.DiscoZombie),
        };


        public ZombieRollTheDice()
        {
            PlayerConnected += player =>
            {
                player.SetField("rtd_canroll", 1);
                player.SetField("zombie_incantation", 0);
                player.SetField("rtd_flag", 0);
                player.SetField("rtd_king", 0);
                player.SetField("rtd_isis", 0);
                player.SetField("rtd_boomer", 0);
                player.SetField("rtd_spider", 0);
                player.SetField("rtd_crawler", 0);
                player.SetField("rtd_exo", 0);
                player.SetField("rtd_disco", 0);
                player.SetField("rtd_tombstone", 0);
                player.SetField("rtd_tombstoneorigin", new Vector3());

                player.Call("notifyonplayercommand", "attack", "+attack");
                player.OnNotify("attack", self =>
                {
                    if (player.GetTeam() == "axis" && player.GetField<int>("rtd_isis") == 1 && player.CurrentWeapon == "c4death_mp" && player.IsAlive && !IsClosingPhD(player))
                    {
                        player.SetField("rtd_isis", 0);
                        AfterDelay(1000, () => player.Notify("isis_exploed"));
                    }
                });

                OnSpawned(player);
                player.SpawnedPlayer += () => OnSpawned(player);
            };
        }

        public void OnSpawned(Entity player)
        {
            if (player.GetTeam() == "axis")
            {
                if (player.GetField<int>("rtd_canroll") == 1)
                {
                    player.AfterDelay(50, e => DoRandom(player));
                }
                if (player.GetField<int>("rtd_tombstone") == 1)
                {
                    player.Call("setorigin", player.GetField<Vector3>("rtd_tombstoneorigin"));
                }
                else if (hasDiscoZombie && curretDiscoZombie != null && curretDiscoZombie.IsPlayer && curretDiscoZombie.IsAlive && curretDiscoZombie.GetField<int>("rtd_disco") == 1)
                {
                    player.Call("setorigin", GetDiscoZombieDeployPoint());
                }
                player.SetField("speed", 1);
                player.SetField("rtd_canroll", 1);
                player.SetField("zombie_incantation", 0);
                player.SetField("rtd_flag", 0);
                player.SetField("rtd_king", 0);
                player.SetField("rtd_isis", 0);
                player.SetField("rtd_boomer", 0);
                player.SetField("rtd_spider", 0);
                player.SetField("rtd_exo", 0);
                player.SetField("rtd_disco", 0);
                player.SetField("rtd_tombstone", 0);
                player.SetField("rtd_tombstoneorigin", new Vector3());

                player.SetField("onhitacid", 0);
            }
        }

        public void DoRandom(Entity player)
        {
            var type = RandomItem();
            if (Utility.GetDvar<int>("global_gobble_burnedout")==1)
            {
                if (!GetRollFullName(type).StartsWith("^1"))
                {
                    DoRandom(player);
                    return;
                }
            }
            switch (type)
            {
                case RollType.Nothing:
                    break;
                case RollType.FlagZombie:
                    player.SetField("rtd_flag", 1);
                    player.Call("attach", GetCarryFlag(), "j_spine4", 1);
                    break;
                case RollType.StaminUp:
                    player.SetField("speed", 1.5f);
                    break;
                case RollType.OneHitKill:
                    player.SetField("maxhealth", 1);
                    player.Health = 1;
                    break;
                case RollType.Juggernaut:
                    player.Call("setmodel", "mp_fullbody_opforce_juggernaut");
                    player.Call("setviewmodel", "viewhands_juggernaut_opforce");
                    player.SetField("maxhealth", player.GetField<int>("maxhealth") * 3);
                    player.Health *= 3;
                    break;
                case RollType.SMAW:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_smaw_mp");
                    player.Call("setweaponammoclip", "iw5_smaw_mp", 1);
                    player.Call("setweaponammostock", "iw5_smaw_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("iw5_smaw_mp"));
                    break;
                case RollType.RPG:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("rpg_mp");
                    player.Call("setweaponammoclip", "rpg_mp", 1);
                    player.Call("setweaponammostock", "rpg_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("rpg_mp"));
                    break;
                case RollType.Stinger:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("stinger_mp");
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("stinger_mp"));
                    break;
                case RollType.AA12:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_aa12_mp");
                    player.Call("setweaponammostock", "iw5_aa12_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("iw5_aa12_mp"));
                    break;
                case RollType.Spas:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_spas12_mp");
                    player.Call("setweaponammostock", "iw5_spas12_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("iw5_spas12_mp"));
                    break;
                case RollType.Turtle:
                    player.SetField("speed", 0.7f);
                    break;
                case RollType.Javelin:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("javelin_mp");
                    player.Call("setweaponammostock", "javelin_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("javelin_mp"));
                    break;
                case RollType.Riotshield:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("riotshield_mp");
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("riotshield_mp"));
                    player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back");
                    break;
                case RollType.KingOfJuggernaut:
                    player.SetField("rtd_king", 1);
                    player.Call("attach", GetCarryFlag(), "j_spine4", 1);
                    player.Call("setmodel", "mp_fullbody_opforce_juggernaut");
                    player.Call("setviewmodel", "viewhands_juggernaut_opforce");
                    player.SetField("maxhealth", player.GetField<int>("maxhealth") * 10);
                    player.Health *= 10;
                    break;
                case RollType.DesertEagle:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_deserteagle_mp");
                    player.Call("setweaponammostock", "iw5_deserteagle_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("iw5_deserteagle_mp"));
                    break;
                case RollType.ThrowingKnife:
                    player.GiveWeapon("throwingknife_mp");
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("throwingknife_mp"));
                    break;
                case RollType.M320:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("m320_mp");
                    player.Call("setweaponammostock", "m320_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("m320_mp"));
                    break;
                case RollType.XM25:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("xm25_mp");
                    player.Call("setweaponammostock", "xm25_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("xm25_mp"));
                    break;
                case RollType.MK14:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_mk14_mp");
                    player.Call("setweaponammoclip", "iw5_mk14_mp", 1);
                    player.Call("setweaponammostock", "iw5_mk14_mp", 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("iw5_mk14_mp"));
                    break;
                case RollType.SVD:
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon(Utilities.BuildWeaponName("iw5_dragunov", "none", "none", 0, 0));
                    player.Call("setweaponammoclip", Utilities.BuildWeaponName("iw5_dragunov", "none", "none", 0, 0), 1);
                    player.Call("setweaponammostock", Utilities.BuildWeaponName("iw5_dragunov", "none", "none", 0, 0), 0);
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate(Utilities.BuildWeaponName("iw5_dragunov", "none", "none", 0, 0)));
                    break;
                case RollType.ISIS:
                    player.SetField("rtd_isis", 1);
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("c4death_mp");
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("c4death_mp"));
                    break;
                case RollType.ISISJuggernaut:
                    player.SetField("rtd_isis", 1);
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("c4death_mp");
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("c4death_mp"));
                    player.Call("setmodel", "mp_fullbody_opforce_juggernaut");
                    player.Call("setviewmodel", "viewhands_juggernaut_opforce");
                    player.SetField("maxhealth", player.GetField<int>("maxhealth") * 3);
                    player.Health *= 3;
                    break;
                case RollType.ZombieIncantation:
                    player.SetField("zombie_incantation", 1);
                    break;
                case RollType.Tombstone:
                    player.SetField("rtd_tombstone", 1);
                    break;
                case RollType.Tank:
                    player.Call("setmodel", "mp_fullbody_opforce_juggernaut");
                    player.Call("setviewmodel", "viewhands_juggernaut_opforce");
                    player.SetField("maxhealth", player.GetField<int>("maxhealth") * 3);
                    player.Health *= 3;
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("riotshield_mp");
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("riotshield_mp"));
                    player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back");
                    break;
                case RollType.Boomer:
                    player.SetField("rtd_boomer", 1);
                    Utility.SetZombieSniperModel(player);
                    break;
                case RollType.Spider:
                    player.SetField("rtd_spider", 1);
                    Utility.SetZombieSniperModel(player);
                    break;
                case RollType.ExoZombie:
                    player.SetField("rtd_exo", 1);
                    break;
                case RollType.ExoJuggernaut:
                    player.SetField("rtd_exo", 1);
                    player.Call("setmodel", "mp_fullbody_opforce_juggernaut");
                    player.Call("setviewmodel", "viewhands_juggernaut_opforce");
                    player.SetField("maxhealth", player.GetField<int>("maxhealth") * 3);
                    player.Health *= 3;
                    break;
                case RollType.ExoISISZombie:
                    player.SetField("rtd_exo", 1);
                    player.SetField("rtd_isis", 1);
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("c4death_mp");
                    player.AfterDelay(300, e => player.SwitchToWeaponImmediate("c4death_mp"));
                    break;
                case RollType.DiscoZombie:
                    if (hasDiscoZombie)
                    {
                        DoRandom(player);
                        return;
                    }
                    player.SetField("rtd_disco", 1);
                    hasDiscoZombie = true;
                    curretDiscoZombie = player;
                    player.SetField("maxhealth", player.GetField<int>("maxhealth") * 2);
                    player.Health *= 2;
                    break;
            }
            PrintRollName(player, type);
        }

        private string GetCarryFlag()
        {
            if (Utility.AfricaMaps.Contains(Utility.MapName))
            {
                return "prop_flag_africa_carry";
            }
            else if (Utility.ICMaps.Contains(Utility.MapName))
            {
                return "prop_flag_ic_carry";
            }
            else
            {
                return "prop_flag_speznas_carry";
            }
        }

        private static RollType RandomItem()
        {
            int count = Rolls.Count;
            var rolls = (from r in Rolls orderby r.Key select r).ToList();

            var dic = new Dictionary<KeyValuePair<int, int>, RollType>();
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

        private void PrintRollName(Entity player, RollType type)
        {
            player.PrintlnBold("You rolled: " + GetRollFullName(type));
            Utility.Println(player.Name + " rolled - " + GetRollFullName(type));

            AIZDebug.DebugLog(typeof(ZombieRollTheDice), player.Name + " rolled - " + GetRollFullName(type));
        }

        private string GetRollFullName(RollType type)
        {
            switch (type)
            {
                case RollType.Nothing:
                    return "^1Nothing";
                case RollType.FlagZombie:
                    return "^1Flag Zombie";
                case RollType.StaminUp:
                    return "^2HK Journalist Zombie";
                case RollType.OneHitKill:
                    return "^1You are one hit kill";
                case RollType.Juggernaut:
                    return "^2Juggernaut";
                case RollType.SMAW:
                    return "^2SMAW";
                case RollType.RPG:
                    return "^2RPG";
                case RollType.Stinger:
                    return "^1Stinger";
                case RollType.AA12:
                    return "^2One Magazine AA12";
                case RollType.Spas:
                    return "^2One Magazine SPAS-12";
                case RollType.Turtle:
                    return "^1Turtle";
                case RollType.Javelin:
                    return "^2Javelin";
                case RollType.Riotshield:
                    return "^2Riotshield";
                case RollType.KingOfJuggernaut:
                    return "^3King of the Juggernaut";
                case RollType.DesertEagle:
                    return "^2One Magazine Desert Eagle";
                case RollType.ThrowingKnife:
                    return "^2One Throwing Knife";
                case RollType.M320:
                    return "^2One Ammo M320";
                case RollType.XM25:
                    return "^2One Magazine XM25";
                case RollType.MK14:
                    return "^2One Ammo MK14";
                case RollType.SVD:
                    return "^2One Ammo SVD";
                case RollType.ISIS:
                    return "^2ISIS Zombie";
                case RollType.ISISJuggernaut:
                    return "^3ISIS Juggernaut";
                case RollType.ZombieIncantation:
                    return "^1Zombie Incantation";
                case RollType.Tombstone:
                    return "^2Tombstone";
                case RollType.Tank:
                    return "^3Tank";
                case RollType.Boomer:
                    return "^2Boomer";
                case RollType.Spider:
                    return "^2Spider";
                case RollType.ExoZombie:
                    return "^2Exo Zombie";
                case RollType.ExoJuggernaut:
                    return "^3Exo Juggernaut";
                case RollType.ExoISISZombie:
                    return "^3Exo ISIS Zombie";
                case RollType.DiscoZombie:
                    return "^3Disco Zombie";
                default:
                    return "";
            }
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (mod == "MOD_SUICIDE" || attacker == null || !attacker.IsPlayer || attacker.GetTeam() == player.GetTeam())
            {
                player.SetField("rtd_canroll", 0);
            }

            if (player.GetField<int>("rtd_flag") == 1)
            {
                player.Call("detach", GetCarryFlag(), "j_spine4");
            }
            if (player.GetField<int>("rtd_king") == 1)
            {
                player.Call("detach", GetCarryFlag(), "j_spine4");
            }
            if (player.GetField<int>("rtd_tombstone") == 1)
            {
                player.SetField("rtd_tombstoneorigin", player.Origin);
            }
            if (player.GetField<int>("rtd_boomer") == 1)
            {
                foreach (var item in GetClosingHumans(player))
                {
                    if (item.GetCurrentGobbleGum().Type==GobbleGumType.Wiper)
                    {
                        item.ActiveGobbleGum();
                    }
                    else
                    {
                        if (item.GetField<int>("perk_cherry") == 1)
                        {
                            item.Call("setblurforplayer", 6, 0.5f);
                            item.Call("shellshock", "concussion_grenade_mp", 2);
                            item.AfterDelay(2000, e =>
                            {
                                item.Call("setblurforplayer", 0, 0.3f);
                            });
                        }
                        else
                        {
                            item.Call("setblurforplayer", 6, 0.5f);
                            item.Call("shellshock", "concussion_grenade_mp", 5);
                            item.AfterDelay(5000, e =>
                            {
                                item.Call("setblurforplayer", 0, 0.3f);
                            });
                        }
                    }
                }
            }
            if (player.GetField<int>("rtd_spider") == 1)
            {
                SpiderAcidArea(player, player.Origin);
            }
            if (player.GetField<int>("rtd_disco") == 1)
            {
                hasDiscoZombie = false;
                curretDiscoZombie = null;
            }

            if (mod == "MOD_HEAD_SHOT")
            {
                player.Call("detachall");
            }
        }

        private bool IsClosingPhD(Entity player)
        {
            foreach (var item in Utility.Players)
            {
                if (item.GetTeam() == "allies" && item.GetField<int>("perk_phd") == 1)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Entity> GetClosingHumans(Entity player)
        {
            var list = new List<Entity>();
            foreach (var item in Utility.Players)
            {
                if (item.GetTeam() == "allies" && item.IsAlive && item.Origin.DistanceTo(player.Origin) <= 500)
                {
                    list.Add(item);
                }
            }

            return list;
        }

        private Vector3 GetDiscoZombieDeployPoint()
        {
            if (curretDiscoZombie == null || !curretDiscoZombie.IsPlayer || !curretDiscoZombie.IsAlive)
                return new Vector3();

            return curretDiscoZombie.Origin;
        }

        private void SpiderAcidArea(Entity player, Vector3 origin)
        {
            bool flag = true;
            var fx = Call<Entity>("spawnfx", Effects.redbeaconfx, origin);
            Call("triggerfx", fx);
            OnInterval(100, () =>
            {
                foreach (var item in Utility.Players)
                {
                    if (item.GetTeam() == "allies" && item.IsAlive && item.Origin.DistanceTo(origin) <= 300)
                    {
                        if (item.GetField<int>("onhitacid") == 0)
                        {
                            if (player.GetCurrentGobbleGum().Type==GobbleGumType.Antidote)
                            {
                                player.ActiveGobbleGum();
                                player.SetField("onhitacid", 1);
                                player.AfterDelay(6000,e=> player.SetField("onhitacid", 0));
                            }
                            else
                            {
                                SpiderAcidAreaThink(item, player);
                            }
                        }
                    }
                }

                return flag;
            });
            AfterDelay(6000, () =>
            {
                fx.Call("delete");
                flag = false;
            });
        }

        private void SpiderAcidAreaThink(Entity player, Entity attacker)
        {
            if (player.GetField<int>("perk_cherry") == 1)
            {
                player.Notify("acid_damage", attacker, 20);
            }
            else
            {
                player.Notify("acid_damage", attacker, 40);
            }
            player.SetField("onhitacid", 1);

            player.AfterDelay(1500, e => player.SetField("onhitacid", 0));
        }
    }
}
