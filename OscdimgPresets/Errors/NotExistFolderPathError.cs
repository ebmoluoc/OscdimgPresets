using CsharpHelpers.Helpers;
using CsharpHelpers.WindowServices;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class NotExistFolderPathError : NotifyDataErrorEditInfo
    {
        public NotExistFolderPathError(string value, [CallerMemberName] string propertyName = null)
        {
            HasError = !PathHelper.DirectoryExists(value);
            ErrorMessage = "This value points to a folder that does not exist.";
            PropertyName = propertyName;
        }
    }

}
