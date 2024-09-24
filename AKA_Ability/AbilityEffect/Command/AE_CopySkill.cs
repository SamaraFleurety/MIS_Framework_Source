using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace AKA_Ability
{
    public class AE_CopySkill : AbilityEffectBase
    {
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            Pawn targetPawn = target.Pawn;
            if (targetPawn == null|| !targetPawn.RaceProps.Humanlike)
            {
                return false;
            }
            Find.WindowStack.Add(new Dialog_ChoseSkill(targetPawn, caster.CasterPawn));
            return base.DoEffect(caster, target);
        }

        /*public override void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {
            if (target == null || !(target is Pawn t) || !t.RaceProps.Humanlike)
            {
                return;
            }
            Find.WindowStack.Add(new Dialog_ChoseSkill(t, user));
        }*/
    }
    public class Dialog_ChoseSkill : Window
    {
        private readonly Pawn Target;
        private readonly Pawn User;
        private readonly int width = 620;
        private SkillRecord ChoosenSkill;
        public override Vector2 InitialSize => new Vector2(620f, 420f);
        public Dialog_ChoseSkill(Pawn target, Pawn user)
        {
            forcePause = true;
            preventSave = true;
            doCloseX = true;
            doCloseButton = false;
            closeOnClickedOutside = false;
            closeOnCancel = false;
            doWindowBackground = false;
            drawShadow = false;
            this.Target = target;
            this.User = user;
        }
        public override void DoWindowContents(Rect inRect)
        {
            Rect rect = new Rect(0f, 0f, width, inRect.height).Rounded();
            Widgets.DrawWindowBackground(rect);
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;
            Rect rectLeftTopLabel = new Rect(rect.x + 20f, rect.y + 10f, rect.width / 2, 20f);
            Widgets.Label(rectLeftTopLabel, "AKA.CopySkillTextTarget".Translate() + Target.Label);
            Text.Font = GameFont.Small;
            Rect rectRightTopLabel = new Rect(rect.x + 250f, rect.y + 10f, rect.width / 2, 20f);
            Widgets.Label(rectRightTopLabel, "AKA.CopySkillTextUser".Translate() + User.Label);
            Rect rect1 = new Rect(rect.x + 20f, rect.y + 40f, rect.width / 2, rect.height);
            Rect rect2 = new Rect(rect.x + 250f, rect.y + 40f, rect.width / 2, rect.height);
            Widgets.BeginGroup(rect1);
            SkillUI.DrawSkillsOf(Target, offset: Vector2.zero, mode: (Current.ProgramState != ProgramState.Playing) ? SkillUI.SkillDrawMode.Menu : SkillUI.SkillDrawMode.Gameplay, container: rect1);
            Widgets.EndGroup();
            Widgets.BeginGroup(rect2);
            SkillUI.DrawSkillsOf(User, offset: Vector2.zero, mode: (Current.ProgramState != ProgramState.Playing) ? SkillUI.SkillDrawMode.Menu : SkillUI.SkillDrawMode.Gameplay, container: rect2);
            Widgets.EndGroup();
            Rect rectButton = new Rect(rect.x + 500f, rect.y + 38f, 80f, 25f);
            List<SkillRecord> TargetSkills = Target.skills.skills;
            for (int i = 0; i < TargetSkills.Count; i++)
            {
                Rect Button = new Rect(rectButton.x, rectButton.y + 27.2f * i, rectButton.width, rectButton.height);
                if (Widgets.ButtonText(Button, "AKA.CopySkillSelect".Translate()))
                {
                    ChoosenSkill = TargetSkills[i];
                    Close();
                }
            }
        }
        public override void PostClose()
        {
            if (ChoosenSkill != null)
            {
                int index = User.skills.skills.FindIndex(skill => skill.def == ChoosenSkill.def);
                User.skills.skills[index] = ChoosenSkill;
                Messages.Message(User.Label + "AKA.CopySkillMessage".Translate() + ChoosenSkill.def.label, MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
