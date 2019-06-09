using CsharpHelpers.Helpers;
using CsharpHelpers.NotifyServices;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class ImageProgramError : NotifyDataErrorEditInfo
    {
        public ImageProgramError(string programName, string value, [CallerMemberName] string propertyName = null)
        {
            HasError = !PathHelper.FileExists(value);
            ErrorMessage = $"Image creation disabled : \"{programName}\" was not found.";
            PropertyName = propertyName;
        }
    }

}
