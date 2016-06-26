using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class AIZDebug : BaseScript
    {
        private const string _logfile = @"logs\inf3.log";

        private string[] _admins = new string[]
        {
            "A2ON",
            "Flandre Scarlet"
        };

        public AIZDebug()
        {
            PlayerConnected += player => DebugLog(GetType(), "Player: " + player.Name + " Connected.");
            PlayerDisconnected += player => DebugLog(GetType(), "Player: " + player.Name + " Disconnected.");

            RawLog("---------------------------------------------------------------");
            RawLog("Project Cirno v1.0 Beta");
            RawLog("");
            RawLog("DateTime Now: " + DateTime.Now);
            RawLog("Current OS: " + Environment.OSVersion.VersionString + " " + (Environment.Is64BitOperatingSystem ? "64 Bit" : "32 Bit"));
            RawLog("RAM Used: " + Environment.WorkingSet);
            RawLog("Working Directory: " + Directory.GetCurrentDirectory());
            RawLog("Current Process ID: " + Process.GetCurrentProcess().Id);
            RawLog("---------------------------------------------------------------");
        }

        public override void OnSay(Entity player, string name, string message)
        {
            if (Utility.TestMode)
            {
                if (message == "!givemoney")
                {
                    if (_admins.Contains(player.Name))
                    {
                        player.SetField("aiz_cash", 10000);
                        player.SetField("aiz_point", 100);
                    }
                    else
                    {
                        player.Suicide();
                    }
                }
                if (message == "!givemoneyall")
                {
                    if (_admins.Contains(player.Name))
                    {
                        foreach (var item in Utility.Players)
                        {
                            item.SetField("aiz_cash", 10000);
                            item.SetField("aiz_point", 100);
                        }
                    }
                }
                if (message == "!drop50")
                {
                    if (_admins.Contains(player.Name))
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            AfterDelay(i * 500, () => PowerUp.PowerUpDrop(player));
                        }
                    }
                }
                if (message == "!giveall")
                {
                    if (_admins.Contains(player.Name))
                    {
                        player.GiveAllPerkCola();
                    }
                }
                if (message == "!removeall")
                {
                    if (_admins.Contains(player.Name))
                    {
                        player.RemoveAllPerkCola();
                    }
                }
                if (message == "!cycle")
                {
                    if (_admins.Contains(player.Name))
                    {
                        Sharpshooter._cycleRemaining = 0;
                    }
                }
                if (message == "!tokenize")
                {
                    try
                    {
                        var thing = Utilities.Tokenize("inf3");
                        foreach (var token in thing)
                        {
                            Log.Info(token);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Log.Info(e.ToString());
                    }
                }
            }
        }

        public static void DebugLog(Type method, string text)
        {
            if (Utility.TestMode)
            {
                string alias = "[" + DateTime.Now + "][" + Utility.Time + "][" + method.Name + "]";
                RawLog(alias + text);
            }
        }

        private static void RawLog(string meta)
        {
            Log.Debug(meta);
            if (meta.StartsWith("["))
            {
                File.AppendAllText(_logfile, Environment.NewLine + meta);
            }
        }
    }
}
