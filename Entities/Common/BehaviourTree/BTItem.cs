using System.Collections.Generic;

namespace GameOff_2019.Entities.Common.BehaviourTree {
    public interface BTItem {
        KeyValuePair<BTResult, BTItem> Execute();
    }
}