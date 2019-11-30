using System.Collections.Generic;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common;
using Planty.Levels.Common.TileMapObjects.BaseObject;
using Planty.Levels.Common.TileMapObjects.TreeObject;

namespace Planty.Entities.PlayerEntity {
    public class RemoveTreeChecker : Area2D {
        private readonly List<Area2D> treeActionRadiusList = new List<Area2D>();

        public override void _Ready() {
            Connect("area_entered", this, nameof(OnAreaEntered));
            Connect("area_exited", this, nameof(OnAreaExited));
        }

        public bool CanRemoveTree(TileMapObject tileMapObject) {
            if (tileMapObject is TreeTileMapObject mapObject) {
                if (!mapObject.EntityInActionRadius(Owner as Entity)) {
                    return true;
                }

                return treeActionRadiusList.Count > 1; //one because if we delete one, we should still be in one radius. Otherwise we can't navigate anymore
            }

            return false;
        }

        private void OnAreaEntered(Area2D area) {
            if (area.IsInGroup(GameConstants.ActionRadiusGroup)) {
                treeActionRadiusList.Add(area);
            }
        }

        private void OnAreaExited(Area2D area) {
            treeActionRadiusList.Remove(area);
        }
    }
}