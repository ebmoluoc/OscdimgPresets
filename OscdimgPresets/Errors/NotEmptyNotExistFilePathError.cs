using CsharpHelpers.Helpers;
using CsharpHelpers.NotifyServices;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class NotEmptyNotExistFilePathError : NotifyDataErrorEditInfo
    {
        public NotEmptyNotExistFilePathError(string value, [CallerMemberName] string propertyName = null)
        {
            HasError = value.Length != 0 && !PathHelper.FileExists(value);
            ErrorMessage = "This value points to a file that does not exist (empty string accepted).";
            PropertyName = propertyName;
        }
    }

}
