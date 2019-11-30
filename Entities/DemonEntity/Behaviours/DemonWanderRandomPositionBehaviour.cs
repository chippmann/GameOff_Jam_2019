using System;
using Godot;
using Planty.EngineUtils;

namespace Planty.Entities.DemonEntity.Behaviours {
    public class DemonWanderRandomPositionBehaviour : BaseDemonWanderBehaviour {
        protected override Vector2 GetWanderWorldPosition() {
            var random = new Random();
            var randomIndex = random.Next(0, 2);
            var wanderWorldPosition = randomIndex == 0 ? movePositionFinder.FindRandomPosition().GetGlobalPosition() : movePositionFinder.FindRandomPositionInForest().GetGlobalPosition();
            Logger.Debug("Demon wander outside to position: " + wanderWorldPosition);
            return wanderWorldPosition;
        }
    }
}