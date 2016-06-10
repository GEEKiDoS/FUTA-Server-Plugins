using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace Bullseye
{
    public class Bullseye : BaseScript
    {
        private Random rng = new Random();
        private string weapon;
        private string[] weapons;
        private int i = 45;

        public Bullseye()
        {
            this.weapon = "throwingknife_mp";
            weapons = new string[]
            {
                "bouncingbetty_mp",
                "c4_mp",
                "claymore_mp",
                "frag_grenade_mp",
                "semtex_mp",
                "throwingknife_mp",
                "smoke_grenade_mp",
                "concussion_grenade_mp",
                "trophy_mp",
                "gl_mp",
                "ac130_25mm_mp",
                "ims_projectile_mp",
                "rpg_mp",
                "iw5_smaw_mp",
                "xm25_mp",
                "javelin_mp",
                "m320_mp",
                "stinger_mp",
                "iw5_1887_mp_camp12",
                "uav_strike_marker_mp",
                "iw5_usas12_mp_grip_xmags_camo13",
                "iw5_m60jugg_mp_eotechlmg_camo07",
                "iw5_mp9_mp_eotechsmg_silencer02",
                "iw5_aa12_mp_grip_xmags_camo12",
                "iw5_mp5_mp_xmags_camo12",
                "iw5_mp412_mp_akimbo",
                "iw5_usp45_mp_akimbo",
                "iw5_l96a1_mp_l96a1scope",
                "ac130_40mm_mp"
            };

            Call("setdvar", "scr_game_hardpoints", 0);
            Call("setdvar", "scr_game_perks", 0);
            Call("setdvar", "scr_game_playerwaittime", "0");
            Call("setdvar", "scr_game_matchstarttime", "0");
            Call("setdvar", "scr_game_allowKillstreaks", "0");

            TimerTick();

            PlayerConnected += new Action<Entity>(player =>
            {
                Call("setdvar", "cg_drawCrosshair", "1");
                player.OnNotify("joined_team", delegate (Entity ent)
                {
                    player.Call("closePopupMenu", new Parameter[0]);
                    player.Call("closeIngameMenu", new Parameter[0]);
                    player.Notify("menuresponse", new Parameter[] { "changeclass", "class0" });
                });
                player.OnNotify("weapon_fired", delegate (Entity self, Parameter weapon)
                {
                    if (weapon.As<string>() == "uav_strike_marker_mp")
                    {
                        Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { player.Call<Vector3>("getplayerangles", new Parameter[0]) });
                        Vector3 vector2 = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                        Call("magicbullet", new Parameter[] { "ac130_105mm_mp", player.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), vector2, self });
                    }
                    if (weapon.As<string>().StartsWith("iw5_usp45_mp"))
                    {
                        Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { player.Call<Vector3>("getplayerangles", new Parameter[0]) });
                        Vector3 vector2 = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                        Call("magicbullet", new Parameter[] { "gl_mp", player.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), vector2, self });
                    }
                });
                player.Call("notifyonplayercommand", "attack", "+attack");
                player.OnNotify("attack", self =>
                {
                    if (player.CurrentWeapon == "stinger_mp")
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
                });
                player.SpawnedPlayer += delegate
                {
                    player.Call("clearperks", new Parameter[0]);
                    player.SetPerk("specialty_rof", true, false);
                    player.SetPerk("specialty_quickdraw", true, false);
                    if (player.GetField<string>("sessionteam") == "allies")
                    {
                        player.Call("setmodel", new Parameter[] { "mp_fullbody_ally_juggernaut" });
                    }
                    else
                    {
                        player.Call("setmodel", new Parameter[] { "mp_fullbody_opforce_juggernaut" });
                    }
                    player.TakeAllWeapons();
                    player.GiveWeapon(weapon);
                    AfterDelay(300, () => player.SwitchToWeapon(weapon));
                    AfterDelay(400, () => player.OnInterval(100, delegate (Entity ent)
                    {
                        player.Call("setweaponammostock", new Parameter[] { player.CurrentWeapon, 0x63 });
                        player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 0x63 });
                        player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 0x63, "left" });
                        player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 0x63, "right" });
                        if (player.CurrentWeapon != weapon)
                        {
                            player.SwitchToWeapon(weapon);
                        }
                        if (!player.IsAlive)
                        {
                            return false;
                        }
                        return true;
                    }));
                };
            });
        }

        private void ChangeWeapon(string weapon)
        {
            this.weapon = weapon;
            using (List<Entity>.Enumerator enumerator = base.Players.GetEnumerator())
            {
                Action function = null;
                Entity item;
                while (enumerator.MoveNext())
                {
                    item = enumerator.Current;
                    if (((item != null) && item.IsPlayer) && item.IsAlive)
                    {
                        item.TakeAllWeapons();
                        item.GiveWeapon(weapon);
                        if (function == null)
                        {
                            function = () => item.SwitchToWeapon(weapon);
                        }
                        AfterDelay(300, function);
                    }
                }
            }
        }

        public override void OnSay(Entity player, string name, string message)
        {
            if (message == "!weapon")
            {
                Utilities.RawSayTo(player, "^2Weapon is: " + weapon);
            }
            if (message.StartsWith("!change "))
            {
                string[] strArray = message.Split(new char[] { ' ' }, 2);
                weapon = strArray[1];
                ChangeWeapon(strArray[1]);
                i = 45;
            }
        }

        private void TimerTick()
        {
            OnInterval(1000, delegate
            {
                i--;
                if (i == 0)
                {
                    ChangeWeapon(weapons[rng.Next(weapons.Length)]);
                    i = 45;
                }
                return true;
            });
        }
    }
}
