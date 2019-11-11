using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.RoundLogic {
    public class GameState : Node2D {
        private int playerPoints = 0;
        private int playerEnergy = 100;
        private int demonPoints = 0;
        private int demonEnergy = 0;

        public bool twoNegativeTweetsReceived = false;

        public override void _PhysicsProcess(float delta) {
            base._PhysicsProcess(delta);
            Logger.Debug($"Points: Player: {playerPoints} Demon: {demonPoints}");
        }

        public void AddPlayerPoints(int points) {
            playerPoints += points;
        }

        public void AddDemonPoints(int points) {
            demonPoints += points;
        }

        public void UsePlayerEnergy(int energyToUse) {
            playerEnergy -= energyToUse;
        }

        public void UseDemonEnergy(int energyToUse) {
            demonEnergy -= energyToUse;
        }

        public int GetPlayerPoints() => playerPoints;
        public int GetDemonPoints() => playerPoints;
        public int GetPlayerEnergy() => playerPoints;
        public int GetDemonEnergy() => playerPoints;
    }
}