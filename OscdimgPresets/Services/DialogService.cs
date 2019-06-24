using CsharpHelpers.DialogServices;
using CsharpHelpers.Helpers;
using OscdimgPresets.Models;
using OscdimgPresets.ViewModels;
using OscdimgPresets.Views;
using System;
using System.IO;
using System.Windows;

namespace OscdimgPresets.Services
{

    public enum BootType
    {
        Bios,
        Uefi
    }


    public interface IDialogService
    {
        string BrowseSource(Window owner, string source);
        string BrowseDestination(Window owner, string destination, string extension);
        string BrowseBoot(Window owner, PresetModel preset, BootType type);
        PresetModel EditPreset(Window owner, PresetModel preset);
    }


    public sealed class DialogService : IDialogService
    {

        public string BrowseSource(Window owner, string source)
        {
            using (var dialog = new FileOpenDialog())
            {
                dialog.SetClientGuid(new Guid("6DD0F3B6-0F48-4F5B-8690-4866D4B9EAF2"));
                dialog.SetDefaultFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                dialog.SetTitle("Source location of the files and folders to be included in the image");
                dialog.SetOkButtonLabel("Select");
                dialog.SetCancelButtonLabel("Cancel");
                dialog.SetFileNameLabel("Source location :");
                dialog.PickFolders = true;
                dialog.DontAddToRecent = true;

                if (PathHelper.DirectoryExists(source))
                    dialog.SetFolder(source);

                return dialog.ShowDialog(owner) == true ? dialog.GetResult() : null;
            }
        }


        public string BrowseDestination(Window owner, string destination, string extension)
        {
            using (var dialog = new FileSaveDialog())
            {
                GetFilePathInfo(destination, out var folderPath, out var fileName, out var fileExtension);

                dialog.SetClientGuid(new Guid("486640B6-B311-4EFD-A15F-616F51FAAF5B"));
                dialog.SetDefaultFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                dialog.SetTitle("Destination file of the image to be created");
                dialog.SetOkButtonLabel("Select");
                dialog.SetCancelButtonLabel("Cancel");
                dialog.SetFileNameLabel("Destination file :");
                dialog.SetDefaultExtension(extension.Substring(1));
                dialog.SetFileTypes($"Image files (*{extension})|*{extension}|All files (*.*)|*.*");
                dialog.SetFileTypeIndex(fileExtension == null || fileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase) ? 1 : 2);
                dialog.DontAddToRecent = true;

                if (fileName != null)
                    dialog.SetFileName(fileName);

                if (PathHelper.DirectoryExists(folderPath))
                    dialog.SetFolder(folderPath);

                return dialog.ShowDialog(owner) == true ? dialog.GetResult() : null;
            }
        }


        public string BrowseBoot(Window owner, PresetModel preset, BootType type)
        {
            using (var dialog = new FileOpenDialog())
            {
                const string BiosFileName = "etfsboot.com";
                const string UefiFileName = "efisys.bin";

                string bootFolderPath;
                string bootFileName;
                string bootFileExtension;
                string extension;
                string clientGuid;
                string title;
                string fileName;

                if (type == BootType.Bios)
                {
                    GetFilePathInfo(preset.BiosBoot, out bootFolderPath, out bootFileName, out bootFileExtension);
                    extension = Path.GetExtension(BiosFileName);
                    clientGuid = "E8BEE349-1A4A-4E04-B8B9-B15FF1EF9125";
                    title = "BIOS";
                    fileName = bootFileName ?? BiosFileName;
                }
                else if (type == BootType.Uefi)
                {
                    GetFilePathInfo(preset.UefiBoot, out bootFolderPath, out bootFileName, out bootFileExtension);
                    extension = Path.GetExtension(UefiFileName);
                    clientGuid = "A6A65946-6FCD-4DCA-AEB1-85ABF5FE3CAE";
                    title = "UEFI";
                    fileName = bootFileName ?? UefiFileName;
                }
                else
                {
                    throw new InvalidOperationException("The specified boot type is not implemented.");
                }

                dialog.SetClientGuid(new Guid(clientGuid));
                dialog.SetDefaultFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                dialog.SetTitle($"{title} boot sector file");
                dialog.SetFileTypes($"Boot files (*{extension})|*{extension}|All files (*.*)|*.*");
                dialog.SetFileName(fileName);
                dialog.SetFileTypeIndex(bootFileExtension == null || bootFileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase) ? 1 : 2);
                dialog.SetOkButtonLabel("Select");
                dialog.SetCancelButtonLabel("Cancel");
                dialog.SetFileNameLabel("Boot sector file :");
                dialog.DontAddToRecent = true;

                if (PathHelper.DirectoryExists(bootFolderPath))
                    dialog.SetFolder(bootFolderPath);

                return dialog.ShowDialog(owner) == true ? dialog.GetResult() : null;
            }
        }


        public PresetModel EditPreset(Window owner, PresetModel preset)
        {
            var viewModel = new PresetViewModel(preset);
            var view = new PresetView { Owner = owner, DataContext = viewModel };

            view.ShowDialog();

            return viewModel.GetResult();
        }


        private static void GetFilePathInfo(string filePath, out string folderPath, out string fileName, out string fileExtension)
        {
            if (PathHelper.IsValidFilePath(filePath))
            {
                var fi = new FileInfo(filePath);
                folderPath = fi.DirectoryName;
                fileName = fi.Name;
                fileExtension = fi.Extension;
            }
            else
            {
                folderPath = null;
                fileName = null;
                fileExtension = null;
            }
        }

    }

}
