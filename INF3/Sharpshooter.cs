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
        public static Weapon _mulekickWeapon;
        public static Weapon _secondeWeapon;

        private HudElem _cycleTimer;

        public static int _cycleRemaining = 30;

        public Sharpshooter()
        {
            _firstWeapon = Weapon.GetRandomFirstWeapon();
            _secondeWeapon = Weapon.GetRandomSecondWeapon();
            _mulekickWeapon = Weapon.GetRandomFirstWeapon();

            while (_mulekickWeapon.BaseName == _firstWeapon.BaseName)
            {
                _mulekickWeapon = Weapon.GetRandomFirstWeapon();
            }

            OnInterval(100, () =>
            {
                if (Call<int>("getteamscore", "axis") == 1)
                {
                    SharpShooter_Tick();
                    return false;
                }
                return true;
            });

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

                if (_cycleRemaining == 0)
                {
                    _cycleRemaining = Utility.Rng.Next(30, 61);

                    UpdateWeapon();
                }

                _cycleTimer.SetText("Weapon Cycling: " + FormatTime(_cycleRemaining));

                return true;
            });
        }

        public void UpdateWeapon()
        {
            _firstWeapon = Weapon.GetRandomFirstWeapon();
            _secondeWeapon = Weapon.GetRandomSecondWeapon();
            _mulekickWeapon = Weapon.GetRandomFirstWeapon();

            while (_mulekickWeapon.BaseName == _firstWeapon.BaseName || _mulekickWeapon.Type == Weapon.WeaponType.Launcher || _mulekickWeapon.Type == Weapon.WeaponType.Special || _mulekickWeapon.Type == Weapon.WeaponType.KillstreakHandheld)
            {
                _mulekickWeapon = Weapon.GetRandomFirstWeapon();
            }

            foreach (var player in Players)
            {
                if (player.GetTeam() == "allies")
                {
                    player.TakeAllWeapons();

                    player.GiveWeapon(_firstWeapon.Code);
                    player.Call("givemaxammo", _firstWeapon.Code);
                    if (player.GetField<int>("perk_mulekick") == 1)
                    {
                        player.GiveWeapon(_mulekickWeapon.Code);
                        player.Call("givemaxammo", _mulekickWeapon.Code);
                    }
                    if (player.HasField("perk_vultrue") && player.GetField<int>("perk_vultrue") == 1)
                    {
                        player.GiveWeapon("uav_strike_marker_mp");
                        player.Call("givemaxammo", "uav_strike_marker_mp");
                    }
                    else
                    {
                        player.GiveWeapon(_secondeWeapon.Code);
                        player.Call("givemaxammo", _secondeWeapon.Code);
                    }
                    player.AfterDelay(100, ent =>
                    {
                        player.GiveWeapon("frag_grenade_mp");
                        player.Call("givemaxammo", "frag_grenade_mp");
                        if (player.GetField<int>("perk_vultrue") == 1)
                        {
                            player.GiveWeapon("trophy_mp");
                            player.Call("givemaxammo", "trophy_mp");

                        }
                    });
                    player.AfterDelay(100, ent =>
                    {
                        ent.SwitchToWeaponImmediate(_firstWeapon.Code);
                    });

                    player.GamblerText("Weapon Cycled", new Vector3(1, 1, 1), new Vector3(0.3f, 0.3f, 0.9f), 1, 0.85f);
                }
            }
        }

        public void OnPlayerSpawned(Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                player.TakeAllWeapons();

                player.GiveWeapon(_firstWeapon.Code);
                player.Call("giveMaxAmmo", _firstWeapon.Code);
                player.GiveWeapon(_secondeWeapon.Code);
                player.Call("giveMaxAmmo", _secondeWeapon.Code);

                player.GiveWeapon("frag_grenade_mp");
                player.GiveWeapon("trophy_mp");

                player.AfterDelay(100, entity =>
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
}
