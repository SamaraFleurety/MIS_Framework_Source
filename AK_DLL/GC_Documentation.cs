using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace AK_DLL
{
    /*public static class Operator_Recruited
    {
        public static List<OperatorDef> RecruitedOperators = new List<OperatorDef>();
    }*/

    //下面这个GameComponent使用了和原版不一样的存读档流程。除非真的知道你在干什么，不然别改。
    public class OperatorDocument : IExposable , ILoadReferenceable
    {
        internal string defName;
        public bool currentExist;
        public Pawn pawn;
        public Thing weapon;
        public Hediff_Operator hediff;
        public Thing apparel; 
        public Dictionary<SkillDef, int> skillLevel;
        public VoicePackDef voicePack;
        public List<CompProperties_Ability> groupedAbilities;
        public int preferedAbility = 0;
        public OperatorDef operatorDef;

        public OperatorDocument(string defName, Pawn p, Thing weapon, OperatorDef operatorDef) : this()
        {
            this.defName = defName;
            this.currentExist = true;
            this.pawn = p;
            this.weapon = weapon;
            this.hediff = p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator")) as Hediff_Operator;
            this.RecordSkills();
        }
        public OperatorDocument() {
            this.skillLevel = new Dictionary<SkillDef, int>();
            groupedAbilities = new List<CompProperties_Ability>();
        }

        public string GetUniqueLoadID()
        {
            return (this.defName + "Doc");
        }

        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                this.RecordSkills();
            }
            Scribe_Defs.Look(ref this.voicePack, "voicePackDef");
            Scribe_Values.Look(ref this.defName, "defName");
            Scribe_Values.Look(ref this.currentExist, "alive");
            Scribe_References.Look<Pawn>(ref this.pawn, "operator", true);
            Scribe_References.Look(ref this.weapon, "weapon", true);
            Scribe_Collections.Look(ref this.skillLevel, "skill", LookMode.Def, LookMode.Value);
            Scribe_Defs.Look(ref this.operatorDef, "def");
            //if (this.pawn == null) { return; }
        }

        //记录当前干员所有技能
        public void RecordSkills()
        {
            if (this.pawn == null || this.pawn.skills == null) return;
            foreach(SkillRecord i in this.pawn.skills.skills)
            {
                if (this.skillLevel.ContainsKey(i.def) == false)
                {
                    this.skillLevel.Add(i.def, i.levelInt);
                }
                else
                {
                    this.skillLevel[i.def] = Math.Max(this.skillLevel[i.def], i.levelInt);
                }
            }
        }

        public void Tick()
        {
            if (!this.currentExist) return;
        }

    }

    public class GameComp_OperatorDocumentation : GameComponent
    {
        public static Dictionary<string, OperatorDocument> operatorDocument;

        public GameComp_OperatorDocumentation(Game game)
        {
            operatorDocument = new Dictionary<string, OperatorDocument>();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter(Translator.Translate("AK_StartLabel"), Translator.Translate("AK_StartDesc"), LetterDefOf.NeutralEvent, null, null));
            if (operatorDocument == null) operatorDocument = new Dictionary<string, OperatorDocument>();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            AK_Tool.LoadedGame();
            if (operatorDocument == null) operatorDocument = new Dictionary<string, OperatorDocument>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //GC是先于entity加载的，所以在这里去指pawn一定是个空指针。去指丢进下面那个函数去做了。
            if (Scribe.mode != LoadSaveMode.ResolvingCrossRefs)
            {
                List<string> key = new List<string>();
                List<OperatorDocument> value = new List<OperatorDocument>();
                try
                {
                    Scribe_Collections.Look(ref operatorDocument, "operatorDocument", LookMode.Value, LookMode.Deep, ref key, ref value);
                }
                catch { Log.Error("没保存起"); }
            }
        }

        //会在加载流程很后面加载。执行pawn的去指。
        public override void FinalizeInit()
        {
            List<string> key = new List<string>();
            List<OperatorDocument> value = new List<OperatorDocument>();
            Scribe.mode = LoadSaveMode.ResolvingCrossRefs;
            try
            {
                Scribe_Collections.Look(ref operatorDocument, "operatorDocument", LookMode.Value, LookMode.Deep, ref key, ref value);
            }
            catch
            {
                Log.Error("没保存起");
            }           
            Scribe.mode = 0;
            foreach (KeyValuePair<string, OperatorDocument> node in operatorDocument)
            {
                node.Value.RecordSkills();
                Log.Message($"当前已招募 {node.Value.defName}");
            }
        }

        public static void AddPawn(string defName, OperatorDef operatorDef , Pawn pawn, Thing weapon)
        {
            if (operatorDocument.ContainsKey(defName) == false)
            {
                operatorDocument.Add(defName, new OperatorDocument(defName, pawn, weapon, operatorDef));
            }
            else
            {
                destroyHeritage(operatorDocument[defName]);
                ReRecruit(operatorDocument[defName] , defName, pawn, weapon);
            }
        }

        public override void GameComponentTick()
        {
            //base.GameComponentTick(); 感觉没用
            /*foreach (KeyValuePair<string, OperatorDocument> node in operatorDocument)
            {
                node.Value.Tick();
            }*/
        }

        public static void destroyHeritage (OperatorDocument doc)
        {
            doc.currentExist = false;
            doc.groupedAbilities.Clear();
            doc.preferedAbility = 0;
            if (doc.pawn != null) 
            { 
                if (doc.pawn.Destroyed == false) doc.pawn.Destroy();
                doc.pawn.Discard();
                if (Find.WorldPawns.Contains(doc.pawn)) Find.WorldPawns.RemoveAndDiscardPawnViaGC(doc.pawn);
            }
            if (doc.weapon != null) doc.weapon.Destroy();
            //if (doc.hediff != null) Log.Error("人都没了但hediff还存在,请提交此bug至制作组FS");
        }

        public static void ReRecruit (OperatorDocument doc, string defName, Pawn p, Thing weapon)
        {
            doc.pawn = p;
            doc.weapon = weapon;
            doc.hediff = p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator")) as Hediff_Operator;
            doc.currentExist = true;
        }
    }
}