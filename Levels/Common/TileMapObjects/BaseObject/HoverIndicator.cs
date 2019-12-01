using System.Collections.Generic;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.Navigation;

public class HoverIndicator : Sprite {
    [Export] private readonly NodePath actionRadiusColliderNodePath = null;
    private Area2D actionRadiusCollider;

    private PathfindingTileMap pathfindingTileMap;
    private readonly List<Node2D> collidingActionRadiusColliderList = new List<Node2D>();

    public override void _Ready() {
        actionRadiusCollider = GetNode<Area2D>(actionRadiusColliderNodePath);
        actionRadiusCollider.Connect("area_entered", this, nameof(OnAreaEntered));
        actionRadiusCollider.Connect("area_exited", this, nameof(OnAreaExited));

        pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
    }

    public override void _Process(float delta) {
        Visible = pathfindingTileMap.WorldToMap(GetGlobalMousePosition()) == pathfindingTileMap.WorldToMap(GetGlobalPosition()) && collidingActionRadiusColliderList.Count > 0;
    }

    private void OnAreaEntered(Node2D body) {
        if (body.IsInGroup(GameConstants.ActionRadiusGroup)) {
            collidingActionRadiusColliderList.Add(body);
        }
    }

    private void OnAreaExited(Node2D body) {
        if (body.IsInGroup(GameConstants.ActionRadiusGroup)) {
            collidingActionRadiusColliderList.Remove(body);
        }
    }
}