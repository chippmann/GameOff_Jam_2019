using System.Collections.Generic;

namespace GameOff_2019.Entities.Common.BehaviourTree {
    public class BTFailer : BTItem {
        public KeyValuePair<BTResult, BTItem> Execute() {
            return new KeyValuePair<BTResult, BTItem>(BTResult.Failure, null);
        }
    }
}