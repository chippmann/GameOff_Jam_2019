using System;
using System.Linq;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.Navigation;
using Planty.Entities.PlayerEntity;
using Planty.Levels.Common.TileMapObjects;
using Planty.Levels.Common.TileMapObjects.TraversableObject;
using Planty.Levels.Common.TileMapObjects.TreeObject;

namespace Planty.Entities.DemonEntity {
    public class MovePositionFinder : Node2D {
        private PathfindingTileMap pathfindingTileMap;
        private TileMapManipulator tileMapManipulator;
        private Player player;

        public override void _Ready() {
            base._Ready();
            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
            tileMapManipulator = pathfindingTileMap.tileMapManipulator;
            player = NodeGetter.GetFirstNodeInGroup<Player>(GetTree(), GameConstants.PlayerGroup, true);
        }

        public bool HasTreeToInfest() {
            return tileMapManipulator.GetTileMapObjectsOfType<TreeTileMapObject>().Any(treeTileMapObject => !treeTileMapObject.IsInfested());
        }

        public TreeTileMapObject FindNearestTree(bool canPlayerBeInReach = false) {
            var treeTileMapObjectWithoutPlayerInActionRadius =
                tileMapManipulator.GetTileMapObjectsOfType<TreeTileMapObject>().Where(treeTileMapObject => (!treeTileMapObject.EntityInActionRadius(player) || canPlayerBeInReach) && !treeTileMapObject.IsInfested()).ToList();
            treeTileMapObjectWithoutPlayerInActionRadius.Sort((treeObject1, treeObject2) =>
                treeObject1.GetTileMapPosition().DistanceTo(pathfindingTileMap.WorldToMap(GetGlobalPosition())).CompareTo(treeObject2.GetTileMapPosition().DistanceTo(pathfindingTileMap.WorldToMap(GetGlobalPosition()))));

            return treeTileMapObjectWithoutPlayerInActionRadius.Count > 0 ? treeTileMapObjectWithoutPlayerInActionRadius[0] : null;
        }

        public TreeTileMapObject FindRandomizedTree() {
            var treeTileMapObjectWithoutPlayerInActionRadius =
                tileMapManipulator.GetTileMapObjectsOfType<TreeTileMapObject>().Where(treeTileMapObject => !treeTileMapObject.EntityInActionRadius(player) && !treeTileMapObject.IsInfested()).ToList();
            if (treeTileMapObjectWithoutPlayerInActionRadius.Count == 0) {
                return null;
            }

            var random = new Random();
            var randomIndex = random.Next(0, treeTileMapObjectWithoutPlayerInActionRadius.Count);
            return treeTileMapObjectWithoutPlayerInActionRadius[randomIndex];
        }

        public TreeTileMapObject FindTreeThatPlayerCantReachOrNearestTree() {
            return FindNearestTree(); //TODO: implement formula
        }

        public TreeTileMapObject FindTreeThatPlayerCantReachOrNearestTreeWithPlayerInReach() {
            return FindNearestTree(true); //TODO: implement formula
        }

        public TraversableTileMapObject FindRandomPosition() {
            var traversableTileMapObjects = tileMapManipulator.GetTileMapObjectsOfType<TraversableTileMapObject>()
                .Where(traversableTileMapObject => pathfindingTileMap.GetCell((int) traversableTileMapObject.GetTileMapPosition().x, (int) traversableTileMapObject.GetTileMapPosition().y) == pathfindingTileMap.traversableId).ToList();
            if (traversableTileMapObjects.Count == 0) {
                return null;
            }

            var random = new Random();
            var randomIndex = random.Next(0, traversableTileMapObjects.Count);
            return traversableTileMapObjects[randomIndex];
        }

        public TraversableTileMapObject FindRandomPositionInForest() {
            var traversableTileMapObjects = tileMapManipulator.GetTileMapObjectsOfType<TraversableTileMapObject>()
                .Where(traversableTileMapObject => pathfindingTileMap.GetCell((int) traversableTileMapObject.GetTileMapPosition().x, (int) traversableTileMapObject.GetTileMapPosition().y) == pathfindingTileMap.playerTraversableId).ToList();
            if (traversableTileMapObjects.Count == 0) {
                return null;
            }

            var random = new Random();
            var randomIndex = random.Next(0, traversableTileMapObjects.Count);
            return traversableTileMapObjects[randomIndex];
        }
    }
}