using System;
using Verse;

//tracker内共享的全局数据
//不会tick
namespace AKA_Ability.SharedData
{
    public abstract class AbilityTrackerSharedDataProperty
    {
        public Type sharedDataType;
    }

    public abstract class AbilityTrackerSharedData_Base : IExposable
    {
        public AbilityTracker tracker;

        public AbilityTrackerSharedDataProperty props;
        //存读档的时候会调用这个
        protected AbilityTrackerSharedData_Base(AbilityTracker tracker)
        {
            this.tracker = tracker;
        }

        //仅初始化的时候会加载一次
        protected AbilityTrackerSharedData_Base(AbilityTracker tracker, AbilityTrackerSharedDataProperty props) : this(tracker)
        {
            this.props = props;
        }

        public virtual void ExposeData()
        {
        }
    }
}
