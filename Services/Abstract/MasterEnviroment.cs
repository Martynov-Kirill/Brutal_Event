using System.Collections.Generic;
using System.Linq;
using BrutalEvent.Enums;
using BrutalEvent.Models;
using UnityEngine;

namespace BrutalEvent.Services.Abstract
{
    public class MasterEnviroment
    {
        private const int AdditionalMaxScrap = 45;
        private const int MaxEnemyPowerIncrement = 10;
        private const int MaxOutsideEnemyPowerIncrement = 5;
        private const int MaxDaytimeEnemyPowerIncrement = 10;
        private readonly SpawnCurveGenerator _curveGenerator;

        public MasterEnviroment()
        {
            _curveGenerator = new SpawnCurveGenerator();
        }

        public void GenerateEnemiesEvent<T>(SelectableLevel currentLevel, float currentRate, float multiplier = 1)
            where T : EnemyAI
        {
            if (multiplier < 1 || multiplier > 3)
                multiplier = 1;

            currentRate = currentRate == 0 ? 1 : currentRate;
            var rarity = new NormalDistributionService().CalculateRarity(currentLevel, currentRate);

            for (int i = 0; i < currentLevel.Enemies.Count; i++)
            {
                var enemy = currentLevel.Enemies[i];
                if (enemy.enemyType.enemyPrefab.GetComponent<T>() != null)
                {
                    enemy.rarity += (int)rarity * Configuration.enemyRaritys[enemy];
                    enemy.enemyType.MaxCount += (int)(multiplier * rarity);
                }
            }
        }

        public void NormalizeEnemiesRarity(SelectableLevel currentLevel, List<string> typesEnemyCollection = null)
        {
            if (!Configuration.OriginalEnemiesRarities.ContainsKey(currentLevel))
            {
                var originalEnemies = new List<SpawnableEnemyWithRarity>(Configuration.Enemies[currentLevel]
                    .Select(e => new SpawnableEnemyWithRarity
                    {
                        enemyType = e.enemyType,
                        rarity = e.rarity
                    }));
                Configuration.OriginalEnemiesRarities[currentLevel] = originalEnemies;
            }

            var originalRarities = Configuration.OriginalEnemiesRarities[currentLevel];
            foreach (var enemy in currentLevel.Enemies)
            {
                if (typesEnemyCollection == null || typesEnemyCollection.Contains(enemy.enemyType.enemyName))
                {
                    var originalEnemy = originalRarities
                        .FirstOrDefault(x => x.enemyType.enemyName == enemy.enemyType.enemyName);
                    if (originalEnemy != null)
                    {
                        enemy.rarity = originalEnemy.rarity;
                    }
                }
            }
        }

        public void ResetEnemiesRarity(SelectableLevel currentLevel,
            List<string> typesEnemyCollection = default)
        {
            // Преобразование списка в HashSet для улучшения производительности поиска
            var typesEnemySet = typesEnemyCollection != null 
                ? new HashSet<string>(typesEnemyCollection) : null;

            foreach (var enemy in currentLevel.Enemies)
            {
                // Проверяем, присутствует ли тип врага в заданном списке, если список предоставлен
                if (typesEnemySet == null || typesEnemySet.Contains(enemy.enemyType.enemyName))
                {
                    enemy.rarity = 0;
                }
            }
        }

