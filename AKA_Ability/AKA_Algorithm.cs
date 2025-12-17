using System;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    public static class AKA_Algorithm
    {
        //模式1是精确搜索，2是找第一个更大值，3是找第一个更小值
        public static int quickSearch(int[] arr, int leftPtr, int rightPtr, int target, int mode)
        {
            int middle;
            middle = 0;
            if (rightPtr - leftPtr < 0) return -1;
            if (target > arr[rightPtr])
            {
                if (mode == 3) return rightPtr;
                else return -1;
            }
            else if (target < arr[leftPtr])
            {
                if (mode == 2) return leftPtr;
                else return -1;
            }
            while (rightPtr >= leftPtr)
            {
                middle = (leftPtr + rightPtr) / 2;
                if (arr[middle] == target)
                {
                    return middle;
                }
                else if (leftPtr == rightPtr) break;
                else if (arr[middle] < target)
                {
                    leftPtr = middle + 1;
                }
                else
                {
                    rightPtr = middle;
                }
            }
            if (mode == 2) return rightPtr;
            else if (mode == 3) return rightPtr - 1;
            else return -1;
        }

        // 生成标准正态分布的随机数（均值为0，标准差为1）
        public static double NextGaussian(double mean = 0, double stdDev = 1)
        {
            System.Random rd = new ();
            // 使用Box-Muller变换
            double u1 = 1.0 - rd.NextDouble(); // 均匀分布随机数 (0,1]
            double u2 = 1.0 - rd.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            return mean + stdDev * randStdNormal;
        }

        //关于圆心center和直径diameter，获得一个基本上在圆内的服从正太分布的点
        public static Vector3 GenerateNormalDistributionPointsAboutCircle(Vector3 center, float diameter)
        {
            // 标准差设置为直径的1/6（ai说约99.7%的点在直径范围内）
            float stdDev = diameter / 6.0f;

            float xOffset = (float)NextGaussian(0, stdDev);
            float zOffset = (float)NextGaussian(0, stdDev);

            return new Vector3(center.x + xOffset, 0, center.z + zOffset);
        }
    }
}