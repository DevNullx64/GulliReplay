using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.ObjectModel
{
    public class ObservableSortedCollection<T> :
        IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : IComparable<T>
    {
        protected readonly ObservableCollection<T> Collection = new ObservableCollection<T>();
        protected readonly IComparer<T> Comparer;

        public T this[int index]
        {
            get => Collection[index];
            set
            {
                RemoveAt(index);
                _Add(value);
            }
        }
        object IList.this[int index]
        {
            get => Collection[index];
            set => this[index] = (T)value;
        }

        public int Count => Collection.Count;

        public bool IsReadOnly => ((IList<T>)Collection).IsReadOnly;
        public bool IsFixedSize => ((IList)Collection).IsFixedSize;
        public object SyncRoot => ((IList)Collection).SyncRoot;
        public readonly SynchronizationContext SynchronizationObject = SynchronizationContext.Current;
        public bool IsSynchronized => ((IList)Collection).IsSynchronized;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected int BinarySearchIndexOf(T value)
        {
            if (value == null)
                return -1;

            int lower = 0;
            int upper = Collection.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = Comparer.Compare(value, Collection[middle]);
                if (comparisonResult == 0)
                    return middle;
                else if (comparisonResult < 0)
                    upper = middle - 1;
                else
                    lower = middle + 1;
            }

            return ~lower;
        }

        private int _Add(T item)
        {
            int index = BinarySearchIndexOf(item);
            if (index < 0)
            {
                int i = ~index;
                index = Collection.Count;
                Collection.Insert(i, item);
            }
            return index;
        }

        protected void OnNotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
            => SynchronizationObject.Post((s) => CollectionChanged?.Invoke(this, s as NotifyCollectionChangedEventArgs), e);

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
            => SynchronizationObject.Post((s) => PropertyChanged?.Invoke(this, s as PropertyChangedEventArgs), e);


        public ObservableSortedCollection(IEnumerable<T> enumerable = null)
            : this(Comparer<T>.Default, enumerable) { }
        public ObservableSortedCollection(IComparer<T> comparer, IEnumerable<T> enumerable = null)
        {
            Comparer = comparer;
            Collection.CollectionChanged += OnNotifyCollectionChangedEventHandler;
            ((INotifyPropertyChanged)Collection).PropertyChanged += OnPropertyChangedEventHandler;
            Add(enumerable);
        }

        public void Add(IEnumerable<T> enumerable)
        {
            if (enumerable != null)
                foreach (T item in enumerable)
                    _Add(item);
        }
        public void Add(T item) => _Add(item);
        public int Add(object value) => _Add((T)value);

        public void Clear() => Collection.Clear();

        public bool Contains(T item) => IndexOf(item) >= 0;
        public bool Contains(object value) => Contains((T)value);

        public void CopyTo(T[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((IList)Collection).CopyTo(array, index);

        public int IndexOf(T item)
        {
            int result = BinarySearchIndexOf(item);
            return (result >= 0) ? result : -1;
        }
        public int IndexOf(object value) => IndexOf((T)value);

        public void Insert(int index, T item) => Collection.Add(item);
        public void Insert(int index, object value) => Insert(index, (T)value);

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            else
                return false;
        }
        public void Remove(object value) => Remove((T)value);
        public void RemoveAt(int index) => Collection.RemoveAt(index);

        public IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}