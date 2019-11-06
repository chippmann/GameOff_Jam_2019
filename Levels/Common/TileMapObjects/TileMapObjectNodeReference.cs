using System.Collections.Generic;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects {
    public class TileMapObjectNodeReference : Reference {
        public Node2D node;
        public readonly List<int> parents = new List<int>();
    }
}