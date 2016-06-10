using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using InfinityScript;

namespace ProMod
{
    public class ProMod : BaseScript
    {
        public readonly Dictionary<string, string> _cvar = new Dictionary<string, string>();
        public readonly Dictionary<string, string> _promod = new Dictionary<string, string>();
        public readonly Dictionary<string, string> _resetpromod = new Dictionary<string, string>();
        public readonly Dictionary<string, string> _server = new Dictionary<string, string>();

        private readonly string file;
        private readonly bool SkipWarn;

        public ProMod() : base()
        {
            Call("setdvarifuninitialized", "promod_profile", "promod");
            Call("setdvarifuninitialized", "promod_skipwarn", 0);

            file = "scripts\\ProMod\\" + Call<string>("getdvar", "promod_profile") + ".txt";
            if (!File.Exists(file))
            {
                Info("ProMod Error! Profile not find! ProMod will disable! Please check and fix.");
                return;
            }
            try
            {
                switch (Call<int>("getdvarint", "promod_skipwarn"))
                {
                    case 0:
                        SkipWarn = false;
                        break;
                    default:
                        SkipWarn = true;
                        break;
                }
            }
            catch (Exception)
            {
                Info("ProMod Error! The dvar \"promod_skipwarn\" is not 0 or 1! ProMod will disable! Please check and fix.");
                return;
            }

            DvarLoader DL = new DvarLoader(file);
            List<DItem> _dvarlist = DL.LoadDvars();
            if (_dvarlist == null || _dvarlist.Count == 0)
            {
                Info("ProMod Error! Target profile doesn't have any valid dvar! ProMod will disable! Please check and fix.");
                return;
            }

            foreach (var item in _dvarlist)
            {
                switch (item.Type)
                {
                    case Dvar.InitClient:
                        if (!_cvar.ContainsKey(item.Key))
                        {
                            _cvar.Add(item.Key, item.Value);
                        }
                        break;
                    case Dvar.ProMod:
                        if (!_promod.ContainsKey(item.Key))
                        {
                            _promod.Add(item.Key, item.Value);
                        }
                        break;
                    case Dvar.ResetProMod:
                        if (!_resetpromod.ContainsKey(item.Key))
                        {
                            _resetpromod.Add(item.Key, item.Value);
                        }
                        break;
                    case Dvar.InitServer:
                        if (!_server.ContainsKey(item.Key))
                        {
                            _server.Add(item.Key, item.Value);
                        }
                        break;
                }
            }

            foreach (var rdvar in _resetpromod)
            {
                if (!_promod.Keys.Contains(rdvar.Key) && SkipWarn == false)
                {
                    Info("Warning! The ResetProMod dvar \"" + rdvar.Key + "\" not in ProMod dvars! System will disable this dvar. Please check and fix!");
                    _resetpromod.Remove(rdvar.Key);
                }
            }

            foreach (var cvar in _cvar)
            {
                if ((_promod.Keys.Contains(cvar.Key) || _resetpromod.Keys.Contains(cvar.Key)) && SkipWarn == false)
                {
                    Info("Warning! The cvar \"" + cvar.Key + "\" in ProMod dvar and ResetProMod dvar! System will disable this cvar. Please check and fix!");
                    _cvar.Remove(cvar.Key);
                }
            }

            foreach (var scvar in _server)
            {
                Call("setdvar", scvar.Key, scvar.Value);
            }

            Info("ProMod successfully initialization!");
            Info("Cvar: " + _cvar.Count + " ProMod Dvar: " + _promod.Count + " ResetProMod Dvar: " + _resetpromod.Count + " Server Dvar: " + _server.Count);

            PlayerConnected += new Action<Entity>(player =>
            {
                foreach (var cvar in _cvar)
                {
                    player.Call("setclientdvar", cvar.Key, cvar.Value);
                }
            });
        }

        public void Info(string message)
        {
            Log.Info(message);
        }

        public override void OnSay(Entity player, string name, string message)
        {
            if (message == "!promod")
            {
                foreach (var dvar in _promod)
                {
                    player.Call("setclientdvar", dvar.Key, dvar.Value);
                }
                player.Call("iprintlnbold", "^3ProMod Enable");
            }
            if (message == "!resetpromod")
            {
                foreach (var dvar in _resetpromod)
                {
                    player.Call("setclientdvar", dvar.Key, dvar.Value);
                }
                player.Call("iprintlnbold", "^3ProMod Disable");
            }
        }
    }
}
