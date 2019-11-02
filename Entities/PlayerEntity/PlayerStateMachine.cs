using GameOff_2019.Entities.Common.StateMachine;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity {
    public class PlayerStateMachine : FiniteStateMachine {
        [Export] private readonly NodePath movement;
    }
}