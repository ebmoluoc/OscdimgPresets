using CsharpHelpers.WindowServices;
using OscdimgPresets.Errors;
using OscdimgPresets.Models;
using OscdimgPresets.Services;
using System.ComponentModel;
using System.Windows;

namespace OscdimgPresets.ViewModels
{

    public sealed class PresetViewModel : NotifyModel
    {

        private const string _BackgroundWhite = "White";
        private const string _BackgroundGray = "#FFE5E5E5";

        private readonly IPresetService _presetService;
        private readonly IDialogService _dialogService;
        private readonly PresetModel _preset;
        private bool _saveRequested;


        public PresetViewModel(PresetModel preset) : this(preset, new PresetService(), new DialogService())
        {
        }


        public PresetViewModel(PresetModel preset, IPresetService presetService, IDialogService dialogService)
        {
            BrowseBiosCommand = new DelegateCommand<Window>(BrowseBiosAction);
            BrowseUefiCommand = new DelegateCommand<Window>(BrowseUefiAction);
            ResetBiosCommand = new DelegateCommand(ResetBiosAction);
            ResetUefiCommand = new DelegateCommand(ResetUefiAction);
            SaveCommand = new DelegateCommand<IClosable>(SaveAction, CanSaveAction);
            CancelCommand = new DelegateCommand<IClosable>(CancelAction);

            _preset = preset;
            _presetService = presetService;
            _dialogService = dialogService;

            IsNewPreset = !_presetService.PresetExists(_preset);
            NameReadOnly = !IsNewPreset;
            NameBackground = IsNewPreset ? _BackgroundWhite : _BackgroundGray;

            Preset = new PresetModel();
            Preset.PropertyChanged += PresetPropertyChanged;
            Preset.GetValues(_preset);

            WindowTitle = IsNewPreset ? "Add Preset" : "Edit Preset";
        }


        public DelegateCommand<Window> BrowseBiosCommand { get; }
        public DelegateCommand<Window> BrowseUefiCommand { get; }
        public DelegateCommand ResetBiosCommand { get; }
        public DelegateCommand ResetUefiCommand { get; }
        public DelegateCommand<IClosable> SaveCommand { get; }
        public DelegateCommand<IClosable> CancelCommand { get; }


        public PresetModel Preset { get; }


        public bool IsNewPreset { get; }


        public bool NameReadOnly { get; }


        public string NameBackground { get; }


        public string WindowTitle { get; }


        public PresetModel GetResult()
        {
            if (_saveRequested)
            {
                _preset.GetValues(Preset);

                if (IsNewPreset)
                    _presetService.AddPreset(_preset);

                _presetService.SavePresets();

                return _preset;
            }

            return null;
        }


        private void PresetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var preset = (PresetModel)sender;

            if (e.PropertyName == nameof(preset.Name) && IsNewPreset)
                preset.SetError(new PresetExistError(_presetService, preset, nameof(preset.Name)));

            SaveCommand.RaiseCanExecuteChanged();
        }


        private void BrowseBiosAction(Window window)
        {
            var bios = _dialogService.BrowseBoot(window, Preset, BootType.Bios);
            if (bios != null)
                Preset.BiosBoot = bios;
        }


        private void BrowseUefiAction(Window window)
        {
            var uefi = _dialogService.BrowseBoot(window, Preset, BootType.Uefi);
            if (uefi != null)
                Preset.UefiBoot = uefi;
        }


        private void ResetBiosAction(object parameter)
        {
            Preset.BiosBoot = "";
        }


        private void ResetUefiAction(object parameter)
        {
            Preset.UefiBoot = "";
        }


        private bool CanSaveAction(object parameter)
        {
            return !Preset.HasErrors;
        }


        private void SaveAction(IClosable obj)
        {
            _saveRequested = true;

            obj.Close();
        }


        private void CancelAction(IClosable obj)
        {
            obj.Close();
        }

    }

}
