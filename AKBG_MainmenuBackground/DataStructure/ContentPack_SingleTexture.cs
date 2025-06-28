using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKBG_MainmenuBackground
{
    public class TexturePathProperties : IExposable, ILoadReferenceable
    {
        [Unsaved]
        string modid;

        string path;  //也是id 同mod肯定唯一
        bool enabled = true;
        int weight = 1;

        //伪链表实现
        public TexturePathProperties prev = null;
        public TexturePathProperties next = null;

        public TexturePathProperties(string modid)
        {
            this.modid = modid;
        }

        public void ExposeData()
        {
            //Scribe_Values.Look(ref modid, "modid");
            Scribe_Values.Look(ref path, "path");
            Scribe_Values.Look(ref enabled, "enabled", true);
            Scribe_Values.Look(ref weight, "weight", 1);

            Scribe_References.Look(ref prev, "prev");
            Scribe_References.Look(ref next, "next");
        }

        public string GetUniqueLoadID()
        {
            return modid + path;
        }
    }
}
