using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace SK
{
    public class SK : BaseScript
    {
        public SK()
        {
            PlayerConnected += new Action<Entity>(player =>
            {
                player.OnInterval(100, self =>
                {
                    if (player.CurrentWeapon.StartsWith("iw5_usp45"))
                    {
                        player.Call("setweaponammoclip", "throwingknife_mp", 99);
                        player.Call("setweaponammoclip", player.CurrentWeapon, 0);
                        player.Call("setweaponammostock", player.CurrentWeapon, 0);
                    }
                    return player.IsPlayer;
                });

                player.SpawnedPlayer += () =>
                {
                    AfterDelay(100, () => player.Call("givemaxammo", player.CurrentWeapon));
                };
            });
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (mod == "MOD_MELEE")
            {
                player.Health += damage;
            }
            else if (weapon.StartsWith("iw5_l96a1") || weapon.StartsWith("iw5_msr") || weapon.StartsWith("iw5_as50"))
            {
                player.Health = 3;
            }
        }
    }
}
