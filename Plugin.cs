using BepInEx;
using BrutalEvent.Common;
using BrutalEvent.Enums;
using BrutalEvent.Models;
using BrutalEvent.Services;
using BrutalEvent.Services.Abstract;
using HarmonyLib;
using UnityEngine;

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
        private static EventFactory _eventFactory;
        private static MasterEnviroment _masterEnviroment;

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            Instance = this;
            _config = new LoadConfig().BindConfigSettings();
            _eventFactory = new EventFactory();
            _masterEnviroment = new MasterEnviroment();

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

            if (RoundManager.Instance.IsHost == false)
            {
                Configuration.mls.LogWarning(" BRUTAL EVENT NOT A LOADING");
                Configuration.mls.LogWarning(" YOU ARE NOT A HOST! (>_<)");
                return false;
            }

            MonoBehaviours.QuotaAjuster.CleanupAllSpawns();
            Configuration.RarityLevelValue.TryGetValue(newLevel, out float currentEventRate);
            Configuration.mls.LogInfo("NormalizeEnemiesRarity");

            _masterEnviroment.NormalizeEnemiesRarity(newLevel);

            // Оптимизация выбора события
            var currentEvent = newLevel.sceneName == "CompanyBuilding"
                ? _eventFactory.CreateEvent(EventEnum.None)
                : _eventFactory.GetRandomEvent();

            currentEvent.OnLoadNewLevel(ref newLevel, _config);
            _masterEnviroment.SetupRateLimit(newLevel, _eventFactory.EventEnum, _config);

            // Оптимизация обработки врагов
            foreach (var enemy in newLevel.Enemies)
            {
                enemy.rarity = Configuration.enemyRaritys.TryGetValue(enemy, out var rarity) ? rarity : enemy.rarity;
                enemy.enemyType.probabilityCurve = Configuration.enemyPropCurves.TryGetValue(enemy, out var curve) ? curve : enemy.enemyType.probabilityCurve;
            }

            UpdateHUDAndCredits(newLevel, currentEventRate);

            _masterEnviroment.SetupLevelScrap(newLevel, _config);
            _masterEnviroment.SetupEnemyChance(newLevel, currentEventRate);

            Configuration.RarityLevelValue[newLevel] = Mathf.Clamp(currentEventRate + 15f, 0f, _config.MaxRate.Value);
            _masterEnviroment.ShowEnemyRarirty(newLevel, currentEventRate);

            Configuration.mls.LogInfo("ShowEnemyRarirty");
            _masterEnviroment.ShowEnemyRarirty(newLevel, currentEventRate);

            return true;
        }
        private static void UpdateHUDAndCredits(SelectableLevel newLevel, float currentEventRate)
        {
            HUDManager.Instance.AddTextToChatOnServer($"<color=orange>MOON IS AT {currentEventRate} %RATE</color>");

            if (currentEventRate > _config.LimitRate.Value)
            {
                HUDManager.Instance.AddTextToChatOnServer(
                    $"<color=red>HEAT LEVEL IS DANGEROUSLY HIGH : {currentEventRate} " +
                    "<color=white>\nVISIT OTHER MOONS TO LOWER HEAT LEVEL.</color>");
            }

            if (!newLevel.sceneName.Contains("CompanyBuilding"))
            {
                // Улучшенная проверка на null
                var terminalCredits = FindObjectOfType<Terminal>() ?? new Terminal();
                terminalCredits.groupCredits += _config.AddCredits.Value;
                terminalCredits.SyncGroupCreditsServerRpc(terminalCredits.groupCredits,
                    terminalCredits.numberOfItemsInDropship);
            }
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

        /// <summary>
        /// Code executed every frame. See below for an example use case
        /// to detect keypress via custom configuration.
        /// </summary>
        private static void Update()
        {
        }
    }
}