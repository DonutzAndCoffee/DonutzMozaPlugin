using log4net.Plugin;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static DonutzMozaPlugin.DonutzMozaPluginSettings;

namespace DonutzMozaPlugin
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly DonutzMozaPlugin _plugin;
        public DonutzMozaPluginSettings Settings { get; }

        private string _mappingFilterText;
        private string _selectedProfileKey;

        public ObservableCollection<KeyValuePair<string, GameSetting>> GameSettingsList { get; private set; }

        public ObservableCollection<KeyValuePair<string, MozaProfile>> FilteredProfiles { get; private set; }

        public void SetSelectedProfile(MozaProfile profile)
        {
            _plugin.SetMozaProfile(profile);
        }

        public string MappingFilterText
        {
            get => _mappingFilterText;
            set
            {
                if (_mappingFilterText != value)
                {
                    _mappingFilterText = value;
                    OnPropertyChanged(nameof(MappingFilterText));
                    RefreshFilteredProfiles();
                }
            }
        }

        public string SelectedProfileKey
        {
            get => _selectedProfileKey;
            set
            {
                if (_selectedProfileKey != value)
                {
                    SimHub.Logging.Current.Info("Setting SelectedProfileKey to: " + value);
                    _selectedProfileKey = value;
                    OnPropertyChanged(nameof(SelectedProfileKey));
                    OnPropertyChanged(nameof(SelectedProfile));
                }
            }
        }

        public MozaProfile SelectedProfile
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedProfileKey) || !Settings.profileMapping.ContainsKey(SelectedProfileKey))
                    return null;
                return Settings.profileMapping[SelectedProfileKey];
            }
        }

        public SettingsViewModel(DonutzMozaPlugin plugin, DonutzMozaPluginSettings settings)
        {
            _plugin = plugin;
            Settings = settings;
            GameSettingsList = new ObservableCollection<KeyValuePair<string, GameSetting>>(Settings.gameSettings);
            FilteredProfiles = new ObservableCollection<KeyValuePair<string, MozaProfile>>(GetFilteredProfiles());
        }

        public void OnProfileEdited(MozaProfile profile)
        {
            _plugin.SetMozaProfile(profile);
        }

        public void DeleteSelectedProfile()
        {
            if (!string.IsNullOrEmpty(SelectedProfileKey))
            {
                string selectedProfileKeySub = SelectedProfileKey.Substring(1, SelectedProfileKey.IndexOf(',') - 1);

                if (Settings.profileMapping.ContainsKey(selectedProfileKeySub))
                {
                    Settings.profileMapping.Remove(selectedProfileKeySub);
                    SelectedProfileKey = null;
                    RefreshFilteredProfiles();
                }
            }
        }

        public void ReloadGameSettings()
        {
            GameSettingsList.Clear();
            foreach (var item in Settings.gameSettings)
            {
                GameSettingsList.Add(item);
            }
            OnPropertyChanged(nameof(GameSettingsList));
        }

        private IEnumerable<KeyValuePair<string, MozaProfile>> GetFilteredProfiles()
        {
            if (string.IsNullOrEmpty(MappingFilterText))
                return Settings.profileMapping;
            return Settings.profileMapping
                .Where(profile => profile.Key.ToLower().Contains(MappingFilterText.ToLower()));
        }

        public void RefreshFilteredProfiles()
        {
            FilteredProfiles.Clear();
            foreach (var profile in GetFilteredProfiles())
            {
                FilteredProfiles.Add(profile);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _plugin.SetMozaProfile();
        }
    }

    public partial class SettingsControl : UserControl
    {
        private readonly SettingsViewModel _viewModel;
        private readonly DonutzMozaPlugin _plugin;

        public SettingsControl(DonutzMozaPlugin plugin, DonutzMozaPluginSettings settings)
        {
            InitializeComponent();
            _viewModel = new SettingsViewModel(plugin, settings);
            _plugin = plugin;
            DataContext = _viewModel;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var profile = (MozaProfile)((Slider)sender).DataContext;
            _viewModel.OnProfileEdited(profile);
            
        }

        private void OnSetProfileButtonClick(object sender, RoutedEventArgs e)
        {
            _plugin.SetMozaProfile();
        }


        private void RefreshGameList_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ReloadGameSettings();
        }

        private void OnDeleteProfileButtonClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Do you really want to delete this entry?",
                                         "Really?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _viewModel.DeleteSelectedProfile();
            }
        }
        private void TitledSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            slider.Value = Math.Round(slider.Value / 10) * 10;
        }

    }




}
