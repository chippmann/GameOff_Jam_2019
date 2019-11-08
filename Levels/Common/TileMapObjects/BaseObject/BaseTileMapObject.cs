using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.BaseObject {
    public class TileMapObject : Node2D {
        public virtual bool CanInteract() {
            return false;
        }

        public virtual void Interact() { }
    }
}