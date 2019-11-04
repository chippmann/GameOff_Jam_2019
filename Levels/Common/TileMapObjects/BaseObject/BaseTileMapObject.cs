using System;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.BaseObject {
    public class TileMapObject : Node2D, ITileMapObject {
        private Vector2 tileMapPosition;
        private Vector2 worldPosition;

        public void Init(Vector2 tileMap, Vector2 world) {
            tileMapPosition = tileMap;
            worldPosition = world;
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

        public virtual bool CanInteract() {
            return false;
        }
    }
}