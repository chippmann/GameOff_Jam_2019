using System.Collections.Generic;
using Godot;

namespace GameOff_2019.Entities.Common.BehaviourTree {
    public abstract class BTAction : Node2D, BTItem {
        public KeyValuePair<BTResult, BTItem> Execute() {
            var result = ExecuteInternal();
            return new KeyValuePair<BTResult, BTItem>(result, result == BTResult.Running ? this : null);
        }

        /// <summary>
        /// Override and return a <see cref="BTResult"/>. <br/>
        /// If you return <see cref="BTResult.Running"/> this BTAction will be returned and solely executed in the next tick until you return <see cref="BTResult.Success"/> or <see cref="BTResult.Failure"/>
        /// </summary>
        /// <returns><see cref="BTResult"/></returns>
        protected abstract BTResult ExecuteInternal();
    }
}