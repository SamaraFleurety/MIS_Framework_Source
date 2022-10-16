using System;
using RimWorld;
using Verse;
using Verse.Sound;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

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
        public VoicePackDef(string operatorName) : this()
        {
            this.defName = "AK_VoicePack_" + operatorName;

            AutoFillVoicePack(operatorName);
            if (DefDatabase<VoicePackDef>.AllDefsListForReading.Contains(this) == false) DefDatabase<VoicePackDef>.Add(this);
        }

        public void AutoFillVoicePack(string operatorName)
        {
            AutoFillSelectSounds(operatorName);

            AutoFillAbilitySounds(operatorName);

            AutoFillDiedSound(operatorName);

            AutoFillDraftSounds(operatorName);

            AutoFillUndraftSound(operatorName);

            AutoFillRecruitSound(operatorName);
        }

        private void AutoFillSelectSounds(string operatorName)
        {
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Select_{operatorName}");
            if (temp != null && this.selectSounds.Contains(temp) == false) this.selectSounds.Add(temp);
            for (int i = 0; i <= 10 && (temp = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Select_{operatorName}" + AK_Tool.romanNumber[i])) != null; ++i)
            {
                if (this.selectSounds.Contains(temp) == false)
                    this.selectSounds.Add(temp);
            }
            if (this.selectSounds.Count == 0) this.selectSounds.Add(DefDatabase<SoundDef>.GetNamed("AK_Voice_Select_Default"));
        }

        private void AutoFillDraftSounds (string operatorName)
        {
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Draft_{operatorName}");
            if (temp != null && this.draftSounds.Contains(temp) == false) this.draftSounds.Add(temp);
            for (int i = 0; i < 10 && (temp = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Draft_{operatorName}" + AK_Tool.romanNumber[i])) != null; ++i)
            {
                if (this.draftSounds.Contains(temp) == false)
                    this.draftSounds.Add(temp);
            }
            if (this.draftSounds.Count == 0) this.draftSounds.Add(DefDatabase<SoundDef>.GetNamed("AK_Voice_Draft_Default"));
        }

        private void AutoFillAbilitySounds(string operatorName)
        {
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Ability_{operatorName}");
            if (temp != null && this.abilitySounds.Contains(temp) == false) this.abilitySounds.Add(temp);
            for (int i = 0; i <= 10 && (temp = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Ability_{operatorName}" + AK_Tool.romanNumber[i])) != null; ++i)
            {
                if (this.abilitySounds.Contains(temp) == false)
                    this.abilitySounds.Add(temp);
            }
            if (this.abilitySounds.Count == 0) this.abilitySounds.Add(DefDatabase<SoundDef>.GetNamed("AK_Voice_Select_Default"));
            //if (this.abilitySounds.Count == 0) this.abilitySounds.Add(DefDatabase<SoundDef>.GetNamed("AK_Voice_Ability_Default"));
        }
        private void AutoFillUndraftSound(string operatorName)
        {
            if (this.undraftSound != null) return;
            SoundDef temp = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Undraft_{operatorName}");
            this.undraftSound = temp != null ? temp : this.draftSounds.RandomElement();
        }

        private void AutoFillRecruitSound(string operatorName)
        {
            if (this.recruitSound != null) return;
            this.recruitSound = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Recruit_{operatorName}");
            if (this.recruitSound == null) this.recruitSound = DefDatabase<SoundDef>.GetNamed("AK_Voice_Recruit_Default");
        }

        private void AutoFillDiedSound   (string operatorName)
        {
            if (this.diedSound != null) return;
            this.diedSound = DefDatabase<SoundDef>.GetNamedSilentFail($"AK_Voice_Die_{operatorName}");
            if (this.diedSound == null) this.diedSound = DefDatabase<SoundDef>.GetNamed("AK_Voice_Die_Default");
        }

        /*public override string ToString()
        {
            string s = "";
            s += this.defName + "\n";
            s += this.undraftSound + "\n";
            return s;
        }*/
    }
}
