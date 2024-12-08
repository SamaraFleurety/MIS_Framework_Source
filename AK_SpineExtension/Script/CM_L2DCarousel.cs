using AK_DLL;
using Spine38;
using Spine38.Unity;
using SpriteEvo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_SpineExtention
{
    //通用的动态立绘切换脚本
    public class CMP_L2DCarousel : CompatibleMonoBehaviourProperties
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
    public class CM_L2DCarousel : CompatibleMonoBehaviour
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
        public int IdleTimes = 0;
        public int ClickCounter = 0;

        SkeletonAnimation skeletonAnimation;
        private void ResetAllParams()
        {
            IdleTimes = 0;
            ClickCounter = 0;
        }
        public override void OnEnable()
        {
            ResetAllParams();
            skeletonAnimation ??= GetComponent<SkeletonAnimation>();
            if (skeletonAnimation == null) return;
            if (props == null) return;

            TrackEntry track0 = skeletonAnimation.AnimationState.SetAnimation(0, Idle, false);
            track0.Complete += delegate { IdleTimes++; };

            skeletonAnimation.AnimationState.Complete += delegate
            {
                if (IdleInterval != 0 && PlaySpecial && this.IdleTimes == this.IdleInterval)
                {
                    TrackEntry track1 = skeletonAnimation.AnimationState.AddAnimation(0, Special, false, 0f);
                    track1.Complete += delegate { this.IdleTimes++; };
                    return;
                }
                if (IdleInterval != 0 && this.IdleTimes == (this.IdleInterval * 2) + 1)
                {
                    TrackEntry track1;
                    if (PlayInteract)
                    {
                        track1 = skeletonAnimation.AnimationState.AddAnimation(0, Interact, false, 0f);
                    }
                    else
                    {
                        track1 = skeletonAnimation.AnimationState.AddAnimation(0, Special, false, 0f);

                    }
                    track1.Complete += delegate { IdleTimes = 0; };
                    return;
                }
                TrackEntry track2 = skeletonAnimation.AnimationState.AddAnimation(0, Idle, false, 0f);
                track2.Complete += delegate { this.IdleTimes++; };
            };
        }
        public override void Start()
        {
        }
        public override void FixedUpdate()
        {
        }
        public override void Update()
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
        public override void OnDisable()
        {
        }
        private void TryDoInteract()
        {
            if (skeletonAnimation?.AnimationState.GetCurrent(0).Animation.Name == "Interact") return;
            TrackEntry track3 = skeletonAnimation.AnimationState.SetAnimation(0, Interact, false);
            track3.Start += delegate { };
            track3.Complete += delegate { this.IdleTimes++; };
        }
    }
}
