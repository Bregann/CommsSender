using CommsSender.Domain.Enums;

namespace CommsSender.Domain.Interfaces.Helpers
{
    public interface IEnvironmentalSettingHelper
    {
        Task LoadEnvironmentalSettings();
        string? TryGetEnviromentalSettingValue(EnvironmentalSettingEnum key);
        Task<bool> UpdateEnviromentalSettingValue(EnvironmentalSettingEnum key, string newValue);
    }
}
