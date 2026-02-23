using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IntensityMapping.Core;
public class NotifyPropertyChangedBase : INotifyPropertyChanged {

    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null) {
        bool propertyChanged = false;

        if (!EqualityComparer<T>.Default.Equals(field, value)) {
            field = value;
            OnPropertyChanged(name);
            propertyChanged = true;
        }

        return propertyChanged;
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}