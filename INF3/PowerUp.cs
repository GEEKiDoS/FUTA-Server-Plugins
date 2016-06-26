using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using InfinityScript;

namespace INF3
{
    public enum PowerUpType
    {
        /// <summary>
        /// 子弹全满
        /// </summary>
        MaxAmmo,
        /// <summary>
        /// 双倍点数
        /// </summary>
        DoublePoints,
        /// <summary>
        /// 一击必杀
        /// </summary>
        InstaKill,
        /// <summary>
        /// 核爆
        /// </summary>
        Nuke,
        /// <summary>
        /// 廉价火力
        /// </summary>
        FireSale,
        /// <summary>
        /// 点数奖励
        /// </summary>
        BonusPoints,
        /// <summary>
        /// 屏障恢复
        /// </summary>
        Carpenter
    }

    public class PowerUpEntity : IDisposable
    {
        public delegate void TriggerUse(Entity player);

        private Timer timer;
        private Timer warntimer = new Timer(500);
        private Timer rotatetimer = new Timer(5000);
        private Timer thinktimer = new Timer(100);

        public PowerUpType Type { get; }
        public Entity Entity { get; }
        public Entity FX { get; }
        public Vector3 Origin
        {
            get
            {
                return Entity.Origin;
            }
        }
        public int Timeout { get; set; }
        public TriggerUse OnTriggerUse { get; set; }

        public PowerUpEntity(PowerUpType type, string model, Vector3 origin, Vector3 angle, int fx, TriggerUse triggeruse)
        {
            Type = type;
            origin = origin += new Vector3(0, 0, 10);

            if (Type == PowerUpType.Nuke)
            {
                origin += new Vector3(0, 0, 40);
            }

            Entity = Utility.Spawn("script_model", origin);
            Entity.Call("setmodel", model);
            Entity.SetField("angles", angle);
            FX = Effects.SpawnFx(fx, origin);
            Timeout = GetTimeout();
            OnTriggerUse += triggeruse;

            PowerUpTimer();
            PowerUpRotate();
            PowerUpThink();

            AIZDebug.DebugLog(GetType(), "Create PowerUp: " + type);
        }

        private int GetTimeout()
        {
            if (Utility.GetDvar<int>("global_gobble_temporalgift") == 1)
            {
                return 45;
            }
            else
            {
                return 30;
            }
        }

        private void PowerUpThink()
        {
            thinktimer.AutoReset = true;
            thinktimer.Elapsed += (sender, e) =>
            {
                foreach (var item in Utility.Players)
                {
                    if (item.IsAlive && item.GetTeam() == "allies" && item.Origin.DistanceTo(Origin) < 50)
                    {
                        OnTriggerUse(item);
                        Dispose();
                    }
                }
            };
            thinktimer.Start();
        }

        private void PowerUpTimer()
        {
            timer = new Timer(Timeout - 1);
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) => PowerUpTimeoutWarning();
            timer.Start();
        }

        private void PowerUpTimeoutWarning()
        {
            const int MAX = 20;

            bool ishide = false;
            int num = 0;
            warntimer.AutoReset = true;
            warntimer.Elapsed += (sender, e) =>
            {
                if (ishide)
                {
                    Entity.Call("hide");
                    ishide = true;
                }
                else
                {
                    Entity.Call("show");
                    ishide = false;
                }
                num++;

                if (num == MAX)
                {
                    timer.Stop();
                    Dispose();
                }
            };
            warntimer.Start();
        }

        private void PowerUpRotate()
        {
            rotatetimer.AutoReset = true;
            rotatetimer.Elapsed += (sender, e) =>
            {
                Entity.Call("rotateyaw", -360, 5);
            };
            timer.Start();
        }

