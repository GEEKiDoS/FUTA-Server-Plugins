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
        private SharpshooterPerk[] _perkList;

        #region SharpshooterPerk
        private class SharpshooterPerk
        {
            public string perkName;
            public string[] perks;
            public bool isWeapon;
        }
        #endregion

        public Sharpshooter()
        {
            Entity e = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            Entity _airdropCollision = Call<Entity>("getent", new Parameter[] { e.GetField<string>("target"), "targetname" });

            Utilities.SetDropItemEnabled(false);

            _switchTime = Call<int>("getDvarInt", "shrp_switchTime", 45);
            _cycleRemaining = _switchTime;

            _perkList = new[]
            {
                new SharpshooterPerk()
                {
                    perkName = "Sleight of Hand",
                    perks = new[]
                    {
                        "specialty_fastreload",
                        "specialty_quickswap"
                    }
                },
                new SharpshooterPerk()
                {
                    perkName = "Quickdraw",
                    perks = new[]
                    {
                        "specialty_quickdraw"
                    }
                },
                new SharpshooterPerk()
                {
                    perkName = "Stalker",
                    perks = new[]
                    {
                        "specialty_stalker"
                    }
                },
                new SharpshooterPerk()
                {
                    perkName = "Marathon",
                    perks = new[]
                    {
                        "specialty_longersprint",
                        "specialty_fastmantle"
                    }
                },
                new SharpshooterPerk()
                {
                    perkName = "Range",
                    perks = new[]
                    {
                        "specialty_longerrange"
                    },
                    isWeapon = true
                },
                new SharpshooterPerk()
                {
                    perkName = "Stability",
                    perks = new[]
                    {
                        "specialty_reducedsway"
                    },
                    isWeapon = true
                },
                new SharpshooterPerk()
                {
                    perkName = "Speed",
                    perks = new[]
                    {
                        "specialty_lightweight"
                    },
                    isWeapon = true
                },
            };

            foreach (var perk in _perkList)
            {
                var shaderName = perk.perks[0] + (perk.isWeapon ? "" : "_upgrade");
                Call("precacheShader", shaderName);
            }

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
                CreatePerkHUD(entity);

                entity.OnNotify("joined_team", player =>
                {
                    entity.Call("closePopupMenu");
                    entity.Call("closeIngameMenu");
                    entity.Notify("menuresponse", "changeclass", "class1");
                });

                entity.OnInterval(300, player =>
                {
                    if (player.IsAlive)
                    {
                        var weapon = player.CurrentWeapon;

                        if (weapon != "none" && weapon != "" && !weapon.Contains(_currentWeapon.Code))
                        {
                            player.TakeAllWeapons();

                            player.GiveWeapon(_currentWeapon.Code);
                            player.Call("giveMaxAmmo", _currentWeapon.Code);

                            player.AfterDelay(100, ex =>
                            {
                                ex.SwitchToWeaponImmediate(_currentWeapon.Code);
                                ex.Call("iprintlnbold", _currentWeapon.Name);
                            });
                        }
                    }

                    return true;
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

                    ResetPerkHUD(entity);

                    entity.SetField("shrp_perkc", 0);
                    entity.SetField("shrp_perks", new Parameter(new string[3]));

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

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (attacker != player && attacker.GetField<string>("classname") == "player" && attacker.IsAlive)
            {
                GiveRandomPerk(attacker);
            }
        }

        private string FormatTime(int seconds)
        {
            return string.Format("{0}:{1}", seconds / 60, (seconds % 60).ToString().PadLeft(2, '0'));
        }


        private void GiveRandomPerk(Entity player)
        {
            var pc = player.GetField<int>("shrp_perkc");

            if (pc < 3)
            {
                var usedPerks = player.GetField<string[]>("shrp_perks");

                var validPerk = false;
                SharpshooterPerk perk = null;

                while (!validPerk)
                {
                    perk = _perkList[_rng.Next(0, _perkList.Length)];

                    if (!usedPerks.Contains(perk.perks[0]))
                    {
                        validPerk = true;
                    }
                }

                foreach (var p in perk.perks)
                {
                    player.SetPerk(p, true, false);
                }

                UpdatePerkHUD(player, pc, perk);

                usedPerks[pc] = perk.perks[0];
                player.SetField("shrp_perkc", pc + 1);
            }
        }

        private void CreatePerkHUD(Entity player)
        {
            var icons = new HudElem[3];

            new[] { -300, -250, -200 }.ToList().ForEach(y =>
            {
                var i = (y + 300) / 50;

                var elem = HudElem.CreateIcon(player, "specialty_quickdraw_upgrade", 40, 40);
                elem.SetPoint("bottom right", "bottom right", -120, y);
                elem.Alpha = 0;
                elem.HideWhenInMenu = true;
                elem.Foreground = true;

                icons[i] = elem;
            });

            player.SetField("shrp_perkIcons", new Parameter(icons));

            var names = new HudElem[3];

            new[] { -275, -225, -175 }.ToList().ForEach(y =>
            {
                var i = (y + 275) / 50;

                var elem = HudElem.NewClientHudElem(player);
                elem.X = 40;
                elem.Y = y;
                elem.AlignX = "right";
                elem.AlignY = "bottom";
                elem.HorzAlign = "right";
                elem.VertAlign = "bottom";
                elem.FontScale = 1.5f;
                elem.SetText("Quickdraw");
                elem.Alpha = 0;
                elem.HideWhenInMenu = true;
                elem.Foreground = true;

                names[i] = elem;
            });

            player.SetField("shrp_perkNames", new Parameter(names));
        }

        private void ResetPerkHUD(Entity player)
        {
            var icons = player.GetField<HudElem[]>("shrp_perkIcons");

            foreach (var icon in icons)
            {
                icon.Alpha = 0;
            }

            var texts = player.GetField<HudElem[]>("shrp_perkNames");

            foreach (var text in texts)
            {
                text.Alpha = 0;
            }
        }

        private void UpdatePerkHUD(Entity player, int index, SharpshooterPerk perk)
        {
            var icons = player.GetField<HudElem[]>("shrp_perkIcons");
            icons[index].SetShader(perk.perks[0] + (perk.isWeapon ? "" : "_upgrade"), 40, 40);
            icons[index].Alpha = 1;

            var texts = player.GetField<HudElem[]>("shrp_perkNames");
            texts[index].SetText(perk.perkName);
            texts[index].Alpha = 1;
        }
    }
}
