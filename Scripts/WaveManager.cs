using Godot;
using System;
using System.Collections.Generic;

namespace TowerSurvivors
{
	public partial class WaveManager : Node
	{
		[Export] public int CurrentWave { get; private set; } = 0;
		[Export] public float SpawnRadius { get; set; } = 600f;
		[Export] public float TimeBetweenWaves { get; set; } = 5f;
		[Export] public int BaseEnemyCount { get; set; } = 3;
		[Export] public int EnemiesPerWave { get; set; } = 3;
		[Export] public int BaseEnemyHealth { get; set; } = 50;
		[Export] public int HealthPerWave { get; set; } = 10;
		[Export] public float BaseEnemySpeed { get; set; } = 80f;
		[Export] public float SpeedPerWave { get; set; } = 5f;
		[Export] public int BaseEnemyDamage { get; set; } = 10;
		[Export] public int DamagePerWave { get; set; } = 2;

		private Vector2 _centerPosition;
		private int _enemiesRemainingInWave = 0;
		private float _waveTimer = 0f;
		private bool _waveInProgress = false;
		private Node2D _enemyContainer;

		[Signal]
		public delegate void WaveStartedEventHandler(int waveNumber);

		[Signal]
		public delegate void WaveCompletedEventHandler(int waveNumber);

		[Signal]
		public delegate void AllWavesCompletedEventHandler();

		public override void _Ready()
		{
			// Create container for enemies - use CallDeferred to avoid blocking
			_enemyContainer = new Node2D();
			_enemyContainer.Name = "Enemies";
			CallDeferred("add_enemy_container");
		}

		private void add_enemy_container()
		{
			GetParent().AddChild(_enemyContainer);
		}

		public void Initialize(Vector2 centerPosition)
		{
			_centerPosition = centerPosition;
			_waveTimer = 2f; // Start first wave after 2 seconds
			GD.Print($"WaveManager initialized! Center: {centerPosition}, Starting in 2 seconds...");
		}

		public override void _Process(double delta)
		{
			if (!_waveInProgress)
			{
				_waveTimer -= (float)delta;
				if (_waveTimer <= 0)
				{
					StartNextWave();
				}
			}
			else
			{
				// Check if wave is complete
				if (_enemiesRemainingInWave <= 0)
				{
					CompleteWave();
				}
			}
		}

		private void StartNextWave()
		{
			CurrentWave++;
			_waveInProgress = true;

			EmitSignal(SignalName.WaveStarted, CurrentWave);

			int enemyCount = CalculateEnemyCount(CurrentWave);
			_enemiesRemainingInWave = enemyCount;

			GD.Print($"Starting wave {CurrentWave} with {enemyCount} enemies");

			SpawnWave(enemyCount);
		}

		private int CalculateEnemyCount(int wave)
		{
			return BaseEnemyCount + (wave - 1) * EnemiesPerWave;
		}

		private void SpawnWave(int enemyCount)
		{
			for (int i = 0; i < enemyCount; i++)
			{
				// Stagger spawns slightly
				GetTree().CreateTimer(i * 0.3f).Timeout += () => SpawnEnemy();
			}
		}

		private void SpawnEnemy()
		{
			var enemy = new Enemy();

			// Random position on circle edge
			float angle = GD.Randf() * Mathf.Tau;
			Vector2 spawnPosition = _centerPosition + new Vector2(
				Mathf.Cos(angle) * SpawnRadius,
				Mathf.Sin(angle) * SpawnRadius
			);

			enemy.GlobalPosition = spawnPosition;
			enemy.TargetPosition = _centerPosition;

			// Scale enemy stats with wave number using exported values
			enemy.MaxHealth = BaseEnemyHealth + (CurrentWave - 1) * HealthPerWave;
			enemy.MoveSpeed = BaseEnemySpeed + (CurrentWave - 1) * SpeedPerWave;
			enemy.Damage = BaseEnemyDamage + (CurrentWave - 1) * DamagePerWave;

			_enemyContainer.AddChild(enemy);

			// Connect signals
			enemy.Died += OnEnemyDied;
			enemy.ReachedCore += OnEnemyReachedCore;
		}

		private void OnEnemyDied(Enemy enemy)
		{
			_enemiesRemainingInWave--;
			// Could emit signal for gold/score here
		}

		private void OnEnemyReachedCore(Enemy enemy, int damage)
		{
			_enemiesRemainingInWave--;
			// Damage to player core handled by GameManager
		}

		private void CompleteWave()
		{
			_waveInProgress = false;
			EmitSignal(SignalName.WaveCompleted, CurrentWave);

			if (CurrentWave >= 20)
			{
				EmitSignal(SignalName.AllWavesCompleted);
			}
			else
			{
				_waveTimer = TimeBetweenWaves;
			}
		}

		public int GetEnemiesRemaining()
		{
			return _enemiesRemainingInWave;
		}

		public bool IsWaveInProgress()
		{
			return _waveInProgress;
		}

		public Node2D GetEnemyContainer()
		{
			return _enemyContainer;
		}
	}
}
