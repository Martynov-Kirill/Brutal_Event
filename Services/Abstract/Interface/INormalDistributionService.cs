namespace BrutalEvent.Services.Abstract.Interface
{
    public interface INormalDistributionService
    {
        float CalculateRarity(SelectableLevel currentLevel, float currentRate);

        float NormalDistribution(float mu, float sigma, int players);
    }
}
