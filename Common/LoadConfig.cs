using BrutalEvent.Models;

namespace BrutalEvent.Common
{
    public class LoadConfig
    {
        private string _sectionName = "BrutalEventSettings";
        public Configuration BindConfigSettings()
        {
            var config = new Configuration();
            config.minScrap = BrutalEventPlugin.Instance.Config
                .Bind(_sectionName, "minScrap", 100f, "Max Rate create monsters");
            config.maxScrap = BrutalEventPlugin.Instance.Config
                .Bind(_sectionName, "maxScrap", 800f, "");
            config.limitRate = BrutalEventPlugin.Instance.Config
                .Bind(_sectionName, "limitRate", 50f, "");
            config.maxRate = BrutalEventPlugin.Instance.Config
                .Bind(_sectionName, "maxRate", 100f, "");
            config.rarityMin = BrutalEventPlugin.Instance.Config
                .Bind(_sectionName, "rarityMin", 10f, "");
            config.rarityMax = BrutalEventPlugin.Instance.Config
                .Bind(_sectionName, "rarityMax", 100f, "s");
            config.startCredits = BrutalEventPlugin.Instance.Config
                .Bind(_sectionName, "startCredits", 45, "");

            return config;
        }
    }
}
