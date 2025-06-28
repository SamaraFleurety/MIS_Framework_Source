using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static AKBG_MainmenuBackground.BackgroundMod_Tableview_Setting;

namespace AKBG_MainmenuBackground
{
    public class ModContentPack_BG : IExposable  //写到这玩意存读档
    {
        string modid;

        Dictionary<string, TexturePathProperties> allBGs = new();
        //为可调顺序播放做的优化
        TexturePathProperties firstBG = null;
        TexturePathProperties lastBG = null;

        public ModContentPack_BG(string modid)
        {
            this.modid = modid;
        }

        public bool BGIDExist(string BGUniqueID)
        {
            return allBGs.ContainsKey(BGUniqueID);
        }

        //不是常见的存读档流程。如果不熟悉此机制勿改。
        List<TexturePathProperties> allBGListForSave = new();
        void ExposeData_PreSave()
        {
            allBGListForSave = new();
            foreach (var innerPath in allBGs.Keys)
            {
                allBGListForSave.Add(allBGs[innerPath]);
            }

        }

        void ExposeData_PostLoad()
        {
            foreach (var texProp in allBGListForSave)
            {
                if (!allBGs.ContainsKey(texProp.GetUniqueLoadID())) allBGs.Add(texProp.GetUniqueLoadID(), texProp);
            }
        }

        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving) ExposeData_PreSave();
            Scribe_Values.Look(ref modid, "modid");
            Scribe_Collections.Look(ref allBGListForSave, "allBG", LookMode.Deep, modid);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) ExposeData_PostLoad();
        }

        public void InsertAtTail(TexturePathProperties prop)
        {
            if (BGIDExist(prop.GetUniqueLoadID())) return;

            allBGs.Add(prop.GetUniqueLoadID(), prop);
            if (firstBG == null)  //显然不可能first和last仅其一是null
            {
                firstBG = lastBG = prop;
                return;
            }
            else
            {
                var lastNode_Old = lastBG;
                lastBG.next = prop;
                prop.prev = lastNode_Old;
            }
        }

        public void RemoveNode(string id)
        {
            if (!BGIDExist(id)) return;

            TexturePathProperties nodeToRemove = allBGs[id];

            //仅有这一个
            if (firstBG == lastBG && firstBG == nodeToRemove)
            {
                firstBG = lastBG = null;
            }
            else if (firstBG == nodeToRemove) //这是第一个
            {
                var next = nodeToRemove.next;
                firstBG = next;
                next.prev = null;
            }
            else if (lastBG == nodeToRemove) //芝士最后一个
            {
                var prev = nodeToRemove.prev;
                lastBG = prev;
                prev.next = null;
            }
            else  //两面包夹芝士
            {
                var next = nodeToRemove.next;
                var prev = nodeToRemove.prev;
                prev.next = next;
                next.prev = prev;
            }
            allBGs.Remove(id);
        }

        public void SwapNode(TexturePathProperties nodeA, TexturePathProperties nodeB)
        {
            TexturePathProperties temp;
            temp = nodeA.prev;
            nodeA.prev = nodeB.prev;
            nodeB.prev = temp;

            temp = nodeB.next;
            nodeB.next = nodeA.next;
            nodeA.next = temp;
        }
    }
}
