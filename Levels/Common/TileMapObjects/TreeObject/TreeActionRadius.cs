using System.Collections.Generic;
using System.Linq;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common;
using Planty.Entities.Common.Navigation;
using Planty.Levels.Common.TileMapObjects.BaseObject;

namespace Planty.Levels.Common.TileMapObjects.TreeObject {
    public class TreeActionRadius : Area2D {
        [Export] private readonly NodePath textureNodePath = null;
        private Sprite treeActionRadiusTexture;
        [Export] private readonly Texture healthyTexture = null;
        [Export] private readonly Texture infestedTexture = null;
        private PathfindingTileMap pathfindingTileMap;
        private TileMapManipulator tileMapManipulator;
        private readonly List<Node2D> nodesInActionRadius = new List<Node2D>();

        public override void _Ready() {
            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
            treeActionRadiusTexture = GetNode<Sprite>(textureNodePath);
            Connect("area_entered", this, nameof(OnEntered));
            Connect("area_exited", this, nameof(OnExited));
            Connect("body_entered", this, nameof(OnEntered));
            Connect("body_exited", this, nameof(OnExited));
        }

        private void OnEntered(Node2D node) {
            nodesInActionRadius.Add(node);
            if (node.Owner is TileMapObject || node.Owner is Entity) {
                nodesInActionRadius.Add(node.Owner as Node2D);
            }
        }

        private void OnExited(Node2D node) {
            nodesInActionRadius.Remove(node);
            nodesInActionRadius.Remove(node.Owner as Node2D);
        }

        public List<TileMapObject> GetOverlappingTileMapObjects() {
            return tileMapManipulator.GetOverlappingTiles(pathfindingTileMap.WorldToMap(GetGlobalPosition())).Select(vector2 => pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(vector2)).ToList();
        }

        public bool IsEntityInActionRadius(Entity entity) {
            return nodesInActionRadius.Contains(entity);
        }

        public void Infested() {
            treeActionRadiusTexture.SetTexture(infestedTexture);
        }

        public void Healed() {
            treeActionRadiusTexture.SetTexture(healthyTexture);
        }
    }
}