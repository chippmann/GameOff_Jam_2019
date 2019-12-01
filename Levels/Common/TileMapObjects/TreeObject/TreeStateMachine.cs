using Godot;
using Planty.Entities.Common.StateMachine;

namespace Planty.Levels.Common.TileMapObjects.TreeObject {
    public class TreeStateMachine : FiniteStateMachine {
        [Export] public readonly NodePath growing;
        [Export] public readonly NodePath fullGrown;
        [Export] public readonly NodePath infested;
        [Export] public readonly NodePath dead;
    }
}