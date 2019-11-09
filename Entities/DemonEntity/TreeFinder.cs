using System;
using System.Linq;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Entities.PlayerEntity;
using GameOff_2019.Levels.Common.TileMapObjects;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject;
using Godot;

namespace GameOff_2019.Entities.DemonEntity {
    public class TreeFinder : Node2D {
        private PathfindingTileMap pathfindingTileMap;
        private TileMapManipulator tileMapManipulator;
        private Player player;

        public override void _Ready() {
            base._Ready();
            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
            tileMapManipulator = pathfindingTileMap.tileMapManipulator;
            player = NodeGetter.GetFirstNodeInGroup<Player>(GetTree(), GameConstants.PlayerGroup, true);
        }


        public TreeTileMapObject FindNearestTree() {
            var treeTileMapObjectWithoutPlayerInActionRadius = tileMapManipulator.GetTileMapObjectsOfType<TreeTileMapObject>().Where(treeTileMapObject => !treeTileMapObject.EntityInActionRadius(player)).ToList();
            treeTileMapObjectWithoutPlayerInActionRadius.Sort((treeObject1, treeObject2) =>
                treeObject1.GetTileMapPosition().DistanceTo(pathfindingTileMap.WorldToMap(GetGlobalPosition())).CompareTo(treeObject2.GetTileMapPosition().DistanceTo(pathfindingTileMap.WorldToMap(GetGlobalPosition()))));

            return treeTileMapObjectWithoutPlayerInActionRadius.Count > 0 ? treeTileMapObjectWithoutPlayerInActionRadius[0] : null;
        }

        public TreeTileMapObject FindRandomizedTree() {
            var treeTileMapObjectWithoutPlayerInActionRadius = tileMapManipulator.GetTileMapObjectsOfType<TreeTileMapObject>().Where(treeTileMapObject => !treeTileMapObject.EntityInActionRadius(player)).ToList();
            if (treeTileMapObjectWithoutPlayerInActionRadius.Count == 0) {
                return null;
            }
            else {
                var random = new Random();
                var randomIndex = random.Next(0, treeTileMapObjectWithoutPlayerInActionRadius.Count);
                return treeTileMapObjectWithoutPlayerInActionRadius[randomIndex];
            }
        }
    }
}