using Godot;
using System;

namespace TowerSurvivors
{
	public partial class GameManager : Node
	{
		private PlayerCore _playerCore;
		private TowerPlacementGrid _towerGrid;
		private WaveManager _waveManager;
		private GameUI _gameUI;
		private UpgradeManager _upgradeManager;
		private UpgradeUI _upgradeUI;
		private TowerShopUI _towerShop;
		private PlacementIndicator _placementIndicator;
		[Export] public  int _gold { get; set; } = 400;		
		private int _towerCost = 50;
		private bool _gameOver = false;

		// Tower placement state
		private string _selectedTowerType = null;
		private int _selectedTowerCost = 0;
		private bool _isPlacingTower = false;

		// Upgrade multipliers
		private float _damageMultiplier = 1.0f;
		private float _attackSpeedMultiplier = 1.0f;
		private float _rangeMultiplier = 1.0f;
		private float _goldMultiplier = 1.0f;
		private int _lightningRank = 0;
		private int _cannonRank = 0;
		private int _iceRank = 0;
		private int _laserRank = 0;
		private bool _lightningEvolved = false;
		private bool _cannonEvolved = false;
		private bool _iceEvolved = false;
		private bool _laserEvolved = false;
		private float _skyLightningTimer = 0f;
		public override void _Ready()
		{
			SetupGame();
		}

		public override void _Process(double delta)
		{
			if (!_gameOver)
			{
				UpdateUI();

				// Sky lightning for evolved lightning tower
				if (_lightningEvolved)
				{
					_skyLightningTimer -= (float)delta;
					if (_skyLightningTimer <= 0f)
					{
						CastSkyLightning();
						_skyLightningTimer = 5f;
					}
				}
			}
		}

		private void SetupGame()
		{
			// Get references to main nodes
			var mainScene = GetParent();

			_playerCore = mainScene.GetNode<PlayerCore>("PlayerCore");
			_towerGrid = mainScene.GetNode<TowerPlacementGrid>("TowerGrid");
			_waveManager = mainScene.GetNode<WaveManager>("WaveManager");
			_gameUI = mainScene.GetNode<GameUI>("GameUI");
			_upgradeManager = mainScene.GetNode<UpgradeManager>("UpgradeManager");
			_upgradeUI = mainScene.GetNode<UpgradeUI>("UpgradeUI");
			_towerShop = mainScene.GetNode<TowerShopUI>("TowerShopUI");
			_placementIndicator = mainScene.GetNode<PlacementIndicator>("PlacementIndicator");

			// Connect signals
			_playerCore.HealthChanged += OnPlayerHealthChanged;
			_playerCore.CoreDestroyed += OnCoreDestroyed;

			_towerGrid.SlotSelected += OnSlotSelected;

			_waveManager.WaveStarted += OnWaveStarted;
			_waveManager.WaveCompleted += OnWaveCompleted;
			_waveManager.AllWavesCompleted += OnAllWavesCompleted;

			_upgradeManager.UpgradeReady += OnUpgradeReady;
			_upgradeUI.UpgradeSelected += OnUpgradeSelected;

			_towerShop.TowerPurchased += OnTowerPurchased;

			// Initialize systems
			_waveManager.Initialize(_playerCore.GlobalPosition);

			// Connect enemy signals (done dynamically when enemies spawn)
			ConnectEnemySignals();

			// Place central tower automatically
			PlaceCentralTower();

			// Update initial UI
			UpdateUI();
			UpdateRankIndicators();
		}

		private void PlaceCentralTower()
		{
			// The center slot in the diamond grid is slot index 13 (middle of 7-slot row)
			var centerSlot = _towerGrid.GetSlot(13);
			if (centerSlot != null)
			{
				var centralTower = new ArrowTower();
				centerSlot.PlaceTower(centralTower);
				GD.Print("Central tower placed!");
			}
		}

		private void ConnectEnemySignals()
		{
			// Connect to existing enemies
			var enemyContainer = _waveManager.GetEnemyContainer();
			foreach (var child in enemyContainer.GetChildren())
			{
				if (child is Enemy enemy)
				{
					enemy.ReachedCore += OnEnemyReachedCore;
					enemy.Died += OnEnemyDied;
				}
			}

			// Also connect to future enemies
			enemyContainer.ChildEnteredTree += (Node node) =>
			{
				if (node is Enemy enemy)
				{
					enemy.ReachedCore += OnEnemyReachedCore;
					enemy.Died += OnEnemyDied;
				}
			};
		}

