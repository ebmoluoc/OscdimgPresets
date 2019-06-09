using CsharpHelpers.Helpers;
using CsharpHelpers.NotifyServices;
using OscdimgPresets.Errors;
using System;

namespace OscdimgPresets.Models
{

    public enum PresetCasing
    {
        MixedCase,
        LowerCase,
        UpperCase
    }


    [Serializable]
    public sealed class PresetModel : NotifyModel, IComparable<PresetModel>, IEquatable<PresetModel>
    {

        public PresetModel() : this("", "", "", "", PresetCasing.MixedCase, PresetCasing.MixedCase)
        {
        }


        public PresetModel(string name) : this(name, "", "", "", PresetCasing.MixedCase, PresetCasing.MixedCase)
        {
        }


        public PresetModel(string name, string options, string biosBoot, string uefiBoot, PresetCasing labelCase, PresetCasing destinationCase)
        {
            Name = name;
            Options = options;
            BiosBoot = biosBoot;
            UefiBoot = uefiBoot;
            LabelCase = labelCase;
            DestinationCase = destinationCase;
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetError(new EmptySpacesError(value));
                SetProperty(ref _name, value);
            }
        }


        private string _options;
        public string Options
        {
            get { return _options; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetProperty(ref _options, value);
            }
        }


        private string _biosBoot;
        public string BiosBoot
        {
            get { return _biosBoot; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetError(new NotEmptyNotExistFilePathError(value));
                SetProperty(ref _biosBoot, value);
            }
        }


        private string _uefiBoot;
        public string UefiBoot
        {
            get { return _uefiBoot; }
            set
            {
                ExceptionHelper.ThrowIfNull(value);
                SetError(new NotEmptyNotExistFilePathError(value));
                SetProperty(ref _uefiBoot, value);
            }
        }


        private PresetCasing _labelCase;
        public PresetCasing LabelCase
        {
            get { return _labelCase; }
            set
            {
                ExceptionHelper.ThrowIfEnumNotValid(value);
                SetProperty(ref _labelCase, value);
            }
        }


        private PresetCasing _destinationCase;
        public PresetCasing DestinationCase
        {
            get { return _destinationCase; }
            set
            {
                ExceptionHelper.ThrowIfEnumNotValid(value);
                SetProperty(ref _destinationCase, value);
            }
        }


        public void GetValues(PresetModel preset)
        {
            Name = preset.Name.Trim();
            Options = preset.Options.Trim();
            BiosBoot = preset.BiosBoot.Trim();
            UefiBoot = preset.UefiBoot.Trim();
            LabelCase = preset.LabelCase;
            DestinationCase = preset.DestinationCase;
        }


        public override int GetHashCode()
        {
            return Name.ToUpper().GetHashCode();
        }


        public override bool Equals(object obj)
        {
            return this == obj as PresetModel;
        }


        public bool Equals(PresetModel preset)
        {
            return this == preset;
        }


        public int CompareTo(PresetModel preset)
        {
            return string.Compare(Name, preset?.Name, StringComparison.CurrentCultureIgnoreCase);
        }


        public static bool operator ==(PresetModel presetA, PresetModel presetB)
        {
            return string.Equals(presetA?.Name, presetB?.Name, StringComparison.CurrentCultureIgnoreCase);
        }


        public static bool operator !=(PresetModel presetA, PresetModel presetB)
        {
            return !(presetA == presetB);
        }

    }

}
