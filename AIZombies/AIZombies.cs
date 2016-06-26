using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;

/* Shitty Zombies by SinX||Bio
 * Written sometime between 2014-2015
 * Hans Krebs is bae
 * The code is dirtier than your mom, forgive me
 */
namespace aiZombies.auto
{
    public class aiZombies : BaseScript
    {
        /* Things to fix... sigh
         * Anti Collide [Attempted fix, Bots clamp together sometimes]
         * Clamp to ground [Attempted fix, seems working :)]
         * Kill event for zombies [Fixed]
         * Mystery lag on the mysterybox *Temp patch added, Redo Mysterybox for efficiency*
         * Last stand revive
         * Gambler
         * HUD Cash shit [Attempted fix, add floaty stuff later]
         * Bot waypoints
         * Killstreaks
         * Drops
         * Issue with waypoints [Niggah it don't work]
         */

        public List<Entity> Bots = new List<Entity>();
        public List<Entity> BotCrates = new List<Entity>();
        public List<Entity> Crates = new List<Entity>();
        public List<Vector3> Spawns = new List<Vector3>();
        public List<Vector3> Waypoints = new List<Vector3>();

        public Entity _airdropCollision;
        public int waves = 0;
        public int realBotNum = 0;
        public int howMany = 0;
        public int health = 0;
        public bool lego = false;
        public bool interMis = false;
        public int spawnNum = 0;
        public bool powerBool = false;
        public int botSpeed = 140;

        public aiZombies()
            : base()
        {

            //Prepare Script
            PlayerConnected += new Action<Entity>(pCon);
            Entity care_package = Call<Entity>("getent", "care_package", "targetname");
            _airdropCollision = Call<Entity>("getent", care_package.GetField<string>("target"), "targetname");
            preCache();
            mainHUD();
            demCrates();
            WaveHandle();
            getSpawn();
            setWaypoints();

        }

        public void pCon(Entity player)
        {
            initPlayer(player);

            player.SpawnedPlayer += () =>
            {
                if (player.GetField<int>("iSpawned") == 0)
                {
                    playerSpawn(player);
                    intro(player);
                }
                else
                {
                    playerSpawn2(player);
                }
            };
        }

        private void initPlayer(Entity player)
        {
            player.SetField("iSpawned", 0);
            player.SetField("hasJug", 0);
            player.SetField("hasBio", 0);
            player.SetField("hasSpeed", 0);
            player.SetField("hasRevive", 0);
            player.SetField("iLastStand", 0);
            //player.SetField("isShooting", 0);
            player.SetField("handlepop", 0);
            player.Health = 50;

            INF3.Utility.SetTeam(player, "axis");

            player.OnNotify("joined_team", delegate (Entity yourmom)
            {
                player.AfterDelay(500, meh => meh.Notify("menuresponse", new Parameter[] { "changeclass", "class1" }));
            });

            //NOH CHANGING
            player.OnNotify("menuresponse", (player2, menu, response) =>
            {
                if (menu.ToString().Equals("class") && response.ToString().Equals("changeclass_marines"))
                {
                    player.AfterDelay(100, (fr) =>
                    {
                        player.Notify("menuresponse", "changeclass", "back");
                    });
                }
            });
        }

        private void playerSpawn(Entity player)
        {
            //setVision(player, "blacktest");
            //AfterDelay(10000, () =>
            //{
            //    setVision(player, Call<string>("getdvar", "mapname"));
            //});
            player.Call("setorigin", Spawns.ElementAtOrDefault(spawnNum));
            player.SetField("iSpawned", 1);
            spawnNum++;
        }

        //When 1 method just isnt enough anymore, greedy bastards
        private void playerSpawn2(Entity player)
        {
            player.Call("setorigin", Spawns.ElementAtOrDefault(Call<int>("randomint", 3)));
            player.SetField("iSpawned", 1);
        }

        //Dis ist mein script
        public void intro(Entity player)
        {
            //HudElem intro = HudElem.CreateFontString(player, "hudbig", 1.6f);
            //intro.Color = new Vector3(0.156f, 0.54f, 0.84f);
            //intro.SetField("glowcolor", new Vector3(0.156f, 0.54f, 0.84f));
            //intro.GlowAlpha = 1f;
            //intro.SetText("Welcome to aiZombies");
            //intro.SetPoint("TOPRIGHT", "TOPRIGHT", -700, 100);
            //intro.Call("moveovertime", 10);
            //intro.X = 500;

            //HudElem intro2 = HudElem.CreateFontString(player, "hudbig", 1.6f);
            //intro2.SetText("CREATED BY SINX||BIO");
            //intro2.Color = new Vector3(0.156f, 0.54f, 0.84f);
            //intro2.SetField("glowcolor", new Vector3(0.156f, 0.54f, 0.84f));
            //intro2.GlowAlpha = 1f;
            //intro2.SetPoint("BOTTOMELEFT", "BOTTOMELEFT", 700, -100);
            //intro2.Call("moveovertime", 10);
            //intro2.X = -500;

            player.AfterDelay(10000, (fe) =>
            {
                if (!lego)
                {
                    startGame();
                }
                //intro.Call("destroy");
                //intro2.Call("destroy");
            });
        }

