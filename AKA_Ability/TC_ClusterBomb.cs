using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    public class TC_ClusterBomb : ThingComp
    {
        public TCP_ClusterBomb Props => props as TCP_ClusterBomb;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            Activate(parent.Position, previousMap);
        }

        public void Activate(IntVec3 position, Map map)
        {
            if (Props.radius < 1)
            {
                FireProjectile(position, position, map);
                return;
            }
            int num = GenRadial.NumCellsInRadius(Props.radius);
            List<IntVec3> affectedCells = new();
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = position + GenRadial.RadialPattern[i];
                affectedCells.Add(intVec);
            }
            for (int j = 0; j < Props.distributeAmount; j++)
            {
                int randIndex = Rand.Range(0, num - 1);
                FireProjectile(affectedCells[randIndex], position, map);
                num--;
                affectedCells.RemoveRange(randIndex, 1);
            }
        }

        public void FireProjectile(IntVec3 target, IntVec3 position, Map map)
        {
            if (Props.spawnedThingProjectile.category == ThingCategory.Projectile)
            {
                Projectile projectile = (Projectile)GenSpawn.Spawn(Props.spawnedThingProjectile, position, map);
                projectile.Launch(parent, target, target, ProjectileHitFlags.All);
            }
            else
            {
                Thing thing = ThingMaker.MakeThing(Props.spawnedThingProjectile);
                thing.SetFaction(parent.Faction);
                if (!thing.def.MadeFromStuff)
                {
                    GenPlace.TryPlaceThing(thing, target, map, ThingPlaceMode.Near);
                }
            }
        }
    }

    public class TCP_ClusterBomb : CompProperties_UseEffect
    {
        public TCP_ClusterBomb()
        {
            this.compClass = typeof(TC_ClusterBomb);
        }

        public ThingDef spawnedThingProjectile;

        public float radius = 10;

        public int distributeAmount = 1;
    }
}
