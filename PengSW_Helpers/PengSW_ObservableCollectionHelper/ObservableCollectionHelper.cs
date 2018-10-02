using System.Collections.Generic;
using System.Linq;

namespace PengSW.ObservableCollectionHelper
{
    public static class ObservableCollectionHelper
    {
        public static bool CanMoveUp<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, T aObject)
        {
            if (aObject == null) return false;
            int aOldIndex = aCollection.IndexOf(aObject);
            return aOldIndex > 0 && aOldIndex < aCollection.Count;
        }

        public static void MoveUp<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, T aObject)
        {
            int aOldIndex = aCollection.IndexOf(aObject);
            aCollection.Move(aOldIndex, aOldIndex - 1);
        }

        public static bool CanMoveDown<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, T aObject)
        {
            if (aObject == null) return false;
            int aOldIndex = aCollection.IndexOf(aObject);
            return aOldIndex >= 0 && aOldIndex < aCollection.Count - 1;
        }

        public static void MoveDown<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, T aObject)
        {
            int aOldIndex = aCollection.IndexOf(aObject);
            aCollection.Move(aOldIndex, aOldIndex + 1);
        }

        class IndexedItem<T>
        {
            public IndexedItem(int aIndex, T aItem)
            {
                Index = aIndex;
                Item = aItem;
            }
            public int Index { get; }
            public T Item { get; }
        }

        public static void Move<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, IEnumerable<T> aItems, int aDelta)
        {
            var aIndexedItems = from r in aItems select new IndexedItem<T>(aCollection.IndexOf(r), r);
            List<IndexedItem<T>> aSortedItems = (aDelta < 0 ? aIndexedItems.OrderBy(r => r.Index) : aIndexedItems.OrderByDescending(r => r.Index)).ToList();
            foreach (IndexedItem<T> aIndexedItem in aSortedItems)
            {
                aCollection.Move(aIndexedItem.Index, aIndexedItem.Index + aDelta);
            }
        }

        public static bool CanMove<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, IEnumerable<T> aObjects, int aDelta)
        {
            if (aCollection == null || aObjects == null || aObjects.Count() < 1) return false;
            int aIndex = aDelta > 0 ? (from r in aObjects select aCollection.IndexOf(r)).Max() : (from r in aObjects select aCollection.IndexOf(r)).Min();
            aIndex += aDelta;
            return aIndex >= 0 && aIndex < aCollection.Count;
        }

        public static void MoveUp<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, IEnumerable<T> aObjects)
        {
            Move<T>(aCollection, aObjects, -1);
        }
        public static bool CanMoveUp<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, IEnumerable<T> aObjects)
        {
            return CanMove<T>(aCollection, aObjects, -1);
        }

        public static bool CanMoveDown<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, IEnumerable<T> aObjects)
        {
            return CanMove<T>(aCollection, aObjects, 1);
        }

        public static void MoveDown<T>(this System.Collections.ObjectModel.ObservableCollection<T> aCollection, IEnumerable<T> aObjects)
        {
            Move<T>(aCollection, aObjects, 1);
        }
    }
}
