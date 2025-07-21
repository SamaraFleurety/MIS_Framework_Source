using LudeonTK;

namespace SFLib
{
    public static class DebugAction
    {
        [DebugAction("LOF-SF Actions", "Force Hide Head & Body", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ForceHideAll()
        {
            Patch_TCPHideBody.forceHideBody = true;
            Patch_TCPHideHead.forceHideHead = true;
        }
    }
}
