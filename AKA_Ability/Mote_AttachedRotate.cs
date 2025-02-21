using Verse;

namespace AKA_Ability
{
    public class Mote_AttachedRotate : MoteAttached
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        protected override void TimeInterval(float deltaTime)
        {
            base.TimeInterval(deltaTime);
            exactRotation += rotationRate * deltaTime;
        }
    }

}
