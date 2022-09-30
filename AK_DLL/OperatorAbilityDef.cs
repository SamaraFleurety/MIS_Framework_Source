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
        public int CD;
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
        public bool needTarget;

        public HediffDef debuff;
        public float debuffSeverity;

        public int maxCharge = 1;
    }
}