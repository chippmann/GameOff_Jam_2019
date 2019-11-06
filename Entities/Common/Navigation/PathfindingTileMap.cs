using System.Collections.Generic;
using System.Linq;
using GameOff_2019.EngineUtils;
using GameOff_2019.Levels.Common.TileMapObjects;
using Godot;
using Godot.Collections;

namespace GameOff_2019.Entities.Common.Navigation {
    public class PathfindingTileMap : TileMap {
        [Export] public readonly int traversableId = 0;
        [Export] public readonly int treeId = 1;
        [Export] public readonly int playerTraversableId = 2;

        [Export] private readonly NodePath pathfindingDebugCanvasNodePath = null;
        [Export] private readonly NodePath tileMapManipulatorNodePath = null;
        public TileMapManipulator tileMapManipulator;
        private DebugPathfindingCanvas pathfindingDebugCanvas;
        [Export] private readonly bool shouldDebugPathfinding = false;

        private readonly AStar demonAStar = new AStar();
        private readonly AStar playerAStar = new AStar();
        private Vector2 halfCellSize;
        private List<Vector2> traversableTiles;


        public override void _Ready() {
            tileMapManipulator = GetNode<TileMapManipulator>(tileMapManipulatorNodePath);
            pathfindingDebugCanvas = GetNode<DebugPathfindingCanvas>(pathfindingDebugCanvasNodePath);
            halfCellSize = GetCellSize() / 2;
        }

        public List<Vector2> FindPathToTargetForDemon(Vector2 start, Vector2 end) {
            var startTile = WorldToMap(start);
            var endTile = WorldToMap(end);

            var startTileId = GetIdForTile(startTile);
            var endTileId = GetIdForTile(endTile);

            if (!demonAStar.HasPoint(startTileId) || !demonAStar.HasPoint(endTileId)) {
                return new List<Vector2>();
            }

            var path = demonAStar.GetPointPath(startTileId, endTileId);
            var worldPath = path.Select(point => MapToWorld(new Vector2(point.x, point.y)) + halfCellSize).ToList();

            if (pathfindingDebugCanvas != null && shouldDebugPathfinding) {
                pathfindingDebugCanvas.DrawDebugPathfindingLine(worldPath);
            }

            worldPath.Insert(0, start);
            return worldPath;
        }

        public List<Vector2> FindPathToTargetForPlayer(Vector2 start, Vector2 end) {
            var startTile = WorldToMap(start);
            var endTile = WorldToMap(end);

            var startTileId = GetIdForTile(startTile);
            var endTileId = GetIdForTile(endTile);

            if (!playerAStar.HasPoint(startTileId) || !playerAStar.HasPoint(endTileId)) {
                return new List<Vector2>();
            }

            var path = playerAStar.GetPointPath(startTileId, endTileId);
            var worldPath = path.Select(point => MapToWorld(new Vector2(point.x, point.y)) + halfCellSize).ToList();

            if (pathfindingDebugCanvas != null && shouldDebugPathfinding) {
                pathfindingDebugCanvas.DrawDebugPathfindingLine(worldPath);
            }

            worldPath.Insert(0, start);
            return worldPath;
        }

        public bool IsWorldPositionInTileMap(Vector2 worldPosition) {
            var mapPosition = WorldToMap(worldPosition);
            return GetCell((int) mapPosition.x, (int) mapPosition.y) != InvalidCell;
        }

        public void UpdateAStarGrid() {
            UpdateDemonAStarGrid();
            UpdatePlayerAStarGrid();
        }

