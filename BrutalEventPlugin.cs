using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;

using BepInEx;
using BrutalEvent.Common;
using BrutalEvent.Enums;
using BrutalEvent.Models;
using BrutalEvent.MonoBehaviours;
using BrutalEvent.Service;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BrutalEvent
{
    // TODO Review this file and update to your own requirements.

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class BrutalEventPlugin : BaseUnityPlugin
    {
        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // Liquid.BrutalEvent
        // Version should be a valid version string.
        // e.g. 1.0.0
        public static BrutalEventPlugin Instance { get; set; }

        private const string MyGUID = "Liquid.BrutalEvent";
        private const string PluginName = "BrutalEvent";
        private const string VersionString = "1.0.0";
        private Harmony _harmony = new Harmony("BrutalEvent");
        private static Configuration _config;

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            Instance = this;
            _config = new LoadConfig().BindConfigSettings();

            EventConfiguration.mls = BepInEx.Logging.Logger.CreateLogSource("BrutalEvent");
            _harmony.PatchAll(typeof(BrutalEventPlugin));
            EventConfiguration.mls = base.Logger;
            EventConfiguration.mls.LogInfo("Loaded NEW Brutal Company and applying patches.");
        }

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        private static bool ModifyLevel(ref SelectableLevel newLevel)
        {
            ShowLogo();
            if (RoundManager.Instance.IsHost)
            {
                QuotaAjuster.CleanupAllSpawns();
                // Generate new random game event
                var eventEnum = (GameEvent)Random.Range(0, Enum.GetValues(typeof(GameEvent)).Length);
                eventEnum = GameEvent.Hoarding;

                // Setup rarity rate
                EventConfiguration.levelHeatVal.TryGetValue(newLevel, out float currentEventRate);
                EventConfiguration.mls.LogInfo("NormalizeEnemiesRarity");
                NormalizeEnemiesRarity(newLevel);
                SetupRateLimit(newLevel, eventEnum, _config);
                EventConfiguration.mls.LogInfo("ShowEnemyRarirty");
                ShowEnemyRarirty(newLevel, currentEventRate);

                // Setup Spawnable enemies
                foreach (var enemy in newLevel.Enemies)
                {
                    if (!EventConfiguration.enemyRaritys.ContainsKey(enemy))
                        EventConfiguration.enemyRaritys.Add(enemy, enemy.rarity);
                    enemy.rarity = EventConfiguration.enemyRaritys[enemy];

                    if (!EventConfiguration.enemyPropCurves.ContainsKey(enemy))
                        EventConfiguration.enemyPropCurves.Add(enemy, enemy.enemyType.probabilityCurve);
                    enemy.enemyType.probabilityCurve = EventConfiguration.enemyPropCurves[enemy];
                }
                HUDManager.Instance.AddTextToChatOnServer($"<color=orange>MOON IS AT {currentEventRate} %RATE</color>");

                if (currentEventRate > _config.limitRate.Value)
                {
                    HUDManager.Instance.AddTextToChatOnServer(
                        $"<color=red>HEAT LEVEL IS DANGEROUSLY HIGH : {currentEventRate} " +
                        "<color=white>\nVISIT OTHER MOONS TO LOWER HEAT LEVEL.</color>");
                }

                if (newLevel.sceneName == "CompanyBuilding")
                    eventEnum = GameEvent.None;

                switch (eventEnum)
                {
                    case GameEvent.None:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: NONE</color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=green>Level event: NONE</color>");
                        break;
                    case GameEvent.Turret:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: TURRET HELL</color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: TURRET HELL</color>");
                        foreach (var spawnObj in newLevel.spawnableMapObjects)
                        {
                            if (spawnObj.prefabToSpawn.GetComponentInChildren<Turret>() != null)
                            {
                                EventConfiguration.mls.LogInfo($"{spawnObj.prefabToSpawn.name}");
                                QuotaAjuster.turret = spawnObj.prefabToSpawn;

                                spawnObj.numberToSpawn = new AnimationCurve(new[]
                                {
                                    new Keyframe(0f, 5f),
                                    new Keyframe(1f, 20f)
                                });
                            }
                        }
                        break;
                    case GameEvent.Landmine:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: LANDMINE HELL</color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: LANDMINE HELL</color>");
                        foreach (var spawnObj in newLevel.spawnableMapObjects)
                        {
                            if (spawnObj.prefabToSpawn.GetComponentInChildren<Landmine>() != null)
                            {
                                QuotaAjuster.landmine = spawnObj.prefabToSpawn;
                                spawnObj.numberToSpawn = new AnimationCurve(new[]
                                {
                                    new Keyframe(0f, 5f),
                                    new Keyframe(1f, 50f)
                                });
                            }
                        }
                        break;
                    case GameEvent.Hoarding:
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: HOARDER TOWN</color>", -1);
                        var collection = new List<string>() 
                            { nameof(DressGirlAI), nameof(FlowermanAI), nameof(CrawlerAI) };
                        EventConfiguration.mls.LogInfo("Hoarding-ResetEnemiesRarity");
                        ResetEnemiesRarity(newLevel, collection);
                        EventConfiguration.mls.LogInfo("GenerateEnemiesEvent");
                        GenerateEnemiesEvent<HoarderBugAI>(newLevel, currentEventRate);
                        break;
                    case GameEvent.Lasso:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: Lasso </color>");
                        GenerateEnemiesEvent<SpringManAI>(newLevel, currentEventRate);
                        break;
                    case GameEvent.Unfair:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: Unfair </color>");
                        break;
                    case GameEvent.OopsAllSnareFlea:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: OopsAllSnareFlea </color>");
                        HUDManager.Instance.AddTextToChatOnServer(
                            "<color=red>Level event: OOPS, ALL SNARE FLEAS!</color>");
                        ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<CentipedeAI>(newLevel, currentEventRate);
                        break;
                    case GameEvent.BrackenAndCoil:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: BrackenAndCoil </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: BREACKE AND COIL</color>");
                        ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<FlowermanAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<SpringManAI>(newLevel, currentEventRate);
                        break;
                    case GameEvent.Chaos:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: Chaos </color>");
                        HUDManager.Instance.AddTextToChatOnServer(
                            "<color=red>Level event: THE WORST CHAOS IN THE WORLD</color>");
                        ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<CrawlerAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<FlowermanAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<SpringManAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<SandSpiderAI>(newLevel, currentEventRate);
                        break;
                    case GameEvent.GoToRend:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: DOGS DOGS DOGS </color>");
                        GenerateEnemiesEvent<MouthDogAI>(newLevel, currentEventRate);
                        break;
                    case GameEvent.All:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: ALL </color>");
                        break;
                    case GameEvent.Delivery:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: Delivery </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=green>Level event: DELIVERY!</color>");
                        int itemCount = Random.Range(3, 6);
                        for (int i = 0; i < itemCount; i++)
                        {
                            int itemIndex = Random.Range(0, 6);
                            FindObjectOfType<Terminal>().orderedItemsFromTerminal.Add(itemIndex);
                        }
                        break;
                    case GameEvent.ReplaceItems:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: REPLACE ITEMS </color>");
                        HUDManager.Instance.AddTextToChatOnServer(
                            "<color=red>Level event: REPLACE ITEMS</color>");
                        var terminal = FindObjectOfType<Terminal>();

                        int item = 1;
                        if (terminal.orderedItemsFromTerminal.Count > 1)
                            item = terminal.orderedItemsFromTerminal.Count;
                        terminal.orderedItemsFromTerminal.Clear();
                        for (int i = 0; i < item; i++)
                        {
                            terminal.orderedItemsFromTerminal.Add(Random.Range(0, 6));
                        }
                        break;
                    case GameEvent.Schizophrenia:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: SCHIZOPHRENIA </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: SCHIZOPHRENIA</color>");
                        ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<BlobAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<DressGirlAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<HoarderBugAI>(newLevel, currentEventRate);
                        break;
                    case GameEvent.ResetEventRate:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: RESET ALL HEAT </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=green>Level event: RESET ALL HEAT</color>");
                        currentEventRate = 0;
                        ResetEnemiesRarity(newLevel);
                        break;
                    case GameEvent.SpawnTurret:
                        EventConfiguration.mls.LogInfo("<color=green>Level event: SpawnTurret </color>");
                        HUDManager.Instance.AddTextToChatOnServer(
                            "<color=red>Level event: THE COMPANIES AUTOMATED DEFENSE SYSTEM</color>");
                        break;
                    default:
                        break;
                }
                newLevel = SetupLevelScrap(newLevel,_config);
                newLevel = SetupEnemyChance(newLevel, eventEnum, currentEventRate);

                EventConfiguration.levelHeatVal.TryGetValue(newLevel, out currentEventRate);
                EventConfiguration.levelHeatVal[newLevel] = Mathf.Clamp(currentEventRate + 15f, 0f, _config.maxRate.Value);

                if (!newLevel.sceneName.Contains("CompanyBuilding"))
                {
                    var terminalCredits = FindObjectOfType<Terminal>();
                    terminalCredits.groupCredits += _config.startCredits.Value;

                    terminalCredits.SyncGroupCreditsServerRpc(terminalCredits.groupCredits,
                        terminalCredits.numberOfItemsInDropship);
                }

                ShowEnemyRarirty(newLevel, currentEventRate);
            }
            return true;
        }

        public static SelectableLevel SetupEnemyChance(SelectableLevel newLevel, GameEvent eventEnum,
            float currentEventRate)
        {
            newLevel.enemySpawnChanceThroughoutDay = new AnimationCurve(new[]
            {
                new Keyframe(0f, 0.1f + currentEventRate),
                new Keyframe(1f, 33f + currentEventRate)
            });

            newLevel.outsideEnemySpawnChanceThroughDay = new AnimationCurve(new[]
            {
                new Keyframe(0f, -10f + currentEventRate),
                new Keyframe(1f, 10f + currentEventRate),
            });

            return newLevel;
        }

        private static SelectableLevel SetupLevelScrap(SelectableLevel newLevel, Configuration config)
        {
            if (!EventConfiguration.levelsModified.Contains(newLevel))
            {
                EventConfiguration.levelsModified.Add(newLevel);

                newLevel.maxScrap += 45;
                newLevel.maxTotalScrapValue += (int)Random.Range(config.minScrap.Value, config.maxScrap.Value);
                newLevel.daytimeEnemySpawnChanceThroughDay = new AnimationCurve(new[]
                {
                    new Keyframe(0f, 7f),
                    new Keyframe(0.5f, 15f)
                });

                newLevel.maxEnemyPowerCount += 10; //8
                newLevel.maxOutsideEnemyPowerCount += 5; //15
                newLevel.maxDaytimeEnemyPowerCount += 10; //20
            }

            return newLevel;
        }

        private static void ShowEnemyRarirty(SelectableLevel newLevel, float currentEventRate)
        {
            EventConfiguration.mls.LogWarning($"|{new string('-', 4)}All ENEMIES RARITY{new string('-', 4)}|");
            EventConfiguration.mls.LogWarning($"| Current Rate: {currentEventRate}%         |");
            foreach (var spawnableEnemy in newLevel.Enemies)
            {
                EventConfiguration.mls.LogInfo(
                    $"| {spawnableEnemy.enemyType.enemyName,17} : {spawnableEnemy.rarity,-3}% |");
            }

            EventConfiguration.mls.LogWarning($"|{new string('-', 26)}|");
        }

        private static void ResetEnemiesRarity(SelectableLevel currentLevel,
            List<string> typesEnemyCollection = default)
        {
            // Сброс редкости
            if (typesEnemyCollection != null && typesEnemyCollection.Any())
            {
                foreach (var enemy in currentLevel.Enemies)
                {
                    if (typesEnemyCollection.Any(x => x.Contains(enemy.enemyType.enemyName)))
                        enemy.rarity = 0;
                }
            }
            else
            {
                foreach (var enemy in currentLevel.Enemies)
                {
                    enemy.rarity = 0;
                }
            }
        }

        private static void NormalizeEnemiesRarity(SelectableLevel currentLevel, List<string> typesEnemyCollection = default)
        {
            if (!EventConfiguration.OriginalEnemiesRarities.ContainsKey(currentLevel))
                EventConfiguration.OriginalEnemiesRarities.Add(currentLevel, default);

            if (EventConfiguration.OriginalEnemiesRarities == null &&
                !EventConfiguration.OriginalEnemiesRarities.Any())
            {
                // Сохранение исходных данных перед сбросом редкости
                var originalEnemies = new List<SpawnableEnemyWithRarity>(EventConfiguration.Enemies[currentLevel]
                    .Select(e => new SpawnableEnemyWithRarity
                    {
                        enemyType = e.enemyType,
                        rarity = e.rarity
                    }));

                // Сохранение оригинальных значений редкости
                EventConfiguration.OriginalEnemiesRarities[currentLevel] = originalEnemies;
            }

            if (typesEnemyCollection != null && typesEnemyCollection.Any())
                foreach (var enemy in currentLevel.Enemies)
                {
                    var originalEnemy = EventConfiguration.OriginalEnemiesRarities[currentLevel]
                        .FirstOrDefault(x => x.enemyType.enemyName == enemy.enemyType.enemyName);
                    if (originalEnemy != null)
                    {
                        enemy.rarity = originalEnemy.rarity;
                    }
                }
            else
                currentLevel.Enemies = EventConfiguration.OriginalEnemiesRarities[currentLevel];
        }

        private static void GenerateEnemiesEvent<T>(SelectableLevel currentLevel, float currentRate)
            where T : EnemyAI
        {
            currentRate = currentRate == 0 ? 1 : currentRate;
            var rarity = new NormalDistributionService().CalculateRarity(currentLevel, currentRate);

            foreach (var enemy in currentLevel.Enemies)
            {
                if (enemy.enemyType.enemyPrefab.GetComponent<T>() != null)
                {
                    if (EventConfiguration.enemyRaritys.TryGetValue(enemy, out var existingRarity))
                    {
                        enemy.rarity += (int)rarity + existingRarity;
                    }
                    else
                    {
                        // Обработка случая, когда редкость врага не найдена в EventConfiguration.enemyRaritys
                        // Например, можно просто увеличить редкость на значение rarity
                        enemy.rarity += (int)rarity;
                    }
                }
            }
        }

        public static void SetupRateLimit(SelectableLevel currentLevel, GameEvent eventEnum, Configuration config)
        {
            if (!EventConfiguration.levelHeatVal.ContainsKey(currentLevel))
                EventConfiguration.levelHeatVal.Add(currentLevel, 0);

            EventConfiguration.mls.LogInfo("Enemies");
            if (EventConfiguration.Enemies == null) 
                EventConfiguration.Enemies = new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();

            EventConfiguration.mls.LogInfo("Start");
            if (!EventConfiguration.Enemies.ContainsKey(currentLevel))
            {
                if(currentLevel == null)
                    EventConfiguration.mls.LogInfo($"Level NULL");
                if (currentLevel.Enemies == null)
                    EventConfiguration.mls.LogInfo($"Enemies NULL");
                
                var enemyList = new List<SpawnableEnemyWithRarity>();
                foreach (var enemy in currentLevel.Enemies)
                {
                    EventConfiguration.mls.LogInfo("item");
                    var item = new SpawnableEnemyWithRarity()
                    {
                        enemyType = enemy.enemyType,
                        rarity = enemy.rarity,
                    };
                    enemyList.Add(item);
                }

                EventConfiguration.Enemies.Add(currentLevel, enemyList);
            }
            EventConfiguration.mls.LogInfo("Mid");
            EventConfiguration.Enemies.TryGetValue(currentLevel, out var enemies);
            currentLevel.Enemies = enemies;

            foreach (var level in EventConfiguration.levelHeatVal.Keys.ToList())
            {
                EventConfiguration.mls.LogInfo("config");
                EventConfiguration.levelHeatVal.TryGetValue(level, out var rate);
                var someValue = Mathf.Clamp(rate, config.rarityMin.Value, config.rarityMax.Value);
                EventConfiguration.levelHeatVal[level] = someValue;

                if (eventEnum == GameEvent.ResetEventRate || eventEnum == GameEvent.All)
                    EventConfiguration.levelHeatVal[level] = 0;
            }
        }

        public static void ShowLogo()
        {
            EventConfiguration.mls.LogInfo(@"
  _         ____               _____      ____    _____    _    _   _______              _          ______  __      __  ______   _   _   _______ 
 | |       / __ \      /\     |  __ \    |  _ \  |  __ \  | |  | | |__   __|     /\     | |        |  ____| \ \    / / |  ____| | \ | | |__   __|
 | |      | |  | |    /  \    | |  | |   | |_) | | |__) | | |  | |    | |       /  \    | |        | |__     \ \  / /  | |__    |  \| |    | |   
 | |      | |  | |   / /\ \   | |  | |   |  _ <  |  _  /  | |  | |    | |      / /\ \   | |        |  __|     \ \/ /   |  __|   | . ` |    | |   
 | |____  | |__| |  / ____ \  | |__| |   | |_) | | | \ \  | |__| |    | |     / ____ \  | |____    | |____     \  /    | |____  | |\  |    | |   
 |______|  \____/  /_/    \_\ |_____/    |____/  |_|  \_\  \____/     |_|    /_/    \_\ |______|   |______|     \/     |______| |_| \_|    |_|   
                                                                                                                                                 
                                                                                                                                                 
");
        }

        public static void OnDestroy()
        {
            if (!EventConfiguration.loaded)
            {
                GameObject gameObject = new GameObject("QuotaChanger");
                DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<QuotaAjuster>();
                EventConfiguration.loaded = true;
            }
        }

        /// <summary>
        /// Code executed every frame. See below for an example use case
        /// to detect keypress via custom configuration.
        /// </summary>
        // TODO - Add your code here or remove this section if not required.
        private static void Update()
        {
        }
    }
}