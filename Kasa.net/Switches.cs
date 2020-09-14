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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;

namespace Kasa
{
    public class Switches : ObservableCollection<Switch>
    {
        private static readonly string settingsFileName = "switches.xml";
        private readonly StorageFolder localFolder;

        public Switches() : base()
        {
            localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Add(new Switch("Lights 1", "1.2.3.4"));
                Add(new Switch("Lights 2", "4.3.2.1"));
                Add(new Switch("Lights 3", "1.1.1.1"));
                Add(new Switch("Lights 4", "1.1.1.1"));
                Add(new Switch("Lights 5", "1.1.1.1"));
                Add(new Switch("Lights 6", "1.1.1.1"));
            }

        }

        public async void Load()
        {
            try
            {
                StorageFile file = await localFolder.CreateFileAsync(settingsFileName, CreationCollisionOption.OpenIfExists);
                Stream stream = await file.OpenStreamForReadAsync();
                XmlSerializer deserializer = new XmlSerializer(typeof(Switches));
                Switches switches = (Switches)deserializer.Deserialize(stream);
                stream.Close();

                foreach (Switch s in switches) Add(s);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not load settings file: " + e.Message);
            }

        }

        public async void Save()
        {
            //Don't save the "all lights" entry
            Switches toSave = new Switches();
            foreach (Switch s in this)
            {
                if (s.IP != "all") toSave.Add(s);
            }

            StorageFile file = await localFolder.CreateFileAsync(settingsFileName, CreationCollisionOption.ReplaceExisting);
            Stream stream = await file.OpenStreamForWriteAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(Switches));
            StreamWriter writer = new StreamWriter(stream);
            serializer.Serialize(writer, toSave);
            stream.Close();
        }
    }
}