        //Server wide HUD
        public void mainHUD()
        {
            HudElem wave = HudElem.CreateServerFontString("hudbig", 0.8f);
            wave.Color = new Vector3(0.77f, 0f, 0.019f);
            wave.SetField("glowcolor", new Vector3(0.77f, 0f, 0.019f));
            wave.GlowAlpha = 1f;
            wave.Alpha = 1f;
            wave.SetPoint("BOTTOMLEFT", "BOTTOMLEFT", 20, -20); ;
            wave.SetText("Wave: 0");
            OnNotify("wave_ended", () =>
            {
                wave.SetText("Wave: " + waves.ToString());
            });

            HudElem zomCount = HudElem.CreateServerFontString("hudbig", 0.8f);
            zomCount.Color = new Vector3(0.77f, 0f, 0.019f);
            zomCount.SetField("glowcolor", new Vector3(0.77f, 0f, 0.019f));
            zomCount.GlowAlpha = 1f;
            zomCount.Alpha = 1f;
            zomCount.SetPoint("BOTTOMLEFT", "BOTTOMLEFT", 20, -40);
            zomCount.SetText("Zombies Alive:");

            HudElem zomCount2 = HudElem.CreateServerFontString("hudbig", 0.8f);
            zomCount2.Color = new Vector3(0.77f, 0f, 0.019f);
            zomCount2.SetField("glowcolor", new Vector3(0.77f, 0f, 0.019f));
            zomCount2.GlowAlpha = 1f;
            zomCount2.Alpha = 1f;
            zomCount2.SetPoint("BOTTOMLEFT", "BOTTOMLEFT", 160, -40);
            zomCount2.Call("setvalue", Bots.Count);

            OnNotify("killed_zombie", () =>
            {
                zomCount2.Call("setvalue", Bots.Count);
            });

            OnNotify("spawned_zombie", () =>
            {
                zomCount2.Call("setvalue", Bots.Count);
            });

            HudElem intermission = HudElem.CreateServerFontString("hudbig", 1.2f);
            intermission.Color = new Vector3(1f, 1f, 1f);
            intermission.SetField("glowcolor", new Vector3(0f, 1f, 0f));
            intermission.GlowAlpha = 1f;
            intermission.SetPoint("TOPCENTER", "TOPCENTER", 0, 40);
            intermission.Alpha = 0f;
            OnNotify("intermission", () =>
            {
                foreach (Entity player in Players)
                {
                    playerSound(player, "mp_level_up");
                }
                int time = 20;
                intermission.Alpha = 1f;
                OnInterval(1000, () =>
                {
                    if (time != 0)
                    {
                        intermission.SetText("Intermission: " + time.ToString());
                        time--;
                        return true;
                    }
                    else
                        Notify("intermission_over");
                    intermission.Alpha = 0f;
                    return false;
                });
            });
        }

        private void hudWaypoint(Entity ent, string shader, Vector3 client)
        {
            HudElem waypoint = HudElem.NewHudElem();
            waypoint.Parent = HudElem.UIParent;
            waypoint.SetShader(shader, 30, 30);
            waypoint.Alpha = 0.5f;
            waypoint.X = client.X;
            waypoint.Y = client.Y;
            waypoint.Z = client.Z + 30;
            waypoint.Call("setwaypoint", true, false);
        }

