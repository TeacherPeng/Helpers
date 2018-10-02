using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PengSW.ObservableCollectionHelper
{
    /// <summary>
    /// 增强的ObservableCollection
    ///     显式化了PropertyChanged事件。
    ///     如果元素支持INotifyPropertyChanged接口，则当元素发出PropertyChanged通知时，会发出Item属性变化。
    ///     如果元素支持INotifyCollectionChanged接口，则当元素发出CollectionChanged通知时，会发出Item属性变化。
    ///     增加了ActiveItem属性。
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    public class ObservableCollectionEx<T> : ObservableCollection<T>, IDisposable, INotifyPropertyChanged where T : class
    {
        public ObservableCollectionEx() { }
        public ObservableCollectionEx(IEnumerable<T> aSource) : base(aSource) { ActiveItem = this.FirstOrDefault(); IsModified = false; }
        public ObservableCollectionEx(List<T> aSource) : base(aSource) { ActiveItem = this.FirstOrDefault(); IsModified = false; }

        public bool IsEmpty { get { return this.Count == 0; } }

        public bool IsModified { get { return _IsModified; } set { SetValue(ref _IsModified, value, nameof(IsModified)); } }
        private bool _IsModified = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            IsModified = true;
            base.OnCollectionChanged(e);
        }

        protected virtual void OnItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // HACK: 这里假设了子项的ActiveItem属性也是用来表征当前项的，不需要触发Item Changed事件。但是这个假设并不能保证总是成立的。
            if (e.PropertyName == nameof(ActiveItem)) return;
            OnPropertyChanged("Item");
        }

        protected virtual void OnItem_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Item");
        }

        protected void RegisteItemEvents(T aItem)
        {
            if (aItem is INotifyPropertyChanged) (aItem as INotifyPropertyChanged).PropertyChanged += OnItem_PropertyChanged;
            if (aItem is INotifyCollectionChanged) (aItem as INotifyCollectionChanged).CollectionChanged += OnItem_CollectionChanged;
        }

        protected void UnregisteItemEvents(T aItem)
        {
            if (aItem is INotifyPropertyChanged) (aItem as INotifyPropertyChanged).PropertyChanged -= OnItem_PropertyChanged;
            if (aItem is INotifyCollectionChanged) (aItem as INotifyCollectionChanged).CollectionChanged -= OnItem_CollectionChanged;
        }

        protected override void RemoveItem(int aIndex)
        {
            T aItem = this[aIndex];
            UnregisteItemEvents(aItem);
            base.RemoveItem(aIndex);
            if (ActiveItem == aItem)
            {
                if (aIndex < Count) ActiveItem = this[aIndex]; else ActiveItem = this.LastOrDefault();
            }
            OnPropertyChanged(nameof(Count));
        }

        protected override void ClearItems()
        {
            foreach (T aItem in this)
            {
                UnregisteItemEvents(aItem);
            }
            base.ClearItems();
            ActiveItem = null;
            OnPropertyChanged(nameof(Count));
        }

        protected override void InsertItem(int index, T aItem)
        {
            if (aItem == null || Contains(aItem)) return;
            base.InsertItem(index, aItem);
            RegisteItemEvents(aItem);
            ActiveItem = aItem;
            OnPropertyChanged(nameof(Count));
        }

        protected override void SetItem(int aIndex, T aItem)
        {
            if (aItem == null || Contains(aItem)) return;
            T aOldItem = this[aIndex];
            UnregisteItemEvents(aOldItem);
            base.SetItem(aIndex, aItem);
            RegisteItemEvents(aItem);
            if (aOldItem == ActiveItem) ActiveItem = aItem;
        }

        public T ActiveItem { get { return _ActiveItem; } set { SetValue(ref _ActiveItem, value, nameof(ActiveItem)); } }
        private T _ActiveItem;

        protected bool SetValue<M>(ref M aMember, M aValue, params string[] aPropertyNames)
        {
            if (object.Equals(aMember, aValue)) return false;
            aMember = aValue;
            foreach (string aPropertyName in aPropertyNames) OnPropertyChanged(aPropertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName]string aPropertyName = null)
        {
            // HACK: 如何更有效地设定不触发Modified状态的属性？
            if (aPropertyName != nameof(ActiveItem) && aPropertyName != nameof(IsModified))
                IsModified = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }

        public virtual void Dispose()
        {
            foreach (T aItem in this)
            {
                UnregisteItemEvents(aItem);
                if (aItem is IDisposable) (aItem as IDisposable).Dispose();
            }
            Clear();
        }

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
