using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GameOff_2019.Entities.Common.Navigation {
    public class DebugPathfindingCanvas : Node2D {
        private readonly Dictionary<DateTime, List<Vector2>> debugPaths = new Dictionary<DateTime, List<Vector2>>();

        public void DrawDebugPathfindingLine(List<Vector2> path) {
            debugPaths.Add(DateTime.Now, path);
        }

        public override void _Process(float delta) {
            if (debugPaths.Count <= 0) return;

            var pathsToRemove = debugPaths.Keys.Where((time, _) => time.AddSeconds(3) < DateTime.Now).ToList();

            foreach (var dateTime in pathsToRemove) {
                debugPaths.Remove(dateTime);
            }

            Update();
        }

        public override void _Draw() {
            foreach (var timeAndPath in debugPaths) {
                foreach (var pointInPath in timeAndPath.Value.Where(pointInPath => timeAndPath.Value.Count > timeAndPath.Value.IndexOf(pointInPath) + 1)) {
                    DrawLine(pointInPath, timeAndPath.Value[timeAndPath.Value.IndexOf(pointInPath) + 1], new Color(1, 1, 1));
                }
            }
        }
    }
}