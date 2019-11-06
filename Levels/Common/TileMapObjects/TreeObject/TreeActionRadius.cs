using System;
using System.Collections.Generic;
using System.Linq;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Levels.Common.TileMapObjects.BaseObject;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class TreeActionRadius : Area2D {
        [Export] private readonly int radiusInTiles = 2;

//        private float internalRadius;
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

//            internalRadius = pathfindingTileMap?.GetCellSize().x * radiusInTiles ?? 192;

//            Connect("area_entered", this, nameof(OnEntered));
//            Connect("area_exited", this, nameof(OnExited));
//            Connect("body_entered", this, nameof(OnEntered));
//            Connect("body_exited", this, nameof(OnExited));
//            Connect("tree_exiting", this, nameof(OnTreeExiting));
//
//            foreach (var overlappingTileMapObject in GetOverlappingTileMapObjects().OfType<TraversableTileMapObject>()) {
//                tileMapManipulator.SetupOrReplaceTileMapObject(overlappingTileMapObject.TileMapPosition(), pathfindingTileMap.playerTraversableId);
//            }
        }

//        private void OnEntered(Node2D node) {
//            nodesInActionRadius.Add(node);
//            if (node.Owner is TileMapObject || node.Owner is EntityBody) {
//                nodesInActionRadius.Add(node.Owner as Node2D);
//            }
//
////            if (node.Owner is TraversableTileMapObject traversableTileMapObject &&
////                pathfindingTileMap.GetCell((int) traversableTileMapObject.TileMapPosition().x, (int) traversableTileMapObject.TileMapPosition().y) == pathfindingTileMap.traversableId) {
////                tileMapManipulator.SetupOrReplaceTileMapObject(traversableTileMapObject.TileMapPosition(), pathfindingTileMap.playerTraversableId);
////            }
//        }

//        private void OnTreeExiting() {
//            Monitoring = false;
//            Disconnect("area_entered", this, nameof(OnEntered));
//            Disconnect("area_exited", this, nameof(OnExited));
//            Disconnect("body_entered", this, nameof(OnEntered));
//            Disconnect("body_exited", this, nameof(OnExited));
//        }

//        private void OnExited(Node2D node) {
//            nodesInActionRadius.Remove(node);
//            nodesInActionRadius.Remove(node.Owner as Node2D);
//
//
//            if (node.Owner is TraversableTileMapObject traversableTileMapObject &&
//                pathfindingTileMap.GetCell((int) traversableTileMapObject.TileMapPosition().x, (int) traversableTileMapObject.TileMapPosition().y) == pathfindingTileMap.playerTraversableId) {
//                tileMapManipulator.SetupOrReplaceTileMapObject(traversableTileMapObject.TileMapPosition(), pathfindingTileMap.traversableId);
//            }
//        }

        public List<TileMapObject> GetOverlappingTileMapObjects() {
            var treeTrunkTileMapCoordinates = pathfindingTileMap.WorldToMap(GetGlobalPosition() /* + (pathfindingTileMap.CellSize / 2)*/);
            var squaredOverlap = new List<Vector2>();
            for (var x = 0; x < radiusInTiles * 2 + 1; x++) {
                for (var y = 0; y < radiusInTiles * 2 + 1; y++) {
                    var tile = treeTrunkTileMapCoordinates + new Vector2(x - radiusInTiles, y - radiusInTiles);
                    if (tile.x >= 0 && tile.x < pathfindingTileMap.GetUsedRect().Size.x && tile.y >= 0 && tile.y < pathfindingTileMap.GetUsedRect().Size.x) {
                        squaredOverlap.Add(tile);
                    }
                }
            }

            squaredOverlap.Remove(treeTrunkTileMapCoordinates);

            var notOverlappingTileMaps = squaredOverlap.Select((vector2, index) => pathfindingTileMap.MapToWorld(vector2) + (pathfindingTileMap.CellSize / 2)).Where(vector2 =>
                vector2.DistanceTo(pathfindingTileMap.MapToWorld(treeTrunkTileMapCoordinates) + (pathfindingTileMap.CellSize / 2)) > radiusInTiles * pathfindingTileMap.CellSize.x).ToList();

            foreach (var notOverlappingTileMap in notOverlappingTileMaps) {
                squaredOverlap.Remove(notOverlappingTileMap);
            }

            return squaredOverlap.Select(vector2 => pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(vector2)).ToList();
        }

        public bool IsEntityInActionRadius(EntityBody entityBody) {
            return nodesInActionRadius.Contains(entityBody);
        }
    }
}