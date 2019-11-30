using Godot;

namespace Planty.Levels.Common.TileMapObjects.TreeObject.Ui {
    public class InteractionOption : VBoxContainer {
        [Export] private readonly string labelKey = "NoLabelKeySet!";
        [Export] private readonly Texture icon = null;
        [Export] private readonly NodePath textureRectNodePath = null;
        private TextureRect textureRect;
        [Export] private readonly NodePath inactivityColorRectNodePath = null;
        private ColorRect inactivityColorRect;
        [Export] private readonly NodePath labelNodePath = null;
        private Label label;

        private bool isActive = true;

        public override void _Ready() {
            base._Ready();
            textureRect = GetNode<TextureRect>(textureRectNodePath);
            inactivityColorRect = GetNode<ColorRect>(inactivityColorRectNodePath);
            label = GetNode<Label>(labelNodePath);
            SetIconSize();
            SetLabelText();
        }

        private void SetLabelText() {
            label.SetText(Tr(labelKey));
        }

        private void SetIconSize() {
            textureRect.SetTexture(icon);
        }

        public void SetActive(bool shouldBeActive) {
            isActive = shouldBeActive;
            inactivityColorRect.SetVisible(!shouldBeActive);
        }

        public bool IsActive() {
            return isActive;
        }
    }
}