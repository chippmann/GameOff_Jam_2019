namespace GameOff_2019.Entities.Common.BehaviourTree {
    public class SimpleBehaviourTree {
        private BTItem currentItem;
        private BTItem nextItem;

        private SimpleBehaviourTree() { }

        public void Tick() {
            currentItem = currentItem != null ? currentItem.Execute().Value : nextItem.Execute().Value;
        }

        public class Builder {
            private readonly SimpleBehaviourTree behaviourTree = new SimpleBehaviourTree();

            public Builder SetCurrentItem(BTItem btActionToSetAsCurrent) {
                behaviourTree.currentItem = btActionToSetAsCurrent;
                return this;
            }

            public SimpleBehaviourTree Build(BTItem nextBtItem) {
                behaviourTree.nextItem = nextBtItem;
                return behaviourTree;
            }
        }
    }
}