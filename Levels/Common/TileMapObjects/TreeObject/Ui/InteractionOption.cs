using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.Ui {
    public class InteractionOption : VBoxContainer {
        [Export] private readonly Color highlightColor = new Color(255, 255, 255, 0.5f);
        [Export] private readonly Color inactiveColor = new Color(10, 10, 10, 0.2f);
        [Export] private readonly string labelKey = "NoLabelKeySet!";
        [Export] private readonly Texture icon = null;
        [Export] private readonly Vector2 iconSizeInPixels = new Vector2(32, 32);
        [Export] private readonly NodePath textureRectNodePath = null;
        private TextureRect textureRect;
        [Export] private readonly NodePath labelNodePath = null;
        private Label label;

        private bool isActive = true;
        private Color initialModulate;

        public override void _Ready() {
            base._Ready();
            initialModulate = GetModulate();
            textureRect = GetNode<TextureRect>(textureRectNodePath);
            label = GetNode<Label>(labelNodePath);
            Connect("mouse_entered", this, nameof(OnMouseEntered));
            Connect("mouse_exited", this, nameof(OnMouseExited));
            SetIconSize();
            SetLabelText();
        }

        private void SetLabelText() {
            label.SetText(Tr(labelKey));
        }

        private void SetIconSize() {
//            var imageSize = icon.GetSize();
//            var image = new Image();
//            image.CreateFromData((int) icon.GetSize().x, (int) icon.GetSize().y, false, Image.Format.Rgba4444, icon.GetData().GetData());
//            image.Lock();
//            image.Resize(32, 32);
//            image.Unlock();
//            var imageTexture = new ImageTexture();
//            imageTexture.CreateFromImage(image);
            textureRect.SetTexture(icon);
////            textureRect.SetScale(new Vector2((imageSize.x / (imageSize.x / iconSizeInPixels.x)) / 50, (imageSize.y / (imageSize.y / iconSizeInPixels.y)) / 50));
        }

        private void OnMouseEntered() {
            if (isActive) {
                SetModulate(highlightColor);
            }
        }

        private void OnMouseExited() {
            if (isActive) {
                SetModulate(initialModulate);
            }
        }

        public void SetActive(bool shouldBeActive) {
            isActive = shouldBeActive;
            SetModulate(isActive ? initialModulate : inactiveColor);
        }

        public bool IsActive() {
            return isActive;
        }
    }
}