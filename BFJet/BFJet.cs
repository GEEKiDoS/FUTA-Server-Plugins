using InfinityScript;
using System;
using System.Text;

namespace BFJet
{
    public class BFJet : BaseScript
    {
        public BFJet()
        {
            Call("precacheshader", new Parameter[]
            {
                "remote_turret_overlay_mp"
            });
            PlayerConnected += delegate (Entity ent)
            {
                ent.SetField("isjet", 0);
                ent.SetField("shoot", 0);
                ent.SetField("launched", 0);
                //ent.SetField("up", 0);
                //ent.SetField("down", 0);
                //ent.SetField("turnleft", 0);
                //ent.SetField("turnright", 0);
                ent.SetField("tumbleleft", 0);
                ent.SetField("tumbleright", 0);
                ent.SetField("boost", 0);
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "up",
                //    "+forward"
                //});
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "Nup",
                //    "-forward"
                //});
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "down",
                //    "+back"
                //});
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "Ndown",
                //    "-back"
                //});
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "turnleft",
                //    "+smoke"
                //});
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "Nturnleft",
                //    "-smoke"
                //});
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "turnright",
                //    "+melee_zoom"
                //});
                //ent.Call("notifyonplayercommand", new Parameter[]
                //{
                //    "Nturnright",
                //    "-melee_zoom"
                //});
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "tumbleleft",
                    "+moveleft"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "Ntumbleleft",
                    "-moveleft"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "tumbleright",
                    "+moveright"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "Ntumbleright",
                    "-moveright"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "shoot",
                    "+attack"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "Nshoot",
                    "-attack"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "launch",
                    "+toggleads_throw"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "boost",
                    "+breath_sprint"
                });
                ent.Call("notifyonplayercommand", new Parameter[]
                {
                    "Nboost",
                    "-breath_sprint"
                });
                ent.OnNotify("shoot", self =>
                {
                    ent.SetField("shoot", 1);
                });
                ent.OnNotify("Nshoot", self =>
                {
                    ent.SetField("shoot", 0);
                });
                ent.OnNotify("boost", self =>
                {
                    ent.SetField("boost", 1);
                });
                ent.OnNotify("Nboost", self =>
                {
                    ent.SetField("boost", 0);
                });
                ent.OnNotify("up", self =>
                {
                    ent.SetField("up", 1);
                });
                ent.OnNotify("Nup", self =>
                {
                    ent.SetField("up", 0);
                });
                ent.OnNotify("down", self =>
                {
                    ent.SetField("down", 1);
                });
                ent.OnNotify("Ndown", self =>
                {
                    ent.SetField("down", 0);
                });
                ent.OnNotify("turnleft", self =>
                {
                    ent.SetField("turnleft", 1);
                });
                ent.OnNotify("Nturnleft", self =>
                {
                    ent.SetField("turnleft", 0);
                });
                ent.OnNotify("turnright", self =>
                {
                    ent.SetField("turnright", 1);
                });
                ent.OnNotify("Nturnright", self =>
                {
                    ent.SetField("turnright", 0);
                });
                ent.OnNotify("tumbleleft", self =>
                {
                    ent.SetField("tumbleleft", 1);
                });
                ent.OnNotify("Ntumbleleft", self =>
                {
                    ent.SetField("tumbleleft", 0);
                });
                ent.OnNotify("tumbleright", self =>
                {
                    ent.SetField("tumbleright", 1);
                });
                ent.OnNotify("Ntumbleright", self =>
                {
                    ent.SetField("tumbleright", 0);
                });

