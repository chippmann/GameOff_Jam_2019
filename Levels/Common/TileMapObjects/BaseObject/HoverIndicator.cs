using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Navigation;
using Godot;

public class HoverIndicator : Sprite {
    private PathfindingTileMap pathfindingTileMap;

    public override void _Ready() {
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
        Visible = pathfindingTileMap.WorldToMap(GetGlobalMousePosition()) == pathfindingTileMap.WorldToMap(GetGlobalPosition());
    }
}