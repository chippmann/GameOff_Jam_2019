using System;
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

            var players = GetTree().GetNodesInGroup(GameConstants.PlayerGroup);
            if (players.Count != 1) {
                throw new Exception("There should be exactly one uiContainer in the sceneTree!");
            }

            if (players[0] is Player) {
                player = players[0] as Player;
            }
            else {
                throw new Exception("Nodes in group \"player\" should always be of type \"Control\"!");
            }
        }

        public override void _Input(InputEvent @event) {
            base._Input(@event);
            if (Visible && @event.IsActionPressed(GameConstants.ControlsActionCancel)) {
                Visible = false;
                //TODO: make invalid option grey!
                AcceptEvent();
            }
        }

        public void Init(TreeTileMapObject owner) {
            treeTileMapObject = owner;
        }

        private void KillTreeSelected(InputEvent inputEvent) {
            if (inputEvent.IsActionPressed(GameConstants.ControlsActionClick) && player.CanRemoveTree(treeTileMapObject.GetGlobalPosition())) {
                treeTileMapObject.Kill();
                AcceptEvent();
            }
        }

        private void HealTreeSelected(InputEvent inputEvent) {
            if (inputEvent.IsActionPressed(GameConstants.ControlsActionClick)) {
                player.HealTree(treeTileMapObject);
                AcceptEvent();
            }
        }
    }
}