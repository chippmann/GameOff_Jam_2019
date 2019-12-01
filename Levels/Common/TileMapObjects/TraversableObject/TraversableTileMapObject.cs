using Godot;
using Planty.Levels.Common.TileMapObjects.BaseObject;

namespace Planty.Levels.Common.TileMapObjects.TraversableObject {
    public class TraversableTileMapObject : TileMapObject {
        [Export] private readonly NodePath hoverIndicatorNodePath = null;
        private HoverIndicator hoverIndicator;

        public override void _Ready() {
            hoverIndicator = GetNode<HoverIndicator>(hoverIndicatorNodePath);
        }

        public override bool CanInteract() {
            return hoverIndicator.Visible;
        }
    }
}