using System;
using TMPro;
using UnityEngine;

namespace LMA_Lib
{
    //一个定时计时更新组件
    public class Mono_TextUpdater : MonoBehaviour
    {
        private const float UpdateInterval = 1f;

        private TextMeshProUGUI text;
        private Func<string> textGetter;
        private DateTime stopTime;
        private float nextUpdateTime;

        public void Bind(Func<string> getter, DateTime endTime)
        {
            textGetter = getter;
            stopTime = endTime;
            text = GetComponent<TextMeshProUGUI>();
            text.text = textGetter();
            if (DateTime.Now >= stopTime)
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
            if (DateTime.Now >= stopTime)
            {
                enabled = false;
                return;
            }

            nextUpdateTime = Time.realtimeSinceStartup + UpdateInterval;
        }
    }
}
