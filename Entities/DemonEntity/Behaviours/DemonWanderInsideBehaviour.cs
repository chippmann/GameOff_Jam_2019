using Godot;
using Planty.EngineUtils;

namespace Planty.Entities.DemonEntity.Behaviours {
    public class DemonWanderInsideBehaviour : BaseDemonWanderBehaviour {
        protected override Vector2 GetWanderWorldPosition() {
            var wanderWorldPosition = movePositionFinder.FindRandomPositionInForest().GetGlobalPosition();
            Logger.Debug("Demon wander inside to position: " + wanderWorldPosition);
            return wanderWorldPosition;
        }
    }
}