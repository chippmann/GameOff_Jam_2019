using GameOff_2019.Entities.Common.StateMachine;
using Godot;

namespace GameOff_2019.Entities.DemonEntity {
    public class DemonStateMachine : FiniteStateMachine {
        [Export] public readonly NodePath idle;
        [Export] public readonly NodePath moveToPosition;
        [Export] public readonly NodePath infestTree;
    }
}