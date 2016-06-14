using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfinityScript;

namespace INF3
{
    public static class Utility
    {
        public static Random rng = new Random();

        public static string MapName
        {
            get
            {
                Function.SetEntRef(-1);
                return Function.Call<string>("getdvar", "mapname");
            }
        }

        public static List<Entity> GetPlayers()
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

        public static Entity Spawn(string spawntype, Vector3 origin)
        {
            Function.SetEntRef(-1);
            return Function.Call<Entity>("spawn", spawntype, origin);
        }

        public static void PreCacheShader(string shader)
        {
            Function.SetEntRef(-1);
            Function.Call("PreCacheShader", shader);
        }

        public static void PreCacheModel(string model)
        {
            Function.SetEntRef(-1);
            Function.Call("PreCacheModel", model);
        }

        public static string GetFlagModel(string mapname)
        {
            switch (mapname)
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

        public static string GetTeam(this Entity e)
        {
            return e.GetField<string>("sessionteam");
        }

        #region Sound

        public static string AxisAliasSound()
        {
            string prefix = string.Empty;
            Function.SetEntRef(-1);
            string mapname = Function.Call<string>("getdvar", "mapname");
            if (mapname == "mp_alpha")
                prefix = "RU_";
            else if (mapname == "mp_bootleg")
                prefix = "RU_";
            else if (mapname == "mp_bravo")
                prefix = "AF_";
            else if (mapname == "mp_carbon")
                prefix = "AF_";
            else if (mapname == "mp_dome")
                prefix = "RU_";
            else if (mapname == "mp_exchange")
                prefix = "RU_";
            else if (mapname == "mp_hardhat")
                prefix = "RU_";
            else if (mapname == "mp_interchange")
                prefix = "RU_";
            else if (mapname == "mp_lambeth")
                prefix = "RU_";
            else if (mapname == "mp_mogadishu")
                prefix = "AF_";
            else if (mapname == "mp_paris")
                prefix = "RU_";
            else if (mapname == "mp_plaza2")
                prefix = "IC_";
            else if (mapname == "mp_seatown")
                prefix = "IC_";
            else if (mapname == "mp_radar")
                prefix = "RU_";
            else if (mapname == "mp_underground")
                prefix = "RU_";
            else if (mapname == "mp_village")
                prefix = "RU_";
            else if (mapname == "mp_cement")
                prefix = "RU_";
            else if (mapname == "mp_italy")
                prefix = "IC_";
            else if (mapname == "mp_meteora")
                prefix = "IC_";
            else if (mapname == "mp_morningwood")
                prefix = "RU_";
            else if (mapname == "mp_overwatch")
                prefix = "RU_";
            else if (mapname == "mp_park")
                prefix = "RU_";
            else if (mapname == "mp_qadeem")
                prefix = "IC_";
            else if (mapname == "mp_boardwalk")
                prefix = "RU_";
            else if (mapname == "mp_moab")
                prefix = "RU_";
            else if (mapname == "mp_nola")
                prefix = "RU_";
            else if (mapname == "mp_roughneck")
                prefix = "RU_";
            else if (mapname == "mp_shipbreaker")
                prefix = "AF_";
            else if (mapname == "mp_terminal_cls")
                prefix = "RU_";
            else if (mapname == "mp_aground_ss")
                prefix = "IC_";
            else if (mapname == "mp_courtyard_ss")
                prefix = "IC_";
            else if (mapname == "mp_hillside_ss")
                prefix = "IC_";
            else if (mapname == "mp_restrepo_ss")
                prefix = "RU_";
            else if (mapname == "mp_crosswalk_ss")
                prefix = "RU_";
            else if (mapname == "mp_burn_ss")
                prefix = "RU_";
            else if (mapname == "mp_six_ss")
                prefix = "IC_";
            else
                prefix = "RU_";
            return prefix;
        }
        public static string AlliesAliasSound()
        {
            string prefix = string.Empty;
            Function.SetEntRef(-1);
            string mapname = Function.Call<string>("getdvar", "mapname");
            if (mapname == "mp_alpha")
                prefix = "US_";
            else if (mapname == "mp_bootleg")
                prefix = "PC_";
            else if (mapname == "mp_bravo")
                prefix = "PC_";
            else if (mapname == "mp_carbon")
                prefix = "PC_";
            else if (mapname == "mp_dome")
                prefix = "US_";
            else if (mapname == "mp_exchange")
                prefix = "US_";
            else if (mapname == "mp_hardhat")
                prefix = "US_";
            else if (mapname == "mp_interchange")
                prefix = "US_";
            else if (mapname == "mp_lambeth")
                prefix = "US_";
            else if (mapname == "mp_mogadishu")
                prefix = "PC_";
            else if (mapname == "mp_paris")
                prefix = "FR_";
            else if (mapname == "mp_plaza2")
                prefix = "FR_";
            else if (mapname == "mp_seatown")
                prefix = "UK_";
            else if (mapname == "mp_radar")
                prefix = "US_";
            else if (mapname == "mp_underground")
                prefix = "UK_";
            else if (mapname == "mp_village")
                prefix = "PC_";
            else if (mapname == "mp_cement")
                prefix = "US_";
            else if (mapname == "mp_italy")
                prefix = "FR_";
            else if (mapname == "mp_meteora")
                prefix = "UK_";
            else if (mapname == "mp_morningwood")
                prefix = "US_";
            else if (mapname == "mp_overwatch")
                prefix = "US_";
            else if (mapname == "mp_park")
                prefix = "US_";
            else if (mapname == "mp_qadeem")
                prefix = "US_";
            else if (mapname == "mp_boardwalk")
                prefix = "US_";
            else if (mapname == "mp_moab")
                prefix = "US_";
            else if (mapname == "mp_nola")
                prefix = "US_";
            else if (mapname == "mp_roughneck")
                prefix = "US_";
            else if (mapname == "mp_shipbreaker")
                prefix = "PC_";
            else if (mapname == "mp_terminal_cls")
                prefix = "US_";
            else if (mapname == "mp_aground_ss")
                prefix = "UK_";
            else if (mapname == "mp_courtyard_ss")
                prefix = "UK_";
            else if (mapname == "mp_hillside_ss")
                prefix = "US_";
            else if (mapname == "mp_restrepo_ss")
                prefix = "US_";
            else if (mapname == "mp_crosswalk_ss")
                prefix = "US_";
            else if (mapname == "mp_burn_ss")
                prefix = "US_";
            else if (mapname == "mp_six_ss")
                prefix = "US_";
            else
                prefix = "RU_";
            return prefix;
        }

        #endregion
    }
}
