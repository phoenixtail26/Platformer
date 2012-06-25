using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// A double-ended queue. Provides fast insertion and removal from the head or tail end, 
    /// and fast indexed access.
    /// </summary>
    /// <typeparam name="T">The type of item to store in the deque.</typeparam>
    public class Deque<T> : IList<T>
    {
        //Constants
        private const int Min_Capacity = 4;
        //Fields
        private int _head = 0;
        private int _capacity = Min_Capacity;
        private int _count;
        private T[] _data;
        /// <summary>
        /// Gets the number of items in the deque.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }
        /// <summary>
        /// Gets or sets the capacity of the deque. If the count exceeds the 
        /// capacity, the capacity will be automatically increased.
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
            set
            {
                int newCapacity = Math.Max( value, Math.Max( _count, Min_Capacity ) );
                T[] temp = new T[newCapacity];
                int length = Math.Min( _capacity - _head, _count );
                Array.Copy( _data, _head, temp, 0, length );
                Array.Copy( _data, 0, temp, length, _count - length );
                _data = temp;
                _head = 0;
                _capacity = newCapacity;
            }
        }
        /// <summary>
        /// Creates a new deque.
        /// </summary>
        public Deque()
        {
            _data = new T[_capacity];
        }
        /// <summary>
        /// Creates a new deque.
        /// </summary>
        /// <param name="capacity">The initial capacity to give the deque.</param>
        public Deque( int capacity )
        {
            _capacity = capacity;
            _data = new T[_capacity];
        }
        /// <summary>
        /// Creates a new deque from a collection.
        /// </summary>
        /// <param name="items">A collection of items of type T.</param>
        public Deque( ICollection<T> items )
        {
            Capacity = items.Count;
            foreach ( T item in items )
            {
                this.Add( item );
            }
        }
        private int Tail
        {
            get { return ( _head + _count ) % _capacity; }
        }
        //Increments (and wraps if necessary) an index
        private int Increment( int index )
        {
            return ( index + 1 ) % _capacity;
        }
        //Decrements (and wraps if necessary) an index
        private int Decrement( int index )
        {
            return ( index + _capacity - 1 ) % _capacity;
        }
        /// <summary>
        /// Adds an item to the tail end of the deque.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddToTail( T item )
        {
            if ( _count + 1 > _capacity ) Capacity *= 2;
            _data[Tail] = item;
            _count++;
        }
        /// <summary>
        /// Removes an item from the tail of the deque.
        /// </summary>
        /// <returns>An item of type T.</returns>
        public T RemoveFromTail()
        {
            if ( _count < 0 )
            {
                throw new InvalidOperationException( "Deque is empty." );
            }
            T item = _data[Tail];
            _data[Tail] = default( T );
            _count--;
            return item;
        }
        /// <summary>
        /// Adds an item to the head of the deque.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddToHead( T item )
        {
			if ( item == null )
			{
				UnityEngine.Debug.Log("Adding null object");
			}
            _count++;
            if ( _count > _capacity ) Capacity *= 2;
            if ( _count > 1 ) _head = Decrement( _head );
            _data[_head] = item;
        }
        /// <summary>
        /// Removes an item from the head of the deque.
        /// </summary>
        /// <returns>An item of type T.</returns>
        public T RemoveFromHead()
        {
            _count--;
            if ( _count < 0 )
            {
                throw new InvalidOperationException( "Deque is empty." );
            }
            T item = _data[_head];
            _data[_head] = default( T );
            _head = Increment( _head );
            return item;
        }
        /// <summary>
        /// Gets the item at the head of the deque.
        /// </summary>
        /// <returns>An item of type T.</returns>
        public T PeekHead()
        {
            return _data[_head];
        }
        /// <summary>
        /// Gets the item at the tail of the deque.
        /// </summary>
        /// <returns>An item of type T.</returns>
        public T PeekTail()
        {
            return _data[Tail];
        }
        /// <summary>
        /// Gets the item at the specified position.
        /// </summary>
        /// <param name="position">The position of the item to return.</param>
        /// <returns>An item of type T.</returns>
        public T this[int position]
        {
            get
            {
                if ( position >= _count || position < 0 ) throw new ArgumentOutOfRangeException( "position" );
                return _data[( _head + position ) % _capacity];
            }
            set
            {
                if ( position >= _count || position < 0 ) throw new ArgumentOutOfRangeException( "position" );
                _data[( _head + position ) % _capacity] = value;
            }
        }
        /// <summary>
        /// Creates an array of the items in the deque.
        /// </summary>
        /// <returns>An array of type T.</returns>
        public T[] ToArray()
        {
            T[] array = new T[_count];
            CopyTo( array, 0 );
            return array;
        }
        /// <summary>
        /// Copies the deque to an array at the specified index.
        /// </summary>
        /// <param name="array">One dimensional array that is the destination of the copied elements.</param>
        /// <param name="arrayIndex">The zero-based index at which copying begins.</param>
        public void CopyTo( T[] array, int arrayIndex )
        {
            if ( _count == 0 ) return;
            int length = Math.Min( _capacity - _head, _count );
            Array.Copy( _data, _head, array, 0, length );
            Array.Copy( _data, 0, array, length, _count - length );
        }
        /// <summary>
        /// Gets and removes an item at the specified index. 
        /// </summary>
        /// <param name="index">The index at which to remove the item.</param>
        /// <returns>An item of type T.</returns>
        public T RemoveAt( int index )
        {
            if ( index >= _count ) throw new ArgumentOutOfRangeException( "index" );
            int i = ( _head + index ) % _capacity;
            T item = _data[i];
            if ( i < _head )
            {
                Array.Copy( _data, i + 1, _data, i, Tail - i );
                _data[Tail] = default( T );
            }
            else
            {
                Array.Copy( _data, _head, _data, _head + 1, i - _head );
                _data[_head] = default( T );
                _head = Increment( _head );
            }
            _count--;
            return item;
        }
        /// <summary>
        /// Clears all items from the deque.
        /// </summary>
        public void Clear()
        {
            Array.Clear( _data, 0, _capacity );
            _head = 0;
            _count = 0;
        }
        /// <summary>
        /// Gets an enumerator for the deque.
        /// </summary>
        /// <returns>An IEnumerator of type T.</returns>
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            for ( int i = 0; i < _count; i++ )
            {
                yield return this[i];
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the tail of the deque. 
        /// </summary>
        /// <param name="item"></param>
        public void Add( T item )
        {
            AddToTail( item );
        }
        /// <summary>
        /// Checks to see if the deque contains the specified item.
        /// </summary>
        /// <param name="item">The item to search the deque for.</param>
        /// <returns>A boolean, true if deque contains item.</returns>
        public bool Contains( T item )
        {
            for ( int i = 0; i < this.Count; i++ )
            {
                if ( this[i].Equals( item ) )
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets whether or not the deque is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes an item from the deque.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>Boolean true if the item was removed.</returns>
        public bool Remove( T item )
        {
            for ( int i = 0; i < this.Count; i++ )
            {
                if ( this[i].Equals( item ) )
                {
                    RemoveAt( i );
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets the first index of an item in the list.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The integer index of the item.</returns>
        public int IndexOf( T item )
        {
            for ( int i = 0; i < this.Count; i++ )
            {
                if ( this[i].Equals( item ) )
                {
                    return i;
                }
            }
            throw new InvalidOperationException( "Item not found." );
        }
        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert( int index, T item )
        {
            if ( index > _count || index < 0 ) throw new ArgumentOutOfRangeException( "index" );
            _count++;
            if ( _count > _capacity ) Capacity *= 2;
            int split = _capacity - _head;
            int i = ( _head + index ) % _capacity;
            if ( index < split )
            {
                Array.Copy( _data, i, _data, i + 1, _count - index );
            }
            else
            {
                Array.Copy( _data, _head, _data, _head - 1, index );
            }
            _data[i] = item;
        }

        void IList<T>.RemoveAt( int index )
        {
            RemoveAt( index );
        }
    }
}