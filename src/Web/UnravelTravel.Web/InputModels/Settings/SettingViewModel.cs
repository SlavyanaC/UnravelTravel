namespace UnravelTravel.Web.ViewModels.Settings
{
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class SettingViewModel : IMapFrom<Setting>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
