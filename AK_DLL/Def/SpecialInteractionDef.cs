using RimWorld;
using Verse;
using System;
using UnityEngine;
using AK_DLL.Traits.Interaction;
using System.Collections.Generic;

namespace AK_DLL
{
    public class SpecialInteractionDef : InteractionDef
    {
        public List<string> involvedPawns = new List<string>();
    }
}
