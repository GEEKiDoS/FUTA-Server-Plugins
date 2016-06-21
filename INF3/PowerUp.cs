using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class PowerUp : BaseScript
    {
        public enum PowerUpType
        {
            MaxAmmo,
            DoublePoints,
            InstaKill,
            Nuke,
            FireSale,
            BonusPoints,
            Carpenter
        }

        private static readonly List<Entity> _bonusdrops = new List<Entity>();

        public PowerUp()
        {
            PlayerConnected += player =>
            {
                UsablePowerUp(player);
            };
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (mod == "MOD_SUICIDE")
                return;

            if (player.GetTeam() == "axis")
            {
                var random = Utility.Rng.Next(5);
                var randomequels = Utility.Rng.Next(5);
                if (random == randomequels)
                {
                    switch ((PowerUpType)Utility.Rng.Next(Enum.GetNames(typeof(PowerUpType)).Length))
                    {
                        case PowerUpType.MaxAmmo:
                            PowerUpReg(PowerUpType.MaxAmmo, "com_plasticcase_friendly", player.Origin, player.GetField<Vector3>("angles"), Call<int>("loadfx", "misc/flare_ambient_green"));
                            break;
                        case PowerUpType.DoublePoints:
                            PowerUpReg(PowerUpType.DoublePoints, "com_plasticcase_friendly", player.Origin, player.GetField<Vector3>("angles"), Call<int>("loadfx", "misc/flare_ambient"));
                            break;
                        case PowerUpType.InstaKill:
                            PowerUpReg(PowerUpType.InstaKill, "com_plasticcase_trap_friendly", player.Origin, player.GetField<Vector3>("angles"), Call<int>("loadfx", "misc/flare_ambient"));
                            break;
                        case PowerUpType.Nuke:
                            PowerUpReg(PowerUpType.Nuke, "projectile_cbu97_clusterbomb", player.Origin, player.GetField<Vector3>("angles") - new Vector3(90, 0, 0), Call<int>("loadfx", "misc/flare_ambient"));
                            break;
                        case PowerUpType.FireSale:
                            PowerUpReg(PowerUpType.FireSale, "com_plasticcase_enemy", player.Origin, player.GetField<Vector3>("angles"), Call<int>("loadfx", "misc/flare_ambient"));
                            break;
                        case PowerUpType.BonusPoints:
                            PowerUpReg(PowerUpType.BonusPoints, "com_plasticcase_enemy", player.Origin, player.GetField<Vector3>("angles"), Call<int>("loadfx", "misc/flare_ambient_green"));
                            break;
                        case PowerUpType.Carpenter:
                            PowerUpReg(PowerUpType.Carpenter, "com_plasticcase_trap_friendly", player.Origin, player.GetField<Vector3>("angles"), Call<int>("loadfx", "fire/flare_ambient_green"));
                            break;
                    }
                }
            }
        }

        private void UsablePowerUp(Entity player)
        {
            OnInterval(100, () =>
            {
                try
                {
                    if (player.GetTeam() == "allies" && player.IsAlive)
                    {
                        foreach (var ent in _bonusdrops)
                        {
                            if (player.Origin.DistanceTo(ent.Origin) <= 75)
                            {
                                PowerUpThink(ent.GetField<PowerUpType>("powerup"), player);
                                PowerUpDestroy(ent);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }

                return true;
            });
        }
        private void PowerUpReg(PowerUpType type, string model, Vector3 origin, Vector3 angles, int fx)
        {
            if (type == PowerUpType.Nuke)
                origin += new Vector3(0, 0, 40);

            var block = Utility.Spawn("script_model", origin);
            block.Call("setmodel", model);
            block.SetField("angles", angles);
            var fxent = Call<Entity>("spawnfx", fx, origin);
            Call("triggerfx", fxent);

            block.SetField("powerup", new Parameter(type));
            block.SetField("fx", fxent);

            PowerUpRotate(block);
            PowerUpTimer(block);

            _bonusdrops.Add(block);
        }

        private void PowerUpThink(PowerUpType powertype, Entity player)
        {
            switch (powertype)
            {
                case PowerUpType.MaxAmmo:
                    MaxAmmo(player);
                    break;
                case PowerUpType.DoublePoints:
                    DoublePoints(player);
                    break;
                case PowerUpType.InstaKill:
                    InstaKill(player);
                    break;
                case PowerUpType.Nuke:
                    Nuke(player);
                    break;
                case PowerUpType.FireSale:
                    FireSale(player);
                    break;
                case PowerUpType.BonusPoints:
                    BonusPoints(player);
                    break;
                case PowerUpType.Carpenter:
                    Carpenter(player);
                    break;
            }
        }

        private static void PowerUpTimer(Entity bonusdrop)
        {
            bonusdrop.AfterDelay(20000, e =>
            {
                if (bonusdrop != null)
                {
                    int i = 0;
                    bool ishide = false;
                    bonusdrop.OnInterval(500, ex =>
                    {
                        if (bonusdrop == null)
                            return false;

                        if (ishide)
                        {
                            bonusdrop.Call("hide");
                            ishide = true;
                        }
                        else
                        {
                            bonusdrop.Call("show");
                            ishide = false;
                        }
                        i++;
                        return i < 10;
                    });
                }
            });
            bonusdrop.AfterDelay(30000, e =>
            {
                if (bonusdrop != null)
                    PowerUpDestroy(bonusdrop);
            });
        }

        private static void PowerUpRotate(Entity bonusdrop)
        {
            bonusdrop.OnInterval(5000, e =>
            {
                bonusdrop.Call("rotateyaw", -360, 5);

                return bonusdrop != null;
            });
        }

        private static void PowerUpDestroy(Entity bonusdrop)
        {
            _bonusdrops.Remove(bonusdrop);
            bonusdrop.GetField<Entity>("fx").Call("delete");
            bonusdrop.Call("delete");
        }

        private static void PowerUpHudTimer(HudElem hud)
        {
            hud.Entity.AfterDelay(20000, e =>
            {
                float i = 0;
                bool ishide = false;
                hud.Entity.OnInterval(500, ex =>
                {
                    if (ishide)
                    {
                        hud.Call("fadeovertime", 0.5f);
                        hud.Alpha = 0;
                        ishide = true;
                    }
                    else
                    {
                        hud.Call("fadeovertime", 0.5f);
                        hud.Alpha = 1;
                        ishide = false;
                    }
                    i++;
                    return i < 5;
                });
                hud.Entity.AfterDelay(5000, ex =>
                {
                    hud.Entity.OnInterval(250, ee =>
                    {
                        if (ishide)
                        {
                            hud.Call("fadeovertime", 0.25f);
                            hud.Alpha = 0;
                            ishide = true;
                        }
                        else
                        {
                            hud.Call("fadeovertime", 0.25f);
                            hud.Alpha = 1;
                            ishide = false;
                        }
                        i += 0.5f;
                        return i < 10;
                    });
                });
            });
            hud.Entity.AfterDelay(30000, e =>
            {
                hud.Call("destroy");
            });
        }

        public static void MaxAmmo(Entity player)
        {
            Hud.BonusDropTakeHud(player, "Max Ammo", "waypoint_ammo_friendly");
            foreach (var item in Utility.GetPlayers())
            {
                if (item.GetTeam() == "allies")
                {
                    item.GiveAmmo();
                }
            }
        }

        public static void DoublePoints(Entity player)
        {
            if (Function.Call<int>("getdvarint", "bonus_double_points") == 1)
                return;

            PowerUpHudTimer(Hud.BonusDropHud("cardicon_spetsnaz", 0));
            Function.Call("setdvar", "bonus_double_points", 1);
            player.AfterDelay(30000, e =>
            {
                Function.Call("setdvar", "bonus_double_points", 0);
            });
        }

        public static void InstaKill(Entity player)
        {
            if (Function.Call<int>("getdvarint", "bonus_insta_kill") == 1)
                return;

            PowerUpHudTimer(Hud.BonusDropHud("cardicon_skull_black", -35));
            Function.Call("setdvar", "bonus_insta_kill", 1);
            player.AfterDelay(30000, e =>
            {
                Function.Call("setdvar", "bonus_insta_kill", 0);
            });
        }

        public static void Nuke(Entity player)
        {
            player.WinCash(400);
            Hud.BonusDropTakeHud(player, "Nuke", "dpad_killstreak_nuke");

            foreach (var item in Utility.GetPlayers())
            {
                if (item.GetTeam() == "axis" && item.IsAlive)
                {
                    item.AfterDelay(600, e => item.SelfExploed());
                }
            }
        }

        public static void FireSale(Entity player)
        {
            if (Function.Call<int>("getdvarint", "bonus_fire_sale") == 1)
                return;

            PowerUpHudTimer(Hud.BonusDropHud("cardicon_joystick", 70));
            Function.Call("setdvar", "bonus_fire_sale", 1);
            player.AfterDelay(30000, e =>
            {
                Function.Call("setdvar", "bonus_fire_sale", 0);
            });
        }

        public static void BonusPoints(Entity player)
        {
            Hud.BonusDropTakeHud(player, "Bonus Points", "cardicon_8ball");

            foreach (var item in Utility.GetPlayers())
            {
                if (item.GetTeam() == "allies" && item.IsAlive)
                {
                    item.WinPoint(5);
                }
            }
        }

        public static void Carpenter(Entity player)
        {
            Hud.BonusDropTakeHud(player, "Carpenter", "cardicon_biohazard");

            foreach (var item in MapEdit.doors)
            {
                item.SetField("hp", item.GetField<int>("maxhp"));
            }
        }
    }
}
