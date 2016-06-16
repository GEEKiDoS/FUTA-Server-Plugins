using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class PerkFunction : BaseScript
    {
        private static List<Entity> GetClosingZombies(Entity player)
        {
            var list = new List<Entity>();
            foreach (var ent in Utility.GetPlayers())
            {
                if (ent.GetTeam() == "axis" && ent.Origin.DistanceTo(player.Origin) <= 500)
                {
                    list.Add(ent);
                }
            }

            return list;
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if(attacker == null || !attacker.IsPlayer || attacker.GetTeam() == player.GetTeam())
                return;

            if (attacker.GetTeam() == "allies")
            {
                if (attacker.GetField<int>("perk_phd") == 1 && mod == "MOD_MELEE")
                {
                    switch (Utility.Rng.Next(2))
                    {
                        case 0:
                            attacker.Health = 1000;
                            AfterDelay(100, () =>
                            {
                                attacker.RadiusExploed(attacker.Origin);
                            });
                            AfterDelay(100, () =>
                            {
                                attacker.Health = attacker.GetField<int>("maxhealth");
                            });
                            break;
                    }
                }
                if (attacker.GetField<int>("perk_deadshot") == 1 && hitLoc.ToLower().Contains("head"))
                {
                    player.Health = 3;
                }
                if (attacker.GetField<int>("perk_widow") == 1 && mod != "MOD_MELEE")
                {
                    attacker.SetField("perk_widow", 2);
                    WidowsWineThink(attacker, player.Origin);
                }
            }
            else if (attacker.GetTeam() == "axis")
            {
                if (player.GetField<int>("perk_phd") == 1 && (mod != "MOD_MELEE" || !mod.Contains("BULLET")))
                {
                    player.Health = player.GetField<int>("maxhealth");
                }
                if (player.GetField<int>("perk_cherry") == 1 && mod.Contains("MELEE"))
                {
                    player.SetField("perk_cherry", 2);
                    player.Health = player.GetField<int>("maxhealth");
                    ElectricCherryThink(player);
                }
            }
        }

        private void QuickReviveThink(Entity player)
        {

        }

        public static void DeadShotThink(Entity player)
        {
            player.OnInterval(100, e =>
            {
                player.Call("recoilscaleon", 0);
                return player.IsPlayer && player.GetField<int>("perk_deadshot") == 1;
            });
        }

        private void ElectricCherryThink(Entity player)
        {
            var zombies = GetClosingZombies(player);
            foreach (var zombie in zombies)
            {
                zombie.ElectricCherryExploed(player);
            }
            AfterDelay(3000, () => player.SetField("perk_cherry", 1));
        }

        private void WidowsWineThink(Entity player, Vector3 origin)
        {
            Effects.WidowsWineExploed(player, origin);
            AfterDelay(10000, () => player.SetField("perk_widow", 1));
        }
    }
}
