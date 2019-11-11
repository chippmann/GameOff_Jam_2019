using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
    public class DemonInfestTreeThatPlayerCantReachBehaviour : BaseInfestTreeBehaviour {
        protected override Vector2 GetTreeToInfestWorldPosition() {
            var treeToInfestWorldPosition = movePositionFinder.FindTreeThatPlayerCantReachOrNearestTree().GetGlobalPosition();
            Logger.Debug("Demon infest tree that player can't reach or nearest at position: " + treeToInfestWorldPosition);
            return treeToInfestWorldPosition;
        }
    }
}