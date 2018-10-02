using PengSW.ObservableCollectionHelper;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace PengSW.DataContextModelHelper
{
    public class ObservableEntitySet<T> : ObservableCollectionEx<T> where T : class
    {
        public ObservableEntitySet(EntitySet<T> aEntitySet, DataContext aDataContext = null)
            : base(aEntitySet)
        {
            _DataContext = aDataContext;
            _EntitySet = aEntitySet;
        }

        protected override void ClearItems()
        {
            T[] aItems = this.ToArray();
            base.ClearItems();
            _EntitySet.Clear();
            if (_DataContext != null) _DataContext.GetTable<T>().DeleteAllOnSubmit(aItems);
        }

        protected override void InsertItem(int aIndex, T aItem)
        {
            base.InsertItem(aIndex, aItem);
            _EntitySet.Add(aItem);
            if (_DataContext != null) _DataContext.GetTable<T>().InsertOnSubmit(aItem);
        }

        protected override void RemoveItem(int index)
        {
            T aItem = Items[index];
            base.RemoveItem(index);
            _EntitySet.Remove(aItem);
            if (_DataContext != null) _DataContext.GetTable<T>().DeleteOnSubmit(aItem);
        }

        private EntitySet<T> _EntitySet;
        private DataContext _DataContext;

        public void AddRange(IEnumerable<T> aSource)
        {
            foreach (T aItem in aSource) Add(aItem);
        }
    }
}
