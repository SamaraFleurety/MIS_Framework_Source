using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AK_DLL
{
    public class Verb_Ability : Verb_CastBase
	{
        protected override bool TryCastShot()
		{
			Pawn casterPawn = this.CasterPawn;
			this.CasterPawn.PlaySound(this.ability.typeSFX);

			ThingWithComps apparel = base.EquipmentSource;

			if (this.ability.needCD)
			{
				if (this.CDs.charge == this.CDs.maxCharge) this.CDs.CD = this.CDs.maxCD;
				this.CDs.charge -= 1;
			}

			IntVec3 intVec3 = this.currentTarget.Cell;
			if (!this.ability.isSectorAbility)
			{
				//只要选了格子就可以释放对格子的技能
				foreach (AbilityEffectBase compEffect in this.ability.compEffectList)
				{
					compEffect.DoEffect_IntVec(intVec3, Caster.Map);
				}
				//单个目标
				if (this.ability.range == null)
				{
					Thing thing = this.currentTarget.Thing;
					if (casterPawn == null || thing == null)
					{
						return false;
					}
					foreach (AbilityEffectBase compEffect in this.ability.compEffectList)
					{
						compEffect.DoEffect_Pawn(casterPawn, thing);
					}
				}
				//多目标?
				else
				{
					List<Pawn> targets_Pawns = new List<Pawn>();
					List<IntVec3> intVec3s = new List<IntVec3>();
					IntVec3? target = null;
					if (this.currentTarget.Pawn != null)
					{
						target = this.currentTarget.Pawn.Position;
					}
					if (this.currentTarget.Cell != null)
					{
						target = this.currentTarget.Cell;
					}
					if (target == null)
					{
						intVec3s.AddRange(GenRadial.RadialCellsAround((IntVec3)target, (float)this.ability.range, true));
						foreach (IntVec3 intVec in intVec3s)
						{
							if (intVec.GetFirstPawn(casterPawn.Map) is Pawn pawn)
							{
								targets_Pawns.Add(pawn);
							}
						}
						foreach (Pawn target_pawn in targets_Pawns)
						{
							foreach (AbilityEffectBase compEffect in this.ability.compEffectList)
							{
								compEffect.DoEffect_Pawn(casterPawn, target_pawn);
							}
						}
					}
					else
					{
						Messages.Message("AK_NoTarget".Translate(), MessageTypeDefOf.CautionInput);
						return false;
					}
				}
			}
			else 
			{
				this.DoSectorAbilityEffect(casterPawn);
			}
			return true;
		}
		public override void DrawHighlight(LocalTargetInfo target)
		{
			if (this.ability.isSectorAbility)
			{
				Pawn pawn = (Pawn)this.Caster;
	            GenDraw.DrawFieldEdges(AK_Tool.GetSector(this.ability,pawn));
			}
			else 
			{
				base.DrawHighlight(target);
			}
        }
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
			needLOSToCenter = true;
			if (this.ability.range != null)
			{
				return (float)this.ability.range;
			}
			else 
			{
				return 0f;
			}
		}
		private void DoSectorAbilityEffect(Pawn pawn) 
		{
			foreach (IntVec3 curIntvec in AK_Tool.GetSector(this.ability, pawn))
			{
				if (!this.ability.compEffectList.NullOrEmpty()) 
				{
					effect.DoEffect_IntVec(curIntvec,pawn.Map,pawn);
					if (curIntvec.GetFirstPawn(pawn.Map) is Pawn target)
					{
						foreach (AbilityEffectBase effect in this.ability.compEffectList)
						{
							effect.DoEffect_Pawn(pawn, target);
						}
					}
				} 
			}
		}

		public AbilityEffectBase effect;
		public OperatorAbilityDef ability;
		public CDandCharge CDs;
		public int i;
    }
}
