using System;
using InfinityScript;

namespace CrazyJUGG
{
    public class CrazyJUGG : BaseScript
    {
        public CrazyJUGG()
        {
            PlayerConnected += new Action<Entity>(player =>
            {
                OnSpawned(player);
                player.SpawnedPlayer += () => OnSpawned(player);

                player.OnNotify("weapon_fired", delegate (Entity self, Parameter weapon)
                {
                    if (weapon.As<string>() != "ac130_105mm_mp")
                    {
                        player.Call("setweaponammostock", new Parameter[] { player.CurrentWeapon, 100 });
                        player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 100 });
                    }
                    else
                    {
                        AfterDelay(3000, () => 
                        {
                            player.Call("setweaponammostock", new Parameter[] { player.CurrentWeapon, 100 });
                            player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 100 });
                        });
                    }
                });
            });
        }

        public void OnSpawned(Entity player)
        {
            player.TakeAllWeapons();
            if (player.GetField<string>("sessionteam") == "allies")
            {
                player.GiveWeapon("ac130_105mm_mp");
                player.GiveWeapon("ac130_40mm_mp");
                player.GiveWeapon("ac130_25mm_mp");

                AfterDelay(200, () => player.SwitchToWeaponImmediate("ac130_105mm_mp"));
            }
            else
            {
                player.GiveWeapon("rpg_mp");
                player.GiveWeapon("gl_mp");
                player.GiveWeapon("javelin_mp");
                AfterDelay(200, () => player.SwitchToWeaponImmediate("rpg_mp"));

                player.SetPerk("specialty_rof", true, false);
                player.SetPerk("specialty_quickdraw", true, false);
            }
        }
    }
}
