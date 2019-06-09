using CsharpHelpers.Helpers;
using OscdimgPresets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OscdimgPresets.Services
{

    using PresetList = ObservableCollection<PresetModel>;


    public interface IPresetService
    {
        IEnumerable<PresetModel> Presets { get; }
        PresetModel DefaultPreset { get; }
        PresetModel GetPreset(string name);
        bool PresetExists(PresetModel preset);
        void AddPreset(PresetModel preset);
        void RemovePreset(PresetModel preset);
        void SavePresets();
    }


    public sealed class PresetService : IPresetService
    {

        private const string _PresetsFileName = "Presets";

        private static readonly string _presetsPath = AppHelper.DataDirectory.GetFilePath($"{_PresetsFileName}.xml");
        private static PresetModel _defaultPreset = new PresetModel("Default", "-h -o -u2 -udfver150", "", "", PresetCasing.UpperCase, PresetCasing.LowerCase);
        private static PresetList _presets;


        public PresetService()
        {
            if (_presets == null)
            {
                LoadPresets();
                SavePresets();
            }
        }


        public IEnumerable<PresetModel> Presets
        {
            get { return _presets; }
        }


        public PresetModel DefaultPreset
        {
            get { return _defaultPreset; }
        }


        public PresetModel GetPreset(string name)
        {
            var index = _presets.IndexOf(new PresetModel(name ?? ""));
            return index != -1 ? _presets[index] : null;
        }


        public bool PresetExists(PresetModel preset)
        {
            return _presets.Contains(preset);
        }


        public void AddPreset(PresetModel preset)
        {
            if (preset.HasErrors)
                throw new InvalidOperationException("Cannot add a preset with errors.");

            if (PresetExists(preset))
                throw new InvalidOperationException("Cannot add an existing preset.");

            var index = CollectionHelper.BinarySearch(_presets, preset);
            _presets.Insert(~index, preset);
        }


        public void RemovePreset(PresetModel preset)
        {
            if (preset == _defaultPreset)
                throw new InvalidOperationException("Cannot remove default preset.");

            _presets.Remove(preset);
        }


        public void SavePresets()
        {
            var dir = Path.GetDirectoryName(_presetsPath);
            Directory.CreateDirectory(dir);

            using (var writer = XmlWriter.Create(_presetsPath))
                new XmlSerializer(typeof(PresetList)).Serialize(writer, _presets);
        }


        private static void LoadPresets()
        {
            if (LoadPresets(out var list))
            {
                _presets = new PresetList();

                foreach (var item in list)
                {
                    item.GetValues(item);
                    if (item.Name.Length != 0 && !_presets.Contains(item))
                        _presets.Add(item);
                }

                var index = _presets.IndexOf(_defaultPreset);
                if (index == -1)
                    _presets.Add(_defaultPreset);
                else
                    _defaultPreset = _presets[index];

                CollectionHelper.Sort(_presets);
            }
        }


        private static bool LoadPresets(out PresetList list)
        {
            try
            {
                using (var reader = XmlReader.Create(_presetsPath, null))
                {
                    list = (PresetList)new XmlSerializer(typeof(PresetList)).Deserialize(reader);
                    return true;
                }
            }
            catch (DirectoryNotFoundException)
            {
                _presets = new PresetList { _defaultPreset };
            }
            catch (FileNotFoundException)
            {
                _presets = new PresetList { _defaultPreset };
            }
            catch (InvalidOperationException)
            {
                File.Copy(_presetsPath, AppHelper.DataDirectory.GetFilePath($"{_PresetsFileName}-{DateTime.Now.ToString("yy-MM-dd-HH-mm-ss")}.xml"), true);
                _presets = new PresetList { _defaultPreset };
            }

            list = null;
            return false;
        }

    }

}
