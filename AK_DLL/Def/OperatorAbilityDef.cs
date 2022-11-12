using System;
using RimWorld;
using System.Linq;
using System.Text;
using Verse;
using System.Collections.Generic;

namespace AK_DLL
{
    public class OperatorAbilityDef : Def
    {
        public string icon;
        public int CD = 1;
        public timeToTick CDUnit = timeToTick.day;
        public AbilityType abilityType;
        public int? range;
        public VerbProperties verb;
        public VerbProperties verb_Reclaim;
        public string iconReclaim;
        public string reclaimLabel;
        public string reclaimDesc;
        public PawnKindDef canReclaimPawn;
        public SoundDef useSound;
        public SFXType typeSFX = SFXType.tact;

        public bool isSectorAbility = false;
        public float sectorRadius;
        public float minAngle;
        public float maxAngle;

        public List<AbilityEffectBase> compEffectList;
        public bool needCD;
        //public bool needTarget;
        public TargetMode targetMode = TargetMode.Self;

        public List<HediffDef> selfHediff;
        public float debuffSeverity;

        public int maxCharge = 1;
        public bool displayOnUndraft = false;
        public bool grouped = false;
    }
}