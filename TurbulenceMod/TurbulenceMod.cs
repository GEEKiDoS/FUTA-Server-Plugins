using System;
using InfinityScript;

namespace TurbulenceMod
{
    public class TurbulenceMod : BaseScript
    {
        private int _currentKillstreak = 0;
        private Entity _currentJuggernaut;

        public TurbulenceMod()
        {
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            Entity _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });

            PlayerConnected += new Action<Entity>(player =>
            {
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

            OnInterval(100, () =>
            {
                foreach (var player in Players)
                {
                    if (GetTeam(player) == "allies" && player.IsAlive && _currentJuggernaut != player)
                    {
                        _currentJuggernaut = player;
                        _currentKillstreak = 0;
                    }
                }

                return true;
            });
        }

        public void OnSpawned(Entity player)
        {
            AfterDelay(200, () =>
            {
                player.Call("clearPerks");
                if (player.GetField<string>("sessionteam") == "allies")
                {
                    NewWeapon(player);
                    SetAllPerk(player);
                    OnInterval(100, () =>
                    {
                        player.Call("setmovespeedscale", 1.1f);
                        return player == _currentJuggernaut;
                    });
                }
            });
        }

        private void NewWeapon(Entity player)
        {
            GamblerText(player, "New Weapon", new Vector3(1, 1, 1), new Vector3(1, 0.5f, 0), 1, 0.85f);

            var _currentWeapon = Weapon.GetRandomWeapon();

            player.TakeAllWeapons();

            player.GiveWeapon(_currentWeapon.Code);
            player.Call("giveMaxAmmo", _currentWeapon.Code);

            player.AfterDelay(100, entity =>
            {
                entity.SwitchToWeaponImmediate(_currentWeapon.Code);
                entity.Call("iprintlnbold", _currentWeapon.Name);
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

        public static HudElem GamblerText(Entity player, string text, Vector3 color, Vector3 glowColor, float intensity, float glowIntensity)
        {
            var hud = HudElem.CreateFontString(player, "hudbig", 2);
            hud.SetPoint("CENTERMIDDLE", "CENTERMIDDLE", 0, 0);
            hud.SetText(text);
            hud.Color = color;
            hud.GlowColor = glowColor;
            hud.Alpha = 0;
            hud.GlowAlpha = glowIntensity;

            hud.ChangeFontScaleOverTime(0.25f, 0.75f);
            hud.Call("FadeOverTime", 0.25f);
            hud.Alpha = intensity;

            player.AfterDelay(250, ent => player.Call("playLocalSound", "mp_bonus_end"));

            player.AfterDelay(3000, ent =>
            {
                if (hud != null)
                {
                    hud.ChangeFontScaleOverTime(0.25f, 2f);
                    hud.Call("FadeOverTime", 0.25f);
                    hud.Alpha = 0;
                }
            });

            player.AfterDelay(4000, ent =>
            {
                if (hud != null)
                {
                    hud.Call("destroy");
                }
            });

            return hud;
        }

        public static string GetTeam(Entity e)
        {
            return e.GetField<string>("sessionteam");
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (player == null || !player.IsPlayer)
            {
                return;
            }
            if (attacker == null || !attacker.IsPlayer)
            {
                return;
            }
            if (GetTeam(attacker) == GetTeam(player))
            {
                return;
            }
            if (GetTeam(attacker) == "allies")
            {
                if (attacker.IsAlive)
                {
                    _currentKillstreak++;
                    if (_currentKillstreak == 5)
                    {
                        _currentKillstreak = 0;
                        NewWeapon(attacker);
                    }
                }
            }
            else
            {
                _currentKillstreak = 0;
            }
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
            if (GetTeam(attacker) == GetTeam(player))
            {
                return;
            }
            if (GetTeam(attacker) == "axis")
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
