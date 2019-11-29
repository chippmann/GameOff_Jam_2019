using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
    public class DemonInfestTreeWithBestDistanceToHealthRationBehaviour : BaseInfestTreeBehaviour {
        protected override Vector2 GetTreeToInfestWorldPosition() {
            var treeToInfestWorldPosition = movePositionFinder.FindTreeThatPlayerCantReachOrNearestTree() ?? movePositionFinder.FindTreeThatPlayerCantReachOrNearestTree();
            Logger.Debug("Demon infest tree with best distance to health ratio at position: " + treeToInfestWorldPosition);
            return treeToInfestWorldPosition.GetGlobalPosition();
        }
    }
}