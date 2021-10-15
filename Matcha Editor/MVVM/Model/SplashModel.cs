using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Matcha_Editor.MVVM.Model
{
    public class SplashModel : INotifyPropertyChanged
    {
        public string Version { get; set; }

        private string m_Status;
        public string Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
