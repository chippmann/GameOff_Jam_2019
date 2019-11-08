using GameOff_2019.Entities.Common.StateMachine;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity {
    public class PlayerStateMachine : FiniteStateMachine {
        [Export] public readonly NodePath idle;
        [Export] public readonly NodePath moveToPosition;
        [Export] public readonly NodePath plantTree;
        [Export] public readonly NodePath healTree;
    }
}