using CsharpHelpers.Helpers;
using CsharpHelpers.NotifyServices;
using CsharpHelpers.WindowServices;
using OscdimgPresets.Models;
using OscdimgPresets.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace OscdimgPresets.ViewModels
{

    public sealed class MainViewModel : NotifyModel
    {

        private readonly IPresetService _presetService;
        private readonly IDialogService _dialogService;


        public MainViewModel() : this(new PresetService(), new DialogService(), new ArgumentService())
        {
        }


        public MainViewModel(IPresetService presetService, IDialogService dialogService, IArgumentService argumentService)
        {
            BrowseSourceCommand = new DelegateCommand<Window>(BrowseSourceAction);
            BrowseDestinationCommand = new DelegateCommand<Window>(BrowseDestinationAction);
            EditPresetCommand = new DelegateCommand<Window>(EditPresetAction);
            AddPresetCommand = new DelegateCommand<Window>(AddPresetAction);
            RemovePresetCommand = new DelegateCommand(RemovePresetAction);
            CreateCommand = new DelegateCommand(CreateAction);
            ResetCommand = new DelegateCommand(ResetAction);

            _presetService = presetService;
            _dialogService = dialogService;

            Presets = _presetService.Presets;

            Image = new ImageModel();
            Image.PropertyChanged += ImagePropertyChanged;
            Image.Source = argumentService.SourcePath ?? "";

            if (argumentService.PresetName == null)
            {
                Image.Preset = _presetService.DefaultPreset;
            }
            else
            {
                var preset = _presetService.GetPreset(argumentService.PresetName);
                if (preset != null)
                    Image.Preset = preset;
            }
        }


        public DelegateCommand<Window> BrowseSourceCommand { get; }
        public DelegateCommand<Window> BrowseDestinationCommand { get; }
        public DelegateCommand<Window> EditPresetCommand { get; }
        public DelegateCommand<Window> AddPresetCommand { get; }
        public DelegateCommand RemovePresetCommand { get; }
        public DelegateCommand CreateCommand { get; }
        public DelegateCommand ResetCommand { get; }


        public IEnumerable<PresetModel> Presets { get; }


        public ImageModel Image { get; }


        private bool _canRemovePreset;
        public bool CanRemovePreset
        {
            get { return _canRemovePreset; }
            private set { SetProperty(ref _canRemovePreset, value); }
        }


        private bool _canCreateImage;
        public bool CanCreateImage
        {
            get { return _canCreateImage; }
            private set { SetProperty(ref _canCreateImage, value); }
        }


        public string WindowTitle
        {
            get { return AppHelper.AssemblyInfo.Title; }
        }


        public string ProgramPathError
        {
            get
            {
                var errors = Image.GetErrors(nameof(Image.ProgramPath))?.GetEnumerator();
                return errors?.MoveNext() == true ? (string)errors.Current : "";
            }
        }


        private void ImagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var image = (ImageModel)sender;

            if (e.PropertyName == nameof(image.Preset))
            {
                if (!_presetService.PresetExists(image.Preset))
                    throw new InvalidOperationException("Cannot use a preset that does not exist.");

                CanRemovePreset = image.Preset != _presetService.DefaultPreset;
            }

            CanCreateImage = !image.HasErrors;
        }


        private void BrowseSourceAction(Window window)
        {
            var source = _dialogService.BrowseSource(window, Image.Source);
            if (source != null)
                Image.Source = source;
        }


        private void BrowseDestinationAction(Window window)
        {
            var destination = _dialogService.BrowseDestination(window, Image.Destination, Image.Extension);
            if (destination != null)
                Image.Destination = destination;
        }


        private void EditPresetAction(Window window)
        {
            EditPreset(window, Image.Preset);
        }


        private void AddPresetAction(Window window)
        {
            EditPreset(window, new PresetModel());
        }


        private void EditPreset(Window window, PresetModel preset)
        {
            preset = _dialogService.EditPreset(window, preset);
            if (preset != null)
                Image.Preset = preset;
        }


        private void RemovePresetAction(object parameter)
        {
            var preset = Image.Preset;

            Image.Preset = _presetService.DefaultPreset;
            _presetService.RemovePreset(preset);
            _presetService.SavePresets();
        }


        private void CreateAction(object parameter)
        {
            Image.Create();
        }


        private void ResetAction(object parameter)
        {
            Image.Reset(_presetService.DefaultPreset);
        }

    }

}
