using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKBG_MainmenuBackground
{
    public class BackgroundDef : Def
    {
        public string texturePath;

        public string modID = null; //mod package id，留空则使用def所在mod

        public bool mainmenuBG = true;  //是否算主菜单背景

        public bool loadingBG = true;   //是否算加载界面背景
    }
}
