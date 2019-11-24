namespace GameOff_2019 {
    public class GameValues {
        #region Player

        public static int plantTreePoints = 10;
        public static int plantTreeEnergyUsage = 20;
        public static int healTreePoints = 10;
        public static int pointsPerSecondForHealthyTree = 1; //multiplied with three growth

        #endregion

        #region Demon

        public static int killTreePoints = 200;
        public static int infestTreePoints = 100;
        public static int infestTreeEnergyUsage = 20;
        public static int pointsPerSecondForInfestedTree = 10; //multiplied with three growth

        #endregion

        public static int tweetEnergy = 40;

        #region Tree

        public static int treeHeal = 5;
        public static int treeInfestionDamage = 5;
        public static float treeAddEnergyPerSecondsInterval = 4f;
        public static int energyForEntitiesInRadius = 1;

        #endregion
    }
}