using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class AIZMain : BaseScript
    {
        public AIZMain()
        {
            //Box
            Utility.PreCacheShader("hudicon_neutral");
            Utility.PreCacheShader("waypoint_ammo_friendly");
            Utility.PreCacheShader("cardicon_8ball");
            Utility.PreCacheShader("cardicon_tictacboom");
            Utility.PreCacheShader("cardicon_bulb");
            Utility.PreCacheShader("cardicon_award_jets");
            Utility.PreCacheShader("cardicon_bear");

            //Perks
            Utility.PreCacheShader("specialty_finalstand"); //Quick Revive
            Utility.PreCacheShader("specialty_fastreload"); //Speed Cola
            Utility.PreCacheShader("cardicon_juggernaut_1"); //Juggernog
            Utility.PreCacheShader("specialty_longersprint"); //Stamin-Up
            Utility.PreCacheShader("specialty_twoprimaries"); //Mule Kick
            Utility.PreCacheShader("specialty_moredamage"); //Double Tap
            Utility.PreCacheShader("cardicon_headshot"); //Dead Shot
            Utility.PreCacheShader("_specialty_blastshield"); //PhD
            Utility.PreCacheShader("cardicon_cod4"); //Electric Cherry
            Utility.PreCacheShader("cardicon_soap_bar"); //Widow's Wine
            Utility.PreCacheShader("specialty_scavenger"); //Vultrue Aid

            //Other
            Utility.PreCacheShader("compass_waypoint_target");
            Utility.PreCacheShader("waypoint_flag_friendly");
            Utility.PreCacheModel("prop_flag_neutral");
            Utility.PreCacheModel(Utility.GetFlagModel(Utility.MapName));
            Utility.PreCacheModel("weapon_scavenger_grenadebag");
            Utility.PreCacheModel("weapon_oma_pack");
            Utility.PreCacheModel("com_laptop_2_open");

            Call("setdvar", "scr_aiz_power", 1);

            //Bouns Drops
            Call("setdvar", "bouns_double_points", 0);
            Call("setdvar", "bouns_insta_kill", 0);
            Call("setdvar", "bouns_fire_sale", 0);
            Call("setdvar", "bouns_zombie_blood", 0);

            PlayerConnected += player =>
            {
                player.SetField("aiz_perkhuds", new Parameter(new List<HudElem>()));

                OnSpawned(player);
                player.SpawnedPlayer += () => OnSpawned(player);

                player.OnInterval(100, ent =>
                {
                    player.Call("setmovespeedscale", player.GetField<float>("speed"));
                    return player.IsPlayer;
                });

                #region Magic Weapons

                player.Call("notifyonplayercommand", "attack", "+attack");
                player.OnNotify("attack", self =>
                {
                    if (player.GetTeam() == "allies")
                    {
                        if (player.CurrentWeapon == "stinger_mp" && player.GetWeaponAmmoClip("stinger_mp") != 0)
                        {
                            if (player.Call<float>("playerads") >= 1f)
                            {
                                if (player.Call<int>("getweaponammoclip", player.CurrentWeapon) != 0)
                                {
                                    Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { player.Call<Vector3>("getplayerangles", new Parameter[0]) });
                                    Vector3 dsa = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                                    Call("magicbullet", new Parameter[] { "stinger_mp", player.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), dsa, self });
                                    player.Call("setweaponammoclip", player.CurrentWeapon, 0);
                                }
                            }
                            else
                            {
                                player.Call("iprintlnbold", "You must be aim first!");
                            }
                        }
                    }
                });

                player.OnNotify("weapon_fired", delegate (Entity self, Parameter weapon)
                {
                });

                #endregion

                var welcomemessages = new List<string>
                {
                    "Welcome " + player.Name,
                    "Project Cirno (INF3) v0.2.2 Beta",
                    "Create by A2ON.",
                    "Source code in: https://github.com/A2ON/",
                    "Current Map: "+Utility.MapName,
                    "Enjoy playing!",
                };

                player.WelcomeMessage(welcomemessages, new Vector3(1, 1, 1), new Vector3(1f, 0.5f, 1f), 1, 0.85f);

                player.CreateCashHud();
                player.CreatePointHud();
                player.Credits();

                //debug
                OnNotify("changeclass", (cmd, choice) =>
                {
                    Log.Debug(choice.As<string>());
                });
            };
        }

        public void OnSpawned(Entity player)
        {
            player.Call("freezecontrols", false);

            player.SetField("aiz_cash", 500);
            player.SetField("aiz_point", 0);

            player.OnInterval(100, e =>
            {
                if (player.GetField<int>("aiz_cash") >= 13000)
                {
                    player.SetField("aiz_cash", 13000);
                }
                if (player.GetField<int>("aiz_point") >= 200)
                {
                    player.SetField("aiz_point", 200);
                }

                return player.IsAlive;
            });

            player.SetField("speed", 1f);
            player.SetField("usingtelepot", 0);
            player.SetField("xpUpdateTotal", 0);

            player.SetField("aiz_perks", 0);

            if (player.GetTeam() == "allies")
            {
                PerkCola.ResetPerks(player);

                player.SetField("incantation", 0);

                player.Call("setviewmodel", "viewmodel_base_viewhands");

                player.Call("clearperks");
                player.SetPerk("specialty_assists", true, false);
                player.SetPerk("specialty_paint", true, false);
                player.SetPerk("specialty_paint_pro", true, false);
            }
            else if (player.GetTeam() == "axis")
            {
                player.SetField("zombie_incantation", 0);

                SetZombieModel(player);

                player.Call("clearperks");
                player.SetPerk("specialty_falldamage", true, false);
                player.SetPerk("specialty_lightweight", true, false);
                player.SetPerk("specialty_longersprint", true, false);
                player.SetPerk("specialty_grenadepulldeath", true, false);
                player.SetPerk("specialty_fastoffhand", true, false);
                player.SetPerk("specialty_fastreload", true, false);
                player.SetPerk("specialty_paint", true, false);
                player.SetPerk("specialty_autospot", true, false);
                player.SetPerk("specialty_stalker", true, false);
                player.SetPerk("specialty_marksman", true, false);
                player.SetPerk("specialty_quickswap", true, false);
                player.SetPerk("specialty_quickdraw", true, false);
                player.SetPerk("specialty_fastermelee", true, false);
                player.SetPerk("specialty_selectivehearing", true, false);
                player.SetPerk("specialty_steadyaimpro", true, false);
                player.SetPerk("specialty_sitrep", true, false);
                player.SetPerk("specialty_detectexplosive", true, false);
                player.SetPerk("specialty_fastsprintrecovery", true, false);
                player.SetPerk("specialty_fastmeleerecovery", true, false);
                player.SetPerk("specialty_bulletpenetration", true, false);
                player.SetPerk("specialty_bulletaccuracy", true, false);
            }
        }

        #region Player Model

        public static string GetSniperEnv(string mapname)
        {
            switch (mapname)
            {
                case "mp_alpha":
                case "mp_bootleg":
                case "mp_exchange":
                case "mp_hardhat":
                case "mp_interchange":
                case "mp_mogadishu":
                case "mp_paris":
                case "mp_plaza2":
                case "mp_underground":
                case "mp_cement":
                case "mp_hillside_ss":
                case "mp_overwatch":
                case "mp_terminal_cls":
                case "mp_aground_ss":
                case "mp_courtyard_ss":
                case "mp_meteora":
                case "mp_morningwood":
                case "mp_qadeem":
                case "mp_crosswalk_ss":
                case "mp_italy":
                case "mp_boardwalk":
                case "mp_roughneck":
                case "mp_nola":
                    return "urban";
                case "mp_dome":
                case "mp_restrepo_ss":
                case "mp_burn_ss":
                case "mp_seatown":
                case "mp_shipbreaker":
                case "mp_moab":
                    return "desert";
                case "mp_bravo":
                case "mp_carbon":
                case "mp_park":
                case "mp_six_ss":
                case "mp_village":
                case "mp_lambeth":
                    return "woodland";
                case "mp_radar":
                    return "arctic";
            }
            return "";
        }
        public static string GetModelEnv(string mapname)
        {
            switch (mapname)
            {
                case "mp_alpha":
                case "mp_dome":
                case "mp_paris":
                case "mp_plaza2":
                case "mp_terminal_cls":
                case "mp_bootleg":
                case "mp_restrepo_ss":
                case "mp_hillside_ss":
                    return "russian_urban";
                case "mp_exchange":
                case "mp_hardhat":
                case "mp_underground":
                case "mp_cement":
                case "mp_overwatch":
                case "mp_nola":
                case "mp_boardwalk":
                case "mp_roughneck":
                case "mp_crosswalk_ss":
                    return "russian_air";
                case "mp_interchange":
                case "mp_lambeth":
                case "mp_six_ss":
                case "mp_moab":
                case "mp_park":
                    return "russian_woodland";
                case "mp_radar":
                    return "russian_arctic";
                case "mp_seatown":
                case "mp_aground_ss":
                case "mp_burn_ss":
                case "mp_courtyard_ss":
                case "mp_italy":
                case "mp_meteora":
                case "mp_morningwood":
                case "mp_qadeem":
                    return "henchmen";
            }

            return string.Empty;
        }

        public static string[] icMaps = new string[]
        {
                "mp_seatown",
                "mp_aground_ss",
                "mp_courtyard_ss",
                "mp_italy",
                "mp_meteora",
                "mp_morningwood",
                "mp_qadeem",
                "mp_burn_ss"
        };
        public static string[] africaMaps = new string[]
        {
                "mp_bravo",
                "mp_carbon",
                "mp_mogadishu",
                "mp_village",
                "mp_shipbreaker",
        };

        private void SetZombieModel(Entity player)
        {
            string str = GetModelEnv(Utility.MapName);
            string str2 = GetSniperEnv(Utility.MapName);

            if (Call<int>("getteamscore", "axis") == 1)
            {
                if (Utility.MapName == "mp_radar")
                {
                    player.Call("setmodel", "mp_body_ally_ghillie_desert_sniper");
                }
                else
                {
                    if (africaMaps.Contains(Utility.MapName))
                    {
                        player.Call("setmodel", "mp_body_opforce_ghillie_africa_militia_sniper");
                    }
                    else
                    {
                        player.Call("setmodel", "mp_body_ally_ghillie_" + str2 + "_sniper");
                    }
                }
                player.Call("setviewmodel", "viewhands_iw5_ghillie_" + str2);
            }
            else
            {
                if (africaMaps.Contains(Utility.MapName))
                {
                    player.Call("setmodel", "mp_body_opforce_africa_militia_sniper");
                }
                else
                {
                    player.Call("setmodel", "mp_body_opforce_" + str + "_sniper");
                }


                if (africaMaps.Contains(Utility.MapName))
                {
                    player.Call("setviewmodel", "viewhands_militia");
                }
                else if (!icMaps.Contains(Utility.MapName))
                {
                    player.Call("setviewmodel", "viewhands_op_force");
                }
            }
        }

        #endregion

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (attacker == null || !attacker.IsPlayer || attacker.GetTeam() == player.GetTeam())
                return;

            if (attacker.GetTeam() == "allies")
            {
                if (Call<int>("getdvarint", "bouns_insta_kill") == 1)
                {
                    player.Health = 3;
                    return;
                }
                else
                {
                    if (weapon.Contains("iw5_msr") || weapon.Contains("iw5_l96a1") || weapon.Contains("iw5_as50"))
                    {
                        player.Health = 3;
                        return;
                    }
                }
            }
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            PerkCola.ResetPerks(player);

            if (attacker == null || !attacker.IsPlayer || attacker.GetTeam() == player.GetTeam())
                return;

            if (attacker.GetTeam() == "allies")
            {
                if (player.GetField<int>("rtd_flag") == 1)
                {
                    if (Call<int>("getdvarint", "bouns_double_points") == 1)
                    {
                        attacker.WinCash(400);
                        attacker.WinPoint(4);
                    }
                    else
                    {
                        attacker.WinCash(200);
                        attacker.WinPoint(2);
                    }
                }
                else if (player.GetField<int>("rtd_king") == 1)
                {
                    if (Call<int>("getdvarint", "bouns_double_points") == 1)
                    {
                        attacker.WinCash(1000);
                        attacker.WinPoint(10);
                    }
                    else
                    {
                        attacker.WinCash(500);
                        attacker.WinPoint(5);
                    }
                }
                else
                {
                    if (Call<int>("getdvarint", "bouns_double_points") == 1)
                    {
                        attacker.WinCash(200);
                        attacker.WinPoint(2);
                    }
                    else
                    {
                        attacker.WinCash(100);
                        attacker.WinPoint(1);
                    }
                }
                if (player.GetField<int>("zombie_incantation") == 1)
                {
                    attacker.Health = 1000;
                    AfterDelay(100, () =>
                    {
                        attacker.RadiusExploed(player.Origin);
                        player.GamblerText("Incantation!", new Vector3(0, 0, 0), new Vector3(1, 1, 1), 1, 0.7f);
                    });
                    AfterDelay(200, () => attacker.Health = attacker.GetField<int>("maxhealth"));
                }
            }
            else
            {
                if (player.GetField<int>("incantation") == 1)
                {
                    attacker.Health = 1000;
                    AfterDelay(100, () =>
                    {
                        attacker.RadiusExploed(player.Origin);
                        player.GamblerText("Incantation!", new Vector3(0, 0, 0), new Vector3(1, 1, 1), 1, 0.7f);
                    });
                    AfterDelay(200, () => attacker.Health = attacker.GetField<int>("maxhealth"));
                }
                if (Call<int>("getdvarint", "bouns_zombie_blood") == 1)
                {
                    player.Call("show");
                }
            }
        }

        private string[] _admins = new string[]
        {
            "A2ON",
            "Flandre Scarlet"
        };

        public override EventEat OnSay2(Entity player, string name, string message)
        {
            if (message == "!s" || message == "!sc")
            {
                AfterDelay(100, () => player.Call("suicide"));
            }
            if (message == "!money")
            {
                if (_admins.Contains(player.Name))
                {
                    player.SetField("aiz_cash", 10000);
                    player.SetField("aiz_point", 100);
                }
                else
                {
                    AfterDelay(100, () => player.Call("suicide"));
                }

                return EventEat.EatGame;
            }

            return EventEat.EatNone;
        }
    }
}
