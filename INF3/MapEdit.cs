using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class MapEdit : BaseScript
    {
        public static Entity _airdropCollision;
        public static Entity _nullCollision;

        private static string _currentfile;
        private int curObjID;
        private static List<Entity> usables = new List<Entity>();

        public MapEdit()
        {
            var _e = Call<Entity>("getent", "care_package", "targetname");
            _airdropCollision = Call<Entity>("getent", _e.GetField<string>("target"), "targetname");

            _nullCollision = Utility.Spawn("script_origin", new Vector3());

            PlayerConnected += player =>
            {
                player.Call("notifyonplayercommand", "triggeruse", "+activate");
                player.OnNotify("triggeruse", ent => UsableThink(player));

                player.SetField("attackeddoor", 0);
                player.SetField("usingtelepot", 0);

                UsableHud(player);
                TrampolineThink(player);
            };

            InitMapEdit();
        }

        private void MakeUsable(Entity ent, string type, int range)
        {
            ent.SetField("usabletype", type);
            ent.SetField("range", range);
            usables.Add(ent);
        }

        #region UsableHud

        private string DoorText(Entity door, Entity player)
        {
            int hp = door.GetField<int>("hp");
            int maxhp = door.GetField<int>("maxhp");
            if (player.GetTeam() == "allies")
            {
                switch (door.GetField<string>("state"))
                {
                    case "open":
                        return "Door is Open. Press ^3[{+activate}] ^7to close it. (" + hp + "/" + maxhp + ")";
                    case "close":
                        return "Door is Closed. Press ^3[{+activate}] ^7to open it. (" + hp + "/" + maxhp + ")";
                    case "broken":
                        return "^1Door is Broken.";
                }
            }
            else if (player.GetTeam() == "axis")
            {
                switch (door.GetField<string>("state"))
                {
                    case "open":
                        return "Door is Open.";
                    case "close":
                        return "Press ^3[{+activate}] ^7to attack the door.";
                    case "broken":
                        return "^1Door is Broken";
                }
            }
            return "";
        }

        private string PayDoorText(Entity door, Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                if (door.GetField<string>("state") == "close")
                {
                    return "Press ^3[{+activate}] ^7to cleanup barriers. [Cost: ^2$^3" + door.GetField<int>("pay") + "^7]";
                }
            }
            return "";
        }

        private string TurretText()
        {
            return "Press ^3[{+activate}] ^7to use turret.";
        }

        private string SentryText()
        {
            return "Press ^3[{+activate}] ^7to use sentey.";
        }

        private string GLText()
        {
            return "Press ^3[{+activate}] ^7to use grenade launcher.";
        }

        private string SAMText()
        {
            return "Press ^3[{+activate}] ^7to use SAM turret.";
        }

        private string ZiplineText(Entity ent)
        {
            if (ent.GetField<string>("state") == "idle")
            {
                return "Press ^3[{+activate}] ^7to use zipline.";
            }
            return "";
        }

        private string TeleporterText(Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                if (Call<int>("getdvarint", "scr_aiz_power") == 0 || Call<int>("getdvarint", "scr_aiz_power") == 2)
                {
                    return "Requires Electricity";
                }
                if (player.GetField<int>("usingtelepot") == 0)
                {
                    return "Press ^3[{+activate}] ^7to use teleporter. [Cost: ^2$^3500^7]";
                }
            }
            return "";
        }

        private string TrampolineText()
        {
            return "Press ^3[{+gostand}] ^7to boost jump.";
        }

        private string PowerText(Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                if (Call<int>("getdvarint", "scr_aiz_power") == 0)
                {
                    return "Press ^3[{+activate}] ^7to activate the electricity. [Cost: ^2$^3700^7]";
                }
            }
            return "";
        }

        private string AmmoText(Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                return "Press ^3[{+activate}] ^7to buy ammo. [Cost: ^2$^3300^7]";
            }
            return "";
        }

        private string GamblerText(Entity ent, Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                if (ent.GetField<string>("state") == "idle")
                {
                    return "Press ^3[{+activate}] ^7to gamble. [Cost: ^2$^3500^7]";
                }
            }
            return "";
        }

        private string AirstrikeText(Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                if (Call<int>("getdvarint", "scr_aiz_power") == 0 || Call<int>("getdvarint", "scr_aiz_power") == 2)
                {
                    return "Requires Electricity";
                }
                return "Press ^3[{+activate}] ^7to buy random airstrike. [Cost: ^310 ^5Bouns Points^7]";
            }
            return "";
        }

        private string PerkText(Entity player, Perks.Perk perk)
        {
            if (player.GetTeam() == "allies")
            {
                if (Call<int>("getdvarint", "scr_aiz_power") == 0 || Call<int>("getdvarint", "scr_aiz_power") == 2)
                {
                    return "Requires Electricity";
                }
                return perk.PerkBoxHintString();
            }
            return "";
        }

        private string RandomPerkText(Entity ent, Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                if (Call<int>("getdvarint", "scr_aiz_power") == 0 || Call<int>("getdvarint", "scr_aiz_power") == 2)
                {
                    return "Requires Electricity";
                }
                return "Press ^3[{+activate}] ^7to use Der Wunderfizz. [Cost: ^310 ^5Bouns Points^7]";
            }
            return "";
        }

        private void UsableHud(Entity player)
        {
            HudElem message = HudElem.CreateFontString(player, "big", 1.5f);
            message.SetPoint("CENTER", "CENTER", 1, 115);
            message.Alpha = 0.65f;

            OnInterval(100, () =>
            {
                try
                {
                    var flag = false;
                    foreach (var ent in usables)
                    {
                        if (player.Origin.DistanceTo(ent.Origin) >= ent.GetField<int>("range"))
                        {
                            continue;
                        }
                        switch (ent.GetField<string>("usabletype"))
                        {
                            case "door":
                                message.SetText(DoorText(ent, player));
                                break;
                            case "paydoor":
                                message.SetText(PayDoorText(ent, player));
                                break;
                            case "turret":
                                message.SetText(TurretText());
                                break;
                            case "sentry":
                                message.SetText(SentryText());
                                break;
                            case "gl":
                                message.SetText(GLText());
                                break;
                            case "sam":
                                message.SetText(SAMText());
                                break;
                            case "zipline":
                                message.SetText(ZiplineText(ent));
                                break;
                            case "teleporter":
                                message.SetText(TeleporterText(player));
                                break;
                            case "trampoline":
                                message.SetText(TrampolineText());
                                break;
                            case "power":
                                message.SetText(PowerText(player));
                                break;
                            case "ammo":
                                message.SetText(AmmoText(player));
                                break;
                            case "gambler":
                                message.SetText(GamblerText(ent, player));
                                break;
                            case "airstrike":
                                message.SetText(AirstrikeText(player));
                                break;
                            case "perk":
                                message.SetText(PerkText(player, ent.GetField<Perks.Perk>("perk")));
                                break;
                            case "randomperk":
                                message.SetText(RandomPerkText(ent, player));
                                break;
                        }
                        flag = true;
                    }
                    if (!flag)
                    {
                        message.SetText("");
                    }
                }
                catch (Exception)
                {
                    message.SetText("");
                }

                return true;
            });
        }

        #endregion

        private void UsableThink(Entity player)
        {
            try
            {
                foreach (var ent in usables)
                {
                    if (player.Origin.DistanceTo(ent.Origin) < ent.GetField<int>("range"))
                    {
                        if (player.IsAlive && !player.CurrentWeapon.Contains("ac130") && !player.CurrentWeapon.Contains("killstreak") && !player.CurrentWeapon.Contains("remote"))
                        {
                            switch (ent.GetField<string>("usabletype"))
                            {
                                case "door":
                                    BoxFunction.UseDoor(ent, player);
                                    break;
                                case "paydoor":
                                    BoxFunction.UsePayDoor(ent, player);
                                    break;
                                case "turret":
                                    break;
                                case "sentry":
                                    break;
                                case "gl":
                                    break;
                                case "sam":
                                    break;
                                case "zipline":
                                    BoxFunction.UseZipline(ent, player);
                                    break;
                                case "teleporter":
                                    if (Call<int>("getdvarint", "scr_aiz_power") == 1)
                                    {
                                        BoxFunction.UseTeleporter(ent, player);
                                    }
                                    break;
                                case "power":
                                    BoxFunction.UsePower(ent, player);
                                    break;
                                case "ammo":
                                    BoxFunction.UseAmmo(player);
                                    break;
                                case "gambler":
                                    BoxFunction.UseGambler(ent, player);
                                    break;
                                case "airstrike":
                                    if (Call<int>("getdvarint", "scr_aiz_power") == 1)
                                    {
                                        BoxFunction.UseAirstrike(player);
                                    }
                                    break;
                                case "perk":
                                    if (Call<int>("getdvarint", "scr_aiz_power") == 1)
                                    {
                                        BoxFunction.UsePerk(player, ent.GetField<Perks.Perk>("perk"));
                                    }
                                    break;
                                case "randomperk":
                                    if (Call<int>("getdvarint", "scr_aiz_power") == 1)
                                    {
                                        BoxFunction.UseRandomPerk(ent, player);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void TrampolineThink(Entity player)
        {
            OnInterval(100, () =>
            {
                try
                {
                    foreach (var ent in usables)
                    {
                        if (player.Origin.DistanceTo(ent.Origin) >= ent.GetField<int>("range"))
                        {
                            continue;
                        }
                        if (ent.GetField<string>("usabletype") == "trampoline" && player.Call<int>("IsOnGround") == 0 && player.IsAlive && !player.CurrentWeapon.Contains("ac130") && !player.CurrentWeapon.Contains("killstreak") && !player.CurrentWeapon.Contains("remote"))
                        {
                            BoxFunction.UseTrampoline(ent, player);
                        }
                    }
                }
                catch (Exception)
                {
                }
                return true;
            });
        }

        public void InitMapEdit()
        {
            if (Directory.Exists("scripts\\inf3-maps\\" + Utility.MapName))
            {
                Directory.CreateDirectory("scripts\\inf3-maps\\" + Utility.MapName);
            }
            var files = Directory.GetFiles("scripts\\inf3-maps\\" + Utility.MapName + "\\");
            if (files.Length > 0)
            {
                _currentfile = files[Utility.rng.Next(files.Length)];
                if (File.Exists(_currentfile))
                {
                    LoadMapEdit();
                }
            }
        }

        private void CreateFlagShader(Entity flag)
        {
            HudElem elem = HudElem.NewHudElem();
            elem.SetShader("waypoint_flag_friendly", 15, 15);
            elem.Alpha = 0.6f;
            elem.X = flag.Origin.X;
            elem.Y = flag.Origin.Y;
            elem.Z = flag.Origin.Z + 100f;
            elem.Call("SetWayPoint", 1, 1);
        }

        private HudElem CreateShader(Entity ent, string shader, string team = "")
        {
            HudElem elem;
            if (team != "")
            {
                elem = HudElem.NewTeamHudElem(team);
            }
            else
            {
                elem = HudElem.NewHudElem();
            }
            elem.SetShader(shader, 15, 15);
            elem.Alpha = 0.6f;
            elem.X = ent.Origin.X;
            elem.Y = ent.Origin.Y;
            elem.Z = ent.Origin.Z + 50f;
            elem.Call("SetWayPoint", 1, 1);

            return elem;
        }

        private Entity CreateLaptop(Entity ent)
        {
            var origin = ent.Origin;
            origin.Z += 17;
            Entity laptop = Utility.Spawn("script_model", origin);
            laptop.Call("setmodel", "com_laptop_2_open");
            OnInterval(100, () =>
            {
                laptop.Call("rotateyaw", new Parameter[] { -360, 7 });
                return true;
            });

            return laptop;
        }

        private int CreateObjective(Entity ent, string shader, string team = "none")
        {
            int num = 31 - curObjID++;
            Call("objective_state", num, "active");
            Call("objective_position", num, ent.Origin);
            Call("objective_icon", num, shader);
            Call("objective_team", num, team);

            return num;
        }

        public void CreateRamp(Vector3 top, Vector3 bottom)
        {
            int num2 = (int)Math.Ceiling(top.DistanceTo(bottom) / 30f);
            Vector3 vector = new Vector3((top.X - bottom.X) / num2, (top.Y - bottom.Y) / num2, (top.Z - bottom.Z) / num2);
            Vector3 vector2 = Call<Vector3>("vectortoangles", top - bottom);
            Vector3 angles = new Vector3(vector2.Z, vector2.Y + 90f, vector2.X);
            for (int i = 0; i <= num2; i++)
            {
                SpawnCrate(bottom + (vector * i), angles);
            }
        }

        public void CreateElevator(Vector3 enter, Vector3 exit)
        {
            Entity flag = Utility.Spawn("script_model", enter);
            flag.Call("setmodel", Utility.GetFlagModel(Utility.MapName));
            Utility.Spawn("script_model", exit).Call("setmodel", "prop_flag_neutral");
            CreateFlagShader(flag);
            CreateObjective(flag, "compass_waypoint_target");
            OnInterval(100, () =>
            {
                foreach (Entity entity in Utility.GetPlayers())
                {
                    if (entity.Origin.DistanceTo(enter) <= 50f)
                    {
                        entity.Call("setorigin", exit);
                    }
                }
                return true;
            });
        }

        public void CreateHiddenTP(Vector3 enter, Vector3 exit)
        {
            Utility.Spawn("script_model", enter).Call("setmodel", "weapon_scavenger_grenadebag");
            Utility.Spawn("script_model", enter).Call("setmodel", "weapon_oma_pack");
            OnInterval(100, delegate
            {
                foreach (Entity entity in Utility.GetPlayers())
                {
                    if (entity.Origin.DistanceTo(enter) <= 50f)
                    {
                        entity.Call("setorigin", exit);
                    }
                }
                return true;
            });
        }

        public void CreateDoor(Vector3 open, Vector3 close, Vector3 angle, int size, int height, int hp, int range)
        {
            double num = ((size / 2) - 0.5) * -1.0;
            Entity ent = Utility.Spawn("script_model", open);
            for (int i = 0; i < size; i++)
            {
                Entity entity2 = SpawnCrate(open + new Vector3(0f, 30f, 0f) * ((float)num), new Vector3(0f, 0f, 0f));
                entity2.Call("setmodel", "com_plasticcase_enemy");
                entity2.Call("enablelinkto");
                entity2.Call("linkto", ent);
                for (int j = 1; j < height; j++)
                {
                    Entity entity3 = SpawnCrate((open + new Vector3(0f, 30f, 0f) * ((float)num)) - (new Vector3(70f, 0f, 0f) * j), new Vector3(0f, 0f, 0f));
                    entity3.Call("setmodel", "com_plasticcase_enemy");
                    entity3.Call("enablelinkto");
                    entity3.Call("linkto", ent);
                }
                num++;
            }
            ent.SetField("angles", angle);
            ent.SetField("state", "open");
            ent.SetField("hp", hp);
            ent.SetField("maxhp", hp);
            ent.SetField("open", open);
            ent.SetField("close", close);

            MakeUsable(ent, "door", range);
        }

        public void CreatePayDoor(Vector3 open, Vector3 close, Vector3 angle, int size, int height, int pay, int range)
        {
            double num = ((size / 2) - 0.5) * -1.0;
            Entity ent = Utility.Spawn("script_model", close);
            for (int i = 0; i < size; i++)
            {
                Entity entity2 = SpawnCrate(close + new Vector3(0f, 30f, 0f) * ((float)num), new Vector3(0f, 0f, 0f));
                entity2.Call("setmodel", "com_plasticcase_enemy");
                entity2.Call("enablelinkto");
                entity2.Call("linkto", ent);
                for (int j = 1; j < height; j++)
                {
                    Entity entity3 = SpawnCrate((close + new Vector3(0f, 30f, 0f) * ((float)num)) - (new Vector3(70f, 0f, 0f) * j), new Vector3(0f, 0f, 0f));
                    entity3.Call("setmodel", "com_plasticcase_enemy");
                    entity3.Call("enablelinkto");
                    entity3.Call("linkto", ent);
                }
                num++;
            }
            ent.SetField("angles", angle);
            ent.SetField("state", "close");
            ent.SetField("pay", pay);
            ent.SetField("open", open);
            ent.SetField("close", close);

            MakeUsable(ent, "paydoor", range);
        }

        public Entity CreateWall(Vector3 start, Vector3 end)
        {
            float num = new Vector3(start.X, start.Y, 0f).DistanceTo(new Vector3(end.X, end.Y, 0f));
            float num2 = new Vector3(0f, 0f, start.Z).DistanceTo(new Vector3(0f, 0f, end.Z));
            int num3 = (int)Math.Round(num / 55f, 0);
            int num4 = (int)Math.Round(num2 / 30f, 0);
            Vector3 v = end - start;
            Vector3 vector2 = new Vector3(v.X / num3, v.Y / num3, v.Z / num4);
            float x = vector2.X / 4f;
            float y = vector2.Y / 4f;
            Vector3 angles = Call<Vector3>("vectortoangles", v);
            angles = new Vector3(0f, angles.Y, 90f);
            Entity entity = Utility.Spawn("script_origin", new Vector3((start.X + end.X) / 2f, (start.Y + end.Y) / 2f, (start.Z + end.Z) / 2f));
            for (int i = 0; i < num4; i++)
            {
                Entity entity2 = SpawnCrate((start + new Vector3(x, y, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto");
                entity2.Call("linkto", entity);
                for (int j = 0; j < num3; j++)
                {
                    entity2 = SpawnCrate(((start + (new Vector3(vector2.X, vector2.Y, 0f) * j)) + new Vector3(0f, 0f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                    entity2.Call("enablelinkto");
                    entity2.Call("linkto", entity);
                }
                entity2 = SpawnCrate((new Vector3(end.X, end.Y, start.Z) + new Vector3(x * -1f, y * -1f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto");
                entity2.Call("linkto", entity);
            }
            return entity;
        }

        public Entity CreateInvWall(Vector3 start, Vector3 end)
        {
            float num = new Vector3(start.X, start.Y, 0f).DistanceTo(new Vector3(end.X, end.Y, 0f));
            float num2 = new Vector3(0f, 0f, start.Z).DistanceTo(new Vector3(0f, 0f, end.Z));
            int num3 = (int)Math.Round(num / 55f, 0);
            int num4 = (int)Math.Round(num2 / 30f, 0);
            Vector3 v = end - start;
            Vector3 vector2 = new Vector3(v.X / num3, v.Y / num3, v.Z / num4);
            float x = vector2.X / 4f;
            float y = vector2.Y / 4f;
            Vector3 angles = Call<Vector3>("vectortoangles", v);
            angles = new Vector3(0f, angles.Y, 90f);
            Entity entity = Utility.Spawn("script_origin", new Vector3((start.X + end.X) / 2f, (start.Y + end.Y) / 2f, (start.Z + end.Z) / 2f));
            for (int i = 0; i < num4; i++)
            {
                Entity entity2 = SpawnInvCrate((start + new Vector3(x, y, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto");
                entity2.Call("linkto", entity);
                for (int j = 0; j < num3; j++)
                {
                    entity2 = SpawnInvCrate(((start + (new Vector3(vector2.X, vector2.Y, 0f) * j)) + new Vector3(0f, 0f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                    entity2.Call("enablelinkto");
                    entity2.Call("linkto", entity);
                }
                entity2 = SpawnInvCrate((new Vector3(end.X, end.Y, start.Z) + new Vector3(x * -1f, y * -1f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto");
                entity2.Call("linkto", entity);
            }
            return entity;
        }

        public Entity CreateFloor(Vector3 corner1, Vector3 corner2)
        {
            float num = corner1.X - corner2.X;
            if (num < 0f)
            {
                num *= -1f;
            }
            float num2 = corner1.Y - corner2.Y;
            if (num2 < 0f)
            {
                num2 *= -1f;
            }
            int num3 = (int)Math.Round(num / 50f, 0);
            int num4 = (int)Math.Round(num2 / 30f, 0);
            Vector3 vector = corner2 - corner1;
            Vector3 vector2 = new Vector3(vector.X / num3, vector.Y / num4, 0f);
            Entity entity = Utility.Spawn("script_origin", new Vector3((corner1.X + corner2.X) / 2f, (corner1.Y + corner2.Y) / 2f, corner1.Z));
            for (int i = 0; i < num3; i++)
            {
                for (int j = 0; j < num4; j++)
                {
                    Entity entity2 = SpawnCrate((corner1 + (new Vector3(vector2.X, 0f, 0f) * i)) + (new Vector3(0f, vector2.Y, 0f) * j), new Vector3(0f, 0f, 0f));
                    entity2.Call("enablelinkto");
                    entity2.Call("linkto", entity);
                }
            }
            return entity;
        }

        public void CreateZipline(Vector3 enter, Vector3 angles, Vector3 exit, int movetime)
        {
            Entity ent = SpawnCrate(enter, angles);
            ent.SetField("state", "idle");
            ent.SetField("exit", exit);
            ent.SetField("movetime", movetime);
            CreateObjective(ent, "hudicon_neutral");

            MakeUsable(ent, "zipline", 50);
        }

        public void CreateTeleporter(Vector3 enter, Vector3 angles, Vector3 exit)
        {
            Entity ent = SpawnCrate(enter, angles);
            ent.SetField("exit", exit);
            CreateObjective(ent, "hudicon_neutral");
            CreateShader(ent, "hudicon_neutral");
            CreateLaptop(ent);

            MakeUsable(ent, "teleporter", 50);
        }

        public void CreateTrampoline(Vector3 enter, Vector3 angles, int high)
        {
            Entity ent = SpawnCrate(enter, angles);
            ent.SetField("high", high);
            CreateObjective(ent, "cardicon_tictacboom");
            CreateShader(ent, "cardicon_tictacboom");

            MakeUsable(ent, "trampoline", 50);
        }

        public void CreatePower(Vector3 origin, Vector3 angles)
        {
            Entity ent = SpawnCrate(origin, angles);
            ent.SetField("player", "");
            var obj = CreateObjective(ent, "cardicon_bulb");
            var shader = CreateShader(ent, "cardicon_bulb");
            Call("setdvar", "scr_aiz_power", 0);
            OnInterval(100, () =>
            {
                if (Call<int>("getdvarint", "scr_aiz_power") == 2)
                {
                    shader.Call("destroy");
                    Call("objective_delete", obj);

                    Vector3 origin2 = ent.Origin;
                    origin2.Z += 1000f;

                    ent.Call("clonebrushmodeltoscriptmodel", _nullCollision);
                    ent.Call("moveto", origin2, 2.3f);
                    AfterDelay(2300, () =>
                    {
                        Call("playfx", Call<int>("loadfx", "explosions/emp_flash_mp"), origin2);
                        ent.Call("playsoundasmaster", "exp_suitcase_bomb_main");
                        usables.Remove(ent);
                        ent.Call("delete");

                        var messages = new List<string>
                        {
                            ent.GetField<string>("player"),
                            "Activated Power!",
                        };
                        foreach (var player in Utility.GetPlayers())
                        {
                            if (player.GetTeam() == "allies")
                            {
                                player.WelcomeMessage(messages, new Vector3(1, 1, 1), new Vector3(1, 0.5f, 0.3f), 1, 0.85f);
                            }
                        }
                        Call("setdvar", "scr_aiz_power", 1);
                    });
                    return false;
                }
                return true;
            });

            MakeUsable(ent, "power", 50);
        }

        public void CreateAmmo(Vector3 origin, Vector3 angles)
        {
            Entity ent = SpawnCrate(origin, angles);
            CreateObjective(ent, "waypoint_ammo_friendly", "allies");
            CreateShader(ent, "waypoint_ammo_friendly", "allies");
            CreateLaptop(ent);

            MakeUsable(ent, "ammo", 50);
        }

        public void CreateGambler(Vector3 origin, Vector3 angles)
        {
            Entity ent = SpawnCrate(origin, angles);
            CreateObjective(ent, "cardicon_8ball", "allies");
            CreateShader(ent, "cardicon_8ball", "allies");
            var lap = CreateLaptop(ent);
            ent.SetField("state", "idle");
            ent.SetField("laptop", lap);

            MakeUsable(ent, "gambler", 50);
        }

        public void CreateAirstrike(Vector3 origin, Vector3 angles)
        {
            Entity ent = SpawnCrate(origin, angles);
            CreateObjective(ent, "cardicon_award_jets", "allies");
            CreateShader(ent, "cardicon_award_jets", "allies");
            CreateLaptop(ent);

            MakeUsable(ent, "airstrike", 50);
        }

        public void CreatePerk(Vector3 origin, Vector3 angles, Perks.Perk perk)
        {
            Entity ent = SpawnCrate(origin, angles);
            ent.SetField("perk", new Parameter(perk));
            CreateObjective(ent, perk.GetPerkIcon(), "allies");
            CreateShader(ent, perk.GetPerkIcon(), "allies");

            MakeUsable(ent, "perk", 50);
        }

        public void CreateRandomPerk(Vector3 origin, Vector3 angles)
        {
            Entity ent = SpawnCrate(origin, angles);
            CreateObjective(ent, "cardicon_tf141", "allies");
            CreateShader(ent, "cardicon_tf141", "allies");
            CreateLaptop(ent);

            MakeUsable(ent, "randomperk", 50);
        }

        public Entity SpawnCrate(Vector3 origin, Vector3 angles)
        {
            Entity entity = Utility.Spawn("script_model", origin);
            entity.Call("setmodel", "com_plasticcase_friendly");
            entity.SetField("angles", angles);
            entity.Call("clonebrushmodeltoscriptmodel", _airdropCollision);
            return entity;
        }

        public Entity SpawnInvCrate(Vector3 origin, Vector3 angles)
        {
            Entity entity = Utility.Spawn("script_model", origin);
            entity.SetField("angles", angles);
            entity.Call("clonebrushmodeltoscriptmodel", _airdropCollision);
            return entity;
        }

        public Entity SpawnModel(string model, Vector3 origin, Vector3 angles)
        {
            Entity entity = Utility.Spawn("script_model", origin);
            entity.Call("setmodel", model);
            entity.SetField("angles", angles);
            return entity;
        }

        private void LoadMapEdit()
        {
            try
            {
                StreamReader reader = new StreamReader(_currentfile);
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    if (!str.StartsWith("//") && !str.Equals(string.Empty))
                    {
                        string[] strArray = str.Split(new char[] { ':' });
                        if (strArray.Length >= 1)
                        {
                            string str2 = strArray[0];
                            switch (str2)
                            {
                                case "crate":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        SpawnCrate(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0));
                                    }
                                    continue;
                                case "ramp":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateRamp(ParseVector3(strArray[0]), ParseVector3(strArray[1]));
                                    }
                                    continue;
                                case "elevator":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateElevator(ParseVector3(strArray[0]), ParseVector3(strArray[1]));
                                    }
                                    continue;
                                case "HiddenTP":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateHiddenTP(ParseVector3(strArray[0]), ParseVector3(strArray[1]));
                                    }
                                    continue;
                                case "door":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 7)
                                    {
                                        CreateDoor(ParseVector3(strArray[0]), ParseVector3(strArray[1]), ParseVector3(strArray[2]), int.Parse(strArray[3]), int.Parse(strArray[4]), int.Parse(strArray[5]), int.Parse(strArray[6]));
                                    }
                                    continue;
                                case "paydoor":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 7)
                                    {
                                        CreatePayDoor(ParseVector3(strArray[0]), ParseVector3(strArray[1]), ParseVector3(strArray[2]), int.Parse(strArray[3]), int.Parse(strArray[4]), int.Parse(strArray[5]), int.Parse(strArray[6]));
                                    }
                                    continue;
                                case "wall":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateWall(ParseVector3(strArray[0]), ParseVector3(strArray[1]));
                                    }
                                    continue;
                                case "invwall":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateInvWall(ParseVector3(strArray[0]), ParseVector3(strArray[1]));
                                    }
                                    continue;
                                case "floor":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateFloor(ParseVector3(strArray[0]), ParseVector3(strArray[1]));
                                    }
                                    continue;
                                case "model":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 3)
                                    {
                                        SpawnModel(strArray[0], ParseVector3(strArray[1]), new Vector3(0, ParseVector3(strArray[2]).Y, 0));
                                    }
                                    continue;
                                case "turret":
                                    continue;
                                case "sentry":
                                    continue;
                                case "gl":
                                    continue;
                                case "sam":
                                    continue;
                                case "zipline":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 4)
                                    {
                                        CreateZipline(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0), ParseVector3(strArray[2]), Convert.ToInt32(strArray[3]));
                                    }
                                    continue;
                                case "teleporter":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 3)
                                    {
                                        CreateTeleporter(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0), ParseVector3(strArray[2]));
                                    }
                                    continue;
                                case "trampoline":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 3)
                                    {
                                        CreateTrampoline(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0), Convert.ToInt32(strArray[2]));
                                    }
                                    continue;
                                case "power":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreatePower(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0));
                                    }
                                    continue;
                                case "ammo":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateAmmo(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0));
                                    }
                                    continue;
                                case "gambler":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateGambler(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0));
                                    }
                                    continue;
                                case "airstrike":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateAirstrike(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0));
                                    }
                                    continue;
                                case "perk":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 3)
                                    {
                                        CreatePerk(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0), (Perks.Perk)Convert.ToInt32(strArray[2]));
                                    }
                                    continue;
                                case "randomperk":
                                    strArray = strArray[1].Split(new char[] { ';' });
                                    if (strArray.Length >= 2)
                                    {
                                        CreateRandomPerk(ParseVector3(strArray[0]) + new Vector3(0, 0, 5), new Vector3(0, ParseVector3(strArray[1]).Y, 0));
                                    }
                                    continue;
                            }
                            Print("Unknown MapEdit Entry {0}... ignoring", str2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Print("error loading mapedit for map {0}: {1}", Utility.MapName, ex.Message);
            }
        }

        private static void Print(string format, params object[] args)
        {
            Log.Write(LogLevel.All, format, args);
        }

        private static Vector3 ParseVector3(string vec3)
        {
            vec3 = vec3.Replace(" ", string.Empty);
            vec3 = vec3.Replace("(", string.Empty);
            vec3 = vec3.Replace(")", string.Empty);
            string[] strArray = vec3.Split(new char[] { ',' });
            return new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
        }
    }
}