        private void spawnBot(Vector3 clientPos, int health, string model, string anim, int speed)
        {
            try
            {
                Entity bot = Call<Entity>("spawn", "script_model", clientPos);
                bot.Call("setmodel", model);
                bot.Call("scriptmodelplayanim", anim);
                bot.SetField("anim", anim);
                bot.Call("enablelinkto");
                Bots.Add(bot);
                bot.SetField("speed", speed);
                bot.SetField("stabilizing", 0);
                bot.SetField("anticollide", 0);
                bot.SetField("isAlive", 1);
                bot.SetField("attackingP", 0);
                bot.SetField("paralyzed", 0);
                bot.SetField("waypointMove", 0);
                Entity crate = Call<Entity>("spawn", "script_model", bot);
                crate.Call("setmodel", "com_plasticcase_green_big_us_dirt");
                crate.Call("solid");
                crate.Call("hide");
                crate.Call("setcandamage", true);
                crate.Health = health;
                Vector3 Angle;
                Angle.X = 0;
                Angle.Y = 0;
                Angle.Z = 0;
                Vector3 Angles;
                Angles.X = -20;
                Angles.Y = 0;
                Angles.Z = -15;
                crate.Call("enablelinkto");
                crate.Call("linkto", bot, "j_head", Angles, Angle);
                crate.Call(33353, _airdropCollision);
                BotCrates.Add(crate);

                botBrain(bot);
                clamptoground(bot);
                bot.AfterDelay(800, (br) =>
                {
                    antiCollide(bot);
                });


                crate.OnNotify("damage", (eInflictor, attacker, victim, iDamage, iDFlags, sMeansOfDeath, sWeapon, vPoint, vDir, sHitLoc, psOffsetTime) =>
                {
                    try
                    {
                        Entity player = (Entity)victim;
                        player.Call("playlocalsound", "MP_hit_alert");
                        if (crate.Health <= 0 && bot.GetField<int>("isAlive") == 1)
                        {
                            bot.SetField("isAlive", 0);
                            crate.Call("unlink");
                            crate.Call("delete");
                            playerSound(player, "generic_death_russian_1");

                            if (Bots.Contains(bot))
                            {
                                Bots.Remove(bot);
                                bot.AfterDelay(75, (dww) =>
                                {
                                    if (realBotNum > 0)
                                    {
                                        // ("DOING DIS");
                                        logD(realBotNum.ToString());
                                        realBotNum -= 1;
                                    }

                                    bot.Call("delete");
                                    //  ("Cleansing....");
                                    Notify("killed_zombie");
                                    player.Notify("killed_zombie");
                                });
                            }


                        }

                    }
                    catch (Exception e)
                    {
                        Log.Write(LogLevel.All, e.ToString());
                    }
                });

                Notify("spawned_zombie");
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.All, e.ToString());
            }
        }


