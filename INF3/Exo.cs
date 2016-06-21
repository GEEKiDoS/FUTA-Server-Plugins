using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class Exo : BaseScript
    {
        public Exo()
        {
            PlayerConnected += new Action<Entity>(player =>
            {
                player.SetField("readyjump", 1);
                player.SetField("readydash", 1);

                player.SetField("w pressed", 0);
                player.SetField("a pressed", 0);
                player.SetField("s pressed", 0);
                player.SetField("d pressed", 0);

                player.Call("notifyonplayercommand", "w down", "+forward");
                player.Call("notifyonplayercommand", "w up", "-forward");
                player.Call("notifyonplayercommand", "a down", "+moveleft");
                player.Call("notifyonplayercommand", "a up", "-moveleft");
                player.Call("notifyonplayercommand", "s down", "+back");
                player.Call("notifyonplayercommand", "s up", "-back");
                player.Call("notifyonplayercommand", "d down", "+moveright");
                player.Call("notifyonplayercommand", "d up", "-moveright");

                player.OnNotify("w down", ent => player.SetField("w pressed", 1));
                player.OnNotify("w up", ent => player.SetField("w pressed", 0));
                player.OnNotify("a down", ent => player.SetField("a pressed", 1));
                player.OnNotify("a up", ent => player.SetField("a pressed", 0));
                player.OnNotify("s down", ent => player.SetField("s pressed", 1));
                player.OnNotify("s up", ent => player.SetField("s pressed", 0));
                player.OnNotify("d down", ent => player.SetField("d pressed", 1));
                player.OnNotify("d up", ent => player.SetField("d pressed", 0));

                player.Call("notifyonplayercommand", "jump", "+gostand");
                player.Call("notifyonplayercommand", "dash", "+breath_sprint");

                player.OnNotify("jump", ent =>
                {
                    if (player.GetTeam() == "axis" && player.GetField<int>("rtd_exo") == 1)
                    {
                        if (player.IsAlive && Ready(player) && player.Call<string>("getstance") == "stand" && player.GetField<int>("readyjump") == 1)
                        {
                            var vel = player.Call<Vector3>("getvelocity");

                            player.Call("setvelocity", new Vector3(vel.X, vel.Y, 500));
                            player.SetField("readyjump", 0);
                            AfterDelay(1500, () => player.SetField("readyjump", 1));
                        }
                    }
                });
                player.OnNotify("dash", ent =>
                {
                    if (player.GetTeam() == "axis" && player.GetField<int>("rtd_exo") == 1)
                    {
                        if (player.IsAlive && Ready(player) && player.Call<string>("getstance") == "stand" && player.GetField<int>("readydash") == 1)
                        {
                            var vel = player.Call<Vector3>("getvelocity");
                            var newvel = GetDashDirection(player) * 500;
                            var len = newvel.Length() / new Vector3(newvel.X, newvel.Y, 0).Length();

                            player.Call("setvelocity", new Vector3(newvel.X * len, newvel.Y * len, vel.Z));
                            player.SetField("readydash", 0);
                            AfterDelay(1500, () => player.SetField("readydash", 1));
                        }
                    }
                });
            });
        }

        public bool Ready(Entity player)
        {
            if (player.Call<int>("IsOnGround") == 1 && player.Call<int>("IsOnLadder") == 0 && player.Call<int>("IsMantling") == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Vector3 GetDashDirection(Entity player)
        {
            var direction = new Vector3();
            List<Vector3> curDirection = new List<Vector3>();

            if (player.GetField<int>("w pressed") == 1)
                curDirection.Add(Call<Vector3>("anglestoforward", player.Call<Vector3>("getplayerangles")));
            if (player.GetField<int>("a pressed") == 1)
                curDirection.Add(Call<Vector3>("anglestoright", player.Call<Vector3>("getplayerangles")) * -1);
            if (player.GetField<int>("s pressed") == 1)
                curDirection.Add(Call<Vector3>("anglestoforward", player.Call<Vector3>("getplayerangles")) * -1);
            if (player.GetField<int>("d pressed") == 1)
                curDirection.Add(Call<Vector3>("anglestoright", player.Call<Vector3>("getplayerangles")));

            if (curDirection.Count > 1)
            {
                for (int i = 0; i < curDirection.Count; i++)
                {
                    direction += curDirection[i];
                }
                direction = Call<Vector3>("vectornormalize", direction);
            }
            else if (curDirection.Count == 1)
            {
                direction = curDirection[0];
            }
            else
            {
                direction = Call<Vector3>("anglestoforward", player.Call<Vector3>("getplayerangles"));
            }

            return direction;
        }
    }
}
