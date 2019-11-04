using System.Collections.Generic;
using GameOff_2019.EngineUtils;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject;
using Godot;
using Godot.Collections;

namespace GameOff_2019.Entities.PlayerEntity {
    public class NavigationChecker : Area2D {
        private readonly HashSet<Area2D> connectedActionRadList = new HashSet<Area2D>();
        private readonly HashSet<Area2D> closedList = new HashSet<Area2D>();
        private readonly HashSet<Node2D> nodesInsideConnectedActionRads = new HashSet<Node2D>();


        public override void _Ready() {
            Connect("area_entered", this, nameof(OnAreaEntered));
            Connect("area_exited", this, nameof(OnAreaExited));
        }

        public bool CanNavigateToPoint(TileMapObject tileMapObject) {
            foreach (var area2D in connectedActionRadList) {
                var overlappingAreas = area2D.GetOverlappingAreas();
                var overlappingBodies = area2D.GetOverlappingBodies();
                var ownerAndOverlapping = new HashSet<Node2D>();
                foreach (var overlappingArea in overlappingAreas) {
                    ownerAndOverlapping.Add(overlappingArea as Node2D);
                    ownerAndOverlapping.Add((overlappingArea as Node2D)?.Owner as Node2D);
                }

                foreach (var overlappingBody in overlappingBodies) {
                    ownerAndOverlapping.Add(overlappingBody as Node2D);
                    ownerAndOverlapping.Add((overlappingBody as Node2D)?.Owner as Node2D);
                }

                foreach (var node2D in ownerAndOverlapping) {
                    if (node2D == tileMapObject) {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnAreaEntered(Area2D area) {
            if (area.IsInGroup(GameConstants.ActionRadiusGroup) && !connectedActionRadList.Contains(area)) {
                connectedActionRadList.Clear();
                closedList.Clear();
                GetConnectedActionRads(area);
            }
        }

        private void OnAreaExited(Area2D area) {
            foreach (var area2D in connectedActionRadList) {
                if (area2D.GetOverlappingAreas().Contains(Owner) || area2D.GetOverlappingBodies().Contains(Owner)) {
                    return;
                }
            }

            connectedActionRadList.Clear();
            closedList.Clear();
        }

        private void GetConnectedActionRads(Area2D area) {
            closedList.Add(area);
            connectedActionRadList.Add(area);
            foreach (var overlappingArea in new Array<Area2D>(area.GetOverlappingAreas())) {
                if (!closedList.Contains(overlappingArea) && overlappingArea.IsInGroup(GameConstants.ActionRadiusGroup)) {
                    GetConnectedActionRads(overlappingArea);
                }
            }
        }
    }
}