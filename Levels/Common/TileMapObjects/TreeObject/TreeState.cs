using Godot;

namespace Planty.Levels.Common.TileMapObjects.TreeObject {
    public class TreeState : Node2D {
        [Export] public readonly int maxHealth = 100;

        public int treeHealth = 10;
        public int treeGrowth = 10;
    }
}