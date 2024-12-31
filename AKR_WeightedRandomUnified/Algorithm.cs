﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random
{
    [StaticConstructorOnStartup]
    public static class Algorithm
    {
        //模式1是精确搜索，2是找第一个更大值，3是找第一个更小值
        public static int QuickSearch(int[] arr, int leftPtr, int rightPtr, int target, int mode)
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

        public static int WeightArrayRand(int[] arr)
        {
            int rd = UnityEngine.Random.Range(1, arr.Last()); //值域是[min, max]

            return QuickSearch(arr, 0, arr.Length - 1, rd, 2);
        }
    }
}