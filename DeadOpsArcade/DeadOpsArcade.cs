using InfinityScript;
using System;

namespace DeadOpsArcade
{
    public class DeadOpsArcade : BaseScript
    {
        public DeadOpsArcade()
        {
            Init();
            PlayerConnected += new Action<Entity>(player =>
            {
                OnSpawnPlayer(player);
                player.SpawnedPlayer += () => OnSpawnPlayer(player);
                player.OnNotify("weapon_fired", delegate (Entity self, Parameter weapon)
                {
                    if (weapon.As<string>().StartsWith("iw5_usp45_mp"))
                    {
                        Vector3 vector = Call<Vector3>("anglestoforward", new Parameter[] { player.Call<Vector3>("getplayerangles", new Parameter[0]) });
                        Vector3 vector2 = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                        Call("magicbullet", new Parameter[] { "gl_mp", player.Call<Vector3>("gettagorigin", new Parameter[] { "tag_weapon_left" }), vector2, self });
                    }
                    player.Call("setweaponammostock", new Parameter[] { player.CurrentWeapon, 100 });
                    player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 100 });
                    player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 100, "left" });
                    player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 100, "right" });
                });
            });
        }

        private void Init()
        {
            Log.Debug("gametype is " + Call<string>("getdvar", new Parameter[] { "g_gametype" }));
            Call("setdvar", "scr_game_playerwaittime", "0");
            Call("setdvar", "scr_game_matchstarttime", "0");
            Call("setdvar", "scr_game_allowKillstreaks", "0");
            Call("setdvar", "scr_game_hardpoints", 0);
            Call("setdvar", "scr_game_perks", 0);
            Call("setdvar", "scr_player_forcerespawn", 1);
            Call("setdvar", "scr_teambalance", 1);
            Call("setdvar", "scr_" + Call<string>("getdvar", new Parameter[] { "g_gametype" }).Trim() + "_timelimit", 10);
            Call("setdvar", "scr_" + Call<string>("getdvar", new Parameter[] { "g_gametype" }).Trim() + "_scorelimit", 0);
        }

        private void MoveCameraDOA(Entity camera, Entity player)
        {
            if (camera != null)
            {
                OnInterval(0, delegate
                {
                    camera.Call("moveto", new Parameter[] { player.Origin + new Vector3(0f, 0f, 500f), 0.2f });
                    return camera != null;
                });
            }
        }

        private void MoveLaserDOA(Entity laser, Entity player)
        {
            if (laser != null)
            {
                //Vector3 origin;
                //Vector3 forward;
                //Vector3 endpoint;
                //Vector3 tracedata;
                OnInterval(0, () =>
                {
                    //origin = player.Call<Vector3>("geteye");
                    //forward = Call<Vector3>("AnglesToForward", player.Call<Vector3>("getplayerangles"));
                    //endpoint = origin + forward * 15000;
                    //tracedata = Call<Vector3>("BulletTrace", origin, endpoint, false, laser);
                    laser.Origin = player.Origin;
                    Call("triggerfx", laser);

                    return laser != null;
                });
            }
        }

        private void OnSpawnPlayer(Entity player)
        {
            //Entity field;
            Entity camera;
            player.SetClientDvar("cl_freelook", "0");
            if (!player.HasField("camera") || (player.GetField<Entity>("camera") == null))
            {
                camera = Call<Entity>("spawn", new Parameter[] { "script_model", player.Origin + new Vector3(0f, 0f, 700f) });
                camera.Call("setmodel", new Parameter[] { "c130_zoomrig" });
                camera.SetField("angles", new Vector3(90f, 90f, 0f));
                camera.Call("notsolid", new Parameter[0]);
                camera.Call("enablelinkto", new Parameter[0]);
                camera.Call("hide");
                camera.Call("showtoplayer", player);
                player.SetField("camera", camera);
                MoveCameraDOA(camera, player);
            }
            else
            {
                camera = player.GetField<Entity>("camera");
            }
            //if (!player.HasField("laser") || (player.GetField<Entity>("laser") == null))
            //{
            //    field = Call<Entity>("spawnfx", new Parameter[] { Call<int>("loadfx", new Parameter[] { "misc/laser_glow" }), player.Origin });
            //    field.Call("hide");
            //    field.Call("showtoplayer", player);
            //    player.SetField("laser", field);
            //    MoveLaserDOA(field, player);
            //}
            //else
            //{
            //    field = player.GetField<Entity>("laser");
            //}
            AfterDelay(100, () =>
            {
                player.Call("cameralinkto", new Parameter[] { camera, "tag_origin" });
                player.Call("allowads", new Parameter[] { 0 });
                player.OnInterval(100, delegate (Entity ent)
                {
                    player.Call("recoilscaleon", new Parameter[] { 0f });
                    return player.IsPlayer && player.IsAlive;
                });
            });
        }
    }
}
