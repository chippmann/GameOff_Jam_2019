using System;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.Movement;
using Planty.Entities.Common.Navigation;

namespace Planty.Entities.Common {
    public class Entity : KinematicBody2D {
        [Export] private readonly NodePath entityMovementNodePath = null;
        protected EntityMovement entityMovement;

        protected PathfindingTileMap pathfindingTileMap;

        public override void _Ready() {
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
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