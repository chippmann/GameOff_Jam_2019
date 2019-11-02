using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class ActionCircle : Node2D {
        public override void _Ready() {
            ZIndex = GetOwner<Node2D>().ZIndex - 1;
        }

        public override void _Draw() {
            base._Draw();
            DrawCircle(GetPosition(), 50, new Color(1, 0, 0));
        }
    }
}