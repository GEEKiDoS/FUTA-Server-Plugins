using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public static class Perks
    {
        public enum Perk
        {
            QuickRevive = 0,
            SpeedCola = 1,
            Juggernog = 2,
            StaminUp = 3,
            MuleKick = 4,
            DoubelTap = 5,
            DeadShot = 6,
            PHD = 7,
            ElectricCherry = 8,
            WidowsWine = 9,
            VultureAid = 10
        }

        public static string PerkBoxHintString(this Perk perk)
        {
            return "Press ^3[{+activate}]^7 to buy " + perk.GetPerkFullName() + ". [Cost: ^2$^3" + perk.GetPerkPay() + "^7]";
        }

        public static string GetPerkIcon(this Perk perk)
        {
            switch (perk)
            {
                case Perk.QuickRevive:
                    return "specialty_finalstand";
                case Perk.SpeedCola:
                    return "specialty_fastreload";
                case Perk.Juggernog:
                    return "cardicon_juggernaut_1";
                case Perk.StaminUp:
                    return "specialty_longersprint";
                case Perk.MuleKick:
                    return "specialty_twoprimaries";
                case Perk.DoubelTap:
                    return "specialty_moredamage";
                case Perk.DeadShot:
                    return "cardicon_headshot";
                case Perk.PHD:
                    return "specialty_blastshield";
                case Perk.ElectricCherry:
                    return "cardicon_cod4";
                case Perk.WidowsWine:
                    return "cardicon_soap_bar";
                case Perk.VultureAid:
                    return "specialty_scavenger";
                default:
                    return "";
            }
        }

        public static string GetPerkFullName(this Perk perk)
        {
            switch (perk)
            {
                case Perk.QuickRevive:
                    return "Quick Revive";
                case Perk.SpeedCola:
                    return "Speed Cola";
                case Perk.Juggernog:
                    return "Juggernog";
                case Perk.StaminUp:
                    return "Stamin-Up";
                case Perk.MuleKick:
                    return "Mule Kick";
                case Perk.DoubelTap:
                    return "Double Tap Root Beer";
                case Perk.DeadShot:
                    return "Deadshot Daiquiri";
                case Perk.PHD:
                    return "PhD Flopper";
                case Perk.ElectricCherry:
                    return "Electric Cherry";
                case Perk.WidowsWine:
                    return "Widow's Wine";
                case Perk.VultureAid:
                    return "Vulture Aid Elixir";
                default:
                    return "";
            }
        }

        private static HudElem ShowPerkHud(this Entity player, Perk perk)
        {
            switch (perk)
            {
                case Perk.QuickRevive:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(0, 1, 1), "Quick Revive");
                case Perk.SpeedCola:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(0.3f, 0.9f, 0.3f), "Speed Cola");
                case Perk.Juggernog:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(1, 0.3f, 0.3f), "Juggernog");
                case Perk.StaminUp:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(0.9f, 0.9f, 0.3f), "Stamin-Up");
                case Perk.MuleKick:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(1, 0.3f, 0.3f), "Mule Kick");
                case Perk.DoubelTap:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(0.9f, 0.5f, 0.2f), "Double Tap II");
                case Perk.DeadShot:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(0.1f, 0.1f, 0.1f), "Deadshot");
                case Perk.PHD:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(0.7f, 0.5f, 1), "PhD");
                case Perk.ElectricCherry:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(1, 0.3f, 0.3f), "Electric Cherry");
                case Perk.WidowsWine:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(1, 0.3f, 0.3f), "Widow's Wine");
                case Perk.VultureAid:
                    return player.PerkHud(perk.GetPerkIcon(), new Vector3(1, 0.8f, 0), "Vulture Aid");
                default:
                    return null;
            }
        }

        public static int GetPerkPay(this Perk perk)
        {
            switch (perk)
            {
                case Perk.QuickRevive:
                    return 500;
                case Perk.SpeedCola:
                    return 700;
                case Perk.Juggernog:
                    return 1000;
                case Perk.StaminUp:
                    return 800;
                case Perk.MuleKick:
                    return 700;
                case Perk.DoubelTap:
                    return 600;
                case Perk.DeadShot:
                    return 500;
                case Perk.PHD:
                    return 700;
                case Perk.ElectricCherry:
                    return 600;
                case Perk.WidowsWine:
                    return 800;
                case Perk.VultureAid:
                    return 600;
                default:
                    return 0;
            }
        }

        public static bool HasPerk(this Entity player, Perk perk)
        {
            switch (perk)
            {
                case Perk.QuickRevive:
                    if (player.GetField<int>("perk_revive") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.SpeedCola:
                    if (player.GetField<int>("perk_speedcola") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.Juggernog:
                    if (player.GetField<int>("perk_juggernog") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.StaminUp:
                    if (player.GetField<int>("perk_staminup") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.MuleKick:
                    if (player.GetField<int>("perk_mulekick") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.DoubelTap:
                    if (player.GetField<int>("perk_doubletap") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.DeadShot:
                    if (player.GetField<int>("perk_deadshot") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.PHD:
                    if (player.GetField<int>("perk_phd") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.ElectricCherry:
                    if (player.GetField<int>("perk_cherry") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.WidowsWine:
                    if (player.GetField<int>("perk_widow") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Perk.VultureAid:
                    if (player.GetField<int>("perk_vultrue") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        public static void GivePerk(this Entity player, Perk perk)
        {
            player.SetField("aiz_perks", player.GetField<int>("aiz_perks") + 1);
            player.GetField<List<HudElem>>("aiz_perkhuds").Add(player.ShowPerkHud(perk));

            switch (perk)
            {
                case Perk.QuickRevive:
                    player.SetField("perk_revive", 1);
                    player.GamblerText("Comming Soon!", new Vector3(1, 1, 1), new Vector3(0.3f, 0.3f, 0.9f), 1, 0.85f);
                    break;
                case Perk.SpeedCola:
                    player.SetField("perk_speedcola", 1);
                    player.SetPerk("specialty_fastreload", true, false);
                    player.SetPerk("specialty_quickswap", true, false);
                    player.SetPerk("specialty_quickdraw", true, false);
                    break;
                case Perk.Juggernog:
                    player.SetField("perk_juggernog", 1);
                    player.Call("setmodel", "mp_fullbody_ally_juggernaut");
                    player.Call("setviewmodel", "viewhands_juggernaut_ally");
                    player.SetField("maxhealth", 300);
                    player.Health = 300;
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
                    player.SetPerk("specialty_bulletdamage", true, false);
                    player.SetPerk("specialty_rof", true, false);
                    break;
                case Perk.DeadShot:
                    player.SetField("perk_deadshot", 1);
                    PerkFunction.DeadShotThink(player);
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
                case Perk.VultureAid:
                    player.SetField("perk_vultrue", 1);
                    player.SetPerk("specialty_scavenger", true, false);
                    break;
            }
        }

        public static void ResetPerks(this Entity player)
        {
            player.SetField("aiz_perks", 0);

            player.SetField("perk_revive", 0);
            player.SetField("perk_juggernog", 0);
            player.SetField("perk_staminup", 0);
            player.SetField("perk_mulekick", 0);
            player.SetField("perk_doubletap", 0);
            player.SetField("perk_deadshot", 0);
            player.SetField("perk_phd", 0);
            player.SetField("perk_cherry", 0);
            player.SetField("perk_widow", 0);
            player.SetField("perk_vultrue", 0);

            foreach (var item in player.GetField<List<HudElem>>("aiz_perkhuds"))
            {
                item.Call("destroy");
            }

            player.GetField<List<HudElem>>("aiz_perkhuds").Clear();
        }
    }
}
