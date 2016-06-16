﻿using System;
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
        public readonly static Random Rng = new Random();

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
    }
}
