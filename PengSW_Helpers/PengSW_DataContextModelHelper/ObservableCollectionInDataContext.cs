using PengSW.ObservableCollectionHelper;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace PengSW.DataContextModelHelper
{
    public class ObservableCollectionInDataContext<T> : ObservableCollectionEx<T> where T : class
    {
        public ObservableCollectionInDataContext(DataContext aDataContext, IEnumerable<T> aItems)
            : base(aItems)
        {
            m_DataContext = aDataContext;
        }

        protected override void ClearItems()
        {
            T[] aItems = this.ToArray();
            base.ClearItems();
            m_DataContext.GetTable<T>().DeleteAllOnSubmit(aItems);
        }

        protected override void InsertItem(int aIndex, T aItem)
        {
            base.InsertItem(aIndex, aItem);
            m_DataContext.GetTable<T>().InsertOnSubmit(aItem);
        }

        protected override void RemoveItem(int aIndex)
        {
            T aItem = Items[aIndex];
            base.RemoveItem(aIndex);
            m_DataContext.GetTable<T>().DeleteOnSubmit(aItem);
        }

        private DataContext m_DataContext;
    }
}
