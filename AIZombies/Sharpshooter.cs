using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public class Sharpshooter : BaseScript
    {
        public static Weapon _firstWeapon;
        public static Weapon _secondeWeapon;

        private HudElem _cycleTimer;

        public static int _cycleRemaining = 30;

        public Sharpshooter()
        {
            UpdateWeapon();

            SharpShooter_Tick();

            PlayerConnected += player =>
            {
                OnPlayerSpawned(player);
                player.SpawnedPlayer += () => OnPlayerSpawned(player);
            };
        }

        private string FormatTime(int seconds)
        {
            return string.Format("{0}:{1}", seconds / 60, (seconds % 60).ToString().PadLeft(2, '0'));

        }

        public void SharpShooter_Tick()
        {
            _cycleTimer = HudElem.CreateServerFontString("objective", 1.4f);
            _cycleTimer.SetPoint("TOPLEFT", "TOPLEFT", 115, 5);
            _cycleTimer.HideWhenInMenu = true;

            OnInterval(1000, () =>
            {
                _cycleRemaining--;

                if (_cycleRemaining <= 0)
                {
                    _cycleRemaining = Utility.Random.Next(45, 90);

                    UpdateWeapon();
                }

                _cycleTimer.SetText("Weapon Cycling: " + FormatTime(_cycleRemaining));

                return true;
            });

            //AIZDebug.DebugLog(GetType(), "Sharpshooter is started.");
        }

        public void UpdateWeapon()
        {
            _firstWeapon = Weapon.GetRandomFirstWeapon();
            _secondeWeapon = Weapon.GetRandomSecondWeapon();

            foreach (var player in Utility.Players)
            {
                player.TakeAllWeapons();

                player.GiveMaxAmmoWeapon(_firstWeapon.Code);
                player.GiveMaxAmmoWeapon(_secondeWeapon.Code);
                player.GiveMaxAmmoWeapon("frag_grenade_mp");
                player.GiveMaxAmmoWeapon("trophy_mp");
                player.AfterDelay(300, ent =>
                {
                    ent.SwitchToWeaponImmediate(_firstWeapon.Code);
                });

                //player.GamblerText("Weapon Cycled", new Vector3(1, 1, 1), new Vector3(0.3f, 0.3f, 0.9f), 1, 0.85f);
            }

            //AIZDebug.DebugLog(GetType(), "Weapon Cycled");
            //AIZDebug.DebugLog(GetType(), "First Weapon: " + _firstWeapon+ " Second Weapon: " + _secondeWeapon+" Mule Kick Weapon: " + _mulekickWeapon);
        }

        public void OnPlayerSpawned(Entity player)
        {
            player.TakeAllWeapons();

            player.GiveMaxAmmoWeapon(_firstWeapon.Code);
            player.GiveMaxAmmoWeapon(_secondeWeapon.Code);

            player.GiveMaxAmmoWeapon("frag_grenade_mp");
            player.GiveMaxAmmoWeapon("trophy_mp");

            player.AfterDelay(300, entity =>
            {
                entity.SwitchToWeaponImmediate(_firstWeapon.Code);
            });

            player.OnInterval(100, e =>
            {
                var weapon = player.CurrentWeapon;

                if (weapon.StartsWith("rpg") || weapon.StartsWith("iw5_smaw") || weapon.StartsWith("m320") || weapon.StartsWith("stinger") || weapon.StartsWith("javelin") || weapon.StartsWith("gl") || weapon.StartsWith("uav"))
                {
                    if (player.IsAlive && player.HasField("perk_vultrue") && player.GetField<int>("perk_vultrue") == 1)
                        player.Call("giveMaxAmmo", weapon);
                }

                if (player.GetTeam() == "axis")
                {
                    return false;
                }

                return true;
            });
        }
    }
}
