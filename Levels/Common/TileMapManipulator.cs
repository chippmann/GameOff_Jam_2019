using GameOff_2019.Entities.Common.Navigation;
using Godot;

public class TileMapManipulator : Node {
    [Export] private readonly NodePath pathfindingTileMapNodePath = null;
    private PathfindingTileMap pathfindingTileMap;

    public override void _Ready() {
        pathfindingTileMap = GetNode<PathfindingTileMap>(pathfindingTileMapNodePath);
    }

    public void MakeTileNotTraversable(Vector2 worldPositionOfTile) {
        var mapPosition = pathfindingTileMap.WorldToMap(worldPositionOfTile);
        if (pathfindingTileMap.GetCell((int) mapPosition.x, (int) mapPosition.y) == pathfindingTileMap.traversableTilesId) {
            pathfindingTileMap.SetCell((int) mapPosition.x, (int) mapPosition.y, pathfindingTileMap.blockedTilesId);
        }
        else {
            pathfindingTileMap.SetCell((int) mapPosition.x, (int) mapPosition.y, pathfindingTileMap.traversableTilesId);
        }

        pathfindingTileMap.UpdateAStarGrid();
    }
}