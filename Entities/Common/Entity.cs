using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Movement;
using GameOff_2019.Entities.Common.Navigation;
using Godot;

namespace GameOff_2019.Entities.Common {
    public class Entity : KinematicBody2D {
        [Export] private readonly NodePath entityMovementNodePath = null;
        protected EntityMovement entityMovement;

        protected PathfindingTileMap pathfindingTileMap;

        public override void _Ready() {
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
            var tileMaps = GetTree().GetNodesInGroup(GameConstants.PathfindingTileMapGroup);
            if (tileMaps.Count != 1) {
                throw new Exception("There should be exactly one pathfindingTileMap in the sceneTree!");
            }

            if (tileMaps[0] is PathfindingTileMap) {
                pathfindingTileMap = tileMaps[0] as PathfindingTileMap;
            }
            else {
                throw new Exception("Nodes in group \"pathfindingTileMap\" should always be of type \"PathfindingTileMap\"!");
            }
        }

        public override void _Process(float delta) {
            ZIndex = (int) pathfindingTileMap.WorldToMap(GetGlobalPosition()).y * 2 + 1;
        }

        public PathfindingTileMap GetPathfinderTileMap() {
            if (pathfindingTileMap != null) {
                return pathfindingTileMap;
            }

            throw new Exception("PathfindingTileMap cannot be null!");
        }
    }
}