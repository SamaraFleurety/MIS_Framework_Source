using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using AlienRace;

namespace AK_DLL
{
    public class HC_ForceColors : HediffComp
    {
        public HCP_ForceColors Props => (HCP_ForceColors)this.props;

        public override string CompLabelInBracketsExtra
        {
            get
            {
                return "forcing colors";
            }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (this.Pawn.story.HairColor != this.Props.hairColor)
            {
                this.Pawn.style.nextHairColor = this.Props.hairColor;
                this.Pawn.style.FinalizeHairColor();
            }
            else if (this.Pawn.story.SkinColor != this.Props.skinColor)
            {
                AlienRace.AlienPartGenerator.AlienComp alienComp = this.Pawn.TryGetComp<AlienRace.AlienPartGenerator.AlienComp>();
                alienComp.OverwriteColorChannel(channel: "skin", this.Props.skinColor, this.Props.skinColor);
                this.Pawn.story.SkinColorBase = this.Props.skinColor;
            }
            else
            {
                this.parent.comps.Remove(this);
            }
        }
    }
}
