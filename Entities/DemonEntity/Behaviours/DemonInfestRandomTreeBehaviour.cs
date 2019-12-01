using Godot;
using Planty.EngineUtils;

namespace Planty.Entities.DemonEntity.Behaviours {
    public class DemonInfestRandomTreeBehaviour : BaseInfestTreeBehaviour {
        protected override Vector2 GetTreeToInfestWorldPosition() {
            var treeToInfest = movePositionFinder.FindRandomizedTree();
            Logger.Debug("Demon infest random tree at position: " + treeToInfest);
            if (treeToInfest == null) {
                return new Vector2(-1, -1);
            }
            else {
                return treeToInfest.GetGlobalPosition();
            }
        }
    }
}