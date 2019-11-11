using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Navigation;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.BaseObject {
    public class TileMapObject : Node2D {
        protected PathfindingTileMap pathfindingTileMap;

        public override void _Ready() {
            base._Ready();
            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
        }

        public virtual bool CanInteract() {
            return false;
        }

        public virtual void Interact() { }

        public Vector2 GetTileMapPosition() {
            if (pathfindingTileMap == null) {
                pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
            }

            return pathfindingTileMap.WorldToMap(GetGlobalPosition());
        }
    }
}