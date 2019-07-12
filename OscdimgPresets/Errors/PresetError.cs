using CsharpHelpers.WindowServices;
using OscdimgPresets.Models;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class PresetError : NotifyDataErrorEditInfo
    {
        public PresetError(PresetModel value, [CallerMemberName] string propertyName = null)
        {
            HasError = value.HasErrors;
            ErrorMessage = "This preset contains errors.";
            PropertyName = propertyName;
        }
    }

}
