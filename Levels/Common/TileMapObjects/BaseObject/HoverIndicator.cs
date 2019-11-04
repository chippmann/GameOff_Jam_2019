using System;
using System.Collections.Generic;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Navigation;
using Godot;

public class HoverIndicator : Sprite {
    [Export] private readonly NodePath actionRadiusColliderNodePath = null;
    private Area2D actionRadiusCollider;

    private PathfindingTileMap pathfindingTileMap;
    private readonly List<Node2D> collidingActionRadiusColliderList = new List<Node2D>();

    public override void _Ready() {
        actionRadiusCollider = GetNode<Area2D>(actionRadiusColliderNodePath);
        actionRadiusCollider.Connect("area_entered", this, nameof(OnAreaEntered));
        actionRadiusCollider.Connect("area_exited", this, nameof(OnAreaExited));
        actionRadiusCollider.Connect("body_entered", this, nameof(OnAreaEntered));
        actionRadiusCollider.Connect("body_exited", this, nameof(OnAreaExited));

        var tileMaps = GetTree().GetNodesInGroup(GameConstants.PathfindingTileMapGroup);
        if (tileMaps.Count != 1) {
            throw new Exception("There should be exactly one pathfindingTileMap in the sceneTree!");
        }

        if (tileMaps[0] is PathfindingTileMap) {
            pathfindingTileMap = tileMaps[0] as PathfindingTileMap;
        }
        else {
            throw new Exception("Nodes in group \"pathfindingTileMap\" should always be of type \"PathfindingTileMap\"!");
        }
    }

    public override void _Process(float delta) {
        if (pathfindingTileMap.WorldToMap(GetGlobalMousePosition()) == pathfindingTileMap.WorldToMap(GetGlobalPosition())) {
            var blubb = collidingActionRadiusColliderList.Count;
        }

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