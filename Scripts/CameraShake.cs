using Godot;
using System;

namespace TowerSurvivors
{
    public partial class CameraShake : Camera2D
    {
        private Vector2 _originalPosition;
        private float _shakeAmount = 0f;
        private float _shakeDuration = 0f;

        public override void _Ready()
        {
            _originalPosition = Position;
        }

        public override void _Process(double delta)
        {
            if (_shakeDuration > 0)
            {
                _shakeDuration -= (float)delta;

                // Random shake offset
                float offsetX = (float)GD.RandRange(-_shakeAmount, _shakeAmount);
                float offsetY = (float)GD.RandRange(-_shakeAmount, _shakeAmount);
                Position = _originalPosition + new Vector2(offsetX, offsetY);

                // Reduce shake over time
                _shakeAmount *= 0.9f;
            }
            else
            {
                Position = _originalPosition;
                _shakeAmount = 0f;
            }
        }

        public void Shake(float amount = 10f, float duration = 0.3f)
        {
            _shakeAmount = amount;
            _shakeDuration = duration;
        }
    }
}
