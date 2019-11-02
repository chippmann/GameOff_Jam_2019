using System.Collections.Generic;
using System.Linq;
using GameOff_2019.EngineUtils;
using Godot;
using Godot.Collections;

namespace GameOff_2019.Entities.Common.Navigation {
    public class PathfindingTileMap : TileMap {
        [Export] public readonly int traversableTilesId = 0;
        [Export] public readonly int blockedTilesId = 1;
        [Export] private readonly NodePath pathfindingDebugCanvasNodePath = null;
        private DebugPathfindingCanvas pathfindingDebugCanvas;
        [Export] private readonly bool shouldDebugPathfinding = false;

        private readonly AStar aStar = new AStar();
        private Vector2 halfCellSize;
        private List<Vector2> traversableTiles;


        public override void _Ready() {
            pathfindingDebugCanvas = GetNode<DebugPathfindingCanvas>(pathfindingDebugCanvasNodePath);
            halfCellSize = GetCellSize() / 2;
            UpdateAStarGrid();
        }

        public List<Vector2> FindPathToTarget(Vector2 start, Vector2 end) {
            var startTile = WorldToMap(start);
            var endTile = WorldToMap(end);

            var startTileId = GetIdForTile(startTile);
            var endTileId = GetIdForTile(endTile);

            if (!aStar.HasPoint(startTileId) || !aStar.HasPoint(endTileId)) {
                return new List<Vector2>();
            }

            var path = aStar.GetPointPath(startTileId, endTileId);
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
            aStar.Clear();
            traversableTiles = new Array<Vector2>(GetUsedCellsById(traversableTilesId)).ToList();
            AddTraversableTilesToAStar(traversableTiles);
            ConnectTraversableTiles(traversableTiles);
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.InvalidatePath));
        }

        private void AddTraversableTilesToAStar(IEnumerable<Vector2> tiles) {
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
        private int GetIdForTile(Vector2 point) {
            var x = point.x - GetUsedRect().Position.x;
            var y = point.y - GetUsedRect().Position.y;
            return (int) (x + y * GetUsedRect().Size.x);
        }

        private void ConnectTraversableTiles(IEnumerable<Vector2> tiles) {
            foreach (var tile in tiles) {
                var tileId = GetIdForTile(tile);

                for (var x = 0; x < 3; x++) {
                    for (var y = 0; y < 3; y++) {
                        var targetTile = tile + new Vector2(x - 1, y - 1);
                        var targetTileId = GetIdForTile(targetTile);

                        if (tile == targetTile || !aStar.HasPoint(targetTileId) || !IsDiagonalAllowed(x, y, tile) || GetCell((int) targetTile.x, (int) targetTile.y) == InvalidCell) {
                            continue;
                        }

                        aStar.ConnectPoints(tileId, targetTileId);
                    }
                }
            }
        }

        private bool IsDiagonalAllowed(int x, int y, Vector2 tile) {
            var leftTile = tile + new Vector2(-1, 0);
            var leftTileId = GetCell((int) leftTile.x, (int) leftTile.y);
            var topTile = tile + new Vector2(0, -1);
            var topTileId = GetCell((int) topTile.x, (int) topTile.y);
            var rightTile = tile + new Vector2(+1, 0);
            var rightTileId = GetCell((int) rightTile.x, (int) rightTile.y);
            var bottomTile = tile + new Vector2(0, +1);
            var bottomTileId = GetCell((int) bottomTile.x, (int) bottomTile.y);

            switch (x) {
                case 0 when y == 0: //topLeft
                    return topTileId != blockedTilesId && leftTileId != blockedTilesId && topTileId != InvalidCell && leftTileId != InvalidCell;
                case 2 when y == 0: //topRight
                    return topTileId != blockedTilesId && rightTileId != blockedTilesId && topTileId != InvalidCell && rightTileId != InvalidCell;
                case 2 when y == 2: //bottomRight
                    return bottomTileId != blockedTilesId && rightTileId != blockedTilesId && bottomTileId != InvalidCell && rightTileId != InvalidCell;
                case 0 when y == 2: //bottomLeft
                    return bottomTileId != blockedTilesId && leftTileId != blockedTilesId && bottomTileId != InvalidCell && leftTileId != InvalidCell;
                default:
                    return true;
            }
        }
    }
}