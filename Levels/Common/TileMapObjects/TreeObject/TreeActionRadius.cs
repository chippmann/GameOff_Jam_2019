using System;
using System.Collections.Generic;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Levels.Common.TileMapObjects.BaseObject;
using GameOff_2019.Levels.Common.TileMapObjects.TraversableObject;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class TreeActionRadius : Area2D {
        private PathfindingTileMap pathfindingTileMap;
        private TileMapManipulator tileMapManipulator;

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

            Connect("area_entered", this, nameof(OnAreaEntered));
            Connect("area_exited", this, nameof(OnAreaExited));
        }

        private void OnAreaEntered(Node2D area) {
            if (area.Owner is TraversableTileMapObject traversableTileMapObject &&
                pathfindingTileMap.GetCell((int) traversableTileMapObject.TileMapPosition().x, (int) traversableTileMapObject.TileMapPosition().y) == pathfindingTileMap.traversableId) {
                tileMapManipulator.SetupOrReplaceTileMapObject(traversableTileMapObject.TileMapPosition(), pathfindingTileMap.playerTraversableId);
            }
        }

        private void OnAreaExited(Node2D area) {
            if (area.Owner is TraversableTileMapObject traversableTileMapObject &&
                pathfindingTileMap.GetCell((int) traversableTileMapObject.TileMapPosition().x, (int) traversableTileMapObject.TileMapPosition().y) == pathfindingTileMap.playerTraversableId) {
                tileMapManipulator.SetupOrReplaceTileMapObject(traversableTileMapObject.TileMapPosition(), pathfindingTileMap.traversableId);
            }
        }

        public HashSet<TileMapObject> GetOverlappingTileMapObjects() {
            var overlappingAreas = GetOverlappingAreas();
            var overlappingBodies = GetOverlappingBodies();

            var overlappingObjects = new HashSet<TileMapObject>();

            foreach (var overlappingArea in overlappingAreas) {
                if (overlappingArea is TileMapObject area) {
                    overlappingObjects.Add(area);
                }
                else if ((overlappingArea as Node)?.GetOwner() is TileMapObject) {
                    overlappingObjects.Add((overlappingArea as Node).GetOwner() as TileMapObject);
                }
            }

            foreach (var overlappingBody in overlappingBodies) {
                if (overlappingBody is TileMapObject area) {
                    overlappingObjects.Add(area);
                }
                else if ((overlappingBody as Node)?.GetOwner() is TileMapObject) {
                    overlappingObjects.Add((overlappingBody as Node).GetOwner() as TileMapObject);
                }
            }

            return overlappingObjects;
        }
    }
}