		private void OnTowerPurchased(string towerType, int cost)
		{
			if (_gameOver) return;

			if (_gold >= cost)
			{
				// Enter placement mode
				_selectedTowerType = towerType;
				_selectedTowerCost = cost;
				_isPlacingTower = true;

				// Show visual feedback
				_placementIndicator.ShowPlacementMode(towerType);
				HighlightAvailableSlots(true);

				GD.Print($"Click on a green slot to place {towerType} tower!");
			}
			else
			{
				GD.Print("Not enough gold!");
			}
		}

		private void OnSlotSelected(TowerSlot slot)
		{
			GD.Print($"GameManager received slot selection: {slot.SlotIndex}, isPlacing: {_isPlacingTower}, gameOver: {_gameOver}");

			if (_gameOver || !_isPlacingTower)
			{
				GD.Print("Ignoring slot click - not in placement mode or game over");
				return;
			}

			// Check if slot is the center slot (index 13)
			if (slot.SlotIndex == 13)
			{
				GD.Print("Cannot place tower on central building!");
				return;
			}

			// Check if slot is unlocked and empty
			if (slot.IsUnlocked && !slot.HasTower)
			{
				// Place the tower
				Tower tower = CreateTowerByType(_selectedTowerType);
				if (tower != null)
				{
					slot.PlaceTower(tower);
					_gold -= _selectedTowerCost;

					// Apply current multipliers to new tower
					tower.Damage = (int)(tower.Damage * _damageMultiplier);
					tower.AttackSpeed *= _attackSpeedMultiplier;
					tower.AttackRange *= _rangeMultiplier;

					GD.Print($"Placed {_selectedTowerType} tower!");

					// Exit placement mode
					_isPlacingTower = false;
					_selectedTowerType = null;
					_selectedTowerCost = 0;

					// Hide visual feedback
					_placementIndicator.HidePlacementMode();
					HighlightAvailableSlots(false);

					UpdateUI();
				}
			}
			else if (slot.HasTower)
			{
				GD.Print("Slot already has a tower!");
			}
			else
			{
				GD.Print("Slot is locked!");
			}
		}

		private Tower CreateTowerByType(string type)
		{
			switch (type)
			{
				case "Arrow":
					return new ArrowTower();
				case "Cannon":
					return new CannonTower();
				case "Laser":
					return new LaserTower();
				case "Ice":
					return new IceTower();
				case "Lightning":
					return new LightningTower();
				default:
					return new ArrowTower();
			}
		}

		private void OnPlayerHealthChanged(int currentHealth, int maxHealth)
		{
			UpdateUI();
		}

		private void OnCoreDestroyed()
		{
			GameOver(false);
		}

		private void OnEnemyReachedCore(Enemy enemy, int damage)
		{
			_playerCore.TakeDamage(damage);
		}

		private void OnEnemyDied(Enemy enemy)
		{
			int goldEarned = (int)(enemy.GoldReward * _goldMultiplier);
			_gold += goldEarned;

			// Give XP to player
			_upgradeManager.AddXP(enemy.XPReward);

			UpdateUI();
		}

		private void OnWaveStarted(int waveNumber)
		{
			GD.Print($"Wave {waveNumber} started!");
			UpdateUI();
		}

		private void OnWaveCompleted(int waveNumber)
		{
			GD.Print($"Wave {waveNumber} completed!");
			// Bonus gold for completing wave
			_gold += 25;
			UpdateUI();
		}

		private void OnAllWavesCompleted()
		{
			GameOver(true);
		}

		private void GameOver(bool victory)
		{
			_gameOver = true;

			if (victory)
			{
				_gameUI.ShowVictoryScreen();
				GD.Print("VICTORY! You survived all waves!");
			}
			else
			{
				_gameUI.ShowGameOverScreen();
				GD.Print("GAME OVER! Your core was destroyed!");
			}
		}

		private void UpdateUI()
		{
			_gameUI.UpdateHealth(_playerCore.CurrentHealth, _playerCore.MaxHealth);
			_gameUI.UpdateWave(_waveManager.CurrentWave);
			_gameUI.UpdateGold(_gold);

			// Update tower shop affordability
			if (_towerShop != null)
			{
				_towerShop.UpdateAffordability(_gold);
			}

			// Update XP bar
			if (_upgradeManager != null)
			{
				float progress = _upgradeManager.GetUpgradeProgress();
				int currentXP = _upgradeManager.GetCurrentXP();
				int xpToNext = _upgradeManager.GetXPToNextLevel();
				int level = _upgradeManager.GetCurrentLevel();
				_gameUI.UpdateXPBar(progress, currentXP, xpToNext, level);
			}
		}

		public void RestartGame()
		{
			GetTree().ReloadCurrentScene();
		}

