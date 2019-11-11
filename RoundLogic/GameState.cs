using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.RoundLogic {
    public class GameState : Node2D {
        private int playerPoints = 0;
        private int playerEnergy = 100;
        private int demonPoints = 0;
        private int demonEnergy = 0;

        public bool twoNegativeTweetsReceived = false;

        public void AddPlayerPoints(int points) {
            playerPoints += points;
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.PointsChanged));
        }

        public void AddDemonPoints(int points) {
            demonPoints += points;
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.PointsChanged));
        }

        public void UsePlayerEnergy(int energyToUse) {
            playerEnergy -= energyToUse;
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.PlayerEnergyChanged));
        }

        public void UseDemonEnergy(int energyToUse) {
            demonEnergy -= energyToUse;
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.DemonEnergyChanged));
        }

        public void AddPlayerEnergy(int energyToUse) {
            if (playerEnergy + energyToUse > 100) {
                playerEnergy = 100;
            }
            else {
                playerEnergy -= energyToUse;
            }

            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.PlayerEnergyChanged));
        }

        public void AddDemonEnergy(int energyToUse) {
            if (demonEnergy + energyToUse > 100) {
                demonEnergy = 100;
            }
            else {
                demonEnergy -= energyToUse;
            }

            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.DemonEnergyChanged));
        }

        public int GetPlayerPoints() => playerPoints;
        public int GetDemonPoints() => demonPoints;
        public int GetPlayerEnergy() => playerEnergy;
        public int GetDemonEnergy() => demonEnergy;
    }
}