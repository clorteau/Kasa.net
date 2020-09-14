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
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kasa
{
    // Identifies one physical Kasa Tp-Link switch by its name and IP address
    public class Switch : INotifyPropertyChanged
    {
        private bool _checked;

        public string Name { get; set; }
        public string IP { get; set; }

        [XmlIgnore]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value) return;
                _checked = value;
                NotifyPropertyChanged("Checked");
            }
        }

        public Switch(string name, string ip)
        {
            Name = name;
            IP = ip;
            _checked = false;
        }

        public Switch()
        {
            Name = "Undefined";
            IP = "0.0.0.0";
            _checked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
