using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace Exo
{
    public static class Perks
    {
        public enum Perk
        {
            QuickRevive,
            SpeedCola,
            Juggernog,
            StaminUp,
            MuleKick,
            DoubelTap,
            DeadShot,
            PHD,
            ElectricCherry,
            WidowsWine
        }

        public static string GetPerkBoxString(Perk perk)
        {
            switch (perk)
            {
                case Perk.QuickRevive:
                    return "Press ^3[{+activate}]^7 to buy Quick Revive. [Cost: ^2$^3500^7]";
                case Perk.SpeedCola:
                    return "Press ^3[{+activate}]^7 to buy Speed Cola. [Cost: ^2$^3700^7]";
                case Perk.Juggernog:
                    return "Press ^3[{+activate}]^7 to buy Juggernog. [Cost: ^2$^31000^7]";
                case Perk.StaminUp:
                    return "Press ^3[{+activate}]^7 to buy Stamin-Up. [Cost: ^2$^3800^7]";
                case Perk.MuleKick:
                    return "Press ^3[{+activate}]^7 to buy Mule Kick. [Cost: ^2$^3700^7]";
                case Perk.DoubelTap:
                    return "Press ^3[{+activate}]^7 to buy Double Tap Root Beer. [Cost: ^2$^3600^7]";
                case Perk.DeadShot:
                    return "Press ^3[{+activate}]^7 to buy Deadshot Daiquiri. [Cost: ^2$^3500^7]"; ;
                case Perk.PHD:
                    return "Press ^3[{+activate}]^7 to buy PhD Flopper. [Cost: ^2$^3700^7]";
                case Perk.ElectricCherry:
                    return "Press ^3[{+activate}]^7 to buy Electric Cherry. [Cost: ^2$^3800^7]";
                case Perk.WidowsWine:
                    return "Press ^3[{+activate}]^7 to buy Widow's Wine. [Cost: ^2$^3900^7]";
                default:
                    return "";
            }
        }

        private static HudElem ShowPerkHud(this Entity player, Perk perk)
        {
            switch (perk)
            {
                case Perk.QuickRevive:
                    return player.PerkHud("", new Vector3(0, 1, 1), "Quick Revive");
                case Perk.SpeedCola:
                    return player.PerkHud("", new Vector3(0.3f, 0.9f, 0.3f), "Fast Reloading Fast Swap Fast Aim");
                case Perk.Juggernog:
                    return player.PerkHud("", new Vector3(1, 0.3f, 0.3f), "Double Health");
                case Perk.StaminUp:
                    return player.PerkHud("", new Vector3(0.9f, 0.9f, 0.3f), "Move Faster");
                case Perk.MuleKick:
                    return player.PerkHud("", new Vector3(0.9f, 0.9f, 0.3f), "Give 2 Weapon in Weapon Cycle");
                case Perk.DoubelTap:
                    return player.PerkHud("", new Vector3(1, 0.3f, 0.3f), "Double Rate of Fire and Double Damage");
                case Perk.DeadShot:
                    return player.PerkHud("", new Vector3(0.1f, 0.1f, 0.1f), "Dead Shot");
                case Perk.PHD:
                    return player.PerkHud("", new Vector3(1, 0.3f, 0.3f), "No Explosion Damage and Explode Knife Kill");
                case Perk.ElectricCherry:
                    return player.PerkHud("", new Vector3(1, 0.3f, 0.3f), "Provide Cover when Zombie Attack");
                case Perk.WidowsWine:
                    return player.PerkHud("", new Vector3(1, 0.3f, 0.3f), "Stick Closing Zombies");
                default:
                    return null;
            }
        }

        public static void SetAizPerk(this Entity player, Perk perk)
        {
            player.SetField("aiz_perks", player.GetField<int>("aiz_perk") + 1);
            player.GetField<List<HudElem>>("aiz_perkhuds").Add(player.ShowPerkHud(perk));

            switch (perk)
            {
                case Perk.QuickRevive:
                    player.SetField("perk_revive", 1);
                    break;
                case Perk.SpeedCola:
                    player.SetField("perk_speedcola", 1);
                    player.SetPerk("specialty_fastreload", true, false);
                    player.SetPerk("specialty_quickswap", true, false);
                    player.SetPerk("specialty_quickdraw", true, false);
                    break;
                case Perk.Juggernog:
                    player.SetField("perk_juggernog", 1);
                    player.SetField("maxhealth", 200);
                    player.Health = 200;
                    break;
                case Perk.StaminUp:
                    player.SetField("perk_staminup", 1);
                    player.SetField("speed", 1.3f);
                    player.SetPerk("specialty_marathon", true, false);
                    player.SetPerk("specialty_lightweight", true, false);
                    player.SetPerk("specialty_fastsprintrecovery", true, false);
                    break;
                case Perk.MuleKick:
                    player.SetField("perk_mulekick", 1);
                    break;
                case Perk.DoubelTap:
                    player.SetField("perk_doubletap", 1);
                    player.SetPerk("specialty_moredamage", true, false);
                    player.SetPerk("specialty_rof", true, false);
                    break;
                case Perk.DeadShot:
                    player.SetField("perk_deadshot", 1);
                    player.SetPerk("specialty_reducedsway", true, false);
                    player.SetPerk("specialty_bulletaccuracy", true, false);
                    break;
                case Perk.PHD:
                    player.SetField("perk_phd", 1);
                    player.SetPerk("_specialty_blastshield", true, false);
                    break;
                case Perk.ElectricCherry:
                    player.SetField("perk_cherry", 1);
                    break;
                case Perk.WidowsWine:
                    player.SetField("perk_widow", 1);
                    break;
            }
        }
    }
}
