using System.Collections.Generic;
using Godot;

namespace Planty.Levels.Common.TileMapObjects {
    public class TileMapObjectNodeReference : Reference {
        public Node2D node;
        public readonly List<int> parents = new List<int>();
    }
}