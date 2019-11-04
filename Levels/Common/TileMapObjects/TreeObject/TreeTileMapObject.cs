using System.Collections.Generic;
using GameOff_2019.Levels.Common.TileMapObjects.BaseObject;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class TreeTileMapObject : TileMapObject {
        [Export] private readonly NodePath hoverIndicatorNodePath = null;
        private HoverIndicator hoverIndicator;
        [Export] private readonly NodePath actionRadiusNodePath = null;
        private TreeActionRadius treeActionRadius;

        public override void _Ready() {
            hoverIndicator = GetNode<HoverIndicator>(hoverIndicatorNodePath);
            treeActionRadius = GetNode<TreeActionRadius>(actionRadiusNodePath);
        }

        public override bool CanInteract() {
            return hoverIndicator.Visible;
        }

        public HashSet<TileMapObject> GetTileMapObjectsInActionRadius() {
            return treeActionRadius.GetOverlappingTileMapObjects();
        }
    }
}