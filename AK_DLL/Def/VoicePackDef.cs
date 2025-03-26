using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class VoicePackDef : Def
    {
        public SoundDef recruitSound;
        public List<SoundDef> draftSounds;
        public SoundDef undraftSound;
        public SoundDef diedSound;
        public List<SoundDef> selectSounds;
        public List<SoundDef> abilitySounds;

        //构造函数
        public VoicePackDef()
        {
            this.selectSounds = new List<SoundDef>();
            this.draftSounds = new List<SoundDef>();
            this.abilitySounds = new List<SoundDef>();
        }
        public VoicePackDef(string prefix, string operatorID) : this()
        {
            this.defName = AK_Tool.GetThingdefNameFrom(operatorID, prefix, "VoicePack");

            AutoFillVoicePack(prefix, operatorID);
            if (DefDatabase<VoicePackDef>.AllDefsListForReading.Contains(this) == false) DefDatabase<VoicePackDef>.Add(this);
        }

        public void AutoFillVoicePack(string prefix, string operatorName)
        {
            AutoFillSelectSounds(prefix, operatorName);

            AutoFillAbilitySounds(prefix, operatorName);

            AutoFillDiedSound(prefix, operatorName);

            AutoFillDraftSounds(prefix, operatorName);

            AutoFillUndraftSound(prefix, operatorName);

            AutoFillRecruitSound(prefix, operatorName);
        }

        private void AutoFillSelectSounds(string prefix, string operatorName)
        {
            string defNameBase = AK_Tool.GetThingdefNameFrom(operatorName, prefix, "Voice_Select");
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail(defNameBase);
            if (temp != null && this.selectSounds.Contains(temp) == false) this.selectSounds.Add(temp);
            for (int i = 0; i <= 10 && (temp = DefDatabase<SoundDef>.GetNamedSilentFail(defNameBase + TypeDef.romanNumber[i])) != null; ++i)
            {
                if (this.selectSounds.Contains(temp) == false)
                    this.selectSounds.Add(temp);
            }
            if (this.selectSounds.Count == 0) this.selectSounds.Add(DefDatabase<SoundDef>.GetNamed("AK_Voice_Select_Default"));
        }

        private void AutoFillDraftSounds (string prefix, string operatorName)
        {
            string defNameBase = AK_Tool.GetThingdefNameFrom(operatorName, prefix, "Voice_Draft");
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail(defNameBase);
            if (temp != null && this.draftSounds.Contains(temp) == false) this.draftSounds.Add(temp);
            for (int i = 0; i < 10 && (temp = DefDatabase<SoundDef>.GetNamedSilentFail(defNameBase + TypeDef.romanNumber[i])) != null; ++i)
            {
                if (this.draftSounds.Contains(temp) == false)
                    this.draftSounds.Add(temp);
            }
            if (this.draftSounds.Count == 0) this.draftSounds.Add(DefDatabase<SoundDef>.GetNamed("AK_Voice_Draft_Default"));
        }

        private void AutoFillAbilitySounds(string prefix, string operatorName)
        {
            string defNameBase = AK_Tool.GetThingdefNameFrom(operatorName, prefix, "Voice_Ability");
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail(defNameBase);
            if (temp != null && this.abilitySounds.Contains(temp) == false) this.abilitySounds.Add(temp);
            for (int i = 0; i <= 10 && (temp = DefDatabase<SoundDef>.GetNamedSilentFail(defNameBase + TypeDef.romanNumber[i])) != null; ++i)
            {
                if (this.abilitySounds.Contains(temp) == false)
                    this.abilitySounds.Add(temp);
            }
            if (this.abilitySounds.Count == 0) this.abilitySounds.Add(DefDatabase<SoundDef>.GetNamed("AK_Voice_Ability_Default"));
        }
        private void AutoFillUndraftSound(string prefix, string operatorName)
        {
            if (this.undraftSound != null) return;
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(operatorName, prefix, "Voice_Undraft"));
            this.undraftSound = temp != null ? temp : this.draftSounds.RandomElement();
        }

        private void AutoFillRecruitSound(string prefix, string operatorName)
        {
            if (this.recruitSound != null) return;
            this.recruitSound = DefDatabase<SoundDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(operatorName, prefix, "Voice_Recruit"));
            if (this.recruitSound == null) this.recruitSound = DefDatabase<SoundDef>.GetNamed("AK_Voice_Recruit_Default");
        }

        private void AutoFillDiedSound   (string prefix, string operatorName)
        {
            if (this.diedSound != null) return;
            this.diedSound = DefDatabase<SoundDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(operatorName, prefix, "Voice_Die"));
            if (this.diedSound == null) this.diedSound = DefDatabase<SoundDef>.GetNamed("AK_Voice_Die_Default");
        }
    }
}
