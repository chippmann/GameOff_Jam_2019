using System;
using Godot;
using Godot.Collections;
using Planty.EngineUtils;
using Planty.Entities.Common.StateMachine;
using Planty.SoundEngine;

namespace Planty.Entities.PlayerEntity.States {
    public class PlayerIdleState : State {
        private Timer timer;
        private SoundEngineNode soundEngineNode;
        private AudioStreamPlayer player;
        [Export] private Array<AudioStreamOGGVorbis> whistleSounds = null;

        public override void _Ready() {
            base._Ready();
            timer = new Timer {
                OneShot = true
            };

            AddChild(timer);
            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
        }

        public override void Enter(IStateMachineMessage message = null) {
            timer.Connect("timeout", this, nameof(OnTimerTimeout));
            timer.Start(new Random().Next(10,30));
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            timer.Disconnect("timeout", this, nameof(OnTimerTimeout));
            timer.Stop();
            player?.Stop();
        }

        public override string GetName() {
            return "PlayerIdleState";
        }

        private void OnTimerTimeout() {
            player = soundEngineNode.PlaySfx(whistleSounds[new Random().Next(0,3)], this);
        }
    }
}