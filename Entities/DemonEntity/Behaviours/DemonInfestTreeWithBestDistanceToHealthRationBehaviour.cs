using Godot;
using Planty.EngineUtils;

namespace Planty.Entities.DemonEntity.Behaviours {
    public class DemonInfestTreeWithBestDistanceToHealthRationBehaviour : BaseInfestTreeBehaviour {
        protected override Vector2 GetTreeToInfestWorldPosition() {
            var treeToInfestWorldPosition = movePositionFinder.FindTreeThatPlayerCantReachOrNearestTree() ?? movePositionFinder.FindTreeThatPlayerCantReachOrNearestTreeWithPlayerInReach();
            Logger.Debug("Demon infest tree with best distance to health ratio at position: " + treeToInfestWorldPosition);
            return treeToInfestWorldPosition.GetGlobalPosition();
        }
    }
}