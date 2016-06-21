using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using InfinityScript;

namespace SXXMFaker
{
    public class SXXMFaker : BaseScript
    {
        public static readonly IntPtr ClantagOffset = (IntPtr)0x01AC5564;
        public static readonly int PlayerDataSize2 = 0x38A4;

        public SXXMFaker()
        {
            PlayerConnected += player =>
            {
                var clantag = Marshal.PtrToStringAnsi(ClantagOffset + PlayerDataSize2 * player.EntRef);

                if (clantag == "SXXM")
                {
                    Print("Player " + player.Name + " clantag is SXXM. Auto change to CN.");
                    OnInterval(1000, () =>
                    {
                        Marshal.Copy(Encoding.ASCII.GetBytes("^1CN"), 0, ClantagOffset + PlayerDataSize2 * player.EntRef, 4);
                        return player != null && player.IsPlayer;
                    });
                }

                player.OnInterval(100, e =>
                {
                    player.Call("freezecontrols", false);

                    return true;
                });
            };
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (mod=="MOD_HEAD_SHOT")
            {
                player.Call("detachall");
            }
        }

        public void Print(string message)
        {
            Log.Info(message);
        }
    }
}
