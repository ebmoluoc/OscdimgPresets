using CsharpHelpers.Helpers;
using System.Windows;

namespace OscdimgPresets
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppHelper.CatchUnhandledException = true;

            var assemblyInfo = new AssemblyInfo(typeof(App).Assembly, "Resources.LICENSE");
            var dataDirectory = new AppDataRoaming(assemblyInfo.Product);
            var logFilePath = dataDirectory.GetFilePath($"{assemblyInfo.FileName}.log");
            var logger = new TextFileLogger(logFilePath);

            AppHelper.AssemblyInfo = assemblyInfo;
            AppHelper.DataDirectory = dataDirectory;
            AppHelper.Logger = logger;
            AppHelper.SetAppMutex($"Global\\{assemblyInfo.Guid}");

            if (!new Services.ArgumentService(e.Args).CreateNow)
            {
                AppHelper.SetInstanceMutex($"Local\\{assemblyInfo.Guid}", true);

                MainWindow = new Views.MainView();
                MainWindow.Show();
            }
            else
            {
                var mvm = new ViewModels.MainViewModel();
                if (mvm.CanCreateImage)
                    mvm.Image.Create();
                else
                    MessageBox.Show("Cannot create an image file from the given parameters. Either the source path or the preset name is invalid.", assemblyInfo.Title, MessageBoxButton.OK, MessageBoxImage.Error);

                AppHelper.BeginInvokeShutdown();
            }
        }
    }
}