        public void Dispose()
        {
            timer.Dispose();
            warntimer.Dispose();
            rotatetimer.Dispose();
            thinktimer.Dispose();

            FX.Call("delete");
            Entity.Call("delete");

            AIZDebug.DebugLog(GetType(), "PowerUp Disposed.");
        }
    }

    public class PowerUp : BaseScript
    {
        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (mod == "MOD_SUICIDE")
                return;

            if (player.GetTeam() == "axis")
            {
                var random = Utility.Random.Next(5);
                var randomequels = Utility.Random.Next(5);
                if (random == randomequels)
                {
                    PowerUpDrop(player);
                }
            }
        }

        public static void PowerUpDrop(Entity player)
        {
            switch ((PowerUpType)Utility.Random.Next(Enum.GetNames(typeof(PowerUpType)).Length))
            {
                case PowerUpType.MaxAmmo:
                    new PowerUpEntity(PowerUpType.MaxAmmo, "com_plasticcase_friendly", player.Origin, player.GetField<Vector3>("angles"), Effects.greenbeaconfx, ent => MaxAmmo(ent));
                    break;
                case PowerUpType.DoublePoints:
                    new PowerUpEntity(PowerUpType.DoublePoints, "com_plasticcase_friendly", player.Origin, player.GetField<Vector3>("angles"), Effects.redbeaconfx, ent => DoublePoints(ent));
                    break;
                case PowerUpType.InstaKill:
                    new PowerUpEntity(PowerUpType.InstaKill, "com_plasticcase_trap_friendly", player.Origin, player.GetField<Vector3>("angles"), Effects.smallfirefx, ent => InstaKill(ent));
                    break;
                case PowerUpType.Nuke:
                    new PowerUpEntity(PowerUpType.Nuke, "projectile_cbu97_clusterbomb", player.Origin, player.GetField<Vector3>("angles") - new Vector3(90, 0, 0), Effects.smallfirefx, ent => Nuke(ent));
                    break;
                case PowerUpType.FireSale:
                    new PowerUpEntity(PowerUpType.FireSale, "com_plasticcase_enemy", player.Origin, player.GetField<Vector3>("angles"), Effects.greenbeaconfx, ent => FireSale(ent));
                    break;
                case PowerUpType.BonusPoints:
                    new PowerUpEntity(PowerUpType.BonusPoints, "com_plasticcase_enemy", player.Origin, player.GetField<Vector3>("angles"), Effects.redbeaconfx, ent => BonusPoints(ent));
                    break;
                case PowerUpType.Carpenter:
                    new PowerUpEntity(PowerUpType.Carpenter, "com_plasticcase_trap_friendly", player.Origin, player.GetField<Vector3>("angles"), Effects.greenbeaconfx, ent => Carpenter(ent));
                    break;
            }
        }

        private static void PowerUpHudTimer(HudElem hud)
        {
            var timer1 = new Timer(20000);
            var timer2 = new Timer(500);
            var timer3 = new Timer(250);

            timer1.AutoReset = false;
            timer1.Elapsed += (sender, e) =>
            {
                float i = 0;
                bool ishide = false;
                timer2.AutoReset = true;
                timer2.Elapsed += (sender2, e2) =>
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
                    if (i >= 5)
                    {
                        timer3.AutoReset = true;
                        timer3.Elapsed += (sender3, e3) =>
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
                            if (i >= 10)
                            {
                                hud.Call("destroy");

                                timer1.Dispose();
                                timer2.Dispose();
                                timer3.Dispose();
                            }
                        };
                        timer3.Start();
                        timer2.Stop();
                    }
                };
                timer2.Start();
            };
            timer1.Start();
        }

        public static void MaxAmmo(Entity player)
        {
            Hud.BonusDropTakeHud(player, "Max Ammo", "waypoint_ammo_friendly");
            foreach (var item in Utility.Players)
            {
                if (item.GetTeam() == "allies")
                {
                    MaxAmmo(player);
                }
            }
        }

        public static void DoublePoints(Entity player)
        {
            if (Utility.GetDvar<int>("bonus_double_points") == 1)
                return;

            PowerUpHudTimer(Hud.BonusDropHud("cardicon_spetsnaz", 0));
            Utility.SetDvar("bonus_double_points", 1);
            player.AfterDelay(30000, e =>
            {
                Function.Call("setdvar", "bonus_double_points", 0);
            });
        }

        public static void InstaKill(Entity player)
        {
            if (Utility.GetDvar<int>("bonus_insta_kill") == 1)
                return;

            PowerUpHudTimer(Hud.BonusDropHud("cardicon_skull_black", -35));
            Utility.SetDvar("bonus_insta_kill", 1);
            player.AfterDelay(30000, e =>
            {
                Function.Call("setdvar", "bonus_insta_kill", 0);
            });
        }

        public static void Nuke(Entity player)
        {
            player.WinCash(400);
            Hud.BonusDropTakeHud(player, "Nuke", "dpad_killstreak_nuke");

            foreach (var item in Utility.Players)
            {
                if (item.GetTeam() == "axis" && item.IsAlive)
                {
                    item.AfterDelay(600, e => item.Notify("self_exploed"));
                }
            }
        }

        public static void FireSale(Entity player)
        {
            if (Utility.GetDvar<int>("bonus_fire_sale") == 1)
                return;

            PowerUpHudTimer(Hud.BonusDropHud("cardicon_joystick", 70));
            Utility.SetDvar("bonus_fire_sale", 1);
            player.AfterDelay(30000, e =>
            {
                Function.Call("setdvar", "bonus_fire_sale", 0);
            });
        }

        public static void BonusPoints(Entity player)
        {
            Hud.BonusDropTakeHud(player, "Bonus Points", "cardicon_8ball");

            foreach (var item in Utility.Players)
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
                item.HP = item.MaxHP;
            }
        }
    }
}
