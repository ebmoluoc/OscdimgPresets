using CsharpHelpers.WindowServices;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Errors
{

    public sealed class EmptySpacesError : NotifyDataErrorEditInfo
    {
        public EmptySpacesError(string value, [CallerMemberName] string propertyName = null)
        {
            HasError = value.Length == 0 || value.StartsWith(" ", System.StringComparison.Ordinal) || value.EndsWith(" ", System.StringComparison.Ordinal);
            ErrorMessage = "This value cannot be empty or have leading/trailing spaces.";
            PropertyName = propertyName;
        }
    }

}
