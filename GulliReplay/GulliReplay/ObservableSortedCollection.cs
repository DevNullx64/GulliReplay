using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace System.Collections.ObjectModel
{
    public class ObservableSortedCollection<T> :
        IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : IComparable<T>
    {
        protected ObservableCollection<T> Collection;

        object IList.this[int index]
        {
            get => Collection[index];
            set => this[index] = (T)value;
        }

        public T this[int index]
        {
            get => Collection[index];
            set
            {
                RemoveAt(index);
                _Add(value);
            }
        }

        public int Count => Collection.Count;

        public bool IsReadOnly => ((IList<T>)Collection).IsReadOnly;
        public bool IsFixedSize => ((IList)Collection).IsFixedSize;
        public object SyncRoot => ((IList)Collection).SyncRoot;
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
                int comparisonResult = value.CompareTo(Collection[middle]);
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

        private SynchronizationContext SyncContext = SynchronizationContext.Current;
        protected void OnNotifyCollectionChangedEventHandlerSync(object sender, NotifyCollectionChangedEventArgs e)
            => SyncContext.Post((s)
                => OnNotifyCollectionChangedEventHandler(this, s as NotifyCollectionChangedEventArgs), e);
        protected void OnNotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
            => CollectionChanged?.Invoke(this, e);

        protected void OnPropertyChangedEventHandlerSync(object sender, PropertyChangedEventArgs e)
            => SyncContext.Post((s)
                => OnPropertyChangedEventHandler(this, s as PropertyChangedEventArgs), e);
        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
            => PropertyChanged?.Invoke(this, e);

        public ObservableSortedCollection(IEnumerable<T> enumerable = null)
        {
            Collection = new ObservableCollection<T>();
            SyncContext = SynchronizationContext.Current;
            if (SyncContext == null)
            {
                Collection.CollectionChanged += OnNotifyCollectionChangedEventHandler;
                ((INotifyPropertyChanged)Collection).PropertyChanged += OnPropertyChangedEventHandler;
            }
            else
            {
                Collection.CollectionChanged += OnNotifyCollectionChangedEventHandlerSync;
                ((INotifyPropertyChanged)Collection).PropertyChanged += OnPropertyChangedEventHandlerSync;
            }
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

        public void Clear() => ((IList<T>)Collection).Clear();

        public bool Contains(T item) => ((IList<T>)Collection).Contains(item);
        public bool Contains(object value) => ((IList)Collection).Contains(value);

        public void CopyTo(T[] array, int arrayIndex) => ((IList<T>)Collection).CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((IList)Collection).CopyTo(array, index);

        public int IndexOf(T item)
        {
            int result = BinarySearchIndexOf(item);
            if (result < -1)
                result = -1;

            return result;
        }
        public int IndexOf(object value) => IndexOf((T)value);

        public void Insert(int index, T item) => Collection.Add(item);
        public void Insert(int index, object value) => Collection.Add((T)value);

        public bool Remove(T item) => ((IList<T>)Collection).Remove(item);
        public void Remove(object value) => ((IList)Collection).Remove(value);
        public void RemoveAt(int index) => ((IList<T>)Collection).RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => ((IList<T>)Collection).GetEnumerator();
        public IEnumerator<T> GetEnumerator() => ((IList<T>)Collection).GetEnumerator();
    }
}
