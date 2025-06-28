using AK_DLL;
using Spine38;
using Spine38.Unity;
using SpriteEvo;
using System.Linq;
using UnityEngine;
using Verse;

namespace AK_SpineExtention
{
    //通用的动态立绘切换脚本
    public class CMP_L2DCarousel : ScriptProperties
    {
        public string Idle = "Idle";
        public string Interact = "Interact";
        public string Special = "Special";
        public bool PlayInteract = false;
        public bool PlaySpecial = true;
        public int IdleInterval = 2;
        public CMP_L2DCarousel()
        {
            scriptClass = typeof(CM_L2DCarousel);
        }
    }

    public class CM_L2DCarousel : ControllerBase<SkeletonGraphic>
    {
        #region Inspector
        private CMP_L2DCarousel Props => props as CMP_L2DCarousel;

        public string Idle => Props.Idle;
        public string Interact => Props.Interact;
        public string Special => Props.Special;
        public bool PlayInteract => Props.PlayInteract;
        public bool PlaySpecial => Props.PlaySpecial;
        public int IdleInterval => Props.IdleInterval;

        #endregion
        private int completeTimes = 0;
        private int ClickCounter = 0;

        protected override void OnEnable()
        {
            ResetAllParams();
            if (SkeletonInstanceInt == null || props == null) return;

            TrackEntry track0 = SkeletonInstanceInt.AnimationState.SetAnimation(0, Idle, false);
            track0.Complete += CompleteTimeCounter;
            track0.Complete += CompleteEventHandler;
            //skeletonAnimation.AnimationState.Complete += CompleteEventHandler; //这BYD会连续调用2次
        }

        protected override void Update()
        {
            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count == 0) return;
            Pawn p = Find.Selector.SelectedPawns.First();
            if (p == null) return;
            OperatorDocument doc = AK_Tool.GetDoc(p);
            if (doc == null) return;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ClickCounter++;
            }
            if (ClickCounter >= 2)
            {
                TryDoInteract();
                ClickCounter = 0;
            }
        }

        private void CompleteTimeCounter(TrackEntry trackEntry) => completeTimes++;
        private void ResetCompleteTime(TrackEntry trackEntry) => completeTimes = 0;
        private void ResetAllParams() { completeTimes = 0; ClickCounter = 0; }

        //改成单独回调了 简单好用
        private void CompleteEventHandler(TrackEntry trackEntry)
        {
            if (PlaySpecial && IdleInterval != 0 && completeTimes == IdleInterval)
            {
                TrackEntry track1 = SkeletonInstanceInt.AnimationState.AddAnimation(0, Special, false, 0f);
                track1.Complete += CompleteTimeCounter;
                track1.Complete += CompleteEventHandler;
                return;
            }
            if (IdleInterval != 0 && (completeTimes == (2 * IdleInterval) + 1))
            {
                TrackEntry track1;
                if (PlayInteract)
                {
                    track1 = SkeletonInstanceInt.AnimationState.AddAnimation(0, Interact, false, 0f);
                }
                else
                {
                    track1 = SkeletonInstanceInt.AnimationState.AddAnimation(0, Special, false, 0f);
                }
                track1.Complete += ResetCompleteTime;
                track1.Complete += CompleteEventHandler;
                return;
            }
            TrackEntry track2 = SkeletonInstanceInt.AnimationState.AddAnimation(0, Idle, false, 0f);
            track2.Complete += CompleteTimeCounter;
            track2.Complete += CompleteEventHandler;
        }

        private void TryDoInteract()
        {
            if (SkeletonInstanceInt?.AnimationState.GetCurrent(0).Animation.Name == "Interact") return;
            TrackEntry track3 = SkeletonInstanceInt.AnimationState.SetAnimation(0, Interact, false);
            //track3.Start += delegate { };
            track3.Complete += CompleteTimeCounter;
            track3.Complete += CompleteEventHandler;
        }
    }
}