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

            //Perks
            Utility.PreCacheShader("specialty_finalstand"); //Quick Revive
            Utility.PreCacheShader("specialty_fastreload"); //Speed Cola
            Utility.PreCacheShader("cardicon_juggernaut_1"); //Juggernog
            Utility.PreCacheShader("specialty_longersprint"); //Stamin-Up
            Utility.PreCacheShader("specialty_twoprimaries"); //Mule Kick
            Utility.PreCacheShader("specialty_moredamage"); //Double Tap
            Utility.PreCacheShader("cardicon_headshot"); //Dead Shot
            Utility.PreCacheShader("specialty_blastshield"); //PhD
            Utility.PreCacheShader("cardicon_trophy"); //Electric Cherry
            Utility.PreCacheShader("cardicon_soap_bar"); //Widow's Wine

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
            Call("setdvar", "bouns_double_point", 0);
            Call("setdvar", "bouns_insta_kill", 0);
            Call("setdvar", "bouns_fire_sale", 0);
            Call("setdvar", "bouns_zombie_blood", 0);

            PlayerConnected += player =>
            {
                OnSpawned(player);
                player.SpawnedPlayer += () => OnSpawned(player);

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

                string[] welcomemessages = new string[]
                {
                    "Welcome "+player.Name,
                    "Project INF3 v0.1 Alpha",
                    "Create by A2ON.",
                    "Source code in: https://github.com/A2ON/",
                    "Map: "+Utility.MapName,
                    "Enjoy playing!",
                };
                player.WelcomeMessage(welcomemessages, new Vector3(1, 1, 1), new Vector3(0.3f, 0.9f, 0.3f), 1, 0.85f);
            };
        }

        public void OnSpawned(Entity player)
        {
            player.SetField("aiz_cash", 500);
            player.SetField("aiz_point", 0);

            player.SetField("speed", 1f);
            player.SetField("recoil", 1f);
            player.SetField("isstick", 0);

            player.SetField("aiz_perks", 0);
            player.SetField("aiz_perkhuds", new Parameter(new List<HudElem>()));

            player.SetField("perk_revive", 0);
            player.SetField("perk_juggernog", 0);
            player.SetField("perk_staminup", 0);
            player.SetField("perk_mulekick", 0);
            player.SetField("perk_doubletap", 0);
            player.SetField("perk_deadshot", 0);
            player.SetField("perk_phd", 0);
            player.SetField("perk_cherry", 0);
            player.SetField("perk_widow", 0);
        }
    }
}
