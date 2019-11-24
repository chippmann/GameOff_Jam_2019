using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.PlayerEntity;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.Ui {
    public class InteractionPopup : PopupPanel {
        [Export] private readonly NodePath killTreeInteractionObjectNodePath = null;
        private InteractionOption killTreeInteractionObject;
        [Export] private readonly NodePath healTreeInteractionObjectNodePath = null;
        private InteractionOption healTreeInteractionObject;

        private TreeTileMapObject treeTileMapObject;
        private Player player;

        public override void _Ready() {
            base._Ready();
            killTreeInteractionObject = GetNode<InteractionOption>(killTreeInteractionObjectNodePath);
            healTreeInteractionObject = GetNode<InteractionOption>(healTreeInteractionObjectNodePath);
            killTreeInteractionObject.Connect("gui_input", this, nameof(KillTreeSelected));
            healTreeInteractionObject.Connect("gui_input", this, nameof(HealTreeSelected));

            player = NodeGetter.GetFirstNodeInGroup<Player>(GetTree(), GameConstants.PlayerGroup, true);
        }

        public override void _Process(float delta) {
            base._Process(delta);
            if (treeTileMapObject != null && IsVisible()) {
                healTreeInteractionObject.SetActive(treeTileMapObject.IsInfested());
                killTreeInteractionObject.SetActive(player.CanRemoveTree(GetGlobalPosition()));
            }
        }

        public override void _Input(InputEvent @event) {
            base._Input(@event);
            if (Visible && @event.IsActionPressed(GameConstants.ControlsActionCancel)) {
                Visible = false;
                AcceptEvent();
            }
        }

        public void Init(TreeTileMapObject owner) {
            treeTileMapObject = owner;
        }

        private void KillTreeSelected(InputEvent inputEvent) {
            if (inputEvent.IsActionPressed(GameConstants.ControlsActionClick) && player.CanRemoveTree(treeTileMapObject.GetGlobalPosition())) {
                treeTileMapObject.Kill();
                Visible = false;
                AcceptEvent();
            }
        }

        private void HealTreeSelected(InputEvent inputEvent) {
            if (inputEvent.IsActionPressed(GameConstants.ControlsActionClick)) {
                if (healTreeInteractionObject.IsActive()) {
                    player.HealTree(treeTileMapObject);
                    Visible = false;
                }

                AcceptEvent();
            }
        }
    }
}