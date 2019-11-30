using Godot;
using Planty.Entities.Common.StateMachine;

namespace Planty.Entities.DemonEntity {
    public class DemonStateMachine : FiniteStateMachine {
        [Export] public readonly NodePath idle;
        [Export] public readonly NodePath moveToPosition;
        [Export] public readonly NodePath infestTree;
    }
}