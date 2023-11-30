using BrutalEvent.Models;

namespace BrutalEvent.Common
{
    public class LoadConfig
    {
        private string _sectionName = "BrutalEventSettings";
        private string _subSectionName = "QuotaAjuster";
        public Config BindConfigSettings()
        {
            var config = new Config();
            config.MinScrap = Plugin.Instance.Config
                .Bind(_sectionName, "MinScrap", 250f, "MinScrap");
            config.MaxScrap = Plugin.Instance.Config
                .Bind(_sectionName, "MaxScrap", 650f, "MaxScrap");
            config.LimitRate = Plugin.Instance.Config
                .Bind(_sectionName, "LimitRate", 50f, "LimitRate");
            config.MaxRate = Plugin.Instance.Config
                .Bind(_sectionName, "MaxRate", 100f, "MaxRate");
            config.RarityMin = Plugin.Instance.Config
                .Bind(_sectionName, "RarityMin", 10f, "RarityMin");
            config.RarityMax = Plugin.Instance.Config
                .Bind(_sectionName, "RarityMax", 100f, "RarityMax");
            config.AddCredits = Plugin.Instance.Config
                .Bind(_sectionName, "AddCredits", 45, "Additional credits on each mission");
            config.StartingQuota = Plugin.Instance.Config.Bind($"{_sectionName}.{_subSectionName}", "StartingQuota", 299, "StartingQuota Value");
            config.StartingCredits = Plugin.Instance.Config.Bind($"{_sectionName}.{_subSectionName}", "StartingCredits", 150, "StartingCredits Value");
            config.BaseIncrease = Plugin.Instance.Config.Bind($"{_sectionName}.{_subSectionName}", "BaseIncrease", 51f, "Increase Quota value each round");
            config.Multiplier = Plugin.Instance.Config.Bind($"{_sectionName}.{_subSectionName}", "RandomizerMultiplier", 1.0f, "Multiplier spawnable chance Value [1.0 .. 3.0]");
            config.DeadlineDaysAmount = Plugin.Instance.Config.Bind($"{_sectionName}.{_subSectionName}", "DeadlineDaysAmount", 5, "Deadline Days Amount");

            return config;
        }
    }
}
