using System;
using System.Collections.Generic;
using System.Linq;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Levels.Common.TileMapObjects.BaseObject;
using Godot;
using Godot.Collections;

namespace GameOff_2019.Levels.Common.TileMapObjects {
    public class TileMapManipulator : Node {
        [Export] private readonly NodePath pathfindingTileMapNodePath = null;
        private PathfindingTileMap pathfindingTileMap;
        [Export] private readonly NodePath tileMapObjectContainerNodePath = null;
        private Node2D tileMapObjectContainer;
        [Export] public readonly int actionRadiusInTiles = 2;


        // ReSharper disable once CollectionNeverUpdated.Local
        [Export] private readonly Godot.Collections.Dictionary<int, PackedScene> tileIdToPackedSceneMapping = new Godot.Collections.Dictionary<int, PackedScene>();

        private readonly Godot.Collections.Dictionary<int, TileMapObjectNodeReference> tileMapObjects = new Godot.Collections.Dictionary<int, TileMapObjectNodeReference>();

        public override async void _Ready() {
            pathfindingTileMap = GetNode<PathfindingTileMap>(pathfindingTileMapNodePath);
            tileMapObjectContainer = GetNode<Node2D>(tileMapObjectContainerNodePath);
            await ToSignal(Owner, "ready");
            SetupTileMapObjectNodeReferences();
            SetupTileChildren();
            AddTileMapObjects();
            pathfindingTileMap.UpdateAStarGrid();
        }

        private void SetupTileMapObjectNodeReferences() {
            var usedTiles = new Array<Vector2>(pathfindingTileMap.GetUsedCells());
            foreach (var tile in usedTiles) {
                AddTileMapObjectNodeReference(tile);
            }
        }

        private void AddTileMapObjectNodeReference(Vector2 tile) {
            var uniqueTileId = pathfindingTileMap.GetIdForTile(tile);
            tileMapObjects.Add(uniqueTileId, new TileMapObjectNodeReference());
        }

        private void SetupTileChildren() {
            var treeTiles = new Array<Vector2>(pathfindingTileMap.GetUsedCells()).Where(treeTile => pathfindingTileMap.GetCell((int) treeTile.x, (int) treeTile.y) == pathfindingTileMap.treeId);
            foreach (var treeTile in treeTiles) {
                var parentUniqueId = pathfindingTileMap.GetIdForTile(treeTile);
                var overlappingTiles = GetOverlappingTiles(treeTile);

                foreach (var overlappingTile in overlappingTiles) {
                    var uniqueTileId = pathfindingTileMap.GetIdForTile(overlappingTile);

                    if (tileMapObjects.TryGetValue(uniqueTileId, out var tileMapObjectNodeReference)) {
                        pathfindingTileMap.SetCell((int) overlappingTile.x, (int) overlappingTile.y, pathfindingTileMap.playerTraversableId);
                        tileMapObjectNodeReference.parents.Add(parentUniqueId);
                    }
                    else {
                        throw new Exception("Expected already setup tileMapObjectNodeReference!");
                    }
                }
            }
        }

        private void AddTileMapObjects() {
            var usedTiles = new Array<Vector2>(pathfindingTileMap.GetUsedCells());
            foreach (var tile in usedTiles) {
                AddTileMapObject(tile);
            }
        }

        private void AddTileMapObject(Vector2 tile) {
            var tileId = pathfindingTileMap.GetCell((int) tile.x, (int) tile.y);
            var tileUniqueId = pathfindingTileMap.GetIdForTile(tile);
            var worldPosition = pathfindingTileMap.MapToWorld(tile) + pathfindingTileMap.CellSize / 2;
            tileIdToPackedSceneMapping.TryGetValue(tileId, out var packedScene);

            if (packedScene?.Instance() is TileMapObject tileMapObject) {
                tileMapObject.Init(tile, worldPosition);
                if (tileMapObjects.TryGetValue(tileUniqueId, out var tileMapObjectNodeReference)) {
                    tileMapObjectNodeReference.node = tileMapObject;
                    tileMapObjectContainer.AddChild(tileMapObjectNodeReference.node);
                    tileMapObjectNodeReference.node.ZIndex = (int) tile.y * 2;
                    tileMapObjectNodeReference.node.SetGlobalPosition(worldPosition);
                }
                else {
                    throw new Exception("Expected already setup tileMapObjectNodeReference!");
                }
            }
            else {
                throw new Exception("Instance is not a tileMapObject");
            }
        }

