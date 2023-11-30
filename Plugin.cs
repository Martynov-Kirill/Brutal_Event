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
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // Liquid.BrutalEvent
        // Version should be a valid version string.
        // e.g. 1.0.0
        public static Plugin Instance { get; set; }

        private const string MyGUID = "Liquid.BetterBrutalEvent";
        private const string PluginName = "BetterBrutalEvent";
        private const string VersionString = "1.0.0";
        private Harmony _harmony = new Harmony("BetterBrutalEvent");

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
                Configuration.mls.LogWarning(" YOU ARE NOT A HOST! (>_<)");
                Configuration.mls.LogWarning(" BRUTAL EVENT DIDN'T LOADING");
                return false;
            }

            MonoBehaviours.QuotaAjuster.CleanupAllSpawns();
            Configuration.RarityLevelValue.TryGetValue(newLevel, out float currentEventRate);

            Configuration.mls.LogInfo("NORMALIZATION ENEMIES");
            _masterEnviroment.NormalizeEnemiesRarity(newLevel);

            Configuration.mls.LogInfo("GENERATE EVENT");
            // Оптимизация выбора события
            var currentEvent = newLevel.sceneName == "CompanyBuilding"
                ? _eventFactory.CreateEvent(EventEnum.None)
                : _eventFactory.GetRandomEvent();

            _masterEnviroment.SetupRateLimit(newLevel, _eventFactory.EventEnum, _config);

            //// Оптимизация обработки врагов
            //foreach (var enemy in newLevel.Enemies)
            //{
            //    enemy.rarity = Configuration.enemyRaritys.TryGetValue(enemy, out var rarity) ? rarity : enemy.rarity;
            //    enemy.enemyType.probabilityCurve = Configuration.enemyPropCurves.TryGetValue(enemy, out var curve) ? curve : enemy.enemyType.probabilityCurve;
            //}

            UpdateHUDAndCredits(newLevel, currentEventRate);

            Configuration.mls.LogInfo("SETUP SCRAP LIMIT");
            _masterEnviroment.SetupLevelScrap(newLevel, _config);

            //Configuration.mls.LogInfo("SETUP ENEMY CHANCE");
            //_masterEnviroment.SetupEnemyChance(newLevel, currentEventRate);

            Configuration.mls.LogInfo("ON LOAD NEW LEVEL");
            currentEvent.OnLoadNewLevel(ref newLevel, _config, currentEventRate);

            Configuration.mls.LogInfo("RARITY BY LEVEL[newLevel]");
            Configuration.RarityLevelValue[newLevel] = Mathf.Clamp(currentEventRate + 15f, 0f, _config.MaxRate.Value);

            HUDManager.Instance.AddTextToChatOnServer($"{currentEvent.GetEventName()}");
            Configuration.mls.LogWarning($"EVENT : {currentEvent.GetEventName()}");

            Configuration.mls.LogInfo("ShowEnemyRarirty");
            _masterEnviroment.ShowEnemyRarirty(newLevel, currentEventRate);

            return true;
        }
        private static void UpdateHUDAndCredits(SelectableLevel newLevel, float currentEventRate)
        {
            HUDManager.Instance.AddTextToChatOnServer($"<color=yellow>MOON IS AT {currentEventRate} %RATE</color>");
            
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
______      _   _             ______            _        _   _____                _   
| ___ \    | | | |            | ___ \          | |      | | |  ___|              | |  
| |_/ / ___| |_| |_ ___ _ __  | |_/ /_ __ _   _| |_ __ _| | | |____   _____ _ __ | |_ 
| ___ \/ _ \ __| __/ _ \ '__| | ___ \ '__| | | | __/ _` | | |  __\ \ / / _ \ '_ \| __|
| |_/ /  __/ |_| ||  __/ |    | |_/ / |  | |_| | || (_| | | | |___\ V /  __/ | | | |_ 
\____/ \___|\__|\__\___|_|    \____/|_|   \__,_|\__\__,_|_| \____/ \_/ \___|_| |_|\__|
                                                                                      
                                                                                      
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