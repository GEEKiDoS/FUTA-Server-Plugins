// -------------------------------------------------
// Class: FUTA
// Project: FUTA dedicated plugin
// Author: A2ON
// 
// for FUTA server only!
// -------------------------------------------------

using System;
using InfinityScript;

namespace FUTA
{
    public class FUTA : BaseScript
    {
        public FUTA()
        {
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            Entity _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });

            PlayerConnected += new Action<Entity>(player =>
            {
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

                player.OnNotify("weapon_fired", delegate (Entity self, Parameter weapon)
                {
                    if (weapon.As<string>() == "uav_strike_marker_mp")
                    {
                        Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { player.Call<Vector3>("getplayerangles", new Parameter[0]) });
                        Vector3 dsa = new Vector3(vector.X * 2000f, vector.Y * 2000f, vector.Z * 2000f);

                        var crate = Call<Entity>("spawn", "script_model", player.Call<Vector3>("gettagorigin", "tag_weapon_left"));
                        if (crate != null)
                        {
                            crate.Call("setmodel", "com_plasticcase_trap_friendly");
                            crate.Call("clonebrushmodeltoscriptmodel", _airdropCollision);
                            crate.Call("physicslaunchserver", new Vector3(), dsa);

                            AfterDelay(4000, () =>
                            {
                                crate.Call("playsound", "javelin_clu_lock");
                                AfterDelay(1000, () =>
                                {
                                    Call("playfx", Call<int>("loadfx", "explosions/tanker_explosion"), crate.Origin);
                                    crate.Call("playsound", "cobra_helicopter_crash");
                                    Call("RadiusDamage", crate.Origin, 400, 200, 50, player, "MOD_EXPLOSIVE", "airdrop_trap_explosive_mp");
                                    crate.Call("delete");
                                });
                            });
                        }
                    }
                    if (weapon.As<string>() == "gl_mp")
                    {
                        Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { player.Call<Vector3>("getplayerangles", new Parameter[0]) });
                        Vector3 dsa = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                        AfterDelay(200, () => Call("magicbullet", new Parameter[] { "gl_mp", player.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), dsa, self }));
                        AfterDelay(400, () => Call("magicbullet", new Parameter[] { "gl_mp", player.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), dsa, self }));
                    }

                });

                player.OnInterval(100, e => 
                {
                    player.Call("freezecontrols", false);

                    return true;
                });

                OnSpawned(player); //Fix S&D 
                player.SpawnedPlayer += () => OnSpawned(player);
            });
        }

        public void OnSpawned(Entity player)
        {
            AfterDelay(100, () =>
            {
                if (player.HasWeapon("iw5_smaw_mp"))
                {
                    player.Call("givemaxammo", "iw5_smaw_mp");
                }
                if (player.HasWeapon("javelin_mp"))
                {
                    player.Call("givemaxammo", "javelin_mp");
                }
                if (player.CurrentWeapon == "m320_mp")
                {
                    player.TakeWeapon("m320_mp");
                    player.GiveWeapon("gl_mp");
                    player.Call("givemaxammo", "gl_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("gl_mp"));
                }
                else if (player.CurrentWeapon == "javelin_mp")
                {
                    player.TakeWeapon("javelin_mp");
                    player.GiveWeapon("uav_strike_marker_mp");
                    player.Call("givemaxammo", "uav_strike_marker_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("uav_strike_marker_mp"));
                }
            });
        }
    }
}
