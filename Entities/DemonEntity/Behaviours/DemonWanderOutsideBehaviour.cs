using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
    public class DemonWanderOutsideBehaviour : BaseDemonWanderBehaviour {
        protected override Vector2 GetWanderWorldPosition() {
            var wanderWorldPosition = movePositionFinder.FindRandomPosition().GetGlobalPosition();
            Logger.Debug("Demon wander outside to position: " + wanderWorldPosition);
            return wanderWorldPosition;
        }
    }
}