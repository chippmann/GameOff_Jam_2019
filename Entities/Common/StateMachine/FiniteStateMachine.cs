using Godot;
using Godot.Collections;
using Planty.EngineUtils;

namespace Planty.Entities.Common.StateMachine {
    public class FiniteStateMachine : Node2D {
        public const string StateMachineGroup = "state_machine";

        [Export] private readonly NodePath initialStateNodePath = null;
        private State currentState;

        public override void _Init() {
            AddToGroup(StateMachineGroup);
        }

        public override async void _Ready() {
            if (Engine.IsEditorHint()) {
                return;
            }

            await ToSignal(GetOwner(), "ready");
            currentState = GetNode<State>(initialStateNodePath);
            currentState.Enter();
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (Engine.IsEditorHint()) {
                return;
            }

            if (GetTree().GetNetworkPeer() != null && IsNetworkMaster()) {
                currentState.UnhandledInput(@event);
            }

            base._UnhandledInput(@event);
        }

        public override void _PhysicsProcess(float delta) {
            if (Engine.IsEditorHint()) {
                return;
            }

            currentState.PhysicsProcess(delta);
        }

        [Puppet]
        public void TransitionTo(NodePath targetStatePath, IStateMachineMessage message = null) {
            if (!HasNode(targetStatePath)) {
                Logger.Error($"This StateMachine has no state with path {targetStatePath}!");
                return;
            }

            if (GetTree().GetNetworkPeer() != null && IsNetworkMaster()) {
                Rpc(nameof(TransitionTo), targetStatePath, message);
            }

            var targetState = GetNode<State>(targetStatePath);
//            Logger.Debug($"Transitioning from state {currentState.GetName()} to {targetState.GetName()}");
            currentState.Exit();
            currentState = targetState;
            targetState.Enter(message);
        }

        public void SetState(State state) {
            currentState = state;
        }

        public State GetCurrentState() {
            return currentState;
        }

        public override string _GetConfigurationWarning() {
            foreach (Dictionary property in GetPropertyList()) {
                property.TryGetValue("name", out var name);
                property.TryGetValue("type", out var type);
                if ((type as Variant.Type? ?? Variant.Type.Nil) == Variant.Type.NodePath && !(name as string).BeginsWith("_")) {
                    if (GetNode(name as string) == null) {
                        return $"{name} has no node assigned!\nPlease add one through the editor.";
                    }
                }
            }

            return CheckIfChildrenConfigured(this);
        }

        private string CheckIfChildrenConfigured(Node nodeToCheck) {
            foreach (var child in GetChildren()) {
                var message = CheckIfChildrenConfigured(child as Node);
                if (!message.Empty()) {
                    return message;
                }
            }

            foreach (Dictionary property in GetPropertyList()) {
                property.TryGetValue("name", out var name);
                property.TryGetValue("type", out var type);
                if ((type as Variant.Type? ?? Variant.Type.Nil) == Variant.Type.NodePath && !(name as string).BeginsWith("_") && !(name as string).BeginsWith("initialState")) {
                    var nodePath = Get(name as string);
                    if (GetPathTo(nodeToCheck).Equals(nodePath)) {
                        return "";
                    }
                }
            }

            if (nodeToCheck.Equals(this) || !(nodeToCheck.GetType() == typeof(State))) {
                return "";
            }

            return "";
        }
    }
}