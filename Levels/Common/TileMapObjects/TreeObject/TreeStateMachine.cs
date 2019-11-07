using GameOff_2019.Entities.Common.StateMachine;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class TreeStateMachine : FiniteStateMachine {
        [Export] public readonly NodePath growing;
        [Export] public readonly NodePath fullGrown;
        [Export] public readonly NodePath infested;
        [Export] public readonly NodePath dead;
    }
}