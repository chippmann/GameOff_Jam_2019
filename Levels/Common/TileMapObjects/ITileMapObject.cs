using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects {
    public interface ITileMapObject {
        void Init(Vector2 tileMap, Vector2 world, PackedScene packedSceneForNode2D);
        Vector2 TileMapPosition();
        Vector2 WorldPosition();
        PackedScene PackedScene();
        void SetObject(Node2D node2D);
        Node2D Object();
    }
}