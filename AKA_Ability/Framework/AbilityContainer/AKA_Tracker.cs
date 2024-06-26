﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace AKA_Ability
{
    public class AKA_AbilityTracker : IExposable
    {
        public Pawn owner;

        public int indexActiveGroupedAbility = -1;

        public List<AKAbility> innateAbilities = new List<AKAbility>();

        public List<AKAbility> groupedAbilities = new List<AKAbility>();

        public static SoundDef[] abilitySFX = new SoundDef[4] { AKADefOf.AK_SFX_Atkboost, AKADefOf.AK_SFX_Defboost, AKADefOf.AK_SFX_Healboost, AKADefOf.AK_SFX_Tactboost };

        //用这个构造器需要手动绑定owner
        public AKA_AbilityTracker()
        {
        }

        public AKA_AbilityTracker(Pawn p)
        {
            owner = p;
        }

        public void Tick()
        {
            foreach(AKAbility i in innateAbilities)
            {
                i.Tick();
            }
            if (groupedAbilities.Count > 0) groupedAbilities[indexActiveGroupedAbility].Tick();
        }

        public IEnumerable<Command> GetGizmos()
        {
            if (owner == null) yield break;

            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count > 1) yield break;

            Command c;
            //固有的 不取决于分组的技能会一直显示
            foreach (AKAbility i in innateAbilities)
            {
                c = i.GetGizmo();
                if (c != null) yield return c;
            }
            //分组技能 仅显示最多1个
            if (indexActiveGroupedAbility != -1 && groupedAbilities.Count > 0)
            {
                c = groupedAbilities[indexActiveGroupedAbility].GetGizmo();
                if (c != null) yield return c;
            }
        }

        public virtual void ExposeData()
        {
            //Scribe_References.Look(ref owner, "p");
            Scribe_Values.Look(ref indexActiveGroupedAbility, "indexAGA");

            Scribe_Collections.Look(ref innateAbilities, "iA", LookMode.Deep);
            Scribe_Collections.Look(ref groupedAbilities, "gA", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (AKAbility i in innateAbilities) i.container = this;
                foreach (AKAbility j in groupedAbilities) j.container = this;
            }
        }

        public virtual void PostPlayAbilitySound(AKAbility ability)
        {
            AKAbilityDef defAbility = ability.def;
            SoundDef defSound = defAbility.useSound;
            if (defSound != null)
            {
                defSound.PlayOneShotOnCamera();
            }
            if (defAbility.typeSFX != SFXType.none)
            {
                abilitySFX[(int)defAbility.typeSFX].PlayOneShot(null);
            }
        }
    }
}
