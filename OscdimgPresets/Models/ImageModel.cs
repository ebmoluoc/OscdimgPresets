using CsharpHelpers.Helpers;
using CsharpHelpers.WindowServices;
using OscdimgPresets.Errors;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace OscdimgPresets.Models
{

    public sealed class ImageModel : NotifyModel
    {

        private const string _ProgramName = "oscdimg.exe";


        public ImageModel()
        {
            var directory = AppHelper.AssemblyInfo.DirectoryPath;
            var paths = PathHelper.GetEnvironmentPaths(directory);
            ProgramPath = PathHelper.GetFilePath(_ProgramName, paths);

            Reset(new PresetModel());
        }


        public string Extension { get; } = ".iso";


        private string _programPath;
        public string ProgramPath
        {
            get { return _programPath; }
            private set
            {
                SetError(new ImageProgramError(_ProgramName, value));
                SetProperty(ref _programPath, value);
            }
        }


        private PresetModel _preset;
        public PresetModel Preset
        {
            get { return _preset; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetError(new PresetError(value));
                SetProperty(ref _preset, value);

                SetArguments();
            }
        }


        private string _source;
        public string Source
        {
            get { return _source; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetError(new NotExistFolderPathError(value));
                SetProperty(ref _source, value);

                if (!SetAutoValues())
                    SetArguments();
            }
        }


        private string _destination;
        public string Destination
        {
            get { return _destination; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetError(new InvalidFilePathError(value));
                SetProperty(ref _destination, value);

                if (!SetAutoValues())
                    SetArguments();
            }
        }


        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetError(new EmptySpacesError(value));
                SetProperty(ref _label, value);

                SetArguments();
            }
        }


        private string _arguments;
        public string Arguments
        {
            get { return _arguments; }
            private set { SetProperty(ref _arguments, value); }
        }


        public void Reset(PresetModel preset)
        {
            Preset = preset;
            Source = "";
            Destination = "";
            Label = "";
        }


        public void Create()
        {
            if (HasErrors)
                throw new InvalidOperationException("Cannot create an image containing errors.");

            var drive = Path.GetDirectoryName(Source) ?? Source;
            var args = $"/k \"cd /d \"{drive}\" & \"{ProgramPath}\" {Arguments}\"";

            Process.Start("cmd.exe", args).Dispose();
        }


        private bool SetAutoValues([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(Source) && GetErrors(nameof(Source)) == null && Destination.Length == 0)
            {
                var di = new DirectoryInfo(Source);
                var directory = di.Parent != null ? di.Parent.FullName : di.Root.FullName;
                var file = di.Parent != null ? di.Name : di.Root.FullName.Substring(0, 1);
                Destination = Path.Combine(directory, file + Extension);

                return true;
            }
            else if (propertyName == nameof(Destination) && GetErrors(nameof(Destination)) == null && Label.Length == 0)
            {
                Label = Path.GetFileNameWithoutExtension(Destination);

                return true;
            }
            else
            {
                return false;
            }
        }


        private void SetArguments()
        {
            if (!HasErrors)
            {
                var label = GetLabelArgument();
                var destination = GetDestinationArgument();
                var boot = GetBootArgument();

                var args = $"-l\"{label}\" \"{Source}\" \"{destination}\"";

                if (boot.Length != 0)
                    args = $"{boot} {args}";

                if (Preset.Options.Length != 0)
                    args = $"{Preset.Options} {args}";

                Arguments = args;
            }
            else
            {
                Arguments = "";
            }
        }


        private string GetLabelArgument()
        {
            switch (Preset.LabelCase)
            {
                case PresetCasing.UpperCase:
                    return Label.ToUpper();
                case PresetCasing.LowerCase:
                    return Label.ToLower();
                default:
                    return Label;
            }
        }


        private string GetDestinationArgument()
        {
            var destination = new FileInfo(Destination);

            switch (Preset.DestinationCase)
            {
                case PresetCasing.LowerCase:
                    return Path.Combine(destination.DirectoryName, destination.Name.ToLower());
                case PresetCasing.UpperCase:
                    return Path.Combine(destination.DirectoryName, destination.Name.ToUpper());
                default:
                    return Destination;
            }
        }


        private string GetBootArgument()
        {
            if (Preset.BiosBoot.Length != 0 && Preset.UefiBoot.Length != 0)
                return $"-bootdata:2#p0,e,b\"{Preset.BiosBoot}\"#pEF,e,b\"{Preset.UefiBoot}\"";
            else if (Preset.BiosBoot.Length != 0)
                return $"-b\"{Preset.BiosBoot}\" -e -p0";
            else if (Preset.UefiBoot.Length != 0)
                return $"-b\"{Preset.UefiBoot}\" -e -pEF";
            else
                return "";
        }

    }

}
