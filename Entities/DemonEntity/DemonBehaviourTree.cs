using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.BehaviourTree;
using GameOff_2019.Entities.DemonEntity.Behaviours;
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

        private SimpleBehaviourTree behaviourTree;


        public override async void _Ready() {
            base._Ready();
            idleBehaviour = GetNode<DemonIdleBehaviour>(idleBehaviourNodePath);
            wanderOutsideBehaviour = GetNode<DemonWanderOutsideBehaviour>(wanderOutsideBehaviourNodePath);
            wanderInsideBehaviour = GetNode<DemonWanderInsideBehaviour>(wanderInsideBehaviourNodePath);
            wanderRandomBehaviour = GetNode<DemonWanderRandomPositionBehaviour>(wanderRandomBehaviourNodePath);
            infestRandomTreeBehaviour = GetNode<DemonInfestRandomTreeBehaviour>(infestRandomTreeBehaviourNodePath);

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
                                .SetEvaluateFunc(HasPlayerLessPointsThanDemon)
                                .Build(
                                    new BTSequence.Builder().Build(
                                        wanderInsideBehaviour
                                        ////TODO: add selector
                                    ),
                                    new BTSelector.Builder()
                                        .SetEvaluateFunc(HasTreeToKillWothoutPlayerIntervention)
                                        .Build(
                                            idleBehaviour, //TODO: replace
                                            idleBehaviour //TODO: replace
                                        )
                                )
                            ,
                            wanderOutsideBehaviour
                        )
                );
        }

        private bool TwoTeetsReceived() {
            return false;
        }

        private bool HasEnergy() {
            return false;
        }

        private bool HasPlayerLessPointsThanDemon() {
            return false;
        }

        private bool HasTreeToKillWothoutPlayerIntervention() {
            return false;
        }
    }
}