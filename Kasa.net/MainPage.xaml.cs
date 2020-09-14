/*
    Kasa.NET - Control Kasa TP-Link smart switches
    Copyright (C) 2020  Clem Lorteau <clem@lorteau.fr>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;


namespace Kasa
{
    public sealed partial class MainPage : Page
    {
        private static MainPage _instance;
        public static MainPage Instance
        {
            get { return _instance; }

        }

        public AppBarToggleButton EditToggleButton
        {
            get { return this.EditButton; }
        }

        private readonly Switches switches;
        public MainPage()
        {
            InitializeComponent();
            _instance = this;

            // override UWP's minimum window size
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(300, 200));
            // load last window size or default
            double width = 370;
            double height = 210;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["width"] != null) width = (double)localSettings.Values["width"];
            if (localSettings.Values["height"] != null) height = (double)localSettings.Values["height"];
            ApplicationView.PreferredLaunchViewSize = new Size(width, height);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            // implement our own title bar just so it can be acrylic like the rest of the window
            CoreApplicationViewTitleBar titleBar = CoreApplication.GetCurrentView().TitleBar;
            titleBar.ExtendViewIntoTitleBar = true;
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // load saved switches
            switches = new Switches();
            theList.ItemsSource = switches;
            switches.Add(new Switch("All switches", "all"));
            switches.Load();
        }

        private void EditButton_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (Switch s in switches)
            {
                s.Checked = false;
            }
            switches.Save();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            List<Switch> switchesToRemove = new List<Switch>();
            foreach (Switch s in switches)
            {
                if (s.Checked) switchesToRemove.Add(s);
            }
            foreach (Switch switchToRemove in switchesToRemove)
            {
                switches.Remove(switchToRemove);
            }
        }

        private void NewSwitchPopupOKButton_Click(object sender, RoutedEventArgs e)
        {
            switches.Add(new Switch(NameInput.Text, IPInput.Text));
            NewSwitchFlyout.Hide();
        }

        private void NewSwitchFlyout_Closed(object sender, object e)
        {
            NameInput.Text = "";
            IPInput.Text = "";
        }

        private void OnOff_Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.OriginalSource;
            Switch s = (Switch)b.DataContext;

            if (s.IP == "all")
            {
                foreach (Switch _switch in switches)
                {
                    if (_switch.IP != "all") SendCommand((string)b.Content, _switch.IP);
                }
            }
            else SendCommand((string)b.Content, s.IP);
        }

        private void SendCommand(string command, string ip)
        {
            if (command == "On")
                Commander.SendCommand(Commander.Command.On, ip);
            else
                Commander.SendCommand(Commander.Command.Off, ip);
        }

        private void Page_Size_Changed(object sender, SizeChangedEventArgs e)
        {
            Size size = e.NewSize;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["width"] = size.Width;
            localSettings.Values["height"] = size.Height;

            MainPage.Instance.theList.Width = size.Width;
        }
    }

    //so that the "all switches" checkbox can't be enabled
    public class CheckboxEnabledtoVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString() == "all") result = false;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    //so that the checkboxes column collapses when the edit button is not toggled
    public class CheckboxesColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int width = 0;
            if (MainPage.Instance.EditToggleButton.IsChecked == true)
                width = 20;

            return new GridLength(width);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
