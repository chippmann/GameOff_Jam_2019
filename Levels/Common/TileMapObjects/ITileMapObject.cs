using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects {
    public interface ITileMapObject {
        void Init(Vector2 tileMap, Vector2 world);
        Vector2 TileMapPosition();
        Vector2 WorldPosition();
        bool CanInteract();
    }
}