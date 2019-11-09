using System.Collections.Generic;
using System.Linq;

namespace GameOff_2019.Entities.Common.BehaviourTree {
    public class BTSequence : BTItem {
        private List<BTItem> items = new List<BTItem>();
        private int currentItem;
        private readonly List<KeyValuePair<BTResult, BTItem>> results = new List<KeyValuePair<BTResult, BTItem>>();

        public KeyValuePair<BTResult, BTItem> Execute() {
            for (var i = currentItem; i < items.Count; i++) {
                var result = items[i].Execute();
                if (result.Key == BTResult.Running) {
                    currentItem = i;
                    return new KeyValuePair<BTResult, BTItem>(BTResult.Running, this);
                }

                results.Add(result);
            }

            var resultToReturn = new KeyValuePair<BTResult, BTItem>(results.Any(result => result.Key == BTResult.Failure) ? BTResult.Failure : BTResult.Success, null);
            currentItem = 0;
            results.Clear();
            return resultToReturn;
        }

        public class Builder {
            private readonly BTSequence btSequence = new BTSequence();

            public BTSequence Build(params BTItem[] itemsToExecuteSequentially) {
                btSequence.items = itemsToExecuteSequentially.ToList();
                return btSequence;
            }
        }
    }
}