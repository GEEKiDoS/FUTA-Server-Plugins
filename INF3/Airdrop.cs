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
            RandomPerk,
        }

        public static Entity _airdropCollision;

        public Airdrop()
        {
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });
        }

        public static Entity DropCrate(Entity self, Entity vehicle, KillstreakType type)
        {
            Entity dropCrateFriendly = Utility.Spawn("script_model", vehicle.Origin);
            dropCrateFriendly.Call("setmodel", "com_plasticcase_friendly");
            dropCrateFriendly.SetField("angles", vehicle.GetField<Vector3>("angles"));
            dropCrateFriendly.Call("clonebrushmodeltoscriptmodel", _airdropCollision);

            dropCrateFriendly.Call("PhysicsLaunchServer", new Vector3(0, 0, 0), new Vector3(0, 0, 0.25f));

            self.AfterDelay(3000, e =>
            {
                switch (type)
                {
                }
            });

            return dropCrateFriendly;
        }

        public static void DropCrateFunc(Entity dropCrate_Friendly, Entity self, KillstreakType type, string shader)
        {
        }
    }
}
