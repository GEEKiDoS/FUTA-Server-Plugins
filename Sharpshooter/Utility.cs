using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfinityScript;

namespace Sharpshooter
{
    public static class Utility
    {
        public static string GetTeam(this Entity e)
        {
            if (!e.IsPlayer)
            {
                throw new Exception("Entity is not player");
            }

            return e.GetField<string>("sessionteam");
        }
    }
}
