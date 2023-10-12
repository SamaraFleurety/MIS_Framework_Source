using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class Ability_Operator : Ability
    {
        public OperatorDocument document;

        public override void AbilityTick()
        {
            return;
        }

        public override IEnumerable<Command> GetGizmos()
        {
            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

    }
}