        public void SetupRateLimit(SelectableLevel currentLevel, EventEnum eventEnum, Config config)
        {
            // Инициализация RateLevelValue для текущего уровня, если это еще не сделано
            if (!Configuration.RarityLevelValue.ContainsKey(currentLevel))
                Configuration.RarityLevelValue.Add(currentLevel, 0);

            // Убедиться, что словарь Enemies инициализирован
            if (Configuration.Enemies == null)
                Configuration.Enemies = new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();

            // Заполнение списка врагов для текущего уровня, если он еще не заполнен
            if (!Configuration.Enemies.ContainsKey(currentLevel))
            {
                Configuration.mls.LogInfo($"Initializing enemies for level {currentLevel?.name ?? "NULL"}");
                var enemyList = currentLevel?.Enemies?
                    .Select(enemy => new SpawnableEnemyWithRarity
                    {
                        enemyType = enemy.enemyType,
                        rarity = enemy.rarity,
                    }).ToList() ?? new List<SpawnableEnemyWithRarity>();

                Configuration.Enemies[currentLevel] = enemyList;
            }

            // Обновление редкости врагов на текущем уровне
            Configuration.Enemies[currentLevel] = Configuration.Enemies[currentLevel]
                .Select(enemy => new SpawnableEnemyWithRarity
                {
                    enemyType = enemy.enemyType,
                    rarity = enemy.rarity,
                }).ToList();

            // Обновление и нормализация RateLevelValue для всех уровней
            foreach (var level in Configuration.RarityLevelValue.Keys.ToList())
            {
                if (Configuration.RarityLevelValue.TryGetValue(level, out var rate))
                {
                    var newRarityValue = Mathf.Clamp(rate, config.RarityMin.Value, config.RarityMax.Value);

                    if (eventEnum == EventEnum.ResetEvent || eventEnum == EventEnum.All)
                        Configuration.RarityLevelValue[level] = 0;
                    else
                        Configuration.RarityLevelValue[level] = newRarityValue;
                }
            }
        }

        public SelectableLevel SetupLevelScrap(SelectableLevel newLevel, Config config)
        {
            if (!Configuration.levelsModified.Contains(newLevel))
            {
                Configuration.levelsModified.Add(newLevel);

                ModifyDaySettings(newLevel, config);
            }

            return newLevel;
        }

        public void ModifyDaySettings(SelectableLevel level, Config config)
        {
            level.maxScrap += AdditionalMaxScrap;
            level.maxTotalScrapValue += (int)Random.Range(config.MinScrap.Value, config.MaxScrap.Value);
            level.maxEnemyPowerCount += MaxEnemyPowerIncrement;
            level.maxOutsideEnemyPowerCount += MaxOutsideEnemyPowerIncrement;
            level.maxDaytimeEnemyPowerCount += MaxDaytimeEnemyPowerIncrement;
        }

        public void ShowEnemyRarirty(SelectableLevel newLevel, float currentEventRate)
        {
            Configuration.mls.LogWarning($"|{new string('-', 4)}All ENEMIES RARITY{new string('-', 4)}|");
            Configuration.mls.LogWarning($"| Current Rate: {currentEventRate}%         |");
            foreach (var spawnableEnemy in newLevel.Enemies)
            {
                Configuration.mls.LogInfo(
                    $"| {spawnableEnemy.enemyType.enemyName,17} : {spawnableEnemy.rarity,-3}% |");
            }

            Configuration.mls.LogWarning($"|{new string('-', 26)}|");
        }

        public SelectableLevel SetupEnemyChance(SelectableLevel newLevel, float currentEventRate, float multiplier = 1)
        {
            if (multiplier < 1 || multiplier > 3)
                multiplier = 1;

            newLevel.daytimeEnemySpawnChanceThroughDay = _curveGenerator.CreateEnemySpawnCurve(
                new[] { 0f, 1f * multiplier },
                new[] { 5f, 25f + currentEventRate * multiplier });

            newLevel.enemySpawnChanceThroughoutDay = _curveGenerator.CreateEnemySpawnCurve(
                new[] { 0f, 1f * multiplier },
                new[] { 0.4f + currentEventRate, 33f + currentEventRate * multiplier });

            newLevel.outsideEnemySpawnChanceThroughDay = _curveGenerator.CreateEnemySpawnCurve(
                new[] { 0f, -10f },
                new[] { 0.8f + currentEventRate, 10f + currentEventRate });

            return newLevel;
        }
    }
}