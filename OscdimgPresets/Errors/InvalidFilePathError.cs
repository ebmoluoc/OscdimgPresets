using CsharpHelpers.Helpers;
using CsharpHelpers.NotifyServices;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class InvalidFilePathError : NotifyDataErrorEditInfo
    {
        public InvalidFilePathError(string value, [CallerMemberName] string propertyName = null)
        {
            HasError = !PathHelper.IsValidFilePath(value);
            ErrorMessage = "This value is an invalid file path.";
            PropertyName = propertyName;
        }
    }

}
