using GameOff_2019.Levels.Common.TileMapObjects.TreeObject;
using Godot;

namespace GameOff_2019.EngineUtils {
    public class Eventing : Node2D {
        public static string EventingNodePath = "/root/Eventing";

        #region Pathfinding

        [Signal]
        public delegate void LevelSetupFinished();

        [Signal]
        public delegate void InvalidateDemonPath();

        [Signal]
        public delegate void InvalidatePlayerPath();

        [Signal]
        public delegate void PlayerTargetReached(object[] list);

        [Signal]
        public delegate void DemonTargetReached(object[] list);

        [Signal]
        public delegate void TargetCannotBeReached(object[] list);

        #endregion

        [Signal]
        public delegate void TreeInfested(TreeTileMapObject infestedTree);

        #region Ui

        [Signal]
        public delegate void PointsChanged();

        [Signal]
        public delegate void PlayerEnergyChanged();

        [Signal]
        public delegate void DemonEnergyChanged();

        [Signal]
        public delegate void IntroFinished();

        [Signal]
        public delegate void StartGamePressed();

        #endregion
    }
}