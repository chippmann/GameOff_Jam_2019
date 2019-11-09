using System;
using System.Collections.Generic;

namespace GameOff_2019.Entities.Common.BehaviourTree {
    public class BTSelector : BTItem {
        private BTItem trueItem;
        private BTItem falseItem;

        private Func<bool> evaluateFunc;

        public KeyValuePair<BTResult, BTItem> Execute() {
            return Evaluate() ? trueItem.Execute() : falseItem.Execute();
        }

        protected virtual bool Evaluate() {
            if (evaluateFunc == null) {
                throw new NotImplementedException();
            }

            return evaluateFunc.Invoke();
        }

        public class Builder {
            private readonly BTSelector btSelector = new BTSelector();

            public Builder SetEvaluateFunc(Func<bool> func) {
                btSelector.evaluateFunc = func;
                return this;
            }

            public BTSelector Build(BTItem trueBTItem, BTItem falseBTItem) {
                btSelector.trueItem = trueBTItem;
                btSelector.falseItem = falseBTItem;
                return btSelector;
            }
        }
    }
}