using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kasa
{
    // Identifies one physical Kasa Tp-Link switch by its name and IP address
    public class Switch : INotifyPropertyChanged
    {
        private bool _checked;

        public string Name { get; set;  }
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
