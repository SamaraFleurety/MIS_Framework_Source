namespace AKA_Ability.Gizmos
{
    /*public class EGD_SelectAllSummoned : ExtraGizmoDrawer_Base
    {
        protected override void InitExtraGizmo()
        {
            cachedGizmo = new Gizmo_SelectAllSummon
            {
                defaultLabel = label,
                defaultDesc = description,
                icon = Icon,
                parent = this.parent,
                action = delegate ()
                {
                    Find.Selector.ClearSelection();
                    foreach (Thing t in parent.container.AllSummoneds())
                    {
                        if (t.Spawned && t.Map == parent.CasterPawn.Map)
                        {
                            Find.Selector.Select(t);
                        }
                    }
                }
            };
        }

        public override void UpdateExtraGizmo()
        {
        }
    }*/
}
