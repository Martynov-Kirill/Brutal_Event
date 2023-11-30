using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BrutalEvent.Common;
using BrutalEvent.Enums;
using BrutalEvent.Models;
using BrutalEvent.Services;
using BrutalEvent.Services.Abstract;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BrutalEvent
{
    // TODO Review this file and update to your own requirements.

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // Liquid.BrutalEvent
        // Version should be a valid version string.
        // e.g. 1.0.0
        public static Plugin Instance { get; set; }

        private const string MyGUID = "Liquid.BrutalEvent";
        private const string PluginName = "BrutalEvent";
        private const string VersionString = "1.0.0";
        private Harmony _harmony = new Harmony("BrutalEvent");
        public static Config _config;

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            Instance = this;
            _config = new LoadConfig().BindConfigSettings();

            Configuration.mls = BepInEx.Logging.Logger.CreateLogSource("BrutalEvent");
            _harmony.PatchAll(typeof(Plugin));
            Configuration.mls = base.Logger;
            Configuration.mls.LogInfo("Loaded NEW Brutal Company and applying patches.");
        }

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        private static bool ModifyLevel(ref SelectableLevel newLevel)
        {
            ShowLogo();

            if (RoundManager.Instance.IsHost)
            {
                MonoBehaviours.QuotaAjuster.CleanupAllSpawns();
                // Generate new random game event
                var eventEnum = (EventEnum)Random.Range(0, Enum.GetValues(typeof(EventEnum)).Length);
                eventEnum = EventEnum.Kleptomania;

                // Setup rarity rate
                Configuration.RarityLevelValue.TryGetValue(newLevel, out float currentEventRate);
                Configuration.mls.LogInfo("NormalizeEnemiesRarity");
                NormalizeEnemiesRarity(newLevel);
                SetupRateLimit(newLevel, eventEnum, _config);
                Configuration.mls.LogInfo("ShowEnemyRarirty");
                ShowEnemyRarirty(newLevel, currentEventRate);

                // Setup Spawnable enemies
                foreach (var enemy in newLevel.Enemies)
                {
                    if (!Configuration.enemyRaritys.ContainsKey(enemy))
                        Configuration.enemyRaritys.Add(enemy, enemy.rarity);
                    enemy.rarity = Configuration.enemyRaritys[enemy];

                    if (!Configuration.enemyPropCurves.ContainsKey(enemy))
                        Configuration.enemyPropCurves.Add(enemy, enemy.enemyType.probabilityCurve);
                    enemy.enemyType.probabilityCurve = Configuration.enemyPropCurves[enemy];
                }
                HUDManager.Instance.AddTextToChatOnServer($"<color=orange>MOON IS AT {currentEventRate} %RATE</color>");

                if (currentEventRate > _config.LimitRate.Value)
                {
                    HUDManager.Instance.AddTextToChatOnServer(
                        $"<color=red>HEAT LEVEL IS DANGEROUSLY HIGH : {currentEventRate} " +
                        "<color=white>\nVISIT OTHER MOONS TO LOWER HEAT LEVEL.</color>");
                }

                if (newLevel.sceneName == "CompanyBuilding")
                    eventEnum = EventEnum.None;

                switch (eventEnum)
                {
                    case EventEnum.None:
                        Configuration.mls.LogInfo("<color=green>Level event: NONE</color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=green>Level event: NONE</color>");
                        break;
                    case EventEnum.Turret:
                        Configuration.mls.LogInfo("<color=green>Level event: TURRET HELL</color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: TURRET HELL</color>");
                        foreach (var spawnObj in newLevel.spawnableMapObjects)
                        {
                            if (spawnObj.prefabToSpawn.GetComponentInChildren<Turret>() != null)
                            {
                                Configuration.mls.LogInfo($"{spawnObj.prefabToSpawn.name}");
                                MonoBehaviours.QuotaAjuster.turret = spawnObj.prefabToSpawn;

                                spawnObj.numberToSpawn = new AnimationCurve(new[]
                                {
                                    new Keyframe(0f, 5f),
                                    new Keyframe(1f, 20f)
                                });
                            }
                        }
                        break;
                    case EventEnum.Landmine:
                        Configuration.mls.LogInfo("<color=green>Level event: LANDMINE HELL</color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: LANDMINE HELL</color>");
                        foreach (var spawnObj in newLevel.spawnableMapObjects)
                        {
                            if (spawnObj.prefabToSpawn.GetComponentInChildren<Landmine>() != null)
                            {
                                MonoBehaviours.QuotaAjuster.landmine = spawnObj.prefabToSpawn;
                                spawnObj.numberToSpawn = new AnimationCurve(new[]
                                {
                                    new Keyframe(0f, 5f),
                                    new Keyframe(1f, 50f)
                                });
                            }
                        }
                        break;
                    case EventEnum.Kleptomania:
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: HOARDER TOWN</color>", -1);
                        var collection = new List<string>() 
                            { nameof(DressGirlAI), nameof(FlowermanAI), nameof(CrawlerAI) };
                        Configuration.mls.LogInfo("Hoarding-ResetEnemiesRarity");
                        MasterEnviroment.ResetEnemiesRarity(newLevel, collection);
                        Configuration.mls.LogInfo("GenerateEnemiesEvent");
                        GenerateEnemiesEvent<HoarderBugAI>(newLevel, currentEventRate);
                        break;
                    case EventEnum.Lasso:
                        Configuration.mls.LogInfo("<color=green>Level event: Lasso </color>");
                        GenerateEnemiesEvent<SpringManAI>(newLevel, currentEventRate);
                        break;
                    case EventEnum.Unfair:
                        Configuration.mls.LogInfo("<color=green>Level event: Unfair </color>");
                        break;
                    case EventEnum.AllSnareFlea:
                        Configuration.mls.LogInfo("<color=green>Level event: OopsAllSnareFlea </color>");
                        HUDManager.Instance.AddTextToChatOnServer(
                            "<color=red>Level event: OOPS, ALL SNARE FLEAS!</color>");
                        MasterEnviroment.ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<CentipedeAI>(newLevel, currentEventRate);
                        break;
                    case EventEnum.BrackenAndCoil:
                        Configuration.mls.LogInfo("<color=green>Level event: BrackenAndCoil </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: BREACKE AND COIL</color>");
                        MasterEnviroment.ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<FlowermanAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<SpringManAI>(newLevel, currentEventRate);
                        break;
                    case EventEnum.Chaos:
                        Configuration.mls.LogInfo("<color=green>Level event: Chaos </color>");
                        HUDManager.Instance.AddTextToChatOnServer(
                            "<color=red>Level event: THE WORST CHAOS IN THE WORLD</color>");
                        MasterEnviroment.ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<CrawlerAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<FlowermanAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<SpringManAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<SandSpiderAI>(newLevel, currentEventRate);
                        break;
                    case EventEnum.GoToRend:
                        Configuration.mls.LogInfo("<color=green>Level event: DOGS DOGS DOGS </color>");
                        GenerateEnemiesEvent<MouthDogAI>(newLevel, currentEventRate);
                        break;
                    case EventEnum.All:
                        Configuration.mls.LogInfo("<color=green>Level event: ALL </color>");
                        break;
                    case EventEnum.Delivery:
                        Configuration.mls.LogInfo("<color=green>Level event: Delivery </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=green>Level event: DELIVERY!</color>");
                        int itemCount = Random.Range(3, 6);
                        for (int i = 0; i < itemCount; i++)
                        {
                            int itemIndex = Random.Range(0, 6);
                            FindObjectOfType<Terminal>().orderedItemsFromTerminal.Add(itemIndex);
                        }
                        break;
                    case EventEnum.ReplaceItems:
                        Configuration.mls.LogInfo("<color=green>Level event: REPLACE ITEMS </color>");
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
                    case EventEnum.DidYouSeeHer:
                        Configuration.mls.LogInfo("<color=green>Level event: SCHIZOPHRENIA </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=red>Level event: SCHIZOPHRENIA</color>");
                        MasterEnviroment.ResetEnemiesRarity(newLevel);
                        GenerateEnemiesEvent<BlobAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<DressGirlAI>(newLevel, currentEventRate);
                        GenerateEnemiesEvent<HoarderBugAI>(newLevel, currentEventRate);
                        break;
                    case EventEnum.ResetEvent:
                        Configuration.mls.LogInfo("<color=green>Level event: RESET ALL HEAT </color>");
                        HUDManager.Instance.AddTextToChatOnServer("<color=green>Level event: RESET ALL HEAT</color>");
                        currentEventRate = 0;
                        MasterEnviroment.ResetEnemiesRarity(newLevel);
                        break;
                    case EventEnum.SecuritySystem:
                        Configuration.mls.LogInfo("<color=green>Level event: SpawnTurret </color>");
                        HUDManager.Instance.AddTextToChatOnServer(
                            "<color=red>Level event: THE COMPANIES AUTOMATED DEFENSE SYSTEM</color>");
                        break;
                    default:
                        break;
                }
                newLevel = MasterEnviroment.SetupLevelScrap(newLevel,_config);
                newLevel = SetupEnemyChance(newLevel, eventEnum, currentEventRate);

                Configuration.RarityLevelValue.TryGetValue(newLevel, out currentEventRate);
                Configuration.RarityLevelValue[newLevel] = Mathf.Clamp(currentEventRate + 15f, 0f, _config.MaxRate.Value);

                if (!newLevel.sceneName.Contains("CompanyBuilding"))
                {
                    var terminalCredits = FindObjectOfType<Terminal>();
                    terminalCredits.groupCredits += _config.AddCredits.Value;

                    terminalCredits.SyncGroupCreditsServerRpc(terminalCredits.groupCredits,
                        terminalCredits.numberOfItemsInDropship);
                }

                ShowEnemyRarirty(newLevel, currentEventRate);
            }
            return true;
        }

        public static SelectableLevel SetupEnemyChance(SelectableLevel newLevel, EventEnum eventEnum,
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

        private static void ShowEnemyRarirty(SelectableLevel newLevel, float currentEventRate)
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

       
       

        public static void ShowLogo()
        {
            Configuration.mls.LogInfo(@"
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
            if (!Configuration.loaded)
            {
                GameObject gameObject = new GameObject("QuotaChanger");
                DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<MonoBehaviours.QuotaAjuster>();
                Configuration.loaded = true;
            }
        }

        /// <summary>
        /// Code executed every frame. See below for an example use case
        /// to detect keypress via custom configuration.
        /// </summary>
        private static void Update()
        {
        }
    }
}