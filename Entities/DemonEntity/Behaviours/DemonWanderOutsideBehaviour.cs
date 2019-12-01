using Godot;
using Planty.EngineUtils;

namespace Planty.Entities.DemonEntity.Behaviours {
    public class DemonWanderOutsideBehaviour : BaseDemonWanderBehaviour {
        protected override Vector2 GetWanderWorldPosition() {
            var wanderWorldPosition = movePositionFinder.FindRandomPosition().GetGlobalPosition();
            Logger.Debug("Demon wander outside to position: " + wanderWorldPosition);
            return wanderWorldPosition;
        }
    }
}