using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public enum TimeToTick
    {
        tick = 1,          //游戏中
        tickRare = 250,
        tickLong = 2000,
        hour = 2500,
        day = hour * 24,   //60k
        season = day * 15, //0.9M
        year = season * 4, //3.6M
        rSecond = 60       //现实中的1秒
    }

    public static class TimeToTickDirect
    {
        public static int tick = 1;          //游戏中
        public static int tickRare = 250;
        public static int tickLong = 2000;
        public static int hour = 2500;
        public static int day = hour * 24;   //60k
        public static int season = day * 15; //0.9M
        public static int year = season * 4; //3.6M
        public static int rSecond = 60;       //现实中的1秒
    }
}
