using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PengSW.NotifyPropertyChanged
{
    [Serializable]
    public class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged实现

        protected void SetValue<T>(ref T aMember, T aValue, params string[] aPropertyNames)
        {
            if (object.Equals(aMember, aValue)) return;
            aMember = aValue;
            OnPropertiesChanged(aPropertyNames);
        }

        protected void OnPropertyChanged(string aPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }

        protected void OnPropertiesChanged(params string[] aPropertyNames)
        {
            foreach (string aPropertyName in aPropertyNames) OnPropertyChanged(aPropertyName);
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