        private void botBrain(Entity bot)
        {
            try
            {

                bot.OnInterval(100, (fu) =>
                {
                    if (bot.GetField<int>("isAlive") == 1)
                    {
                        float distance = 9999999f;
                        Entity target = null;
                        string angles = "";
                        Vector3 MoveLoc = new Vector3(0, 0, 0);

                        foreach (Entity player in INF3.Utility.Players)
                        {
                            if (player.IsAlive)
                            {
                                if (Call<int>(116, bot.Origin + new Vector3(0, 0, 75), player.Origin + new Vector3(0, 0, 65), false, bot) >= 1)
                                {
                                    if (player.GetField<string>("sessionteam") == "axis" || player.GetField<string>("sessionteam") == "allies")
                                    {
                                        if (player.GetField<int>("iLastStand") == 0)
                                        {
                                            if (Call<float>("distancesquared", bot.Origin, player.Origin) < distance)
                                            {

                                                distance = Call<float>("distancesquared", bot.Origin, player.Origin);
                                                target = player;
                                                bot.SetField("targetP", player.Name);
                                                angles = "player";
                                            }

                                            if (angles == "player")
                                            {
                                                MoveLoc = Call<Vector3>("vectortoangles", target.Origin - bot.Origin);
                                                bot.SetField("waypointMove", 0);
                                                if (bot.GetField<int>("anticollide") == 0)
                                                {
                                                    //bot.Call("scriptmodelplayanim", bot.GetField<string>("anim"));
                                                    bot.AfterDelay(100, (fgu) =>
                                                    {
                                                        bot.Call("rotateto", new Vector3(0, MoveLoc.Y, 0), 0.1f);
                                                        //(Call<float>("distance", bot.Origin, target.Origin).ToString());
                                                        if (bot.GetField<int>("stabilizing") == 0)
                                                        {
                                                            if (bot.Origin.DistanceTo(target.Origin) <= 70)
                                                            {
                                                                if (bot.Origin.DistanceTo(target.Origin) <= 50)
                                                                {
                                                                    if (bot.GetField<int>("attackingP") == 0)
                                                                        zombieAttack(bot, target);
                                                                }
                                                                bot.Call("moveto", bot.Origin, 0.1f);
                                                            }
                                                            else
                                                            {
                                                                bot.Call("moveto", target.Origin, Call<float>("distance", bot.Origin, target.Origin) / bot.GetField<int>("speed"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Vector3 temp = target.Origin;
                                                            temp.Z = (bot.GetField<int>("stabilizing"));
                                                            if (bot.Origin.DistanceTo(target.Origin) <= 70)
                                                            {
                                                                if (bot.Origin.DistanceTo(target.Origin) <= 50)
                                                                {
                                                                    if (bot.GetField<int>("attackingP") == 0)
                                                                        zombieAttack(bot, target);
                                                                }
                                                                bot.Call("moveto", bot.Origin, 0f);
                                                            }
                                                            else
                                                            {
                                                                bot.Call("moveto", target.Origin, Call<float>("distance", bot.Origin, target.Origin) / bot.GetField<int>("speed"));
                                                            }
                                                        }
                                                    });
                                                }
                                                else
                                                {
                                                    //bot.Call("scriptmodelplayanim", "mp_stand_idle");
                                                }
                                            }

                                            if (target == null && bot.GetField<int>("waypointMove") == 0)
                                            {
                                                //Do waypoint things
                                                logD("Waypoint");
                                                bot.SetField("waypointMove", 1);
                                                botwaypoint(bot);
                                            }
                                            else
                                            {
                                                //Do other things
                                            }

                                        }
                                    }
                                }
                                else if (bot.GetField<int>("waypointMove") == 0)
                                {
                                    //Do waypoint things
                                    logD("Waypoint");
                                    bot.SetField("waypointMove", 1);
                                    botwaypoint(bot);
                                }
                            }
                            else if (bot.GetField<int>("waypointMove") == 0)
                            {
                                //Do waypoint things
                                botwaypoint(bot);
                            }
                        }
                        return true;
                    }

                    else
                    {
                        return false;
                    }

                });
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.All, e.ToString());
            }
        }

        private void zombieAttack(Entity bot, Entity target)
        {
            if (bot.GetField<int>("attackingP") == 0)
            {
                bot.SetField("attackingP", 1);
                //Entity knife = Call<Entity>("spawn", "script_model", bot.Call<Vector3>("gettagorigin", "tag_inhand"));
                //knife.Call("setmodel", "weapon_parabolic_knife");
                //knife.Call("linkto", bot, "tag_inhand");
                bot.Call("scriptmodelplayanim", "pt_melee_pistol_1");
                bot.AfterDelay(200, (frf) =>
                {
                    bot.Call("playsound", "melee_knife_stab");
                    //var oldHealth = target.Health;
                    //target.Health -= 25;
                    target.Call("finishplayerdamage", target, target, 25, 0, 0, "bomb_site_mp", target.Origin, "MOD_SUICIDE", 0);
                    // target.Notify("damage", (oldHealth - target.Health), bot, new Vector3(0, 0, 0), new Vector3(0, 0, 0), "MOD_MELEE", "", "", "", 0, "throwingknife_mp");
                    bot.AfterDelay(100, (frff) =>
                    {
                        //knife.Call("delete");
                        bot.Call("scriptmodelplayanim", bot.GetField<string>("anim"));
                    });
                });
                bot.AfterDelay(1500, (fdd) =>
                {
                    bot.SetField("attackingP", 0);
                });
            }
        }

        private void antiCollide(Entity bot)
        {

            bot.OnInterval(100, (fu) =>
            {
                if (bot.GetField<int>("isAlive") == 1)
                {
                    float distance = 999999f;
                    Entity target = null;

                    foreach (Entity ai in Bots)
                    {
                        if (bot != ai && ai.GetField<int>("anticollide") == 0)
                        {
                            if (Call<float>("distance", bot.Origin, ai.Origin) < distance)
                            {
                                distance = Call<float>("distance", bot.Origin, ai.Origin);
                                if (distance < 60)
                                {
                                    target = ai;
                                }
                            }
                        }
                    }
                    if (target != null && distance < 60 && bot.GetField<int>("anticollide") == 0)
                    {
                        bot.SetField("anticollide", 1);
                        bot.Call("moveto", bot.Origin, 0f);
                        //  ("[AntiCollide]: Stalling");
                        bot.AfterDelay(300, (fu2) =>
                        {
                            bot.SetField("anticollide", 0);
                        });
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        private void clamptoground(Entity bot)
        {
            switch (Call<string>("getdvar", "mapname"))
            {
                //case "mp_dome":
                //    break;

                case "mp_hardhat":
                    Vector3 EdgeOfShiz = new Vector3(-1318f, -815f, 540);
                    bot.OnInterval(1000, (frrr) =>
                    {
                        //Safety Delete [Temp fix]
                        if (bot.Origin.DistanceTo(new Vector3(0, 0, 0)) == 0)
                            bot.Call("delete");
                        if (bot.GetField<int>("isAlive") == 1)
                        {
                            //("[ClampToGround]: " + bot.Origin.ToString() + "   " + EdgeOfShiz.ToString());
                            //("[ClampToGround]: " + (bot.Origin.Z - EdgeOfShiz.Z).ToString());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    bot.OnInterval(50, (fu) =>
                    {
                        if (bot.GetField<int>("isAlive") == 1)
                        {
                            if (bot.Origin.DistanceTo(EdgeOfShiz) < 100)
                            {
                                if ((EdgeOfShiz.Z - bot.Origin.Z) > 5)
                                {
                                    bot.SetField("stabilizing", Convert.ToInt32(EdgeOfShiz.Z));
                                    Vector3 temp = bot.Origin;
                                    temp.Z = EdgeOfShiz.Z;
                                    bot.Call("moveto", temp, 0.1f);
                                    // ("stabilizing");
                                    bot.AfterDelay(200, (fy) =>
                                    {
                                        bot.SetField("stabilizing", 0);
                                    });
                                }
                            }
                            else if (bot.Origin.DistanceTo(EdgeOfShiz) > 100)
                            {
                                if ((EdgeOfShiz.Z - bot.Origin.Z) < -5)
                                {
                                    bot.SetField("stabilizing", Convert.ToInt32(EdgeOfShiz.Z));
                                    Vector3 temp = bot.Origin;
                                    temp.Z = EdgeOfShiz.Z;
                                    bot.Call("moveto", temp, 0.1f);
                                    //   ("stabilizing");
                                    bot.AfterDelay(200, (fy) =>
                                    {
                                        bot.SetField("stabilizing", 0);
                                    });
                                }
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    break;

                case "mp_village":
                    float distance = 999999f;
                    Vector3 target = new Vector3(0, 0, 0);
                    List<Vector3> clamps = new List<Vector3>();
                    clamps.Add(new Vector3(649f, 793f, 287f));
                    clamps.Add(new Vector3(1020f, 983f, 240f));
                    clamps.Add(new Vector3(1542, 303f, 237f));
                    clamps.Add(new Vector3(1129f, 167f, 267f));
                    clamps.Add(new Vector3(820f, 533f, 317f));
                    clamps.Add(new Vector3(1207f, 528f, 294f));

                    bot.OnInterval(100, (di) =>
                    {
                        if (bot.GetField<int>("isAlive") == 1)
                        {
                            foreach (Vector3 clamp in clamps)
                            {
                                if (Call<float>("distance", bot.Origin, clamp) < distance)
                                {
                                    target = clamp;
                                }
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });

                    bot.OnInterval(50, (fu) =>
                    {
                        if (bot.GetField<int>("isAlive") == 1)
                        {
                            if (bot.Origin.DistanceTo(target) < 150)
                            {
                                if ((target.Z - bot.Origin.Z) > 5)
                                {
                                    bot.SetField("stabilizing", Convert.ToInt32(target.Z));
                                    Vector3 temp = bot.Origin;
                                    temp.Z = target.Z;
                                    bot.Call("moveto", temp, 0.1f);
                                    //     ("stabilizing");
                                    bot.AfterDelay(200, (fy) =>
                                    {
                                        bot.SetField("stabilizing", 0);
                                    });
                                }
                            }
                            else if (bot.Origin.DistanceTo(target) > 150)
                            {
                                if ((target.Z - bot.Origin.Z) < -5)
                                {
                                    bot.SetField("stabilizing", Convert.ToInt32(target.Z));
                                    Vector3 temp = bot.Origin;
                                    temp.Z = target.Z;
                                    bot.Call("moveto", temp, 0.1f);
                                    //   ("stabilizing");
                                    bot.AfterDelay(200, (fy) =>
                                    {
                                        bot.SetField("stabilizing", 0);
                                    });
                                }
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    break;

                //case "mp_bootleg":
                //    break;

                default:
                    break;
            }
        }

        private void botwaypoint(Entity bot)
        {
            try
            {
                float distance = 999999f;

                bot.OnInterval(100, (fee) =>
                {
                    if (bot.GetField<int>("isAlive") == 1 && bot.GetField<int>("waypointMove") == 1)
                    {
                        foreach (Vector3 waypoint in Waypoints)
                        {
                            if (Call<float>("distance", bot.Origin, waypoint) < distance)
                            {
                                distance = Call<float>("distance", bot.Origin, waypoint);
                                bot.Call("moveto", waypoint, Call<float>("distance", bot.Origin, waypoint) / bot.GetField<int>("speed"));
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                logD(e.ToString());
            }
        }

        private void setWaypoints()
        {
            switch (Call<string>("getdvar", "mapname"))
            {
                case "mp_village":
                    Waypoints.Add(new Vector3(501f, 783f, 279f));
                    Waypoints.Add(new Vector3(1117f, 1157f, 248f));
                    Waypoints.Add(new Vector3(1505f, 1006f, 250f));
                    Waypoints.Add(new Vector3(1614f, 309f, 242f));
                    Waypoints.Add(new Vector3(1733f, -475f, 239f));
                    Waypoints.Add(new Vector3(985f, 129f, 275f));
                    Waypoints.Add(new Vector3(683f, 486f, 284f));
                    break;

                case "mp_hardhat":
                    break;
            }
        }

        private Vector3 zombieSpawn()
        {
            switch (Call<string>("getdvar", "mapname"))
            {
                //case "mp_dome":
                //    switch (Call<int>("randomint", 6))
                //    {
                //        case 1:
                //            break;
                //        case 2:
                //            break;
                //        case 3:
                //            break;
                //        case 4:
                //            break;
                //        case 5:
                //            break;
                //        default:
                //            break;
                //    }
                //    break;

                case "mp_hardhat":
                    switch (Call<int>("randomint", 4))
                    {
                        case 0:
                            return new Vector3(-1413f, -1162f, 470f);
                        case 1:
                            return new Vector3(-1286f, -1146f, 465f);
                        case 2:
                            return new Vector3(-1269F, -944F, 477F);
                        case 3:
                            return new Vector3(-1413f, -1162f, 469f);
                        default:
                            return new Vector3(-1287f, -1146f, 465f);
                    }

                case "mp_village":
                    switch (Call<int>("randomint", 3))
                    {
                        case 0:
                            return new Vector3(1756f, -484f, 236f);
                        case 1:
                            return new Vector3(408f, 790f, 271f);
                        case 2:
                            return new Vector3(1551f, 1100f, 249f);
                        default:
                            return new Vector3(1756f, -484f, 236f);
                    }

                //case "mp_seatown":
                //    switch (Call<int>("randomint", 6))
                //    {
                //        case 1:
                //            break;
                //        case 2:
                //            break;
                //        case 3:
                //            break;
                //        case 4:
                //            break;
                //        case 5:
                //            break;
                //        default:
                //            break;
                //    }
                //    break;

                //case "mp_bootleg":
                //    switch (Call<int>("randomint", 6))
                //    {
                //        case 1:
                //            break;
                //        case 2:
                //            break;
                //        case 3:
                //            break;
                //        case 4:
                //            break;
                //        case 5:
                //            break;
                //        default:
                //            break;
                //    }
                //    break;

                default:
                    return new Vector3(0, 0, 0);
            }
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (attacker == null || !attacker.IsPlayer || INF3.Utility.GetTeam(attacker) == INF3.Utility.GetTeam(player))
                return;
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            INF3.Utility.SetTeam(player, "axis");
        }


        private int randomspeed()
        {
            switch (Call<int>("randomint", 3))
            {
                case 0:
                    return 150;

                case 1:
                    return 140;

                case 2:
                    return 130;

                default:
                    return 150;
            }
        }

        private void iprint(Entity player, string message)
        {
            player.Call("iprintlnbold", message);
        }
        private void setVision(Entity player, string vision)
        {
            player.Call("visionsetnakedforplayer", vision);
        }
        private void getSpawn()
        {
            switch (Call<string>("getdvar", "mapname"))
            {
                case "mp_hardhat":
                    Spawns.Add(new Vector3(-1236f, 1086f, 547f));
                    Spawns.Add(new Vector3(-1299f, 1086f, 541f));
                    Spawns.Add(new Vector3(-1345f, 1086f, 542f));
                    Spawns.Add(new Vector3(-1405f, 1086f, 543f));
                    Spawns.Add(new Vector3(-1415f, 987f, 540f));
                    Spawns.Add(new Vector3(-1245f, 940f, 533f));
                    break;

                case "mp_village":
                    Spawns.Add(new Vector3(980f, 295f, 280f));
                    Spawns.Add(new Vector3(1149f, 144f, 277f));
                    Spawns.Add(new Vector3(1533f, 266f, 245f));
                    Spawns.Add(new Vector3(1642f, 763f, 252f));
                    Spawns.Add(new Vector3(1048f, 1116f, 248f));
                    Spawns.Add(new Vector3(372f, 648f, 273f));
                    break;
            }
        }

        public string getModel()
        {
            switch (Call<string>("getdvar", "mapname"))
            {
                case "mp_hardhat":
                    switch (Call<int>("randomint", 2))
                    {
                        case 0:
                            return "mp_body_delta_elite_assault_aa";
                        case 1:
                            return "mp_body_delta_elite_lmg_a";
                        case 2:
                            return " mp_body_delta_elite_smg_a";
                        default:
                            return "mp_body_delta_elite_assault_aa";
                    }

                case "mp_village":
                    switch (Call<int>("randomint", 1))
                    {
                        case 0:
                            return "mp_body_pmc_africa_assault_a";
                        default:
                            return "mp_body_pmc_africa_assault_a";
                    }

                default:
                    return "";
            }
        }


        private void logD(string mes)
        {
            Log.Write(LogLevel.All, mes);
        }

        private void playerSound(Entity player, string sound)
        {
            player.Call("playlocalsound", sound);
        }

        [STAThread]
        private void preCache()
        {
            Call("setdvar", "g_hardcore", 1);
            Call("precachemodel", "com_plasticcase_friendly");
            Call("precachemodel", "com_plasticcase_enemy");
            Call("precachemodel", "com_plasticcase_green_big_us_dirt");
            Call("precachempanim", "pb_sprint");
            Call("precachempanim", "pb_run_heavy");
            Call("precachempanim", "pt_melee_pistol_1");
            Call("precachempanim", "pb_shotgun_death_spinL");
            Call("precacheshader", "hud_icon_g36c");
            Call("precacheshader", "hud_icon_g36c_mp");
            Call("precachemodel", "mp_fullbody_opforce_juggernaut");
            Call("precacheshader", "waypoint_ammo_friendly");
            Call("precacheshader", "iw5_cardicon_coffee");
            Call("precacheshader", "iw5_cardicon_juggernaut_a");
            Call("precacheshader", "specialty_finalstand");
            Call("precacheshader", "iw5_cardicon_cat");
            Call("precacheshader", "cardicon_treasurechest");
            Call("precacheshader", "cardicon_girlskull");
            Call("precacheshader", "cardicon_skull_black");
            Call("precacheshader", "cardicon_bulb");
            Call("precachemodel", "mp_body_delta_elite_assault_aa");
            Call("precachemodel", "mp_body_delta_elite_lmg_a");
            Call("precachemodel", "mp_body_delta_elite_smg_a");
        }

        private void initSDvar(string dvar, string value)
        {
            Call("setdvarifuninitialized", dvar, value);
        }

        private void initIDvar(string dvar, int value)
        {
            Call("setdvarifuninitialized", dvar, value);
        }

        private string stringDvar(string dvar)
        {
            return Call<string>("getdvar", dvar);
        }

        private int intDvar(string dvar)
        {
            return Call<int>("getdvar", dvar);
        }

        private void demCrates()
        {
            string map = Call<string>("getdvar", "mapname");

            switch (map)
            {
                case "mp_dome":
                    break;

                case "mp_hardhat":
                    CreateWall(new Vector3(-1500f, 1241f, 650f), new Vector3(-1170, 1241f, 530f));
                    CreateWall(new Vector3(-1159f, 1241f, 650f), new Vector3(-1160f, 580f, 500f));
                    CreateWall(new Vector3(-1512f, 1232f, 650f), new Vector3(-1512f, 300f, 520f));
                    CreateWall(new Vector3(-1196f, -823f, 601f), new Vector3(-1196f, -1740f, 400f));
                    break;

                case "mp_village":
                    CreateWall(new Vector3(1383f, -512f, 285f), new Vector3(1430f, -681f, 368f));
                    CreateWall(new Vector3(1109f, -50f, 290f), new Vector3(1028f, -66f, 365f));
                    CreateWall(new Vector3(1153f, 1274f, 248f), new Vector3(1004f, 1217f, 371f));
                    CreateWall(new Vector3(412f, 942f, 296f), new Vector3(159f, 732f, 369f));
                    break;
            }
        }

        private Entity spawnCrate(Vector3 client, string model, Vector3 angles, string shader)
        {
            try
            {
                Entity crate = Call<Entity>("spawn", "script_model", client);
                crate.Call("setmodel", model);
                crate.SetField("angles", angles);
                crate.Call(33353, _airdropCollision);
                hudWaypoint(crate, shader, client);
                return crate;
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.All, e.ToString());
                return null;
            }
        }

        private Entity spawnSpecialCrate(Vector3 client, string model, Vector3 angles, string shader)
        {
            try
            {
                Vector3 temp = client;
                temp.Z += 300;
                Entity crate = Call<Entity>("spawn", "script_model", temp);
                crate.Call("setmodel", model);
                crate.SetField("angles", angles);
                crate.Call(33353, _airdropCollision);
                crate.Call("moveto", client, 3f);
                hudWaypoint(crate, shader, client);
                return crate;
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.All, e.ToString());
                return null;
            }
        }

        private Entity spawnCrate2(Vector3 client, string model, Vector3 angles)
        {
            try
            {
                Entity crate = Call<Entity>("spawn", "script_model", client);
                crate.Call("setmodel", model);
                crate.SetField("angles", angles);
                crate.Call(33353, _airdropCollision);
                return crate;
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.All, e.ToString());
                return null;
            }
        }

        //Stolen create wall, Too lazy to remake...
        private Entity CreateWall(Vector3 start, Vector3 end)
        {
            float D = new Vector3(start.X, start.Y, 0).DistanceTo(new Vector3(end.X, end.Y, 0));
            float H = new Vector3(0, 0, start.Z).DistanceTo(new Vector3(0, 0, end.Z));
            int blocks = (int)Math.Round(D / 55, 0);
            int height = (int)Math.Round(H / 30, 0);

            Vector3 C = end - start;
            Vector3 A = new Vector3(C.X / blocks, C.Y / blocks, C.Z / height);
            float TXA = A.X / 4;
            float TYA = A.Y / 4;
            Vector3 angle = Call<Vector3>("vectortoangles", new Parameter(C));
            angle = new Vector3(0, angle.Y, 90);
            Entity center = Call<Entity>("spawn", "script_origin", new Parameter(new Vector3(
                (start.X + end.X) / 2, (start.Y + end.Y) / 2, (start.Z + end.Z) / 2)));
            for (int h = 0; h < height; h++)
            {
                Entity crate = spawnCrate2((start + new Vector3(TXA, TYA, 10) + (new Vector3(0, 0, A.Z) * h)), "com_plasticcase_friendly", angle);
                crate.Call("enablelinkto");
                crate.Call("linkto", center);
                for (int i = 0; i < blocks; i++)
                {
                    crate = spawnCrate2(start + (new Vector3(A.X, A.Y, 0) * i) + new Vector3(0, 0, 10) + (new Vector3(0, 0, A.Z) * h), "com_plasticcase_friendly", angle);
                    crate.Call("enablelinkto");
                    crate.Call("linkto", center);
                }
                crate = spawnCrate2(new Vector3(end.X, end.Y, start.Z) + new Vector3(TXA * -1, TYA * -1, 10) + (new Vector3(0, 0, A.Z) * h), "com_plasticcase_friendly", angle);
                crate.Call("enablelinkto");
                crate.Call("linkto", center);
            }
            return center;
        }
        private void WaveHandle()
        {
            //Deternmine how many bots to spawn
            //Determine Health, speed based on player numbers
            //Crawler waves
            //Randomize Speed||Animation

            OnNotify("intermission_over", () =>
            {
                lego = false;
            });

            OnNotify("game_lego", () =>
            {
                AfterDelay(5000, () =>
                {
                    waves++;
                    if (health > 100)
                    {
                        health = 250;
                    }
                    else
                    {
                        health += 50;
                    }
                    howMany += 10;
                    //botSpeed += 5;
                    //   ("Wave Handle");
                    realBotNum = howMany;
                    OnInterval(400, () =>
                    {
                        if (realBotNum != 0)
                        {
                            if (Bots.Count < 20 && !interMis && realBotNum > 0)
                            {
                                switch (waves)
                                {
                                    case 1:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 2:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 3:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 4:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 5:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 6:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 7:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 8:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 9:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 10:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 11:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 12:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 13:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 14:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 15:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 16:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 17:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 18:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 19:
                                        spawnBot(zombieSpawn(), health, getModel(), "pb_sprint", randomspeed());
                                        break;

                                    case 20:
                                        howMany = 10;
                                        spawnBot(zombieSpawn(), 10000, "mp_fullbody_opforce_juggernaut", "pb_run_heavy", 100);
                                        break;
                                }
                            }
                            return true;
                        }
                        else if (Bots.Count == 0)
                        {
                            AfterDelay(3000, () =>
                            {
                                //   ("Intermission...");

                                //Because fuck my shitty coding

                                foreach (Entity ent in Bots)
                                {
                                    ent.Call("delete");
                                }

                                foreach (Entity ent2 in BotCrates)
                                {
                                    ent2.Call("delete");
                                }

                                Notify("intermission");
                                Notify("wave_ended");
                            });
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    });

                });
            });
        }

        private void startGame()
        {
            //If Playerz contains more than 1 start game
            //If game is started and player joins && players contains 1 fast_restart||rotate
            OnInterval(1000, () =>
            {
                if (INF3.Utility.Players.Count > 0)
                {
                    if (!lego)
                    {
                        //   ("Game ready!");
                        Notify("game_lego");
                        lego = true;
                    }
                }
                return true;
            });

            OnNotify("game_lego", () =>
            {
                HudElem begin = HudElem.CreateServerFontString("hudbig", 2f);
                begin.SetPoint("CENTER", "CENTER", 0, 0);
                begin.Color = new Vector3(0.77f, 0f, 0.019f);
                begin.SetField("glowcolor", new Vector3(0.77f, 0f, 0.019f));
                begin.GlowAlpha = 1f;
                begin.Alpha = 1f;
                begin.SetText("The Zombies Are Coming...");

                if (lego)
                {
                    foreach (Entity player in INF3.Utility.Players)
                    {
                        playerSound(player, "fasten_seatbelts");
                    }
                    AfterDelay(5000, () =>
                    {
                        foreach (Entity player in INF3.Utility.Players)
                        {
                            playerSound(player, "missile_incoming");
                        }
                        begin.Call("destroy");
                    });
                }
            });
        }
    }
}