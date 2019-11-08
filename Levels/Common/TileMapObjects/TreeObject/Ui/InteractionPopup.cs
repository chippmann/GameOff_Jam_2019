using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.Ui {
    public class InteractionPopup : PopupPanel {
        [Export] private readonly NodePath killTreeInteractionObjectNodePath = null;
        private InteractionOption killTreeInteractionObject;
        [Export] private readonly NodePath healTreeInteractionObjectNodePath = null;
        private InteractionOption healTreeInteractionObject;


        public override void _Ready() {
            base._Ready();
            killTreeInteractionObject = GetNode<InteractionOption>(killTreeInteractionObjectNodePath);
            healTreeInteractionObject = GetNode<InteractionOption>(healTreeInteractionObjectNodePath);
            killTreeInteractionObject.Connect("gui_input", this, nameof(KillTreeSelected));
            healTreeInteractionObject.Connect("gui_input", this, nameof(HealTreeSelected));
            SetAsToplevel(true);
        }


        public override void _Input(InputEvent @event) {
            base._Input(@event);
            if (Visible && @event.IsActionPressed(GameConstants.ControlsActionCancel)) {
                Visible = false;
                AcceptEvent();
            }
        }

        private void KillTreeSelected(InputEvent inputEvent) {
            if (inputEvent.IsActionPressed(GameConstants.ControlsActionClick)) {
                Logger.Debug("Kill Tree selected");
                AcceptEvent();
            }
        }

        private void HealTreeSelected(InputEvent inputEvent) {
            if (inputEvent.IsActionPressed(GameConstants.ControlsActionClick)) {
                Logger.Debug("Heal Tree selected");
                AcceptEvent();
            }
        }
    }
}