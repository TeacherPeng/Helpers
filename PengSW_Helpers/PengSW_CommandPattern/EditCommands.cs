using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PengSW.ObservableCollectionHelper;

namespace PengSW.CommandPattern
{
    public class IndexedItem<T>
    {
        public IndexedItem(int aIndex, T aItem)
        {
            Index = aIndex;
            Item = aItem;
        }
        public int Index { get; }
        public T Item { get; }
    }
    public class AddCommand<T> : ICommandPattern where T : class
    {
        public AddCommand(IList<T> aList, int aIndex, IEnumerable<T> aNewItems)
        {
            List = aList;
            Index = aIndex;
            NewItems = aNewItems.ToList();
        }
        public IList<T> List { get; }
        public int Index { get; }
        public List<T> NewItems { get; }
        public void Do()
        {
            int i = Index;
            if (i < 0) i = 0; else if (i > List.Count) i = List.Count;
            foreach (T aItem in NewItems)
            {
                List.Insert(i++, aItem);
            }
        }
        public void Undo()
        {
            foreach (T aItem in NewItems)
            {
                List.Remove(aItem);
            }
        }
    }
    public class DeleteCommand<T> : ICommandPattern where T : class
    {
        public DeleteCommand(IList<T> aList, IEnumerable<T> aItems)
        {
            List = aList;
            IndexedItems = new List<IndexedItem<T>>();
            foreach (T aItem in aItems)
            {
                IndexedItems.Add(new IndexedItem<T>(List.IndexOf(aItem), aItem));
            }
        }
        public IList<T> List { get; }
        public List<IndexedItem<T>> IndexedItems { get; }
        public void Do()
        {
            foreach (IndexedItem<T> aIndexedItem in IndexedItems)
            {
                List.Remove(aIndexedItem.Item);
            }
        }
        public void Undo()
        {
            foreach (IndexedItem<T> aIndexedItem in from r in IndexedItems orderby r.Index select r)
            {
                List.Insert(aIndexedItem.Index, aIndexedItem.Item);
            }
        }
    }
    public class MoveCommand<T> : ICommandPattern where T : class
    {
        public MoveCommand(System.Collections.ObjectModel.ObservableCollection<T> aList, IEnumerable<T> aItems, int aDelta)
        {
            List = aList;
            Items = aItems.ToList();
            Delta = aDelta;
        }
        public ObservableCollection<T> List { get; }
        public List<T> Items { get; }
        public int Delta { get; }
        public void Do()
        {
            List.Move<T>(Items, Delta);
        }
        public void Undo()
        {
            List.Move<T>(Items, -Delta);
        }
    }
}
