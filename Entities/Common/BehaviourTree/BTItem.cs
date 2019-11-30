using System.Collections.Generic;

namespace Planty.Entities.Common.BehaviourTree {
    public interface BTItem {
        KeyValuePair<BTResult, BTItem> Execute();
    }
}