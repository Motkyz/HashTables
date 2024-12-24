using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    public class HashTableOpenAddressing<TKey, TValue>
    {
        private (TKey key, TValue value)?[] _table;
        private int count;
        private static int _size = 10000;

        public enum ProbingMethod
        {
            LinearProbing,
            QuadraticProbing,
            DoubleHashing,
            Fibonacci,
            ExponentialProbing
        }

        public HashTableOpenAddressing(ProbingMethod method)
        {
            _table = new (TKey key, TValue value)?[_size];
            count = 0;
            _probingMethod = method;
        }

        private readonly ProbingMethod _probingMethod;

        public void Insert(TKey key, TValue value)
        {
            if ((double)count / _size >= 0.5)
            {
                Rehash();
            }

            int index = HashBKDR(key);
            int step = 0;

            while (step < _size)
            {
                int newIndex = GetProbing(index, step, key);

                if (_table[newIndex] == null || _table[newIndex].Value.key.Equals(key))
                {
                    _table[newIndex] = (key, value);
                    count++;
                    return;
                }
                step++;
            }
        }

        public TValue Search(TKey key)
        {
            int index = HashBKDR(key);
            int step = 0;

            while (step < _size)
            {
                int newIndex = GetProbing(index, step, key);

                if (_table[newIndex].Value.key == null)
                {
                    return default;
                }

                if (_table[newIndex].Value.key.Equals(key))
                {
                    return _table[newIndex].Value.value;
                }

                step++;
            }

            return default;
        }

        public void Delete(TKey key)
        {
            int index = HashBKDR(key);
            int step = 0;

            while (step < _size)
            {
                int newIndex = GetProbing(index, step, key);

                if (_table[newIndex].Value.key == null)
                {
                    return;
                }
                
                if (_table[newIndex].Value.key.Equals(key))
                {
                    _table[newIndex] = default;
                    count--;
                    return;
                }

                step++;
            }
        }

        private void Rehash()
        {
            int newSize = GetNextPrime(_size * 2);
            var oldTable = _table;

            _table = new (TKey, TValue)?[newSize];
            _size = newSize;
            count = 0;

            foreach (var pair in oldTable)
            {
                if (pair is not null)
                    Insert(pair.Value.key, pair.Value.value);
            }
        }

        private int GetNextPrime(int start)
        {
            while (!IsPrime(start)) start++;
            return start;
        }

        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            for (int i = 2; i * i <= number; i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        private int GetProbing(int index, int step, TKey key) 
        {
            switch (_probingMethod)
            {
                case ProbingMethod.LinearProbing:
                    return LinearProbe(index, step);
                case ProbingMethod.QuadraticProbing:
                    return QuadraticProbe(index, step);
                case ProbingMethod.DoubleHashing:
                    return DoubleHashProbe(index, step, key);
                case ProbingMethod.Fibonacci:
                    return FibonacciHash(key, step);
                case ProbingMethod.ExponentialProbing:
                    return ExponentialProbingHash(key, step);
                default:
                    throw new InvalidOperationException("Неизвестный метод разрешения коллизий");
            }
        }

        private int LinearProbe(int index, int step)
        {
            return (index + step) % _size;
        }

        private int QuadraticProbe(int index, int step)
        {
            return (index + step * step) % _size;
        }

        private int DoubleHashProbe(int index, int step, TKey key)
        {
            return (index + step * Hash2(key)) % _size;
        }

        private int Hash2(TKey key)
        {
            return 7 - (HashBKDR(key) % 7);
        }

        private int ExponentialProbingHash(TKey key, int i)
        {
            int hash = SimpleHash(key, _size);
            return (hash + (int)Math.Pow(2, i)) % _size;
        }

        private int FibonacciHash(TKey key, int i)
        {
            int hash = SimpleHash(key, _size);
            int fib = (int)((Math.Pow((1 + Math.Sqrt(5)) / 2, i) - Math.Pow((1 - Math.Sqrt(5)) / 2, i)) / Math.Sqrt(5));
            return (hash + fib) % _size;
        }

        public static int SimpleHash(TKey key, int tableSize)
        {
            return key.GetHashCode() % tableSize;
        }

        private int HashBKDR(TKey key)
        {
            int hash = 0;
            if (key is string strKey)
            {
                const int seed = 131;
                foreach (char c in strKey)
                {
                    hash = hash * seed + c;
                }
            }
            else
            {
                hash = key.GetHashCode();
            }
            return Math.Abs(hash % _size);
        }

        public int LongestCluster()
        {
            int longest = 0;
            int currentClusterLength = 0;

            for (int i = 0; i < _size; i++)
            {
                if (_table[i] != null)
                {
                    currentClusterLength++;
                }
                else
                {
                    if (currentClusterLength > longest)
                    {
                        longest = currentClusterLength;
                    }
                    currentClusterLength = 0;
                }
            }

            return Math.Max(longest, currentClusterLength);
        }
    }
}
