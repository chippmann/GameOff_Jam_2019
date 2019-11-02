using Godot;

namespace GameOff_2019.EngineUtils {
    public class Eventing : Node2D {
        public static string EventingNodePath = "/root/Eventing";

        //pathfinding
        [Signal]
        public delegate void InvalidatePath();
    }
}