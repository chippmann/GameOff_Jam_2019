using System;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class TileMapObject : Node2D, ITileMapObject {
        private Vector2 tileMapPosition;
        private Vector2 worldPosition;
        private PackedScene packedScene;
        private Node2D node2D;

        public void Init(Vector2 tileMap, Vector2 world, PackedScene packedSceneForNode2D) {
            tileMapPosition = tileMap;
            worldPosition = world;
            packedScene = packedSceneForNode2D;
        }

        public Vector2 TileMapPosition() {
            if (tileMapPosition == null) {
                throw new Exception("TileMapObject was not initialized! Initialize it first before accessing its values");
            }

            return tileMapPosition;
        }

        public Vector2 WorldPosition() {
            if (worldPosition == null) {
                throw new Exception("TileMapObject was not initialized! Initialize it first before accessing its values");
            }

            return worldPosition;
        }

        public PackedScene PackedScene() {
            if (packedScene == null) {
                throw new Exception("TileMapObject was not initialized! Initialize it first before accessing its values");
            }

            return packedScene;
        }

        public void SetObject(Node2D newObject) {
            node2D = newObject;
        }

        public Node2D Object() {
            return node2D;
        }
    }
}