		private void OnUpgradeReady(Upgrade upgrade1, Upgrade upgrade2, Upgrade upgrade3)
		{
			GD.Print("Upgrade time! Showing 3 choices...");
			_upgradeUI.ShowUpgrades(upgrade1, upgrade2, upgrade3);
		}

		private void OnUpgradeSelected(Upgrade upgrade)
		{
			GD.Print($"Upgrade selected: {upgrade.Name}");
			ApplyUpgrade(upgrade);
			_upgradeManager.ResumeGame();
		}

		private void ApplyUpgrade(Upgrade upgrade)
		{
			switch (upgrade.Type)
			{
				case UpgradeType.TowerDamage:
					_damageMultiplier += upgrade.Value;
					ApplyMultipliersToAllTowers();
					break;

				case UpgradeType.TowerAttackSpeed:
					_attackSpeedMultiplier += upgrade.Value;
					ApplyMultipliersToAllTowers();
					break;

				case UpgradeType.TowerRange:
					_rangeMultiplier += upgrade.Value;
					ApplyMultipliersToAllTowers();
					break;

				case UpgradeType.UnlockSlot:
					for (int i = 0; i < (int)upgrade.Value; i++)
					{
						_towerGrid.UnlockNextSlot();
					}
					break;

				case UpgradeType.CoreHealth:
					_playerCore.MaxHealth += (int)upgrade.Value;
					_playerCore.Heal((int)upgrade.Value);
					break;

				case UpgradeType.GoldMultiplier:
					_goldMultiplier += upgrade.Value;
					break;

				// Tower-specific upgrades
				case UpgradeType.LightningChainCount:
					ApplyLightningChainUpgrade((int)upgrade.Value);
					break;

				case UpgradeType.CannonExplosionRadius:
					ApplyCannonRadiusUpgrade(upgrade.Value);
					break;

				case UpgradeType.IceMultiShot:
					ApplyIceMultiShotUpgrade((int)upgrade.Value);
					break;

				case UpgradeType.LaserMultiShot:
					ApplyLaserMultiShotUpgrade((int)upgrade.Value);
					break;

				// TODO: Implement pierce, explosion, regen later
			}

			UpdateUI();
		}

		private void ApplyMultipliersToAllTowers()
		{
			foreach (var slot in _towerGrid.GetAllSlots())
			{
				if (slot.HasTower && slot.CurrentTower is Tower tower)
				{
					tower.Damage = (int)(tower.Damage * _damageMultiplier);
					tower.AttackSpeed *= _attackSpeedMultiplier;
					tower.AttackRange *= _rangeMultiplier;
				}
			}
		}

		private void HighlightAvailableSlots(bool active)
		{
			foreach (var slot in _towerGrid.GetAllSlots())
			{
				// Don't highlight center slot (index 13)
				if (slot.SlotIndex != 13)
				{
					slot.SetPlacementHighlight(active);
				}
			}
		}

		private void ApplyLightningChainUpgrade(int additionalChains)
		{
			_lightningRank++;
			GD.Print($"Lightning rank: {_lightningRank}/3");

			if (_lightningRank >= 3 && !_lightningEvolved)
			{
				EvolveLightningTower();
			}
			else
			{
				foreach (var slot in _towerGrid.GetAllSlots())
				{
					if (slot.HasTower && slot.CurrentTower is LightningTower lightningTower)
					{
						lightningTower.ChainCount += additionalChains;
						GD.Print($"Lightning tower chains increased to {lightningTower.ChainCount}");
					}
				}
			}

			UpdateRankIndicators();
		}

		private void EvolveLightningTower()
		{
			_lightningEvolved = true;
			_skyLightningTimer = 5f;

			foreach (var slot in _towerGrid.GetAllSlots())
			{
				if (slot.HasTower && slot.CurrentTower is LightningTower lightningTower)
				{
					lightningTower.ChainCount = 6;
					lightningTower.Evolved = true;
					GD.Print("⚡ LIGHTNING EVOLVED! Chains set to 6, Sky lightning active!");
				}
			}
		}

