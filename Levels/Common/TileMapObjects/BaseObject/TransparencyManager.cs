using System;
using System.Collections.Generic;
using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.BaseObject {
    public class TransparencyManager : Area2D {
        [Export] private readonly int transparencyPercentage = 50;
        [Export] private readonly NodePath spriteNodePath = null;
        private Sprite sprite;
        private readonly List<Node2D> objectsBehind = new List<Node2D>();

        public override void _Ready() {
            sprite = GetNode<Sprite>(spriteNodePath);
            Connect("body_entered", this, nameof(OnAreaEntered));
            Connect("body_exited", this, nameof(OnAreaExited));
            Connect("area_entered", this, nameof(OnAreaEntered));
            Connect("area_exited", this, nameof(OnAreaExited));
        }

        private void OnAreaEntered(Node2D body) {
            if (body.IsInGroup(GameConstants.PlayerGroup) && body.ZIndex < GetOwner<Node2D>().ZIndex) {
                objectsBehind.Add(body);
                MakeTransparent();
            }
        }

        private void OnAreaExited(Node2D body) {
            if (body.IsInGroup(GameConstants.PlayerGroup)) {
                objectsBehind.Remove(body);
                MakeOpaque();
            }
        }

        private void MakeTransparent() {
            var color = GetOwner<Node2D>().Modulate;
            if (Math.Abs(color.a - 1) < 0.01) {
                sprite.Modulate = new Color(color.r, color.g, color.b, color.a / 100 * transparencyPercentage);
            }
        }

        private void MakeOpaque() {
            if (objectsBehind.Count == 0 && sprite != null) {
                var color = sprite.Modulate;
                sprite.Modulate = new Color(color.r, color.g, color.b);
            }
        }
    }
}