using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfinityScript;

namespace INF3
{
    /// <summary>
    /// 提供一个方法集，允许 INF3 与 InfinityScript 之间进行交互，此外还提供控制玩家属性和行为的拓展方法
    /// </summary>
    public static class Utility
    {
        #region Global

        /// <summary>
        /// 提供一个公共随机数生成器，此字段为只读
        /// </summary>
        public static readonly Random Random = new Random();
        /// <summary>
        /// 获取服务器的当前地图，此字段为只读
        /// </summary>
        public static readonly string MapName = Function.Call<string>("getdvar", "mapname");

        /// <summary>
        /// 获取服务器内所有玩家，并返回包含玩家实体的集合
        /// </summary>
        public static List<Entity> Players
        {
            get
            {
                var list = new List<Entity>();
                for (int i = 0; i < 18; i++)
                {
                    var ent = Entity.GetEntity(i);
                    if (ent != null && ent.IsPlayer)
                    {
                        list.Add(ent);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// 获取 INF3 是否已开启测试模式
        /// </summary>
        public static bool TestMode
        {
            get
            {
                Function.SetEntRef(-1);
                return GetDvar<int>("inf3_allow_test") == 1;
            }
        }

        /// <summary>
        /// 获取服务器当前模式自开始运行到现在的毫秒数
        /// </summary>
        public static int Time
        {
            get
            {
                Function.SetEntRef(-1);
                return Function.Call<int>("gettime");
            }
        }

        /// <summary>
        /// 使用 InfinityScript 运行时部署一个实体，并返回这个实体
        /// </summary>
        /// <param name="spawntype">实体的类型</param>
        /// <param name="origin">部署在地图中的位置</param>
        /// <returns>返回的实体，如果实体创建失败，则返回null</returns>
        public static Entity Spawn(string spawntype, Vector3 origin)
        {
            Function.SetEntRef(-1);
            return Function.Call<Entity>("spawn", spawntype, origin);
        }

        /// <summary>
        /// 使用 InfinityScript 运行时预载指定图标，以供 HudElem 使用
        /// </summary>
        /// <param name="shader">图标代码</param>
        public static void PreCacheShader(string shader)
        {
            Function.SetEntRef(-1);
            Function.Call("PreCacheShader", shader);
        }

        /// <summary>
        /// 使用 InfinityScript 运行时预载指定模型，以供实体使用
        /// </summary>
        /// <param name="model"></param>
        public static void PreCacheModel(string model)
        {
            Function.SetEntRef(-1);
            Function.Call("PreCacheModel", model);
        }

        /// <summary>
        /// 使用 InfinityScript 运行时设置一个服务器全局变量，并为其赋值
        /// </summary>
        /// <param name="dvar">要设置的变量名称</param>
        /// <param name="value">变量值，该值类型只能是int、float、string和Vector3</param>
        public static void SetDvar(string dvar, Parameter value)
        {
            Function.SetEntRef(-1);
            Function.Call("setdvar", dvar, value);
        }

        /// <summary>
        /// 获取一个服务器全局变量的值
        /// </summary>
        /// <typeparam name="T">要获取的服务器全局变量的值的类型，只能是int、float、string和Vector3</typeparam>
        /// <param name="dvar">要获取的服务器全局变量名称</param>
        /// <returns>获取的服务器全局变量的值</returns>
        public static T GetDvar<T>(string dvar)
        {
            if (typeof(T) == typeof(int))
            {
                Function.SetEntRef(-1);
                return (T)Convert.ChangeType(Function.Call<int>("getdvarint", dvar), typeof(T));
            }
            else if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
            {
                Function.SetEntRef(-1);
                return (T)Convert.ChangeType(Function.Call<float>("getdvarfloat", dvar), typeof(T));
            }
            else if (typeof(T) == typeof(Vector3))
            {
                Function.SetEntRef(-1);
                return (T)Convert.ChangeType(Function.Call<Vector3>("getdvarvector", dvar), typeof(T));
            }
            else if (typeof(T) == typeof(string))
            {
                Function.SetEntRef(-1);
                return (T)Convert.ChangeType(Function.Call<string>("getdvar", dvar), typeof(T));
            }
            else
            {
                throw new Exception("Unknown Type.");
            }
        }

        #endregion

        #region Model

        public static string GetFlagModel()
        {
            switch (MapName)
            {
                case "mp_alpha":
                case "mp_dome":
                case "mp_hardhat":
                case "mp_interchange":
                case "mp_cement":
                case "mp_hillside_ss":
                case "mp_morningwood":
                case "mp_overwatch":
                case "mp_park":
                case "mp_qadeem":
                case "mp_restrepo_ss":
                case "mp_terminal_cls":
                case "mp_roughneck":
                case "mp_boardwalk":
                case "mp_moab":
                case "mp_nola":
                case "mp_radar":
                    return "prop_flag_delta";
                case "mp_exchange":
                    return "prop_flag_american05";
                case "mp_bootleg":
                case "mp_bravo":
                case "mp_mogadishu":
                case "mp_village":
                case "mp_shipbreaker":
                    return "prop_flag_pmc";
                case "mp_paris":
                    return "prop_flag_gign";
                case "mp_plaza2":
                case "mp_aground_ss":
                case "mp_courtyard_ss":
                case "mp_italy":
                case "mp_meteora":
                case "mp_underground":
                    return "prop_flag_sas";
                case "mp_seatown":
                case "mp_carbon":
                case "mp_lambeth":
                    return "prop_flag_seal";
            }
            return "";
        }

        public static string GetViewModelEnv()
        {
            switch (MapName)
            {
                case "mp_alpha":
                case "mp_bootleg":
                case "mp_exchange":
                case "mp_hardhat":
                case "mp_interchange":
                case "mp_mogadishu":
                case "mp_paris":
                case "mp_plaza2":
                case "mp_underground":
                case "mp_cement":
                case "mp_hillside_ss":
                case "mp_overwatch":
                case "mp_terminal_cls":
                case "mp_aground_ss":
                case "mp_courtyard_ss":
                case "mp_meteora":
                case "mp_morningwood":
                case "mp_qadeem":
                case "mp_crosswalk_ss":
                case "mp_italy":
                case "mp_boardwalk":
                case "mp_roughneck":
                case "mp_nola":
                    return "urban";
                case "mp_dome":
                case "mp_restrepo_ss":
                case "mp_burn_ss":
                case "mp_seatown":
                case "mp_shipbreaker":
                case "mp_moab":
                    return "desert";
                case "mp_bravo":
                case "mp_carbon":
                case "mp_park":
                case "mp_six_ss":
                case "mp_village":
                case "mp_lambeth":
                    return "woodland";
                case "mp_radar":
                    return "arctic";
            }
            return "";
        }
        public static string GetModelEnv()
        {
            switch (MapName)
            {
                case "mp_alpha":
                case "mp_dome":
                case "mp_paris":
                case "mp_plaza2":
                case "mp_terminal_cls":
                case "mp_bootleg":
                case "mp_restrepo_ss":
                case "mp_hillside_ss":
                    return "urban";
                case "mp_exchange":
                case "mp_hardhat":
                case "mp_underground":
                case "mp_cement":
                case "mp_overwatch":
                case "mp_nola":
                case "mp_boardwalk":
                case "mp_roughneck":
                case "mp_crosswalk_ss":
                    return "air";
                case "mp_interchange":
                case "mp_lambeth":
                case "mp_six_ss":
                case "mp_moab":
                case "mp_park":
                    return "woodland";
                case "mp_radar":
                    return "arctic";
            }

            return string.Empty;
        }

        public static string[] ICMaps = new string[]
        {
                "mp_seatown",
                "mp_aground_ss",
                "mp_courtyard_ss",
                "mp_italy",
                "mp_meteora",
                "mp_morningwood",
                "mp_qadeem",
                "mp_burn_ss"
        };
        public static string[] AfricaMaps = new string[]
        {
                "mp_bravo",
                "mp_carbon",
                "mp_mogadishu",
                "mp_village",
                "mp_shipbreaker",
        };

        public static void SetZombieModel(Entity player)
        {
            if (AfricaMaps.Contains(MapName))
            {
                player.Call("setmodel", "mp_body_opforce_africa_militia_sniper");
            }
            else if (ICMaps.Contains(MapName))
            {
                player.Call("setmodel", "mp_body_opforce_henchmen_sniper");
            }
            else
            {
                player.Call("setmodel", "mp_body_opforce_russian_" + GetModelEnv() + "_sniper");
            }

            if (AfricaMaps.Contains(MapName))
            {
                player.Call("setviewmodel", "viewhands_militia");
            }
            else if (!ICMaps.Contains(MapName))
            {
                player.Call("setviewmodel", "viewhands_op_force");
            }
        }

        public static void SetZombieSniperModel(Entity player)
        {
            if (MapName == "mp_radar")
            {
                player.Call("setmodel", "mp_body_opforce_ghillie_desert_sniper");
            }
            else
            {
                if (AfricaMaps.Contains(MapName))
                {
                    player.Call("setmodel", "mp_body_opforce_ghillie_africa_militia_sniper");
                }
                else
                {
                    player.Call("setmodel", "mp_body_opforce_ghillie_" + GetViewModelEnv() + "_sniper");
                }
            }
            player.Call("setviewmodel", "viewhands_iw5_ghillie_" + GetViewModelEnv());
        }

        #endregion

        #region Print

        /// <summary>
        /// 向所有玩家发送消息，该消息将显示在玩家屏幕中间位置
        /// </summary>
        /// <param name="message">要发送的消息</param>
        public static void PrintlnBold(string message)
        {
            Function.SetEntRef(-1);
            Function.Call("iprintlnbold", message);
        }

        /// <summary>
        /// 向指定玩家发送消息，该消息将显示在玩家屏幕中间位置
        /// </summary>
        /// <param name="player">要发给的玩家</param>
        /// <param name="message">要发送的消息</param>
        public static void PrintlnBold(this Entity player, string message)
        {
            player.Call("iprintlnbold", message);
        }

        /// <summary>
        /// 向所有玩家发送消息，该消息将显示在玩家屏幕左下角位置
        /// </summary>
        /// <param name="message">要发送的消息</param>
        public static void Println(string message)
        {
            Function.SetEntRef(-1);
            Function.Call("iprintln", message);
        }

        /// <summary>
        /// 向指定玩家发送消息，该消息将显示在玩家屏幕左下角位置
        /// </summary>
        /// <param name="player">要发给的玩家</param>
        /// <param name="message">要发送的消息</param>
        public static void Println(this Entity player, string message)
        {
            player.Call("iprintln", message);
        }

        /// <summary>
        /// 在服务器内记录日志，日志将会输出在服务器控制台以及保存至服务器日志文件
        /// </summary>
        /// <param name="message">要记录的内容</param>
        public static void PrintLog(string message)
        {
            Log.Write(LogLevel.All, message);
        }

        #endregion

        #region Player Function

        /// <summary>
        /// 获取指定玩家的阵营信息
        /// </summary>
        /// <param name="player">要获取的玩家</param>
        /// <returns>玩家的阵营代码</returns>
        public static string GetTeam(this Entity player)
        {
            return player.GetField<string>("sessionteam");
        }

        /// <summary>
        /// 设置玩家的阵营信息
        /// </summary>
        /// <param name="player">要设置的玩家</param>
        /// <param name="team">要设置的阵营</param>
        public static void SetTeam(this Entity player, string team)
        {
            player.SetField("sessionteam", team);
            player.SetField("team", team);
            player.Notify("menuresponse", "team_marinesopfor", team);
        }

        /// <summary>
        /// 设置玩家的移动速度的倍数
        /// </summary>
        /// <param name="player">要设置的玩家</param>
        /// <param name="speed">倍数</param>
        public static void SetSpeed(this Entity player, float speed) => player.SetField("speed", speed);

        /// <summary>
        /// 给指定玩家指定的武器并补给该武器全部弹药
        /// </summary>
        /// <param name="player">指定玩家</param>
        /// <param name="weapon">武器代码</param>
        public static void GiveMaxAmmoWeapon(this Entity player, string weapon)
        {
            player.GiveWeapon(weapon);
            player.Call("givemaxammo", weapon);
        }

        /// <summary>
        /// 立即将指定玩家生命值恢复到生命值上限
        /// </summary>
        /// <param name="player">指定玩家</param>
        public static void SetMaxHealth(this Entity player) => player.Health = player.GetField<int>("maxhealth");

        /// <summary>
        /// 击杀玩家，并设置击杀标示为自杀
        /// </summary>
        /// <param name="player">指定玩家</param>
        public static void Suicide(this Entity player) => player.AfterDelay(100, e => player.Call("suicide"));

        /// <summary>
        /// 删除指定玩家的指定技能，注意该技能是游戏内技能而不是Perk-a-Cola
        /// </summary>
        /// <param name="player">指定玩家</param>
        /// <param name="perk">要删除的技能的代码</param>
        public static void DeletePerk(this Entity player, string perk) => player.Call("unsetperk", perk);

        /// <summary>
        /// 为指定玩家设置玩家特效
        /// </summary>
        /// <param name="player">指定玩家</param>
        /// <param name="shock">特效代码</param>
        /// <param name="time">持续时间</param>
        public static void ShellShock(this Entity player, string shock, int time) => player.Call("shellshock", shock, time);

        #endregion
    }
}
