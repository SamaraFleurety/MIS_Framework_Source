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
            //Log.Message($"Pawn: {this.Pawn.Name}\n hairColor: {this.Props.hairColor}\n skincolor: {this.Props.skinColor}\n Props == props ? : {Props == props}");
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
                //Log.Message((this.Def.comps.Contains(this.props)).ToString());
                this.Def.comps.Remove(this.Props);
            }
        }
    }
}
