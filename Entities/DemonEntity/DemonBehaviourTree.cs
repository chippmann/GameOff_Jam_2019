using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.BehaviourTree;
using Planty.Entities.DemonEntity.Behaviours;
using Planty.RoundLogic;

namespace Planty.Entities.DemonEntity {
    public class DemonBehaviourTree : Node2D {
        [Export] private readonly NodePath idleBehaviourNodePath = null;
        private DemonIdleBehaviour idleBehaviour;
        [Export] private readonly NodePath wanderOutsideBehaviourNodePath = null;
        private DemonWanderOutsideBehaviour wanderOutsideBehaviour;
        [Export] private readonly NodePath wanderInsideBehaviourNodePath = null;
        private DemonWanderInsideBehaviour wanderInsideBehaviour;
        [Export] private readonly NodePath wanderRandomBehaviourNodePath = null;
        private DemonWanderRandomPositionBehaviour wanderRandomBehaviour;
        [Export] private readonly NodePath infestRandomTreeBehaviourNodePath = null;
        private DemonInfestRandomTreeBehaviour infestRandomTreeBehaviour;
        [Export] private readonly NodePath infestTreeThatPlayerCantReachBehaviourNodePath = null;
        private DemonInfestTreeThatPlayerCantReachBehaviour infestTreeThatPlayerCantReachBehaviour;
        [Export] private readonly NodePath infestTreeWithBestDistanceToHealthBehaviourNodePath = null;
        private DemonInfestTreeWithBestDistanceToHealthRationBehaviour infestTreeWithBestDistanceToHealthBehaviour;
        [Export] private readonly NodePath movePositionFinderNodePath = null;
        private MovePositionFinder movePositionFinder;

        private SimpleBehaviourTree behaviourTree;
        private GameState gameState;


        public override async void _Ready() {
            base._Ready();
            idleBehaviour = GetNode<DemonIdleBehaviour>(idleBehaviourNodePath);
            wanderOutsideBehaviour = GetNode<DemonWanderOutsideBehaviour>(wanderOutsideBehaviourNodePath);
            wanderInsideBehaviour = GetNode<DemonWanderInsideBehaviour>(wanderInsideBehaviourNodePath);
            wanderRandomBehaviour = GetNode<DemonWanderRandomPositionBehaviour>(wanderRandomBehaviourNodePath);
            infestRandomTreeBehaviour = GetNode<DemonInfestRandomTreeBehaviour>(infestRandomTreeBehaviourNodePath);
            infestTreeThatPlayerCantReachBehaviour = GetNode<DemonInfestTreeThatPlayerCantReachBehaviour>(infestTreeThatPlayerCantReachBehaviourNodePath);
            infestTreeWithBestDistanceToHealthBehaviour = GetNode<DemonInfestTreeWithBestDistanceToHealthRationBehaviour>(infestTreeWithBestDistanceToHealthBehaviourNodePath);
            movePositionFinder = GetNode<MovePositionFinder>(movePositionFinderNodePath);
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            await ToSignal(GetNode<Eventing>(Eventing.EventingNodePath), nameof(Eventing.LevelSetupFinished));
            SetupBehaviourTree();
        }

        public override void _Process(float delta) {
            base._Process(delta);
            if (!gameState.isTileMapSetup) {
                return;
            }

            behaviourTree?.Tick();
        }

        private void SetupBehaviourTree() {
            behaviourTree = new SimpleBehaviourTree.Builder()
                .Build(
                    new BTSelector.Builder()
                        .SetEvaluateFunc(TwoTeetsReceived)
                        .Build(
                            new BTSelector.Builder()
                                .SetEvaluateFunc(HasEnergy)
                                .Build(
                                    new BTSelector.Builder()
                                        .SetEvaluateFunc(HasTreeToInfest)
                                        .Build(
                                            new BTSelector.Builder()
                                                .SetEvaluateFunc(HasLessPointsThanPlayer)
                                                .Build(
                                                    new BTSelector.Builder()
                                                        .SetEvaluateFunc(HasTreeToKillWithoutPlayerIntervention)
                                                        .Build(
                                                            infestTreeThatPlayerCantReachBehaviour,
                                                            infestTreeWithBestDistanceToHealthBehaviour
                                                        ),
                                                    new BTSelector.Builder()
                                                        .SetEvaluateFunc(RandomTimeSinceLastTreeInfestElapsed)
                                                        .Build(
                                                            infestRandomTreeBehaviour,
                                                            wanderInsideBehaviour
                                                        )
                                                ),
                                            wanderInsideBehaviour
                                        )
                                    ,
                                    new BTSelector.Builder()
                                        .SetEvaluateFunc(HasLessPointsThanPlayer)
                                        .Build(
                                            wanderInsideBehaviour,
                                            wanderOutsideBehaviour
                                        )
                                ),
                            wanderOutsideBehaviour
                        )
                );
        }

        private bool HasTreeToInfest() {
            return movePositionFinder.HasTreeToInfest();
        }

        private bool TwoTeetsReceived() {
            return gameState.negativeTweetCount >= 2;
        }

        private bool HasEnergy() {
            return gameState.GetDemonEnergy() >= infestRandomTreeBehaviour.energyConsumption;
        }

        private bool HasLessPointsThanPlayer() {
            return gameState.GetPlayerPoints() > gameState.GetDemonPoints();
        }

        private bool RandomTimeSinceLastTreeInfestElapsed() {
            return infestRandomTreeBehaviour.timer.IsStopped();
        }

        private bool HasTreeToKillWithoutPlayerIntervention() {
            return false;
        }
    }
}