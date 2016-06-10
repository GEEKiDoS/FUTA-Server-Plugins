using System;
using System.Collections.Generic;
using System.IO;

namespace ProMod
{
    public enum Dvar
    {
        InitClient = 0,
        ProMod = 1,
        ResetProMod = 2,
        InitServer = 3,
        Unknow = 4,
    }

    public class DvarLoader
    {
        private List<string> _strlines = new List<string>();

        private struct dItem
        {
            public Dvar Type;
            public string Key;
            public string Value;
        }

        public DvarLoader(string file)
        {
            string item = null;
            if (File.Exists(file))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        for (item = reader.ReadLine(); item != null; item = reader.ReadLine())
                        {
                            item = item.Trim();
                            if (item != "" && !item.StartsWith("//"))
                            {
                                _strlines.Add(item);
                            }
                        }
                        reader.Dispose();
                    }
                    return;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            throw new FileNotFoundException("file not find!");
        }

        public List<DItem> LoadDvars()
        {
            if (_strlines.Count <= 0)
            {
                return null;
            }

            List<DItem> list = new List<DItem>();
            foreach (string line in _strlines)
            {
                string[] sline = line.Split(new char[] { ' ' }, 3);
                Dvar type;
                if (sline[0] == "cvar")
                {
                    type = Dvar.InitClient;
                }
                else if (sline[0] == "dvar")
                {
                    type = Dvar.ProMod;
                }
                else if (sline[0] == "rdvar")
                {
                    type = Dvar.ResetProMod;
                }
                else if (sline[0] == "sdvar")
                {
                    type = Dvar.InitServer;
                }
                else
                {
                    type = Dvar.Unknow;
                    continue;
                }
                DItem ditem = new DItem
                {
                    Type = type,
                    Key = sline[1],
                    Value = sline[2]
                };

                list.Add(ditem);
            }

            return list;
        }
    }
}
