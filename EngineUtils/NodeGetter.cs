using System;
using Godot;

namespace GameOff_2019.EngineUtils {
    public class NodeGetter : Node2D {
        public static T GetFirstNodeInGroup<T>(SceneTree tree, string nodeGroup, bool onlyOneCanExist = false) where T : Node {
            var nodes = tree.GetNodesInGroup(nodeGroup);
            if (onlyOneCanExist && nodes.Count != 1) {
                throw new Exception("There should be exactly one player in the sceneTree!");
            }

            if (nodes[0] is T) {
                return nodes[0] as T;
            }

            throw new Exception("Node in group \"" + nodeGroup + "\" is not of expected Type!");
        }
    }
}