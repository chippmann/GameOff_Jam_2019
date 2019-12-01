using System.Collections.Generic;

namespace Planty.Entities.Common.BehaviourTree {
    public class BTFailer : BTItem {
        public KeyValuePair<BTResult, BTItem> Execute() {
            return new KeyValuePair<BTResult, BTItem>(BTResult.Failure, null);
        }
    }
}