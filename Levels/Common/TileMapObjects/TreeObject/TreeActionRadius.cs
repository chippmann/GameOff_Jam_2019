using System;
using System.Collections.Generic;
using System.Linq;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Levels.Common.TileMapObjects.BaseObject;
using GameOff_2019.Levels.Common.TileMapObjects.TraversableObject;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class TreeActionRadius : Area2D {
        private PathfindingTileMap pathfindingTileMap;
        private TileMapManipulator tileMapManipulator;
        private readonly List<Node2D> nodesInActionRadius = new List<Node2D>();

        public override void _Ready() {
            var tileMaps = GetTree().GetNodesInGroup(GameConstants.PathfindingTileMapGroup);
            if (tileMaps.Count != 1) {
                throw new Exception("There should be exactly one pathfindingTileMap in the sceneTree!");
            }

            if (tileMaps[0] is PathfindingTileMap) {
                pathfindingTileMap = tileMaps[0] as PathfindingTileMap;
                tileMapManipulator = pathfindingTileMap?.tileMapManipulator;
            }
            else {
                throw new Exception("Nodes in group \"pathfindingTileMap\" should always be of type \"PathfindingTileMap\"!");
            }

            Connect("area_entered", this, nameof(OnEntered));
            Connect("area_exited", this, nameof(OnExited));
            Connect("body_entered", this, nameof(OnEntered));
            Connect("body_exited", this, nameof(OnExited));
        }

        private void OnEntered(Node2D node) {
            nodesInActionRadius.Add(node);
            if (node.Owner is TileMapObject || node.Owner is EntityBody) {
                nodesInActionRadius.Add(node.Owner as Node2D);
            }

            if (node.Owner is TraversableTileMapObject traversableTileMapObject &&
                pathfindingTileMap.GetCell((int) traversableTileMapObject.TileMapPosition().x, (int) traversableTileMapObject.TileMapPosition().y) == pathfindingTileMap.traversableId) {
                tileMapManipulator.SetupOrReplaceTileMapObject(traversableTileMapObject.TileMapPosition(), pathfindingTileMap.playerTraversableId);
            }
        }

        private void OnExited(Node2D node) {
            nodesInActionRadius.Remove(node);
            nodesInActionRadius.Remove(node.Owner as Node2D);
            if (node.Owner is TraversableTileMapObject traversableTileMapObject &&
                pathfindingTileMap.GetCell((int) traversableTileMapObject.TileMapPosition().x, (int) traversableTileMapObject.TileMapPosition().y) == pathfindingTileMap.playerTraversableId) {
                tileMapManipulator.SetupOrReplaceTileMapObject(traversableTileMapObject.TileMapPosition(), pathfindingTileMap.traversableId);
            }
        }

        public List<TileMapObject> GetOverlappingTileMapObjects() {
            return nodesInActionRadius.OfType<TileMapObject>().ToList();
        }

        public bool IsEntityInActionRadius(EntityBody entityBody) {
            return nodesInActionRadius.Contains(entityBody);
        }
    }
}