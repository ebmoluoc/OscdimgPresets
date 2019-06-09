using CsharpHelpers.NotifyServices;
using OscdimgPresets.Models;
using OscdimgPresets.Services;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class PresetExistError : NotifyDataErrorEditInfo
    {
        public PresetExistError(IPresetService svc, PresetModel value, [CallerMemberName] string propertyName = null)
        {
            HasError = svc.PresetExists(value);
            ErrorMessage = "This preset already exists.";
            PropertyName = propertyName;
        }
    }

}
