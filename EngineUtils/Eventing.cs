using Godot;

namespace GameOff_2019.EngineUtils {
    public class Eventing : Node2D {
        public static string EventingNodePath = "/root/Eventing";

        //pathfinding
        [Signal]
        public delegate void InvalidateDemonPath();

        [Signal]
        public delegate void InvalidatePlayerPath();

        [Signal]
        public delegate void PlayerTargetReached(object[] list);

        [Signal]
        public delegate void PlayerTargetCannotBeReached(object[] list);

        [Signal]
        public delegate void TileMapSetupFinished();
    }
}