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
        public TreeStateMachine stateMachine;
        [Export] private readonly NodePath treeStateNodePath = null;
        public TreeState treeState;
        [Export] private readonly PackedScene interactionPopupPackedScene = null;
        private InteractionPopup interactionPopup;

        private Control uiContainer;

        public override void _Ready() {
            base._Ready();
            hoverIndicator = GetNode<HoverIndicator>(hoverIndicatorNodePath);
            treeActionRadius = GetNode<TreeActionRadius>(actionRadiusNodePath);
            stateMachine = GetNode<TreeStateMachine>(stateMachineNodePath);
            treeState = GetNode<TreeState>(treeStateNodePath);
            interactionPopup = interactionPopupPackedScene.Instance() as InteractionPopup;

            uiContainer = NodeGetter.GetFirstNodeInGroup<Control>(GetTree(), GameConstants.UiContainerGroup, true);

            uiContainer?.AddChild(interactionPopup);
            interactionPopup.Init(this);
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

        public bool EntityInActionRadius(Entity entity) {
            return treeActionRadius.IsEntityInActionRadius(entity);
        }

        public void Infest() {
            if (!(stateMachine.GetCurrentState() is DeadState)) {
                stateMachine.TransitionTo(stateMachine.infested);
                treeActionRadius.Infested();
            }
        }

        public bool IsInfested() {
            return stateMachine.GetCurrentState() is InfestedState;
        }

        public void Heal() {
            stateMachine.TransitionTo(stateMachine.growing);
            treeActionRadius.Healed();
        }

        public void Kill() {
            stateMachine.TransitionTo(stateMachine.dead);
        }

        public bool CanBeHealedFurther() {
            return treeState.treeHealth < treeState.maxHealth && (!(stateMachine.GetCurrentState() is InfestedState) || !(stateMachine.GetCurrentState() is DeadState));
        }

        public void IncreaseHealthByAmount(int amount) {
            if (CanBeHealedFurther()) {
                treeState.treeHealth += amount;
            }
        }

        public Vector2 GetTilePositionNextToTree() {
            var traversableTiles = new List<KeyValuePair<Vector2, TileMapObject>>();
            var tilesToCheck = new List<Vector2>();
            var treeTileMapPosition = pathfindingTileMap.WorldToMap(GetGlobalPosition());
            tilesToCheck.Add(treeTileMapPosition + new Vector2(-1, 0));
            tilesToCheck.Add(treeTileMapPosition + new Vector2(1, 0));
            tilesToCheck.Add(treeTileMapPosition + new Vector2(0, 1));
            tilesToCheck.Add(treeTileMapPosition + new Vector2(0, -1));

            foreach (var tileMapInActionRadius in tilesToCheck) {
                if (pathfindingTileMap.GetCell((int) tileMapInActionRadius.x, (int) tileMapInActionRadius.y) == pathfindingTileMap.playerTraversableId) {
                    traversableTiles.Add(new KeyValuePair<Vector2, TileMapObject>(tileMapInActionRadius, pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(tileMapInActionRadius)));
                }
            }

            traversableTiles.Sort((pair1, pair2) => pair1.Key.DistanceTo(pathfindingTileMap.WorldToMap(GetGlobalPosition())).CompareTo(pair2.Key.DistanceTo(pathfindingTileMap.WorldToMap(GetGlobalPosition()))));
            return traversableTiles.Count > 0 ? traversableTiles[0].Key : new Vector2(-1, -1);
        }

        private void OnTreeExiting() {
            interactionPopup.QueueFree();
        }
    }
}