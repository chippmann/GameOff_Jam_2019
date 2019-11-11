using System;
using System.Collections.Generic;

namespace GameOff_2019.Entities.Common.BehaviourTree {
    public class BTSelector : BTItem {
        private BTItem trueItem;
        private BTItem falseItem;

        private Func<bool> evaluateFunc;
        private bool shouldFailOnFalseItem = false;
        private bool shouldReturnSelfOnFailItem = false;

        public KeyValuePair<BTResult, BTItem> Execute() {
            var evaluationResult = Evaluate();
            var result = Evaluate() ? trueItem.Execute() : falseItem.Execute();

            if (!evaluationResult && shouldFailOnFalseItem) {
                return shouldReturnSelfOnFailItem ? new KeyValuePair<BTResult, BTItem>(BTResult.Failure, this) : new KeyValuePair<BTResult, BTItem>(BTResult.Failure, null);
            }

            return result;
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

            public Builder ShouldFailOnFalseItem(bool shouldFail) {
                btSelector.shouldFailOnFalseItem = shouldFail;
                return this;
            }

            public Builder ShouldReturnSelfOnFalseItem(bool shouldReturnSelf) {
                btSelector.shouldReturnSelfOnFailItem = shouldReturnSelf;
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