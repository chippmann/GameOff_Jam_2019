using Godot;

namespace GameOff_2019.RoundLogic {
    public class GameState : Node2D {
        public int playerPoints = 0;
        public int playerEnergy = 100;
        public int demonPoints = 0;
        public int demonEnergy = 0;

        public bool twoNegativeTweetsReceived = false;
    }
}