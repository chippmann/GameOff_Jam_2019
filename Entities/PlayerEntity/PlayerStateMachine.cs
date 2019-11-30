using Godot;
using Planty.Entities.Common.StateMachine;

namespace Planty.Entities.PlayerEntity {
    public class PlayerStateMachine : FiniteStateMachine {
        [Export] public readonly NodePath idle;
        [Export] public readonly NodePath moveToPosition;
        [Export] public readonly NodePath plantTree;
        [Export] public readonly NodePath healTree;
    }
}