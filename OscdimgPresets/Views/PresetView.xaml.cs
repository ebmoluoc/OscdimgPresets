using CsharpHelpers.Helpers;
using CsharpHelpers.WindowServices;
using System.Windows;

namespace OscdimgPresets.Views
{

    public partial class PresetView : Window, IClosable
    {

        public PresetView()
        {
            InitializeComponent();

            new WindowResizeCursor(this);

            new WindowPlacement(this)
            {
                PlacementType = PlacementType.SizeOnly,
                PlacementPath = AppHelper.DataDirectory.GetFilePath(GetType().Name)
            };

            new WindowSystemMenu(this)
            {
                IconRemoved = true,
                MaximizeRemoved = true,
                MinimizeRemoved = true
            };
        }

    }

}
