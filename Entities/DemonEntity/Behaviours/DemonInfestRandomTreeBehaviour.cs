using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
    public class DemonInfestRandomTreeBehaviour : BaseInfestTreeBehaviour {
        protected override Vector2 GetTreeToInfestWorldPosition() {
            var treeToInfestWorldPosition = movePositionFinder.FindRandomizedTree().GetGlobalPosition();
            Logger.Debug("Demon infest random tree at position: " + treeToInfestWorldPosition);
            return treeToInfestWorldPosition;
        }
    }
}