using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace Exo
{
    public class Airdrop : BaseScript
    {
        public enum KillstreakType
        {
            Harrier,
            PredatorMissile,
            UAVStrike,
            AC130Strike,
            A10Strike,
            Juggernog,
            StaminUp,
            MuleKick,
            DoubleTap,
        }

        public static Entity _airdropCollision;

        public Airdrop()
        {
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });
        }

        public static Entity DropCrate(Entity self, Entity vehicle, KillstreakType type)
        {
            Entity dropCrateFriendly = Utility.Script_Model(vehicle.Origin);
            dropCrateFriendly.Call("setmodel", "com_plasticcase_trap_friendly");
            dropCrateFriendly.SetField("angles", vehicle.GetField<Vector3>("angles"));
            dropCrateFriendly.Call("clonebrushmodeltoscriptmodel", _airdropCollision);

            dropCrateFriendly.Call("PhysicsLaunchServer", new Vector3(0, 0, 0), new Vector3(0, 0, 0.25f));

            self.AfterDelay(3000, e =>
            {
                switch (type)
                {
                    case KillstreakType.Harrier:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_precision_airstrike_crate");
                        break;
                    case KillstreakType.PredatorMissile:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_predator_missile_crate");
                        break;
                    case KillstreakType.UAVStrike:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_precision_airstrike_crate");
                        break;
                    case KillstreakType.AC130Strike:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_ac130_crate");
                        break;
                    case KillstreakType.A10Strike:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_precision_airstrike_crate");
                        break;
                    case KillstreakType.Juggernog:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_precision_airstrike_crate");
                        break;
                    case KillstreakType.StaminUp:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_precision_airstrike_crate");
                        break;
                    case KillstreakType.MuleKick:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_precision_airstrike_crate");
                        break;
                    case KillstreakType.DoubleTap:
                        DropCrateFunc(dropCrateFriendly, self, type, "specialty_precision_airstrike_crate");
                        break;
                }
            });

            return dropCrateFriendly;
        }

        public static void DropCrateFunc(Entity dropCrate_Friendly, Entity self, KillstreakType type, string shader)
        {
        }
    }
}
