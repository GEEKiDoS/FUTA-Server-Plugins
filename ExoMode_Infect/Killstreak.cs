using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace Exo
{
    public class Killstreak : BaseScript
    {
        public enum KillStreaks
        {
            Airdrop,
            PredatorMissile,
            Harrier,
            A10Strike,
            UAVStrike,
            AC130Strike,
            SmartBotSMG,
            SmartBotLMG,    
            MagicStrike,     
            MOAB,
        }

        public enum IsUse
        {
            playerDropCrateIsUse,
            playerAC130IsUse
        }

        public Killstreak()
        {
            PlayerConnected += player =>
            {
                player.SetField("KillstreaksList", new Parameter(new List<KillStreaks>()));
                player.SetField("KillStreakIsUse", new Parameter(new List<IsUse>()));

                player.SetField("AmmoCountTurret", 0);

                player.Call("notifyonplayercommand", "ActivatePressed", "+activate");
                player.Call("notifyonplayercommand", "SelectedTarget", "+attack");

                player.Call("notifyonplayercommand", "give_killstreak", "+actionslot 4");
                player.OnNotify("give_killstreak", e =>
                {
                });
            };
        }

        public static void setRemoteUsing(Entity self, bool used)
        {
            if (used)
                self.Notify("using_remote");
            else
                self.Notify("stopped_using_remote");
        }

        public static int getKillstreakIndex(string streakName)
        {
            return Function.Call<int>("tableLookupRowNum", "mp/killstreakTable.csv", 1, streakName) - 1;
        }

        public static string getKillstreakWeapon(string streakName)
        {
            string text = string.Empty;
            text = Function.Call<string>("tableLookup", "mp/killstreakTable.csv", 1, streakName, 12);
            Log.Write(LogLevel.Info, "Killstreak weapon: " + text);
            return text;
        }


        public static void killstreakSoundInBound(Entity owner, string soundFriendly, string soundEnemy)
        {
            string TeamOwnerPrefix = string.Empty;
            if (owner.GetTeam() == "allies")
                TeamOwnerPrefix = Utility.AlliesAliasSound();
            else if (owner.GetTeam() == "axis")
                TeamOwnerPrefix = Utility.AxisAliasSound();
            else
                TeamOwnerPrefix = "RU_";

            string TeamEnemyPrefix = string.Empty;
            if (owner.GetTeam() != "allies")
                TeamEnemyPrefix = Utility.AlliesAliasSound();
            else if (owner.GetTeam() != "axis")
                TeamEnemyPrefix = Utility.AxisAliasSound();
            else
                TeamEnemyPrefix = "UK_";

            foreach (Entity selfs in Utility.Players)
            {
                if (selfs.GetTeam() == owner.GetTeam())
                    selfs.Call("playLocalSound", TeamOwnerPrefix + soundFriendly);
                else
                    selfs.Call("playLocalSound", TeamEnemyPrefix + soundEnemy);
            }
        }

        public static void giveKillstreakWeapon(Entity ent, string streakName, int slot, bool acivate)
        {
            string killstreakWeapon = getKillstreakWeapon(streakName);
            if (!string.IsNullOrEmpty(killstreakWeapon))
            {
                ent.Call("setActionSlot", slot, "weapon", killstreakWeapon);
                ent.Call("SetPlayerData", "killstreaksState", "hasStreak", slot, acivate);
                ent.Call("SetPlayerData", "killstreaksState", "icons", slot, getKillstreakIndex(streakName));
            }
        }
    }
}
