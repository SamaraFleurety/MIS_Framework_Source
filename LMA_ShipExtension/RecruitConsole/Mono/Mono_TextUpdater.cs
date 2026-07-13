using System;
using TMPro;
using UnityEngine;
using Verse;

namespace LMA_Lib
{
    //一个定时计时更新组件
    public class Mono_TextUpdater : MonoBehaviour
    {
        private const float UpdateInterval = 1f;

        private TextMeshProUGUI text;
        private Func<string> textGetter;
        private int stopTick;
        private float nextUpdateTime;

        public void Bind(Func<string> getter, int endTick)
        {
            textGetter = getter;
            stopTick = endTick;
            text = GetComponent<TextMeshProUGUI>();
            text.text = textGetter();
            if (Find.TickManager.TicksGame >= stopTick)
            {
                enabled = false;
                return;
            }

            nextUpdateTime = Time.realtimeSinceStartup + UpdateInterval;
        }

        void Update()
        {
            if (textGetter == null || Time.realtimeSinceStartup < nextUpdateTime) return;

            text.text = textGetter();
            if (Find.TickManager.TicksGame >= stopTick)
            {
                enabled = false;
                return;
            }

            nextUpdateTime = Time.realtimeSinceStartup + UpdateInterval;
        }
    }
}
