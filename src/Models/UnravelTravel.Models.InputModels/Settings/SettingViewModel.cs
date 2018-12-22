namespace UnravelTravel.Models.InputModels.Settings
{
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class SettingViewModel : IMapFrom<Setting>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