        private void UpdateDemonAStarGrid() {
            demonAStar.Clear();
            traversableTiles = new Array<Vector2>(GetUsedCellsById(traversableId)).ToList();
            traversableTiles.AddRange(new Array<Vector2>(GetUsedCellsById(playerTraversableId)));
            AddTraversableTilesToAStar(traversableTiles, demonAStar);
            ConnectTraversableTiles(traversableTiles, demonAStar);
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.InvalidateDemonPath));
        }

        private void UpdatePlayerAStarGrid() {
            playerAStar.Clear();
            traversableTiles = new Array<Vector2>(GetUsedCellsById(playerTraversableId)).ToList();
            AddTraversableTilesToAStar(traversableTiles, playerAStar);
            ConnectTraversableTiles(traversableTiles, playerAStar);
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.InvalidatePlayerPath));
        }

        private void AddTraversableTilesToAStar(IEnumerable<Vector2> tiles, AStar aStar) {
            foreach (var tile in tiles) {
                var tileId = GetIdForTile(tile);
                aStar.AddPoint(tileId, new Vector3(tile.x, tile.y, 0));
            }
        }

        /// <summary>
        /// Returns a unique id for a tile in the tileMap
        /// </summary>
        /// <param name="point">tile in tileMap</param>
        /// <returns>unique id</returns>
        public int GetIdForTile(Vector2 point) {
            var x = point.x - GetUsedRect().Position.x;
            var y = point.y - GetUsedRect().Position.y;
            return (int) (x + y * GetUsedRect().Size.x);
        }

        private void ConnectTraversableTiles(IEnumerable<Vector2> tiles, AStar aStar) {
            foreach (var tile in tiles) {
                var tileId = GetIdForTile(tile);

                for (var x = 0; x < 3; x++) {
                    for (var y = 0; y < 3; y++) {
                        var targetTile = tile + new Vector2(x - 1, y - 1);
                        var targetTileId = GetIdForTile(targetTile);

                        if (tile == targetTile || !aStar.HasPoint(targetTileId) || !IsDiagonalAllowed(x, y, tile, aStar == playerAStar) || GetCell((int) targetTile.x, (int) targetTile.y) == InvalidCell) {
                            continue;
                        }

                        aStar.ConnectPoints(tileId, targetTileId);
                    }
                }
            }
        }

        private bool IsDiagonalAllowed(int x, int y, Vector2 tile, bool isPlayerAStar) {
            var leftTile = tile + new Vector2(-1, 0);
            var leftTileId = GetCell((int) leftTile.x, (int) leftTile.y);
            var topTile = tile + new Vector2(0, -1);
            var topTileId = GetCell((int) topTile.x, (int) topTile.y);
            var rightTile = tile + new Vector2(+1, 0);
            var rightTileId = GetCell((int) rightTile.x, (int) rightTile.y);
            var bottomTile = tile + new Vector2(0, +1);
            var bottomTileId = GetCell((int) bottomTile.x, (int) bottomTile.y);

            if (isPlayerAStar) {
                switch (x) {
                    case 0 when y == 0: //topLeft
                        return topTileId == playerTraversableId && leftTileId == playerTraversableId && topTileId != InvalidCell && leftTileId != InvalidCell;
                    case 2 when y == 0: //topRight
                        return topTileId == playerTraversableId && rightTileId == playerTraversableId && topTileId != InvalidCell && rightTileId != InvalidCell;
                    case 2 when y == 2: //bottomRight
                        return bottomTileId == playerTraversableId && rightTileId == playerTraversableId && bottomTileId != InvalidCell && rightTileId != InvalidCell;
                    case 0 when y == 2: //bottomLeft
                        return bottomTileId == playerTraversableId && leftTileId == playerTraversableId && bottomTileId != InvalidCell && leftTileId != InvalidCell;
                    default:
                        return true;
                }
            }

            switch (x) {
                case 0 when y == 0: //topLeft
                    return (topTileId == traversableId || topTileId == playerTraversableId) && (leftTileId == traversableId || leftTileId == playerTraversableId) && topTileId != InvalidCell && leftTileId != InvalidCell;
                case 2 when y == 0: //topRight
                    return (topTileId == traversableId || topTileId == playerTraversableId) && (rightTileId == traversableId || rightTileId == playerTraversableId) && topTileId != InvalidCell && rightTileId != InvalidCell;
                case 2 when y == 2: //bottomRight
                    return (bottomTileId == traversableId || bottomTileId == playerTraversableId) && (rightTileId == traversableId || rightTileId == playerTraversableId) && bottomTileId != InvalidCell && rightTileId != InvalidCell;
                case 0 when y == 2: //bottomLeft
                    return (bottomTileId == traversableId || bottomTileId == playerTraversableId) && (leftTileId == traversableId || leftTileId == playerTraversableId) && bottomTileId != InvalidCell && leftTileId != InvalidCell;
                default:
                    return true;
            }
        }
    }
}