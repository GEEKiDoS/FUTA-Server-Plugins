using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public static class BoxFunction
    {
        #region Cash Point System

        public static int GetCash(this Entity player)
        {
            return player.GetField<int>("aiz_cash");
        }

        public static void WinCash(this Entity player, int cash)
        {
            player.SetField("aiz_cash", player.GetField<int>("aiz_cash") + cash);
        }

        public static void PayCash(this Entity player, int cash)
        {
            player.SetField("aiz_cash", player.GetField<int>("aiz_cash") - cash);
        }

        public static int GetPoint(this Entity player)
        {
            return player.GetField<int>("aiz_point");
        }

        public static void WinPoint(this Entity player, int point)
        {
            player.SetField("aiz_point", player.GetField<int>("aiz_point") + point);
        }

        public static void PayPoint(this Entity player, int point)
        {
            player.SetField("aiz_point", player.GetField<int>("aiz_point") - point);
        }

        #endregion

        public static void UseDoor(Entity door, Entity player)
        {
            if (!player.IsAlive) return;
            if (door.GetField<int>("hp") > 0)
            {
                if (player.GetTeam() == "allies")
                {
                    if (door.GetField<string>("state") == "open")
                    {
                        door.Call("moveto", door.GetField<Vector3>("close"), 1);
                        door.AfterDelay(300, ent =>
                        {
                            door.SetField("state", "close");
                        });
                    }
                    else if (door.GetField<string>("state") == "close")
                    {
                        door.Call("moveto", door.GetField<Vector3>("open"), 1);
                        door.AfterDelay(300, ent =>
                        {
                            door.SetField("state", "open");
                        });
                    }
                }
                else if (player.GetTeam() == "axis")
                {
                    if (door.GetField<string>("state") == "close")
                    {
                        if (player.GetField<int>("attackeddoor") == 0)
                        {
                            int hitchance = 0;
                            switch (player.Call<string>("getstance"))
                            {
                                case "prone":
                                    hitchance = 20;
                                    break;
                                case "couch":
                                    hitchance = 45;
                                    break;
                                case "stand":
                                    hitchance = 90;
                                    break;
                                default:
                                    break;
                            }
                            if (Utility.Rng.Next(100) < hitchance)
                            {
                                door.SetField("hp", door.GetField<int>("hp") - 1);
                                player.Call("iprintlnbold", "HIT: " + door.GetField<int>("hp") + "/" + door.GetField<int>("maxhp"));
                            }
                            else
                            {
                                player.Call("iprintlnbold", "^1MISS");
                            }
                            player.SetField("attackeddoor", 1);
                            player.AfterDelay(1000, (e) => player.SetField("attackeddoor", 0));
                        }
                    }
                }
            }
            else if (door.GetField<int>("hp") == 0 && door.GetField<string>("state") != "broken")
            {
                if (door.GetField<string>("state") == "close")
                    door.Call("moveto", door.GetField<Vector3>("open"), 1f);
                door.SetField("state", "broken");
            }
        }

        public static void UsePayDoor(Entity door, Entity player)
        {
            if (!player.IsAlive) return;
            if (door.GetField<string>("state") == "close" && player.GetTeam() == "allies")
            {
                if (player.GetCash() >= door.GetField<int>("pay"))
                {
                    player.PayCash(door.GetField<int>("pay"));
                    door.Call("moveto", door.GetField<Vector3>("open"), 1);
                    door.AfterDelay(300, ent =>
                    {
                        door.SetField("state", "open");
                    });
                }
                else
                {
                    player.Call("iprintln", "^1Not enough cash for cleanup barriers. Need ^2$^3" + door.GetField<int>("pay"));
                }
            }
        }

        public static void UseZipline(Entity ent, Entity player)
        {
            if (!player.IsAlive) return;
            if (ent.GetField<string>("state") == "idle")
            {
                var start = ent.Origin;
                ent.SetField("state", "using");

                ent.Call("clonebrushmodeltoscriptmodel", MapEdit._nullCollision);
                player.Call("playerlinkto", ent);
                ent.Call("moveto", ent.GetField<Vector3>("exit"), ent.GetField<int>("movetime"));
                ent.AfterDelay(ent.GetField<int>("movetime") * 1000, e =>
                {
                    if (player.Call<int>("islinked") != 0)
                    {
                        player.Call("unlink");
                        player.Call("setorigin", ent.GetField<Vector3>("exit"));
                    }
                    ent.Call("moveto", start, 1);
                });
                ent.AfterDelay(ent.GetField<int>("movetime") * 1000 + 2000, en =>
                {
                    ent.SetField("state", "idle");
                    ent.Call("clonebrushmodeltoscriptmodel", MapEdit._airdropCollision);
                });
            }
        }

        public static void UseTeleporter(Entity ent, Entity player)
        {
            if (!player.IsAlive) return;
            if (player.GetTeam() == "allies" && player.GetField<int>("usingtelepot") == 0)
            {
                if (player.GetCash() >= 500)
                {
                    player.PayCash(500);
                    player.SetField("usingtelepot", 1);
                    Vector3 start = player.Origin;
                    player.Call("shellshock", "frag_grenade_mp", 3);
                    player.AfterDelay(2000, e =>
                    {
                        player.Call("shellshock", "concussion_grenade_mp", 3);
                        player.Call("setorigin", ent.GetField<Vector3>("exit"));
                    });
                    player.AfterDelay(32000, en =>
                    {
                        if (player.GetTeam() == "allies")
                        {
                            if (player.Call<int>("islinked") != 0)
                            {
                                player.Call("unlink");
                            }
                            player.Call("shellshock", "concussion_grenade_mp", 3);
                            player.Call("setorigin", start);
                            player.SetField("usingtelepot", 0);
                        }
                    });
                }
                else
                {
                    player.Call("iprintln", "^1Not enough cash for Teleporter. Need ^2$^3500");
                }
            }
        }

        public static void UseTrampoline(Entity ent, Entity player)
        {
            if (!player.IsAlive) return;
            var vel = player.Call<Vector3>("getvelocity");
            player.Call("setvelocity", new Vector3(vel.X, vel.Y, ent.GetField<int>("high")));
        }

        public static void UsePower(Entity ent, Entity player)
        {
            if (!player.IsAlive) return;
            if (player.GetTeam() == "allies")
            {
                if (player.GetCash() >= 700)
                {
                    player.PayCash(700);
                    ent.SetField("player", player.Name);
                    Function.SetEntRef(-1);
                    Function.Call("setdvar", "scr_aiz_power", 2);
                }
                else
                {
                    player.Call("iprintln", "^1Not enough cash for activate the electricity. Need ^2$^3700");
                }
            }
        }

        public static void UseAmmo(Entity player)
        {
            if (!player.IsAlive) return;
            if (player.GetTeam() == "allies")
            {
                if (player.GetCash() >= 100)
                {
                    player.PayCash(100);
                    player.GiveAmmo();
                    player.Call("playlocalsound", "ammo_crate_use");
                }
                else
                {
                    player.Call("iprintln", "^1Not enough cash for Ammo. Need ^2$^3100");
                }
            }
        }

        public static void UseGambler(Entity ent, Entity player)
        {
            if (!player.IsAlive) return;
            if (ent.GetField<string>("state") != "idle") return;
            if (player.GetTeam() == "allies")
            {
                if (player.GetCash() >= 500)
                {
                    player.PayCash(500);
                    ent.SetField("state", "using");
                    Entity laptop = ent.GetField<Entity>("laptop");
                    laptop.Call("moveto", laptop.Origin + new Vector3(0, 0, 30), 2);
                    laptop.AfterDelay(8000, e =>
                    {
                        laptop.Call("moveto", laptop.Origin - new Vector3(0, 0, 30), 2);
                    });
                    laptop.AfterDelay(10000, e =>
                    {
                        ent.SetField("state", "idle");
                    });
                    player.Gamble();
                }
                else
                {
                    player.Call("iprintln", "^1Not enough cash for Gambler. Need ^2$^3500");
                }
            }
        }

        public static void UseAirstrike(Entity player)
        {
            if (!player.IsAlive) return;
            if (player.GetTeam() == "allies")
            {
                if (player.GetPoint() >= 10)
                {
                    player.PayPoint(10);
                    player.GiveRandomAirstrike();
                }
                else
                {
                    player.Call("iprintln", "^1Not enough ^5Bonus Points ^1for Airstrike. Need ^310 ^5Bonus Points");
                }
            }
        }

        public static void UsePerk(Entity player, PerkCola perk)
        {
            if (!player.IsAlive) return;
            if (player.GetTeam() == "allies")
            {
                if (player.GetCash() >= perk.Pay)
                {
                    if (player.GetField<int>("aiz_perks") >= 5)
                    {
                        player.Call("iprintln", "^1You already have 5 Perk-a-Cola.");
                        return;
                    }
                    if (PerkCola.HasPerkCola(player, perk))
                    {
                        player.Call("iprintln", "^1You already have " + perk.FullName + ".");
                        return;
                    }
                    player.PayCash(perk.Pay);
                    PerkCola.GivePerkCola(player, perk);
                }
                else
                {
                    player.Call("iprintln", "^1Not enough cash for " + perk.FullName + ". Need ^2$^3" + perk.Pay);
                }
            }
        }

        public static void UseRandomPerk(Entity ent, Entity player)
        {
            if (!player.IsAlive) return;
            if (player.GetTeam() == "allies")
            {
                if (player.GetPoint() >= 10)
                {
                    if (player.GetField<int>("aiz_perks") >= 5)
                    {
                        player.Call("iprintln", "^1You already have 5 Perk-a-Cola.");
                        return;
                    }
                    player.PayPoint(10);
                    player.RandomPerk(ent);
                }
                else
                {
                    player.Call("iprintln", "^1Not enough ^5Bonus Points ^1for Der Wunderfizz. Need ^310 ^5Bonus Points");
                }
            }
        }

        public static void GiveAmmo(this Entity player)
        {
            player.Call("givemaxammo", Sharpshooter._currentWeapon.Code);
            if (player.GetField<int>("perk_mulekick") == 1)
            {
                player.Call("givemaxammo", Sharpshooter._mulekickWeapon.Code);
            }

            if (!player.HasWeapon("trophy_mp"))
            {
                player.GiveWeapon("trophy_mp");
            }
            if (!player.HasWeapon("frag_grenade_mp"))
            {
                player.GiveWeapon("frag_grenade_mp");
            }
            player.Call("setweaponammoclip", "trophy_mp", 99);
            player.Call("setweaponammoclip", "flag_grenade_mp", 99);
            player.Call("givemaxammo", "trophy_mp");
            player.Call("givemaxammo", "flag_grenade_mp");
        }

        private static void Gamble(this Entity player)
        {
            player.Call("iprintlnbold", new Parameter[] { "^210" });
            player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" });
            player.AfterDelay(1000, e => player.Call("iprintlnbold", "^29"));
            player.AfterDelay(1000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(2000, e => player.Call("iprintlnbold", "^28"));
            player.AfterDelay(2000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(3000, e => player.Call("iprintlnbold", "^27"));
            player.AfterDelay(3000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(4000, e => player.Call("iprintlnbold", "^26"));
            player.AfterDelay(4000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(5000, e => player.Call("iprintlnbold", "^25"));
            player.AfterDelay(5000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(6000, e => player.Call("iprintlnbold", "^24"));
            player.AfterDelay(6000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(7000, e => player.Call("iprintlnbold", "^23"));
            player.AfterDelay(7000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(8000, e => player.Call("iprintlnbold", "^22"));
            player.AfterDelay(8000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(9000, e => player.Call("iprintlnbold", "^21"));
            player.AfterDelay(9000, e => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
            player.AfterDelay(10000, e =>
            {
                switch (Utility.Rng.Next(19))
                {
                    case 0:
                        player.PrintGambleInfo("You win nothing.", GambleType.Bad);
                        return;
                    case 1:
                        player.PrintGambleInfo("You win $500.", GambleType.Good);
                        player.WinCash(500);
                        return;
                    case 2:
                        player.PrintGambleInfo("You win $1000.", GambleType.Good);
                        player.WinCash(1000);
                        return;
                    case 3:
                        player.PrintGambleInfo("You win $2000.", GambleType.Good);
                        player.WinCash(2000);
                        return;
                    case 4:
                        player.PrintGambleInfo("You lose $500.", GambleType.Bad);
                        player.WinCash(500);
                        return;
                    case 5:
                        player.PrintGambleInfo("You lose all money.", GambleType.Bad);
                        player.SetField("aiz_cash", 0);
                        return;
                    case 6:
                        player.PrintGambleInfo("You win $10000.", GambleType.Excellent);
                        player.WinCash(10000);
                        return;
                    case 7:
                        player.PrintGambleInfo("You win 10 Bouns Points.", GambleType.Good);
                        player.WinPoint(10);
                        return;
                    case 8:
                        player.PrintGambleInfo("You win 50 Bouns Points.", GambleType.Excellent);
                        player.WinPoint(50);
                        return;
                    case 9:
                        player.PrintGambleInfo("You lose all Bouns Points.", GambleType.Bad);
                        player.SetField("aiz_point", 0);
                        return;
                    case 10:
                        player.PrintGambleInfo("You live or die after 5 second.", GambleType.Bad);
                        player.AfterDelay(1000, ex => player.Call("iprintlnbold", "^15"));
                        player.AfterDelay(1000, ex => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
                        player.AfterDelay(2000, ex => player.Call("iprintlnbold", "^14"));
                        player.AfterDelay(2000, ex => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
                        player.AfterDelay(3000, ex => player.Call("iprintlnbold", "^13"));
                        player.AfterDelay(3000, ex => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
                        player.AfterDelay(4000, ex => player.Call("iprintlnbold", "^12"));
                        player.AfterDelay(4000, ex => player.Call("playlocalsound", "ui_mp_nukebomb_timer"));
                        player.AfterDelay(5000, ex => player.Call("iprintlnbold", "^11"));
                        player.AfterDelay(6000, ex =>
                        {
                            switch (Utility.Rng.Next(2))
                            {
                                case 0:
                                    player.PrintGambleInfo("You live!", GambleType.Good);
                                    break;
                                case 1:
                                    player.PrintGambleInfo("You die!", GambleType.Bad);
                                    player.SelfExploed();
                                    break;
                            }
                        });
                        return;
                    case 11:
                        player.PrintGambleInfo("Gambler Restart.", GambleType.Bad);
                        player.AfterDelay(1000, ex => player.Gamble());
                        return;
                    case 12:
                        player.PrintGambleInfo("Wallhack for 1 min.", GambleType.Good);
                        player.Call("thermalvisionfofoverlayon");
                        player.AfterDelay(60000, ex =>
                        {
                            player.Call("iprintlnbold", "Wallhack off!");
                            player.Call("thermalvisionfofoverlayoff");
                        });
                        return;
                    case 13:
                        player.PrintGambleInfo("Incantation", GambleType.Bad);
                        player.SetField("incantation", 1);
                        return;
                    case 14:
                        player.PrintGambleInfo("Give all human $500", GambleType.Excellent);
                        player.WinCash(500);
                        foreach (var item in Utility.GetPlayers())
                        {
                            if (item.GetTeam() == "allies" && item.IsAlive && item != player)
                            {
                                item.WinCash(500);
                                item.GamblerText("Player " + player.Name + " give you $500.", new Vector3(1, 1, 1), new Vector3(0, 1, 0), 1, 0.85f);
                            }
                        }
                        return;
                    case 15:
                        player.PrintGambleInfo("You infected.", GambleType.Bad);
                        player.SelfExploed();
                        return;
                    case 16:
                        player.PrintGambleInfo("You lose all weapon.", GambleType.Bad);
                        player.TakeAllWeapons();
                        return;
                    case 17:
                        player.PrintGambleInfo("You win riotshield in your back.", GambleType.Good);
                        player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back");
                        return;
                    case 18:
                        player.PrintGambleInfo("Surprise!", GambleType.Terrible);
                        foreach (var item in Utility.GetPlayers())
                        {
                            if (item.GetTeam() == "allies" && item.IsAlive)
                            {
                                item.SetField("aiz_cash", 0);
                                item.SetField("aiz_point", 0);
                                if (player != item)
                                {
                                    item.GamblerText("Surprise!", new Vector3(1, 1, 1), new Vector3(0, 0, 0), 1, 0.85f);
                                }
                            }
                        }
                        return;
                }
            });
        }

        private enum GambleType
        {
            Good,
            Bad,
            Excellent,
            Terrible,
        }

        private static void PrintGambleInfo(this Entity player, string text, GambleType type)
        {
            switch (type)
            {
                case GambleType.Good:
                    player.GamblerText(text, new Vector3(1, 1, 1), new Vector3(0, 1, 0), 1, 0.85f);
                    Function.SetEntRef(-1);
                    Function.Call("iprintln", player.Name + " gambled - ^2" + text);
                    break;
                case GambleType.Bad:
                    player.GamblerText(text, new Vector3(1, 1, 1), new Vector3(1, 0, 0), 1, 0.85f);
                    Function.SetEntRef(-1);
                    Function.Call("iprintln", player.Name + " gambled - ^1" + text);
                    break;
                case GambleType.Excellent:
                    player.GamblerText(text, new Vector3(1, 1, 1), new Vector3(1, 1, 0), 1, 0.85f);
                    Function.SetEntRef(-1);
                    Function.Call("iprintln", player.Name + " gambled - ^3" + text);
                    break;
                case GambleType.Terrible:
                    player.GamblerText(text, new Vector3(0, 0, 0), new Vector3(1, 1, 1), 1, 0.85f);
                    Function.SetEntRef(-1);
                    Function.Call("iprintln", player.Name + " gambled - ^0" + text);
                    break;
            }
        }

        private static void GiveRandomAirstrike(this Entity player)
        {
            player.GamblerText("Comming Soon!", new Vector3(1, 1, 1), new Vector3(0.3f, 0.3f, 0.9f), 1, 0.85f);
        }

        private static void RandomPerk(this Entity player, Entity ent)
        {
            var perk = PerkCola.RandomPerk();
            while (PerkCola.HasPerkCola(player, perk))
            {
                perk = PerkCola.RandomPerk();
            }
            PerkCola.GivePerkCola(player, perk);
        }
    }
}
