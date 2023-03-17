using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace AK_DLL
{
    public class RIWindow_MainMenu : Dialog_NodeTree
    {
        private static readonly int maxX = 1920, maxY = 1080;
        private static readonly int btnHeight = 20;
        private static readonly int btnWidth = 80;
        private static readonly int xMargin = 10;
        private static readonly int yMargin = 10;
        private static readonly int classSideLength = 70;
        private static readonly int classMargin;
        public OperatorDef secretaryDef
        {
            get { return AK_Tool.GetDef(AK_ModSettings.secretary); }
        }
        public RIWindow_MainMenu(DiaNode startNode, bool radioMode) : base(startNode, radioMode, false, null)
        {
        }
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1920f, 1080f);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            float crntX = inRect.x + xMargin;
            float crntY = inRect.y + yMargin;
            //右边的助理
            Rect secretaryRect = new Rect(inRect.xMax - 700, inRect.y, 700, inRect.yMax - inRect.yMin);
            Widgets.DrawTextureFitted(secretaryRect, ContentFinder<Texture2D>.Get(secretaryDef.stand), secretaryDef.standRatio);
            if (Widgets.ButtonText(new Rect(inRect.xMax - 300, inRect.yMax - 300, 50, 50), "更换秘书".Translate()))
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindow.MainMenu, false);
            }

            Rect rect_Back = new Rect(inRect.xMax - xMargin - classSideLength, inRect.y + yMargin, classSideLength, classSideLength);
            //退出按钮
            if (Widgets.ButtonText(rect_Back, "AK_Escape".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
            {
                this.Close();
            }



            Widgets.Label(new Rect(inRect.xMax / 2 - 50, inRect.yMin, 100, 50), "假装prts logo");
            
            //左边的主要功能
            Rect FeatureBtn = new Rect(inRect.x, inRect.y + 80, 400, 500);
            if (Widgets.ButtonText(FeatureBtn, "招募图标")) 
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindow.Series);
            }
            FeatureBtn.x += 420;
            if (Widgets.ButtonText(FeatureBtn, "其他图标"))
            {

            }
            FeatureBtn.x += 420;
            if (Widgets.ButtonText(FeatureBtn, "其他图标"))
            {

            }
            FeatureBtn.x -= 840;
            FeatureBtn.y += 520;
            if (Widgets.ButtonText(FeatureBtn, "其他图标"))
            {

            }
            FeatureBtn.x += 420;
            if (Widgets.ButtonText(FeatureBtn, "其他图标"))
            {

            }
            FeatureBtn.x += 420;
            if (Widgets.ButtonText(FeatureBtn, "其他图标"))
            {

            }
            //选择助理
            if (false)
            {

            }
        }
    }
}