        public List<Vector2> GetOverlappingTiles(Vector2 parentTilePosition) {
            var squaredOverlap = new List<Vector2>();
            for (var x = 0; x < actionRadiusInTiles * 2 + 1; x++) {
                for (var y = 0; y < actionRadiusInTiles * 2 + 1; y++) {
                    var tile = parentTilePosition + new Vector2(x - actionRadiusInTiles, y - actionRadiusInTiles);
                    if (tile.x >= 0 && tile.x < pathfindingTileMap.GetUsedRect().Size.x && tile.y >= 0 && tile.y < pathfindingTileMap.GetUsedRect().Size.x) {
                        squaredOverlap.Add(tile);
                    }
                }
            }

            squaredOverlap.Remove(parentTilePosition);

            var notOverlappingTileMaps = squaredOverlap.Select((vector2, index) => pathfindingTileMap.MapToWorld(vector2) + (pathfindingTileMap.CellSize / 2)).Where(vector2 =>
                vector2.DistanceTo(pathfindingTileMap.MapToWorld(parentTilePosition) + (pathfindingTileMap.CellSize / 2)) > actionRadiusInTiles * pathfindingTileMap.CellSize.x).ToList();

            foreach (var notOverlappingTileMap in notOverlappingTileMaps) {
                squaredOverlap.Remove(notOverlappingTileMap);
            }

            return squaredOverlap;
        }


        public void SetTree(Vector2 treeWorldPosition) {
            var tileMapPosition = pathfindingTileMap.WorldToMap(treeWorldPosition);
            var uniqueCellId = pathfindingTileMap.GetIdForTile(tileMapPosition);
            pathfindingTileMap.SetCell((int) tileMapPosition.x, (int) tileMapPosition.y, pathfindingTileMap.treeId);

            if (tileMapObjects.TryGetValue(uniqueCellId, out var objectNodeReference)) {
//                objectNodeReference.parents.Clear();
                objectNodeReference.node?.QueueFree();

                if (tileIdToPackedSceneMapping.TryGetValue(pathfindingTileMap.treeId, out var treePackedScene)) {
                    objectNodeReference.node = treePackedScene.Instance() as Node2D;
                    tileMapObjectContainer.AddChild(objectNodeReference.node);
                    if (objectNodeReference.node != null) {
                        objectNodeReference.node.ZIndex = (int) tileMapPosition.y * 2;
                        objectNodeReference.node?.SetGlobalPosition(pathfindingTileMap.MapToWorld(tileMapPosition) + pathfindingTileMap.CellSize / 2);
                    }
                }
                else {
                    throw new Exception("PackedScene cannot be null!");
                }

                var overlappingTiles = GetOverlappingTiles(tileMapPosition);

                foreach (var overlappingTile in overlappingTiles) {
                    var uniqueTileId = pathfindingTileMap.GetIdForTile(overlappingTile);

                    if (tileMapObjects.TryGetValue(uniqueTileId, out var tileMapObjectNodeReference)) {
                        if (pathfindingTileMap.GetCell((int) overlappingTile.x, (int) overlappingTile.y) == pathfindingTileMap.traversableId) {
                            tileMapObjectNodeReference.node?.QueueFree();
                            pathfindingTileMap.SetCell((int) overlappingTile.x, (int) overlappingTile.y, pathfindingTileMap.playerTraversableId);

                            if (tileIdToPackedSceneMapping.TryGetValue(pathfindingTileMap.playerTraversableId, out var packedScene)) {
                                tileMapObjectNodeReference.node = packedScene.Instance() as Node2D;
                                tileMapObjectContainer.AddChild(tileMapObjectNodeReference.node);
                                if (objectNodeReference.node != null) {
                                    objectNodeReference.node.ZIndex = (int) tileMapPosition.y * 2;
                                    tileMapObjectNodeReference.node?.SetGlobalPosition(pathfindingTileMap.MapToWorld(overlappingTile) + pathfindingTileMap.CellSize / 2);
                                }
                            }
                            else {
                                throw new Exception("PackedScene cannot be null!");
                            }
                        }

                        tileMapObjectNodeReference.parents.Add(uniqueCellId);
                    }
                    else {
                        throw new Exception("Expected already setup tileMapObjectNodeReference!");
                    }
                }
            }
            else {
                throw new Exception("Expected already setup tileMapObjectNodeReference!");
            }

            pathfindingTileMap.UpdateAStarGrid();
        }

