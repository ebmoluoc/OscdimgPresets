using CsharpHelpers.NotifyServices;
using OscdimgPresets.Models;
using OscdimgPresets.Services;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class PresetNotExistError : NotifyDataErrorEditInfo
    {
        public PresetNotExistError(IPresetService svc, PresetModel value, [CallerMemberName] string propertyName = null)
        {
            HasError = !svc.PresetExists(value);
            ErrorMessage = "This preset does not exist.";
            PropertyName = propertyName;
        }
    }

}