		private void CastSkyLightning()
		{
			var enemyContainer = _waveManager.GetEnemyContainer();
			if (enemyContainer != null)
			{
				var enemies = new System.Collections.Generic.List<Enemy>();
				foreach (var child in enemyContainer.GetChildren())
				{
					if (child is Enemy enemy && IsInstanceValid(enemy))
						enemies.Add(enemy);
				}

				if (enemies.Count > 0)
				{
					var target = enemies[(int)(GD.Randi() % enemies.Count)];

					var line = new Line2D();
					line.DefaultColor = new Color(1f, 1f, 0.5f, 1f);
					line.Width = 15f;

					var mainScene = GetTree().Root.GetChild(0);
					mainScene.AddChild(line);

					Vector2 skyPos = new Vector2(target.GlobalPosition.X, 0);
					line.AddPoint(skyPos);
					line.AddPoint(target.GlobalPosition);

					target.TakeDamage(100);

					var camera = GetTree().Root.GetNode<CameraShake>("Main/Camera2D");
					if (camera != null)
						camera.Shake(15f, 0.4f);

					var tween = GetTree().CreateTween();
					tween.TweenProperty(line, "modulate:a", 0f, 0.5f);
					tween.TweenCallback(Callable.From(() => line.QueueFree()));

					GD.Print("⚡ SKY LIGHTNING STRIKE!");
				}
			}
		}

		private void ApplyCannonRadiusUpgrade(float radiusMultiplier)
		{
			_cannonRank++;
			GD.Print($"Cannon rank: {_cannonRank}/3");

			if (_cannonRank >= 3 && !_cannonEvolved)
			{
				EvolveCannonTower();
			}
			else
			{
				foreach (var slot in _towerGrid.GetAllSlots())
				{
					if (slot.HasTower && slot.CurrentTower is CannonTower cannonTower)
					{
						cannonTower.SplashRadius *= (1f + radiusMultiplier);
						GD.Print($"Cannon explosion radius increased to {cannonTower.SplashRadius}");
					}
				}
			}

			UpdateRankIndicators();
		}

		private void EvolveCannonTower()
		{
			_cannonEvolved = true;

			foreach (var slot in _towerGrid.GetAllSlots())
			{
				if (slot.HasTower && slot.CurrentTower is CannonTower cannonTower)
				{
					cannonTower.SplashRadius = 360f;
					GD.Print("💥 CANNON EVOLVED! Explosion covers half the screen!");
				}
			}
		}

		private void ApplyIceMultiShotUpgrade(int additionalProjectiles)
		{
			_iceRank++;
			GD.Print($"Ice rank: {_iceRank}/3");

			if (_iceRank >= 3 && !_iceEvolved)
			{
				EvolveIceTower();
			}
			else
			{
				foreach (var slot in _towerGrid.GetAllSlots())
				{
					if (slot.HasTower && slot.CurrentTower is IceTower iceTower)
					{
						iceTower.ProjectileCount += additionalProjectiles;
						GD.Print($"Ice tower now fires {iceTower.ProjectileCount} projectiles");
					}
				}
			}

			UpdateRankIndicators();
		}

		private void EvolveIceTower()
		{
			_iceEvolved = true;

			foreach (var slot in _towerGrid.GetAllSlots())
			{
				if (slot.HasTower && slot.CurrentTower is IceTower iceTower)
				{
					iceTower.AttackSpeed = 10f;
					GD.Print("❄️ ICE EVOLVED! Ultra-fast continuous firing!");
				}
			}
		}

		private void ApplyLaserMultiShotUpgrade(int additionalBeams)
		{
			_laserRank++;
			GD.Print($"Laser rank: {_laserRank}/3");

			if (_laserRank >= 3 && !_laserEvolved)
			{
				EvolveLaserTower();
			}
			else
			{
				foreach (var slot in _towerGrid.GetAllSlots())
				{
					if (slot.HasTower && slot.CurrentTower is LaserTower laserTower)
					{
						laserTower.ExtraBeams += additionalBeams;
						GD.Print($"Laser tower now fires {laserTower.ExtraBeams + 1} beams total");
					}
				}
			}

			UpdateRankIndicators();
		}

		private void EvolveLaserTower()
		{
			_laserEvolved = true;

			foreach (var slot in _towerGrid.GetAllSlots())
			{
				if (slot.HasTower && slot.CurrentTower is LaserTower laserTower)
				{
					laserTower.AttackSpeed = 20f;
					GD.Print("🔴 LASER EVOLVED! Continuous beam attack!");
				}
			}
		}

		private void UpdateRankIndicators()
		{
			_upgradeUI.TowerRanks["Lightning"] = _lightningRank;
			_upgradeUI.TowerRanks["Cannon"] = _cannonRank;
			_upgradeUI.TowerRanks["Ice"] = _iceRank;
			_upgradeUI.TowerRanks["Laser"] = _laserRank;

			var rankUI = GetParent().GetNode<RankIndicatorUI>("RankIndicatorUI");
			if (rankUI != null)
			{
				rankUI.UpdateRank("Lightning", _lightningRank);
				rankUI.UpdateRank("Cannon", _cannonRank);
				rankUI.UpdateRank("Ice", _iceRank);
				rankUI.UpdateRank("Laser", _laserRank);
			}
		}
	}
}