        public void DeleteTree(Vector2 treeWorldPosition) {
            var tileMapPosition = pathfindingTileMap.WorldToMap(treeWorldPosition);
            var uniqueCellId = pathfindingTileMap.GetIdForTile(tileMapPosition);

            if (tileMapObjects.TryGetValue(uniqueCellId, out var objectNodeReference)) {
                objectNodeReference.node.QueueFree();

                var overlappingTiles = GetOverlappingTiles(tileMapPosition).Where((vector2, index) => pathfindingTileMap.GetCell((int) vector2.x, (int) vector2.y) != pathfindingTileMap.treeId);
                foreach (var overlappingTile in overlappingTiles) {
                    var uniqueChildTileId = pathfindingTileMap.GetIdForTile(overlappingTile);

                    if (tileMapObjects.TryGetValue(uniqueChildTileId, out var childObjectNodeReference)) {
                        childObjectNodeReference.parents.Remove(uniqueCellId);

                        if (childObjectNodeReference.parents.Count == 0) {
                            childObjectNodeReference.node.QueueFree();

                            if (tileIdToPackedSceneMapping.TryGetValue(pathfindingTileMap.traversableId, out var traversablePackedScene)) {
                                pathfindingTileMap.SetCell((int) overlappingTile.x, (int) overlappingTile.y, pathfindingTileMap.traversableId);
                                childObjectNodeReference.node = traversablePackedScene.Instance() as Node2D;
                                tileMapObjectContainer.AddChild(childObjectNodeReference.node);
                                objectNodeReference.node.ZIndex = (int) tileMapPosition.y * 2;
                                childObjectNodeReference.node?.SetGlobalPosition(pathfindingTileMap.MapToWorld(overlappingTile) + pathfindingTileMap.CellSize / 2);
                            }
                            else {
                                throw new Exception("PackedScene cannot be null!");
                            }
                        }
                    }
                    else {
                        throw new Exception("Expected already setup tileMapObjectNodeReference!");
                    }
                }

                if (objectNodeReference.parents.Count > 0) {
                    pathfindingTileMap.SetCell((int) tileMapPosition.x, (int) tileMapPosition.y, pathfindingTileMap.playerTraversableId);
                    if (tileIdToPackedSceneMapping.TryGetValue(pathfindingTileMap.playerTraversableId, out var traversablePackedScene)) {
                        objectNodeReference.node = traversablePackedScene.Instance() as Node2D;
                        tileMapObjectContainer.AddChild(objectNodeReference.node);
                        if (objectNodeReference.node != null) {
                            objectNodeReference.node.ZIndex = (int) tileMapPosition.y * 2;
                            objectNodeReference.node?.SetGlobalPosition(pathfindingTileMap.MapToWorld(tileMapPosition) + pathfindingTileMap.CellSize / 2);
                        }
                    }
                    else {
                        throw new Exception("PackedScene cannot be null!");
                    }
                }
                else {
                    pathfindingTileMap.SetCell((int) tileMapPosition.x, (int) tileMapPosition.y, pathfindingTileMap.traversableId);
                    if (tileIdToPackedSceneMapping.TryGetValue(pathfindingTileMap.traversableId, out var traversablePackedScene)) {
                        objectNodeReference.node = traversablePackedScene.Instance() as Node2D;
                        tileMapObjectContainer.AddChild(objectNodeReference.node);
                        if (objectNodeReference.node != null) {
                            objectNodeReference.node.ZIndex = (int) tileMapPosition.y * 2;
                            objectNodeReference.node?.SetGlobalPosition(pathfindingTileMap.MapToWorld(tileMapPosition) + pathfindingTileMap.CellSize / 2);
                        }
                    }
                    else {
                        throw new Exception("PackedScene cannot be null!");
                    }
                }
            }
            else {
                throw new Exception("Expected already setup tileMapObjectNodeReference!");
            }

            pathfindingTileMap.UpdateAStarGrid();
        }

        public TileMapObject GetTileMapObjectWithTileMapCoordinates(Vector2 position) {
            var tileId = pathfindingTileMap.GetIdForTile(position);
            tileMapObjects.TryGetValue(tileId, out var tileMapObjectReference);
            if (tileMapObjectReference?.node is TileMapObject mapObject) {
                return mapObject;
            }

            throw new Exception(tileMapObjectReference?.node + " does not extend ITileMapObject! This should never happen!");
        }
    }
}