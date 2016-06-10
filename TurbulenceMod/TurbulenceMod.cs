using System;
using InfinityScript;

namespace TurbulenceMod
{
    public class TurbulenceMod : BaseScript
    {
        public TurbulenceMod()
        {
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            Entity _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });

            PlayerConnected += new Action<Entity>(player =>
            {
                player.Notify("menuresponse", "changeclass", "class0");

                OnSpawned(player);
                player.SpawnedPlayer += () => OnSpawned(player);

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
            });
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (attacker.GetField<string>("sessionteam") == "allies")
            {
                if (attacker.IsAlive || mod != "MOD_MELEE")
                {
                    var w = Weapon.GetRandomWeapon();
                    attacker.TakeAllWeapons();
                    attacker.GiveWeapon(w.Code);
                    attacker.Call("givemaxammo", w.Code);
                    AfterDelay(100, () =>
                    {
                        attacker.SwitchToWeaponImmediate(w.Code);
                        attacker.Call("iprintlnbold", w.Name);
                    });
                }
            }
        }

        public void OnSpawned(Entity player)
        {
            AfterDelay(200, () =>
            {
                player.Call("clearPerks");
                if (player.GetField<string>("sessionteam") == "allies")
                {
                    var w = Weapon.GetRandomWeapon();
                    player.TakeAllWeapons();
                    player.GiveWeapon(w.Code);
                    player.Call("givemaxammo", w.Name);
                    AfterDelay(100, () =>
                    {
                        player.SwitchToWeaponImmediate(w.Name);
                        player.Call("iprintlnbold", w.Name);
                    });
                    SetAllPerk(player);
                }
                else
                {
                    player.TakeAllWeapons();
                    player.GiveWeapon("iw5_usp45_mp_tactical");
                    player.Call("setweaponammostock", new Parameter[] { "iw5_usp45_mp_tactical", 0 });
                    player.Call("setweaponammoclip", new Parameter[] { "iw5_usp45_mp_tactical", 0 });
                    AfterDelay(200, () => player.SwitchToWeaponImmediate("iw5_usp45_mp_tactical"));
                }
            });
        }

        private void SetAllPerk(Entity player)
        {
            player.SetPerk("specialty_paint", true, false);
            player.SetPerk("specialty_fastreload", true, false);
            player.SetPerk("specialty_blindeye", true, false);
            player.SetPerk("specialty_longersprint", true, false);
            player.SetPerk("specialty_scavenger", true, false);
            player.SetPerk("specialty_quickdraw", true, false);
            player.SetPerk("_specialty_blastshield", true, false);
            player.SetPerk("specialty_hardline", true, false);
            player.SetPerk("specialty_coldblooded", true, false);
            player.SetPerk("specialty_twoprimaries", true, false);
            player.SetPerk("specialty_autospot", true, false);
            player.SetPerk("specialty_detectexplosive", true, false);
            player.SetPerk("specialty_bulletaccuracy", true, false);
            player.SetPerk("specialty_quieter", true, false);
            player.SetPerk("specialty_fastmantle", true, false);
            player.SetPerk("specialty_quickswap", true, false);
            player.SetPerk("specialty_extraammo", true, false);
            player.SetPerk("specialty_fasterlockon", true, false);
            player.SetPerk("specialty_armorpiercing", true, false);
            player.SetPerk("specialty_paint_pro", true, false);
            player.SetPerk("specialty_rollover", true, false);
            player.SetPerk("specialty_assists", true, false);
            player.SetPerk("specialty_spygame", true, false);
            player.SetPerk("specialty_empimmune", true, false);
            player.SetPerk("specialty_fastoffhand", true, false);
            player.SetPerk("specialty_overkillpro", true, false);
            player.SetPerk("specialty_stun_resistance", true, false);
            player.SetPerk("specialty_holdbreath", true, false);
            player.SetPerk("specialty_selectivehearing", true, false);
            player.SetPerk("specialty_fastsprintrecovery", true, false);
            player.SetPerk("specialty_falldamage", true, false);
            player.SetPerk("specialty_marksman", true, false);
            player.SetPerk("specialty_bulletpenetration", true, false);
            player.SetPerk("specialty_bling", true, false);
            player.SetPerk("specialty_sharp_focus", true, false);
            player.SetPerk("specialty_reducedsway", true, false);
            player.SetPerk("specialty_longerrange", true, false);
            player.SetPerk("specialty_fastermelee", true, false);
            player.SetPerk("specialty_lightweight", true, false);
            player.SetPerk("specialty_moredamage", true, false);
        }
    }
}
