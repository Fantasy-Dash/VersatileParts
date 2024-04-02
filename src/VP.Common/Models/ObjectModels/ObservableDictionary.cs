using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VP.Common.Models.ObjectModels
{
    /// <summary>
    /// 基于<see cref="Dictionary{TKey,TValue}" />
    /// 实现INotifyCollectionChanged
    /// 当添加、删除元素或者刷新整个列表时通知监听器
    /// </summary>
    /// <remarks>
    /// 初始化一个新的ObservableDictionary实例
    /// </remarks>
    /// <param name="capacity">内容大小</param>
    /// <param name="comparer">相等比较器</param>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class ObservableDictionary<TKey, TValue>(int capacity, IEqualityComparer<TKey>? comparer) : Dictionary<TKey, TValue>(capacity, comparer), IDictionary, ICollection<KeyValuePair<TKey, TValue>>, INotifyCollectionChanged, INotifyPropertyChanged where TKey : notnull
    {
        private SimpleMonitor?  _monitor; // 仅当子类调用BlockReentrancy() 或 在序列化期间 延迟分配。不要重命名(二进制序列化)

        [NonSerialized]
        private int _blockReentrancyCount;

        /// <inheritdoc cref="ObservableDictionary(int,IEqualityComparer{TKey})"/>
        public ObservableDictionary() : this(0, null) { }

        /// <inheritdoc cref="ObservableDictionary(int,IEqualityComparer{TKey})"/>
        public ObservableDictionary(int capacity) : this(capacity, null) { }

        /// <inheritdoc cref="ObservableDictionary(int,IEqualityComparer{TKey})"/>
        public ObservableDictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

        /// <inheritdoc cref="ObservableDictionary(int,IEqualityComparer{TKey})"/>
        /// <param name="dictionary">要复制到新字典中的源字典</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

        /// <inheritdoc cref="ObservableDictionary(IDictionary{TKey, TValue})"/>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) :
            this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException($"{dictionary}不能为空");
            foreach (var item in dictionary)
                Add(item.Key, item.Value);
        }

        /// <inheritdoc cref="ObservableDictionary(IDictionary{TKey, TValue})"/>
        /// <param name="collection">要复制到新字典中的源键值对集合</param>
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, null) { }

        /// <inheritdoc cref="ObservableDictionary(IEnumerable{KeyValuePair{TKey, TValue}})"/>
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) :
            this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer)
        {
            if (collection == null)
                throw new ArgumentNullException($"{collection}不能为空");
            foreach (var item in collection)
                Add(item.Key, item.Value);
        }

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                CheckReentrancy();
                var list = this.ToList();
                var index = list.FindIndex(row => row.Key.Equals(key));
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                var isUpdate = TryGetValue(key, out var v);

                base[key]=value;
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                if (isUpdate)
                    OnDictionaryChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, new KeyValuePair<TKey, TValue>(key, v!), index));
                else
                    OnDictionaryChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, Count-1));
            }
        }

        /// <summary>
        /// PropertyChanged 事件 (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }

        /// <summary>
        /// 在字典更改时(通过添加或删除项)发生。
        /// </summary>
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// 当字典被清空时;
        /// 对所有监听器引发DictionaryChanged事件。
        /// </summary>
        public new void Clear()
        {
            CheckReentrancy();
            base.Clear();
            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnDictionaryChanged(EventArgsCache.ResetCollectionChanged);
        }

        /// <summary>
        /// 将项添加到字典时;向所有监听器引发DictionaryChanged事件
        /// </summary>
        public new bool TryAdd(TKey key, TValue value)
        {
            CheckReentrancy();
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            if (base.TryAdd(pair.Key, pair.Value))
            {
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnDictionaryChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, Count-1));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将项添加到字典时;向所有监听器引发DictionaryChanged事件
        /// </summary>
        public new void Add(TKey key, TValue value)
        {
            CheckReentrancy();
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            base.Add(pair.Key, pair.Value);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnDictionaryChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, Count-1));
        }

        /// <summary>
        /// 当一个元素从列表中移除时;
        /// 对所有监听器引发DictionaryChanged事件。
        /// </summary>
        public new void Remove(TKey key)
        {
            CheckReentrancy();
            var list = this.ToList();
            var index = list.FindIndex(row => row.Key.Equals(key));

            base.Remove(key);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnDictionaryChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list[index], 0));
        }

        /// <summary>
        /// 当一个元素从列表中移除时;
        /// 对所有监听器引发DictionaryChanged事件。
        /// </summary>
        public new bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            CheckReentrancy();
            var list = this.ToList();
            var index = list.FindIndex(row => row.Key.Equals(key));

            if (base.Remove(key, out value))
            {
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnDictionaryChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list[index], 0));
                return true;
            }
            return false;
        }

        object? IDictionary.this[object key]
        {
            get
            {
                try
                {
                    return this[(TKey)key];
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException($"{key}无法转换为{typeof(TKey).Name}");
                }
            }
            set
            {
                try
                {
                    try
                    {
                        this[(TKey)key] = (TValue)value!;
                    }
                    catch (InvalidCastException)
                    {
                        throw new ArgumentException($"{value}无法转换为{typeof(TValue).Name}");

                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException($"{key}无法转换为{typeof(TKey).Name}");
                }
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) =>
    Add(keyValuePair.Key, keyValuePair.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair) => Remove(keyValuePair.Key, out _);

        /// <summary>
        /// 触发PropertyChanged事件 (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// PropertyChanged 事件 (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        [field: NonSerialized]
        protected event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 向所有监听器引发DictionaryChanged事件。
        /// 属性/方法修改此ObservableCollection将通过此虚拟方法引发集合更改事件。
        /// </summary>
        /// <remarks>
        /// 重写这个方法时，要么调用它的基础实现
        ///或者调用<see cref="BlockReentrancy"/>来防止可重入的集合变化。
        /// </remarks>
        protected virtual void OnDictionaryChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler? handler = CollectionChanged;
            if (handler != null)
            {
                // 这里没有调用BlockReentrancy()来避免SimpleMonitor的分配
                _blockReentrancyCount++;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _blockReentrancyCount--;
                }
            }
        }

        /// <summary>
        /// 不允许重入尝试更改此集合。例如，事件处理程序不允许对该集合进行更改
        /// </summary>
        /// <remarks>
        /// typical usage is to wrap e.g. a OnDictionaryChanged call with a using() scope:
        /// <code>
        ///         using (BlockReentrancy())
        ///         {
        ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
        ///         }
        /// </code>
        /// </remarks>
        protected IDisposable BlockReentrancy()
        {
            _blockReentrancyCount++;
            return EnsureMonitorInitialized();
        }

        /// <summary> 检查并断言更改此集合的可重入尝试 </summary>
        /// <exception cref="InvalidOperationException"> 在另一个集合更改仍在通知其他监听器时 更改集合时抛出 </exception>
        protected void CheckReentrancy()
        {
            if (_blockReentrancyCount > 0)
            {
                // we can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if (CollectionChanged?.GetInvocationList().Length > 1)
                    throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
            }
        }

        /// <summary>
        /// 为计数属性引发PropertyChanged事件
        /// </summary>
        private void OnCountPropertyChanged() => OnPropertyChanged(EventArgsCache.CountPropertyChanged);

        /// <summary>
        /// 为索引器属性引发PropertyChanged事件
        /// </summary>
        private void OnIndexerPropertyChanged() => OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

        private SimpleMonitor EnsureMonitorInitialized() => _monitor ??= new SimpleMonitor(this);

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            EnsureMonitorInitialized();
            _monitor!._busyCount = _blockReentrancyCount;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_monitor != null)
            {
                _blockReentrancyCount = _monitor._busyCount;
                _monitor._dictionary = this;
            }
        }

        // 这个类有助于防止重复调用
        [Serializable]
        private sealed class SimpleMonitor : IDisposable
        {
            internal int _busyCount; // 仅在(反)序列化期间使用，以保持与桌面的兼容性。不要重命名(二进制序列化)

            [NonSerialized]
            internal ObservableDictionary<TKey, TValue> _dictionary;

            public SimpleMonitor(ObservableDictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(dictionary != null);
                _dictionary = dictionary;
            }

            public void Dispose() => _dictionary._blockReentrancyCount--;
        }
    }

    internal static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new("Item[]");
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);
    }
}
