using System;
using System.Collections.Generic;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using GameOff_2019.Levels.Common.TileMapObjects.BaseObject;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject.TreeStates;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject.Ui;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject {
    public class TreeTileMapObject : TileMapObject {
        [Export] private readonly NodePath hoverIndicatorNodePath = null;
        private HoverIndicator hoverIndicator;
        [Export] private readonly NodePath actionRadiusNodePath = null;
        private TreeActionRadius treeActionRadius;
        [Export] private readonly NodePath stateMachineNodePath = null;
        private TreeStateMachine stateMachine;
        [Export] private readonly NodePath treeStateNodePath = null;
        private TreeState treeState;
        [Export] private readonly PackedScene interactionPopupPackedScene = null;
        private InteractionPopup interactionPopup;

        private CanvasLayer uiContainer;

        public override void _Ready() {
            hoverIndicator = GetNode<HoverIndicator>(hoverIndicatorNodePath);
            treeActionRadius = GetNode<TreeActionRadius>(actionRadiusNodePath);
            stateMachine = GetNode<TreeStateMachine>(stateMachineNodePath);
            treeState = GetNode<TreeState>(treeStateNodePath);
            interactionPopup = interactionPopupPackedScene.Instance() as InteractionPopup;

            var uiContainers = GetTree().GetNodesInGroup(GameConstants.UiContainerGroup);
            if (uiContainers.Count != 1) {
                throw new Exception("There should be exactly one uiContainer in the sceneTree!");
            }

            if (uiContainers[0] is CanvasLayer) {
                uiContainer = uiContainers[0] as CanvasLayer;
            }
            else {
                throw new Exception("Nodes in group \"uiContainer\" should always be of type \"Control\"!");
            }

            uiContainer?.AddChild(interactionPopup);
            Connect("tree_exiting", this, nameof(OnTreeExiting));
        }

        public override bool CanInteract() {
            return hoverIndicator.Visible;
        }

        public override void Interact() {
            if (!interactionPopup.Visible) {
                interactionPopup.Popup_();
                interactionPopup.SetGlobalPosition(GetGlobalPosition());
            }
        }

        public List<TileMapObject> GetTileMapObjectsInActionRadius() {
            return treeActionRadius.GetOverlappingTileMapObjects();
        }

        public bool EntityInActionRadius(EntityBody entityBody) {
            return treeActionRadius.IsEntityInActionRadius(entityBody);
        }

        public void Infest() {
            if (!(stateMachine.GetCurrentState() is DeadState)) {
                stateMachine.TransitionTo(stateMachine.infested);
            }
        }

        public void Heal() {
            stateMachine.TransitionTo(stateMachine.growing);
        }

        public bool CanBeHealedFurther() {
            return treeState.treeHealth < treeState.maxHealth && (!(stateMachine.GetCurrentState() is InfestedState) || !(stateMachine.GetCurrentState() is DeadState));
        }

        public void IncreaseHealthByAmount(int amount) {
            if (CanBeHealedFurther()) {
                treeState.treeHealth += amount;
            }
        }

        private void OnTreeExiting() {
            interactionPopup.QueueFree();
        }
    }
}