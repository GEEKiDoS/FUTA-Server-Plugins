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
            if (attacker == null || !attacker.IsPlayer || (attacker.GetTeam() == player.GetTeam() && attacker != player))
                return;

            if (attacker.GetTeam() == "allies")
            {
                if (attacker.GetField<int>("perk_phd") == 1 && mod == "MOD_MELEE")
                {
                    switch (Utility.rng.Next(2))
                    {
                        case 0:
                            Call("playfx", Call<int>("loadfx", "explosions/tanker_explosion"), player.Origin);
                            player.Call("playsound", "cobra_helicopter_crash");
                            Call("RadiusDamage", player.Origin, 500, 500, 500, player, "MOD_EXPLOSIVE", "bomb_site_mp");
                            break;
                    }
                }
                if (attacker.GetField<int>("perk_deadshot") == 1 && hitLoc.ToLower().Contains("head"))
                {
                    player.Health = 3;
                }
                if (attacker.GetField<int>("perk_widow") == 1 && mod != "MOD_MELEE" && mod != "MOD_EXPLOSIVE")
                {
                    attacker.SetField("perk_widow", 2);
                    WidowsWineThink(player);
                }
            }
            else if (attacker.GetTeam() == "axis")
            {
                if (player.GetField<int>("perk_phd") == 1 && (mod.Contains("EXPLOSIVE") || mod.Contains("PROJECTILE")))
                {
                    player.Health = player.GetField<int>("maxhealth");
                }
                if (player.GetField<int>("perk_cherry") == 1 && mod == "MOD_MELEE")
                {
                    switch (Utility.rng.Next(2))
                    {
                        case 0:
                            player.Health = player.GetField<int>("maxhealth");
                            ElectricCherryThink(player);
                            break;
                    }
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
                zombie.Call("shellshock", "concussion_grenade_mp", 3);
                zombie.Call("finishplayerdamage", zombie, player, 50, 0, 0, "bomb_site_mp", zombie.Origin, "MOD_EXPLOSIVE", 0);
            }
        }

        public static void WidowsWineThink(Entity player)
        {
            var center = Utility.Spawn("script_origin", player.Origin);

            player.OnInterval(100, e =>
            {
                var zombies = GetClosingZombies(center);
                foreach (var zombie in zombies)
                {
                    if (zombie.GetField<int>("isstick") == 0)
                    {
                        zombie.Call("shellshock", "concussion_grenade_mp", 3);
                        zombie.SetField("isstick", 1);
                        zombie.SetField("speed", 0.3f);
                    }
                }

                return center != null && player.IsPlayer && player.IsAlive && player.HasField("perk_widow") && player.GetField<int>("perk_widow") == 2;
            });

            player.AfterDelay(10000, en =>
            {
                player.SetField("perk_widow", 1);
                center.Call("delete");
            });
        }
    }
}
