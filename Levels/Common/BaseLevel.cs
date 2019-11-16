using GameOff_2019.EngineUtils;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Levels.Common {
    public class BaseLevel : Node2D {
        public override void _Ready() {
            base._Ready();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.LevelSetupFinished));
        }

        public override void _UnhandledInput(InputEvent @event) {
            base._UnhandledInput(@event);
            if (@event.IsAction("debugAddEvilTweet")) {
                NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true).negativeTweetCount++;
            }
        }
    }
}