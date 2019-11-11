using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
    public class DemonWanderInsideBehaviour : BaseDemonWanderBehaviour {
        protected override Vector2 GetWanderWorldPosition() {
            var wanderWorldPosition = movePositionFinder.FindRandomPositionInForest().GetGlobalPosition();
            Logger.Debug("Demon wander inside to position: " + wanderWorldPosition);
            return wanderWorldPosition;
        }
    }
}