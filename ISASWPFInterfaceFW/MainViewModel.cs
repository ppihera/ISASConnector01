using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISASWPFInterfaceFW
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ErrorDTO> Errors { get; set; } = new ObservableCollection<ErrorDTO>();

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
