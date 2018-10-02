using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PengSW.NotifyPropertyChanged
{
    [Serializable]
    public class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged实现

        protected bool SetValue<T>(ref T aMember, T aValue, params string[] aPropertyNames)
        {
            if (object.Equals(aMember, aValue)) return false;
            aMember = aValue;
            OnPropertiesChanged(aPropertyNames);
            return true;
        }

        protected void OnPropertyChanged(string aPropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));

        protected void OnPropertiesChanged(params string[] aPropertyNames)
        {
            foreach (string aPropertyName in aPropertyNames) OnPropertyChanged(aPropertyName);
        }
        
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
