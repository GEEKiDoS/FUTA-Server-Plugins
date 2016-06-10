using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace Exo
{
    public class Sharpshooter : BaseScript
    {
        private Weapon _currentWeapon;
        private Random _rng = new Random();
        private HudElem _cycleTimer;
        private int _cycleRemaining = 30;

        public Sharpshooter()
        {
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            Entity _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });

            _currentWeapon = Weapon.GetRandomWeapon();

            OnInterval(100, () =>
            {
                if (Call<int>("getteamscore", "axis") == 1)
                {
                    SharpShooter_Tick();
                    return false;
                }
                return true;
            });

            PlayerConnected += player =>
            {
                OnPlayerSpawned(player);
                player.SpawnedPlayer += () => OnPlayerSpawned(player);

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
            };
        }

        private string FormatTime(int seconds)
        {
            return string.Format("{0}:{1}", seconds / 60, (seconds % 60).ToString().PadLeft(2, '0'));

        }

        public void SharpShooter_Tick()
        {
            _cycleTimer = HudElem.CreateServerFontString("objective", 1.4f);
            _cycleTimer.SetPoint("TOPLEFT", "TOPLEFT", 115, 5);
            _cycleTimer.HideWhenInMenu = true;

            OnInterval(1000, () =>
            {
                _cycleRemaining--;

                if (_cycleRemaining == 0)
                {
                    _cycleRemaining = _rng.Next(30, 61);

                    UpdateWeapon();
                }

                _cycleTimer.SetText("Weapon Cycling: " + FormatTime(_cycleRemaining));

                return true;
            });
        }

        public void UpdateWeapon()
        {
            _currentWeapon = Weapon.GetRandomWeapon();

            foreach (var player in Players)
            {
                if (player.GetTeam() == "allies")
                {
                    player.TakeAllWeapons();

                    player.GiveWeapon(_currentWeapon.Code);
                    player.Call("giveMaxAmmo", _currentWeapon.Code);

                    player.AfterDelay(100, entity =>
                    {
                        entity.SwitchToWeaponImmediate(_currentWeapon.Code);
                        entity.Call("iprintlnbold", _currentWeapon.Name);
                    });

                    player.GamblerText("Weapon Cycled", new Vector3(1, 1, 1), new Vector3(0.3f, 0.3f, 0.9f), 1, 0.85f);
                }
            }
        }

        public void OnPlayerSpawned(Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                player.TakeAllWeapons();

                player.GiveWeapon(_currentWeapon.Code);
                player.Call("giveMaxAmmo", _currentWeapon.Code);

                player.AfterDelay(100, entity =>
                {
                    entity.SwitchToWeaponImmediate(_currentWeapon.Code);
                    entity.Call("iprintlnbold", _currentWeapon.Name);
                });

                SetAllPerk(player);

                player.OnInterval(1000, e =>
                {
                    if (player.IsAlive)
                    {
                        var weapon = player.CurrentWeapon;

                        if (weapon.StartsWith("rpg") || weapon.StartsWith("iw5_smaw") || weapon.StartsWith("m320") || weapon.StartsWith("uav") || weapon.StartsWith("stinger") || weapon.StartsWith("javelin") || weapon.StartsWith("gl"))
                        {
                            player.Call("giveMaxAmmo", weapon);
                        }

                        return true;
                    }
                    return false;
                });
            }
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

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (player == null || !player.IsPlayer)
            {
                return;
            }
            if (attacker == null || !attacker.IsPlayer)
            {
                return;
            }
            if (attacker.GetTeam() == player.GetTeam())
            {
                return;
            }
            if (attacker.GetTeam() == "allies")
            {
                if (weapon.StartsWith("iw5_msr") || weapon.StartsWith("iw5_l96a1") || weapon.StartsWith("iw5_as50"))
                {
                    player.Health = 3;
                    return;
                }
                if (mod == "MOD_MELEE")
                {
                    player.Health = 3;
                    return;
                }
            }
        }
    }
}