                MakeHUD(ent);
                ent.SpawnedPlayer += delegate
                {
                    PlayerSpawned(ent);
                };
            };
        }

        private void ResetDefault(Entity ent)
        {
            ent.SetField("isjet", 0);
            ent.SetField("shoot", 0);
            ent.SetField("launched", 0);
            ent.SetField("up", 0);
            ent.SetField("down", 0);
            ent.SetField("turnleft", 0);
            ent.SetField("turnright", 0);
            ent.SetField("tumbleleft", 0);
            ent.SetField("tumbleright", 0);
            ent.SetField("boost", 0);
        }

        private void PlayerSpawned(Entity player)
        {
            ResetDefault(player);

            player.Origin = new Vector3(player.Origin.X, player.Origin.Y, player.Origin.Z + 5000f);
            JetStart(player, 100, 150);
        }

        private void JetStart(Entity player, int Speed, int BoostSpeed)
        {
            if (player.GetField<int>("isjet") == 1)
            {
                return;
            }
            player.SetField("isjet", 1);
            player.Call("hide");
            player.TakeAllWeapons();

            player.Call("attach", "vehicle_av8b_harrier_jet_mp");

            Entity Damagebox = Call<Entity>("SpawnVehicle", "vehicle_av8b_harrier_jet_mp", "airstrikeheight", "mp_killstreak_jet",player.Origin,player.Call<Vector3>("getplayerangles"));
            Damagebox.Call("setVehWeapon", "harrier_20mm_mp");
            Damagebox.Call("setcandamage");
            Damagebox.Call("hide");

            Damagebox.OnNotify("damage", delegate (Entity self, Parameter damage, Parameter attacker, Parameter direction_vec, Parameter point, Parameter meansOfDeath, Parameter modelName, Parameter tagName, Parameter partName, Parameter iDFlags, Parameter weapon)
            {
                player.Call("finishplayerdamage", self, attacker.As<Entity>(), damage.As<int>(), iDFlags.As<int>(), meansOfDeath.As<string>(), weapon.As<string>(), direction_vec.As<Vector3>(), point.As<Vector3>(), tagName.As<string>(), 0);
            });

            //player.Call("cameralinkto", Jet);
            player.Call("controlslinkto", Damagebox);

            Damagebox.Call("linkto", player);

            player.SetField("damagebox", Damagebox);

            OnInterval(100, () =>
            {
                JetMoving(player, Speed, BoostSpeed);

                return player.IsAlive;
            });

            Call("playfxontag", new Parameter[]
            {
                Call<int>("loadfx", new Parameter[]
                {
                    "smoke/jet_contrail"
                }),
                player,
                "tag_right_wingtip"
            });
            Call("playfxontag", new Parameter[]
            {
                Call<int>("loadfx", new Parameter[]
                {
                    "smoke/jet_contrail"
                }),
                player,
                "tag_left_wingtip"
            });
            Call("playfxontag", new Parameter[]
            {
                Call<int>("loadfx", new Parameter[]
                {
                    "fire/jet_afterburner"
                }),
                player,
                "tag_engine_right"
            });
            Call("playfxontag", new Parameter[]
            {
                Call<int>("loadfx", new Parameter[]
                {
                    "fire/jet_afterburner"
                }),
                player,
                "tag_engine_left"
            });
            player.Call("playloopsound", new Parameter[]
            {
                "veh_aastrike_flyover_loop"
            });
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (player.GetField<int>("isjet") == 1)
            {
                Call("playfx", Call<int>("loadfx", "explosions/aerial_explosion_ac130_coop"), player.Origin);
                player.Call("playsound", new Parameter[] { "cobra_helicopter_crash" });
                player.Call("stopLoopSound");
                player.Call("detachall");
                player.Call("entityradiusdamage", player.Origin, 300, 400, 200);

                player.GetField<Entity>("damagebox").Call("delete");
                player.SetField("isjet", 0);
            }
        }

        private void MakeHUD(Entity player)
        {
            HudElem screen = HudElem.NewClientHudElem(player);
            screen.X = 0f;
            screen.Y = 0f;
            screen.AlignX = "left";
            screen.AlignY = "top";
            screen.HorzAlign = "fullscreen";
            screen.VertAlign = "fullscreen";
            screen.SetShader("remote_turret_overlay_mp", 640, 480);
            screen.Sort = -10;
            screen.Archived = true;

            OnInterval(100, () =>
            {
                if (player.GetField<int>("isjet") == 0)
                {
                    screen.Alpha = 0f;
                }
                else
                {
                    screen.Alpha = 1f;
                }

                return player != null;
            });
        }

        private void JetMoving(Entity player, int Speed, int BoostSpeed)
        {
            int limitspeed = Speed + BoostSpeed;
            int minspeed = Speed;
            Vector3 angles = player.Call<Vector3>("getplayerangles");

            Entity engine = Call<Entity>("spawn", "script_origin", player.Origin);
            player.Call("playerlinkto", engine);
            player.Call("setstablemissile");

            if (player.GetField<int>("tumbleleft") == 1)
            {
                angles.Z = angles.Z + 5;
            }
            if (player.GetField<int>("tumbleright") == 1)
            {
                angles.Z = angles.Z - 5;
            }

            if (player.GetField<int>("boost") == 1)
            {
                if (Speed < limitspeed)
                {
                    Speed = Speed + 5;
                }
            }
            else
            {
                if (Speed > minspeed)
                {
                    Speed = Speed - 5;
                }
            }

            player.Call("setplayerangles", angles);

            Vector3 Tvector = Call<Vector3>("anglestoforward", angles);
            Vector3 Target = new Vector3(Tvector.X * Speed, Tvector.Y * Speed, Tvector.Z * Speed);
            engine.Origin += Target;
        }

        private void MakeWeapon(Entity player)
        {

        }

        private Vector3 GetCursorPos(Entity player)
        {
            Vector3 vector = player.Call<Vector3>("gettagorigin", new Parameter[]
            {
                "tag_eye"
            });
            Vector3 vector2 = base.Call<Vector3>("anglestoforward", new Parameter[]
            {
                player.Call<Vector3>("getplayerangles", new Parameter[0])
            });
            Vector3 result = new Vector3(vector2.X * 1000000f, vector2.Y * 1000000f, vector2.Z * 1000000f);
            return result;
        }
    }
}
