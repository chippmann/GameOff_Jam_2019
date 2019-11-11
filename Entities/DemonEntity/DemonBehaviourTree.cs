using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.BehaviourTree;
using GameOff_2019.Entities.DemonEntity.Behaviours;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Entities.DemonEntity {
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
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            await ToSignal(GetNode<Eventing>(Eventing.EventingNodePath), nameof(Eventing.LevelSetupFinished));
            SetupBehaviourTree();
        }

        public override void _Process(float delta) {
            base._Process(delta);
            behaviourTree?.Tick();
        }

        private void SetupBehaviourTree() {
            behaviourTree = new SimpleBehaviourTree.Builder()
                .SetCurrentItem(
                    new BTSequence.Builder().Build(
                        new BTSelector.Builder()
                            .SetEvaluateFunc(TwoTeetsReceived)
                            .ShouldFailOnFalseItem(true)
                            .ShouldReturnSelfOnFalseItem(true)
                            .Build(
                                infestRandomTreeBehaviour,
                                wanderOutsideBehaviour
                            )
                    )
                )
                .Build(
                    new BTSelector.Builder()
                        .SetEvaluateFunc(HasEnergy)
                        .Build(
                            new BTSelector.Builder()
                                .SetEvaluateFunc(HasLessPointsThanPlayer)
                                .Build(
                                    new BTSequence.Builder().Build(
                                        wanderInsideBehaviour,
                                        new BTSelector.Builder()
                                            .SetEvaluateFunc(RandomTimeSinceLastTreeInfestElapsed)
                                            .Build(
                                                infestRandomTreeBehaviour,
                                                new BTFailer()
                                            )
                                    ),
                                    new BTSelector.Builder()
                                        .SetEvaluateFunc(HasTreeToKillWithoutPlayerIntervention)
                                        .Build(
                                            infestTreeThatPlayerCantReachBehaviour,
                                            infestTreeWithBestDistanceToHealthBehaviour
                                        )
                                )
                            ,
                            wanderOutsideBehaviour
                        )
                );
        }

        private bool TwoTeetsReceived() {
            return gameState.twoNegativeTweetsReceived;
        }

        private bool HasEnergy() {
            return gameState.demonEnergy >= infestRandomTreeBehaviour.energyConsumption;
        }

        private bool HasLessPointsThanPlayer() {
            return gameState.playerPoints > gameState.demonPoints;
        }

        private bool RandomTimeSinceLastTreeInfestElapsed() {
            return infestRandomTreeBehaviour.timer.IsStopped();
        }

        private bool HasTreeToKillWithoutPlayerIntervention() {
            return false;
        }
    }
}