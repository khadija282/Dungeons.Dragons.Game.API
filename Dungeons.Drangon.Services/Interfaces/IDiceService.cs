namespace Dungens.Drangon.Services.Interfaces
{
    public interface IDiceService
    {
        int Roll(string formula, int? seed = null);
    }
}