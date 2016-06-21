using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class PerkCola
    {
        private const int QUICK_REVIVE = 0;
        private const int SPEED_COLA = 1;
        private const int JUGGERNOG = 2;
        private const int STAMIN_UP = 3;
        private const int MULE_KICK = 4;
        private const int DOUBLE_TAP = 5;
        private const int DEAD_SHOT = 6;
        private const int PHD = 7;
        private const int ELECTRIC_CHERRY = 8;
        private const int WIDOW_S_WINE = 9;
        private const int VULTURE_AID = 10;

        public int Type { get; private set; }
        public string FullName
        {
            get
            {
                return GetPerkFullName(this);
            }
        }

        public string PerkIcon
        {
            get
            {
                return GetPerkIcon(this);
            }
        }
        public int Pay
        {
            get
            {
                return GetPerkPay(this);
            }
        }

        private PerkCola()
        {

        }

        public PerkCola(int type)
        {
            Type = type;
        }

        public static PerkCola RandomPerk()
        {
            return new PerkCola(Utility.Rng.Next(11));
        }

        public static string PerkBoxHintString(PerkCola perk)
        {
            if (Function.Call<int>("getdvarint", "bonus_fire_sale") == 1)
            {
                return "Press ^3[{+activate}]^7 to buy " + perk.FullName + ". [Cost: ^2$^610^7]";
            }
            return "Press ^3[{+activate}]^7 to buy " + perk.FullName + ". [Cost: ^2$^3" + perk.Pay + "^7]";
        }

        private string GetPerkIcon(PerkCola perk)
        {
            if (perk.Type == QUICK_REVIVE)
            {
                return "specialty_finalstand";
            }
            if (perk.Type == SPEED_COLA)
            {
                return "specialty_fastreload";
            }
            if (perk.Type == JUGGERNOG)
            {
                return "cardicon_juggernaut_1";
            }
            if (perk.Type == STAMIN_UP)
            {
                return "specialty_longersprint";
            }
            if (perk.Type == MULE_KICK)
            {
                return "specialty_twoprimaries";
            }
            if (perk.Type == DOUBLE_TAP)
            {
                return "specialty_moredamage";
            }
            if (perk.Type == DEAD_SHOT)
            {
                return "cardicon_headshot";
            }
            if (perk.Type == PHD)
            {
                return "specialty_blastshield";
            }
            if (perk.Type == ELECTRIC_CHERRY)
            {
                return "cardicon_cod4";
            }
            if (perk.Type == WIDOW_S_WINE)
            {
                return "cardicon_soap_bar";
            }
            if (perk.Type == VULTURE_AID)
            {
                return "specialty_scavenger";
            }
            return "";
        }

        private string GetPerkFullName(PerkCola perk)
        {
            if (perk.Type == QUICK_REVIVE)
            {
                return "Quick Revive";
            }
            if (perk.Type == SPEED_COLA)
            {
                return "Speed Cola";
            }
            if (perk.Type == JUGGERNOG)
            {
                return "Juggernog";
            }
            if (perk.Type == STAMIN_UP)
            {
                return "Stamin-Up";
            }
            if (perk.Type == MULE_KICK)
            {
                return "Mule Kick";
            }
            if (perk.Type == DOUBLE_TAP)
            {
                return "Double Tap Root Beer";
            }
            if (perk.Type == DEAD_SHOT)
            {
                return "Deadshot Daiquiri";
            }
            if (perk.Type == PHD)
            {
                return "PhD Flopper";
            }
            if (perk.Type == ELECTRIC_CHERRY)
            {
                return "Electric Cherry";
            }
            if (perk.Type == WIDOW_S_WINE)
            {
                return "Widow's Wine";
            }
            if (perk.Type == VULTURE_AID)
            {
                return "Vulture Aid Elixir";
            }
            return "";
        }

        private static HudElem ShowPerkHud(Entity player, PerkCola perk)
        {
            return player.PerkHudNoEffect(perk.PerkIcon);
        }

        private int GetPerkPay(PerkCola perk)
        {
            if (perk.Type == QUICK_REVIVE)
            {
                return 500;
            }
            if (perk.Type == SPEED_COLA)
            {
                return 700;
            }
            if (perk.Type == JUGGERNOG)
            {
                return 1000;
            }
            if (perk.Type == STAMIN_UP)
            {
                return 800;
            }
            if (perk.Type == MULE_KICK)
            {
                return 700;
            }
            if (perk.Type == DOUBLE_TAP)
            {
                return 600;
            }
            if (perk.Type == DEAD_SHOT)
            {
                return 600;
            }
            if (perk.Type == PHD)
            {
                return 800;
            }
            if (perk.Type == ELECTRIC_CHERRY)
            {
                return 600;
            }
            if (perk.Type == WIDOW_S_WINE)
            {
                return 1000;
            }
            if (perk.Type == VULTURE_AID)
            {
                return 500;
            }
            return 0;
        }

        public static bool HasPerkCola(Entity player, PerkCola perk)
        {
            switch (perk.Type)
            {
                case QUICK_REVIVE:
                    if (player.GetField<int>("perk_revive") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case SPEED_COLA:
                    if (player.GetField<int>("perk_speedcola") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case JUGGERNOG:
                    if (player.GetField<int>("perk_juggernog") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case STAMIN_UP:
                    if (player.GetField<int>("perk_staminup") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case MULE_KICK:
                    if (player.GetField<int>("perk_mulekick") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case DOUBLE_TAP:
                    if (player.GetField<int>("perk_doubletap") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case DEAD_SHOT:
                    if (player.GetField<int>("perk_deadshot") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case PHD:
                    if (player.GetField<int>("perk_phd") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case ELECTRIC_CHERRY:
                    if (player.GetField<int>("perk_cherry") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case WIDOW_S_WINE:
                    if (player.GetField<int>("perk_widow") == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case VULTURE_AID:
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

        public static void GivePerkCola(Entity player, PerkCola perk)
        {
            player.SetField("aiz_perks", player.GetField<int>("aiz_perks") + 1);
            player.GetField<List<HudElem>>("aiz_perkhuds").Add(ShowPerkHud(player, perk));

            switch (perk.Type)
            {
                case QUICK_REVIVE:
                    player.SetField("perk_revive", 1);
                    break;
                case SPEED_COLA:
                    player.SetField("perk_speedcola", 1);
                    player.SetPerk("specialty_fastreload", true, false);
                    player.SetPerk("specialty_quickswap", true, false);
                    player.SetPerk("specialty_quickdraw", true, false);
                    break;
                case JUGGERNOG:
                    player.SetField("perk_juggernog", 1);
                    player.SetField("oldmodel", player.GetField<string>("model"));
                    player.Call("setmodel", "mp_fullbody_ally_juggernaut");
                    player.Call("setviewmodel", "viewhands_juggernaut_ally");
                    player.SetField("maxhealth", 300);
                    player.Health = 300;
                    break;
                case STAMIN_UP:
                    player.SetField("perk_staminup", 1);
                    player.SetField("speed", 1.3f);
                    player.SetPerk("specialty_marathon", true, false);
                    player.SetPerk("specialty_lightweight", true, false);
                    player.SetPerk("specialty_fastsprintrecovery", true, false);
                    break;
                case MULE_KICK:
                    player.SetField("perk_mulekick", 1);
                    player.GiveWeapon(Sharpshooter._mulekickWeapon.Code);
                    player.Call("givemaxammo", Sharpshooter._mulekickWeapon.Code);
                    break;
                case DOUBLE_TAP:
                    player.SetField("perk_doubletap", 1);
                    player.SetPerk("specialty_moredamage", true, false);
                    player.SetPerk("specialty_bulletdamage", true, false);
                    player.SetPerk("specialty_rof", true, false);
                    break;
                case DEAD_SHOT:
                    player.SetField("perk_deadshot", 1);
                    PerkFunction.DeadShotThink(player);
                    player.SetPerk("specialty_reducedsway", true, false);
                    player.SetPerk("specialty_bulletaccuracy", true, false);
                    break;
                case PHD:
                    player.SetField("perk_phd", 1);
                    player.SetPerk("_specialty_blastshield", true, false);
                    break;
                case ELECTRIC_CHERRY:
                    player.SetField("perk_cherry", 1);
                    break;
                case WIDOW_S_WINE:
                    player.SetField("perk_widow", 1);
                    player.SetPerk("specialty_fastermelee", true, false);
                    break;
                case VULTURE_AID:
                    player.SetField("perk_vultrue", 1);
                    player.SetPerk("specialty_scavenger", true, false);
                    break;
            }
        }

        public static void TakeAllPerkCola(Entity player)
        {
            player.Call("clearperks");

            if (player.GetField<int>("perk_juggernog") == 1)
            {
                player.Call("setmodel", player.GetField<string>("oldmodel"));
                player.Call("setviewmodel", "viewmodel_base_viewhands");
                player.SetField("maxhealth", 100);
                player.Health = 100;
            }
            if (player.GetField<int>("perk_staminup") == 1)
            {
                player.SetField("speed", 1);
            }
            if (player.GetField<int>("perk_mulekick") == 1)
            {
                player.TakeWeapon(Sharpshooter._mulekickWeapon.Code);
                player.AfterDelay(300, e => player.SwitchToWeaponImmediate(Sharpshooter._firstWeapon.Code));
            }
            if (player.GetField<int>("perk_vultrue") == 1)
            {
                player.TakeWeapon("uav_strike_marker_mp");
                player.GiveWeapon(Sharpshooter._secondeWeapon.Code);
                player.Call("givemaxammo", Sharpshooter._secondeWeapon.Code);
                player.AfterDelay(300, e => player.SwitchToWeaponImmediate(Sharpshooter._firstWeapon.Code));
            }

            ResetPerkCola(player);
        }

        public static void ResetPerkCola(Entity player)
        {
            player.SetField("aiz_perks", 0);

            player.SetField("perk_revive", 0);
            player.SetField("perk_speedcola", 0);
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
