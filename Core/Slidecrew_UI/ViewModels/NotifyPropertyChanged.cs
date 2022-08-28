using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Avalonia.Threading;

namespace Slidecrew.ViewModels
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (_sycnContext == Dispatcher.UIThread)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            else
            {
                _sycnContext.Post(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                });
            }
        }

        private Dispatcher _sycnContext;
        public NotifyPropertyChanged()
        {
            _sycnContext = Dispatcher.UIThread;
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            object value;
            if (_propertyChangedBackingStore.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        protected bool SetValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            if (EqualityComparer<T>.Default.Equals(newValue, GetValue<T>(propertyName))) return false;

            _propertyChangedBackingStore[propertyName] = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetValueNoCheck<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            //if (EqualityComparer<T>.Default.Equals(newValue, GetValue<T>(propertyName))) return false;

            _propertyChangedBackingStore[propertyName] = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<string, object> _propertyChangedBackingStore = new Dictionary<string, object>();
    }
}
