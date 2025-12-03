using System;

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

    //现实中的系统时间
    //这里面的tick不再是环世界的tick，而是指windows的tick。1tick=0.1纳秒
    public static class RealTimeDirect
    {
        public static long CurrentTimeTick => DateTime.Now.Ticks;

        public static long tick = 1;
        public static long second = 10000000;  //多少tick算1秒
    }
}
