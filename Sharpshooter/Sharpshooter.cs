// -------------------------------------------------
// Class: FUTA
// Project: FUTA dedicated plugin
// Author: NTAuthority
// Modify: A2ON
// 
// Imitate BO2 Sharpshooter mode. need Free-for-all dsr.
// -------------------------------------------------

using System;
using System.Linq;
using InfinityScript;

namespace Sharpshooter
{
    public class Sharpshooter : BaseScript
    {
        private int _switchTime;
        private Weapon _currentWeapon;
        private Random _rng = new Random();
        private HudElem _cycleTimer;
        private int _cycleRemaining = 30;

        public Sharpshooter()
        {
            Entity e = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            Entity _airdropCollision = Call<Entity>("getent", new Parameter[] { e.GetField<string>("target"), "targetname" });

            Utilities.SetDropItemEnabled(false);

            _switchTime = Call<int>("getDvarInt", "shrp_switchTime", 45);
            _cycleRemaining = _switchTime;

            _currentWeapon = Weapon.GetRandomWeapon();

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

            PlayerConnected += new Action<Entity>(entity =>
            {
                entity.OnNotify("joined_team", player =>
                {
                    entity.Call("closePopupMenu");
                    entity.Call("closeIngameMenu");
                    entity.Notify("menuresponse", "changeclass", "class1");
                });

                entity.OnInterval(3500, player =>
                {
                    if (player.IsAlive)
                    {
                        var weapon = player.CurrentWeapon;

                        if (weapon.StartsWith("rpg") || weapon.StartsWith("iw5_smaw") || weapon.StartsWith("m320") || weapon.StartsWith("uav") || weapon.StartsWith("stinger") || weapon.StartsWith("javelin") || weapon.StartsWith("gl"))
                        {
                            player.Call("giveMaxAmmo", weapon);
                        }
                    }

                    return true;
                });

                entity.Call("notifyonplayercommand", "attack", "+attack");
                entity.OnNotify("attack", self =>
                {
                    if (entity.CurrentWeapon == "stinger_mp")
                    {
                        if (entity.Call<float>("playerads") >= 1f)
                        {
                            if (entity.Call<int>("getweaponammoclip", entity.CurrentWeapon) != 0)
                            {
                                Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { entity.Call<Vector3>("getplayerangles", new Parameter[0]) });
                                Vector3 dsa = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                                Call("magicbullet", new Parameter[] { "stinger_mp", entity.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), dsa, self });
                                entity.Call("setweaponammoclip", entity.CurrentWeapon, 0);
                            }
                        }
                        else
                        {
                            entity.Call("iprintlnbold", "You must be aim first!");
                        }
                    }
                });

                entity.OnNotify("weapon_fired", delegate (Entity self, Parameter weapon)
                {
                    if (weapon.As<string>() == "uav_strike_marker_mp")
                    {
                        Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { entity.Call<Vector3>("getplayerangles", new Parameter[0]) });
                        Vector3 dsa = new Vector3(vector.X * 2000f, vector.Y * 2000f, vector.Z * 2000f);

                        var crate = Call<Entity>("spawn", "script_model", entity.Call<Vector3>("gettagorigin", "tag_weapon_left"));
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
                                    Call("RadiusDamage", crate.Origin, 400, 200, 50, entity, "MOD_EXPLOSIVE", "airdrop_trap_explosive_mp");
                                    crate.Call("delete");
                                });
                            });
                        }
                    }
                    if (weapon.As<string>() == "gl_mp")
                    {
                        Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { entity.Call<Vector3>("getplayerangles", new Parameter[0]) });
                        Vector3 dsa = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                        AfterDelay(200, () => Call("magicbullet", new Parameter[] { "gl_mp", entity.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), dsa, self }));
                        AfterDelay(400, () => Call("magicbullet", new Parameter[] { "gl_mp", entity.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), dsa, self }));
                    }
                });

                entity.SpawnedPlayer += new Action(() =>
                {
                    entity.TakeAllWeapons();
                    entity.Call("clearPerks");

                    entity.GiveWeapon(_currentWeapon.Code);
                    entity.Call("giveMaxAmmo", _currentWeapon.Code);

                    entity.AfterDelay(100, player =>
                    {
                        player.SwitchToWeaponImmediate(_currentWeapon.Code);
                        player.Call("iprintlnbold", _currentWeapon.Name);
                    });
                });
            });
        }

        public void UpdateWeapon()
        {
            _currentWeapon = Weapon.GetRandomWeapon();

            foreach (var player in Players)
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

        private string FormatTime(int seconds)
        {
            return string.Format("{0}:{1}", seconds / 60, (seconds % 60).ToString().PadLeft(2, '0'));
        }
    }
}
