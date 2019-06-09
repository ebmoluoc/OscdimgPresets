using CsharpHelpers.DialogServices;
using CsharpHelpers.Helpers;
using CsharpHelpers.WindowServices;
using System.Windows;

namespace OscdimgPresets.Views
{

    public partial class MainView : Window
    {

        public MainView()
        {
            InitializeComponent();

            new WindowShowMain(this);

            new WindowResizeCursor(this);

            new WindowPlacement(this)
            {
                PlacementType = PlacementType.SizeAndPosition,
                PlacementPath = AppHelper.DataDirectory.GetFilePath(GetType().Name)
            };

            new WindowSystemMenu(this)
            {
                AboutDialog = new AboutDialog1()
            };
        }

    }

}
