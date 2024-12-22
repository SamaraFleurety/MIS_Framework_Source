using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random
{
    public interface IRandomizer
    {
        public IEnumerable<IWeightedRandomable> Candidates { get; }
        public abstract IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null);
    }
}
