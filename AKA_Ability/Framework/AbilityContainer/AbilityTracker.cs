﻿using AKA_Ability.SharedData;
using System;
using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    //不能保证一个pawn只有一个tracker。
    public class AbilityTracker : IExposable
    {
        public Pawn owner;

        public int indexActiveGroupedAbility = -1;

        public List<AKAbility_Base> innateAbilities = new List<AKAbility_Base>();

        public List<AKAbility_Base> groupedAbilities = new List<AKAbility_Base>();

        //fixme:没做完
        public AKAbility_Base barDisplayedAbility = null;   //舟味ui显示技能指示时 有多个技能则仅显示此技能。不允许是未被选中的分组技能。

        //召唤技能列表，是技能的子集。存在这个是因为有时候需要调用召唤物
        public List<AKAbility_Summon> summonAbilities = new();

        public bool shouldRefreshActiveStatus = true;

        //对于使用共享数据(比如cd)这里面的才是真实数据
        public AbilityTrackerSharedData_Base sharedData = null;
        public AKAbility_Base SelectedGroupedAbility
        {
            get
            {
                if (indexActiveGroupedAbility < 0 || groupedAbilities.Count <= indexActiveGroupedAbility) return null;
                return groupedAbilities[indexActiveGroupedAbility];
            }
        }

        //用这个构造器需要手动绑定owner
        public AbilityTracker()
        {
        }

        public AbilityTracker(Pawn p) : this()
        {
            owner = p;
        }

        public void Tick()
        {
            if (owner == null || !owner.IsColonist) return;
            foreach (AKAbility_Base i in innateAbilities)
            {
                i.Tick();
            }

            if (indexActiveGroupedAbility > -1 && groupedAbilities.Count > indexActiveGroupedAbility) groupedAbilities[indexActiveGroupedAbility].Tick();
        }

        public IEnumerable<Command> GetGizmos()
        {
            if (owner == null) yield break;

            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count > 1) yield break;

            Command c;
            //固有的 不取决于分组的技能会一直显示
            foreach (AKAbility_Base i in innateAbilities)
            {
                foreach (Command c1 in i.GetGizmos())
                {
                    yield return c1;
                }
                /*c = i.GetGizmos();
                if (c != null) yield return c;*/
            }
            //分组技能 仅显示最多1个
            if (indexActiveGroupedAbility != -1 && groupedAbilities.Count > 0)
            {
                foreach (Command c2 in groupedAbilities[indexActiveGroupedAbility].GetGizmos())
                {
                    yield return c2;
                }
                /*c = groupedAbilities[indexActiveGroupedAbility].GetGizmos();
                if (c != null) yield return c;*/
            }
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "[AKA]立刻回复SP",
                    defaultDesc = "desc",
                    icon = BaseContent.BadTex,
                    action = delegate ()
                    {
                        if (SelectedGroupedAbility != null) SelectedGroupedAbility.cooldown.charge = SelectedGroupedAbility.cooldown.MaxCharge;
                        foreach (AKAbility_Base ab in innateAbilities)
                        {
                            ab.cooldown.charge = ab.cooldown.MaxCharge;
                        }
                    }
                };
            }
        }

        public virtual AKAbility_Base AddAbility(AKAbilityDef def)
        {
            AKAbility_Base ability = (AKAbility_Base)Activator.CreateInstance(def.abilityClass, def, this);

            List<AKAbility_Base> allAbilities = this.innateAbilities;
            if (def.grouped)
            {
                allAbilities = this.groupedAbilities;
                this.indexActiveGroupedAbility = 0;
            }

            if (ability is AKAbility_Summon ab_summon)
            {
                summonAbilities.Add(ab_summon);
            }

            //fixme:临时的 以后优化
            for (int i = 0; i < allAbilities.Count; ++i)
            {
                AKAbility_Base ab = allAbilities[i];
                if (ab.def.sortOrder >= def.sortOrder)
                {
                    allAbilities.Insert(i, ability);
                    return ability;
                }
            }

            allAbilities.Add(ability);

            /*if (def.grouped)
            {
                //this.groupedAbilities.Add(ability);
            }*/
            //else this.innateAbilities.Add(ability);



            return ability;
        }

        //技能成功释放后调用 现在用来播放干员音效
        public virtual void Notify_AbilityCasted(AKAbility_Base ability)
        {

        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref indexActiveGroupedAbility, "indexAGA");

            Scribe_Collections.Look(ref innateAbilities, "iA", LookMode.Deep, this);
            Scribe_Collections.Look(ref groupedAbilities, "gA", LookMode.Deep, this);

            Scribe_References.Look(ref barDisplayedAbility, "barA");
            Scribe_Collections.Look(ref summonAbilities, "summonA", LookMode.Reference);

            Scribe_Deep.Look(ref sharedData, "sharedData", this);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (AKAbility_Base i in innateAbilities) i.container = this;
                foreach (AKAbility_Base j in groupedAbilities) j.container = this;

                if (owner != null) SpawnSetup();
            }
        }

        //技能被装载上了pawn调用这个
        public virtual void SpawnSetup()
        {
            foreach (AKAbility_Base ia in innateAbilities)
            {
                ia.SpawnSetup();
            }
            SelectedGroupedAbility?.SpawnSetup();
        }

        //技能不再被绑定调用这个 va永远不被卸载，但是tcp上面的有可能会
        public virtual void PostDespawn()
        {
            foreach (AKAbility_Base ia in innateAbilities)
            {
                ia.PostDespawn();
            }
            foreach (AKAbility_Base ga in groupedAbilities)
            {
                ga.PostDespawn();
            }
        }

        public IEnumerable<Thing> AllSummoneds()
        {
            foreach (AKAbility_Summon i in summonAbilities)
            {
                //还没想好召唤之后又切换技能怎么算
                //if (i.def.grouped && SelectedGroupedAbility != i) continue;
                foreach (Thing summoned in i.summoneds)
                {
                    yield return summoned;
                }
            }
        }

        public T TryGetSharedData<T>() where T : AbilityTrackerSharedData_Base
        {
            if (sharedData != null && sharedData is T data) return data;
            return null;
        }

        //遍历，因为技能没几个 要是以后多了找个缓存
        public AKAbility_Base TryGetAbility(AKAbilityDef def)
        {
            foreach (AKAbility_Base ab in innateAbilities)
            {
                if (ab.def == def) return ab;
            }
            foreach (AKAbility_Base ab in groupedAbilities)
            {
                if (ab.def == def) return ab;
            }
            return null;
        }
    }
}
