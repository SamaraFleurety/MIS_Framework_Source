﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class HediffStat
    {
        public HediffDef hediff;
        public BodyPartDef part = null;
        public float serverity = 1f;
        public int randWeight = 1;
        public int randWorseMin = 0;
        public int randWorseMax = 1;
    }
}