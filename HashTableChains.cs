using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    class HashTableChains<TKey, TValue>
    {
        private const int _size = 1000;
        private List<(TKey, TValue)>[] _table;

        public enum HashMethod
        {
            Division,
            Multiplication,
            Fnv1a,
            Jenkins,
            Murmur,
            BKDR
        }

        private readonly HashMethod _hashMethod;

        public HashTableChains(HashMethod method)
        {
            _table = new List<(TKey, TValue)>[_size];
            for (int i = 0; i < _size; i++)
            {
                _table[i] = new List<(TKey, TValue)>();
            }
            _hashMethod = method;
        }

        public void Insert(TKey key, TValue value)
        {
            int index = GetHash(key) % _size;
            var chain = _table[index];

            for (var i = 0; i < chain.Count; i++)
            {
                if (chain[i].Item1.Equals(key))
                {
                    chain[i] = (key, value);
                    return;
                }
            }

            chain.Add((key, value));
        }

        public TValue Search(TKey key)
        {
            int index = GetHash(key);
            var chain = _table[index];

            foreach (var pair in chain)
            {
                if (pair.Item1.Equals(key))
                {
                    return pair.Item2;
                }
            }

            return default(TValue);
        }

        public bool Delete(TKey key)
        {
            int index = GetHash(key);
            var chain = _table[index];

            for (var i = 0; i < chain.Count; i++)
            {
                if (chain[i].Item1.Equals(key))
                {
                    chain.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private int GetHash(TKey key)
        {
            switch (_hashMethod)
            {
                case HashMethod.Division:
                    return HashDivision(key);
                case HashMethod.Multiplication:
                    return HashMultiplication(key);
                case HashMethod.Fnv1a:
                    return HashFnv1a(key);
                case HashMethod.Jenkins:
                    return HashJenkins(key);
                case HashMethod.Murmur:
                    return HashMurmur(key);
                case HashMethod.BKDR:
                    return HashBKDR(key);
                default:
                    throw new InvalidOperationException("Неизвестный метод хеширования");
            }
        }

        // Метод деления
        private int HashDivision(TKey key)
        {
            if (key == null)
            {
                return 0;
            }

            if (key is int intK)
            {
                return Math.Abs(intK) % _size;
            }

            if (key is string strK)
            {
                return Math.Abs(strK.Length) % _size;
            }

            return Math.Abs(key.GetHashCode()) % _size;
        }

        // Метод умножения
        private int HashMultiplication(TKey key)
        {
            double phi = (Math.Sqrt(5) - 1) / 2; // Золотое сечение

            if (key == null)
            {
                return 0;
            }

            if (key is int intK)
            {
                return (int)(_size * ((intK * phi) % 1));
            }

            if (key is string strK)
            {
                return (int)(_size * ((strK.Length * phi) % 1));
            }

            return (int)(_size * ((key.GetHashCode() * phi) % 1));
        }

        // Собственный метод хеширования (простой и быстрый)
        private int CustomHashFunction(int key)
        {
            return ((key ^ (key >> 16)) & 0x7FFFFFFF) % _size; // XOR и битовое сдвигование
        }

        private int HashFnv1a(TKey key)
        {
            const uint fnvPrime = 16777619;
            const uint fnvOffsetBasis = 2166136261;

            uint hash = fnvOffsetBasis ^ (uint)(key.GetHashCode() & 0xFFFFFFFF);
            hash *= fnvPrime;
            return (int)(hash % _size);
        }

        private int HashJenkins<TKey>(TKey key)
        {
            int hash = 0;

            if (key is string strKey)
            {
                foreach (char c in strKey)
                {
                    hash += c;
                    hash += (hash << 10);
                    hash ^= (hash >> 6);
                }
            }
            else
            {
                hash = key.GetHashCode();
            }

            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);

            return Math.Abs(hash % _size);
        }

        //BKDR - Brian Kernighan's Dickson's RISE
        private int HashBKDR<TKey>(TKey key)
        {
            const int seed = 131;
            int hash = 0;

            if (key is string strKey)
            {
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

        public int HashMurmur<TKey>(TKey key)
        {
            if (key is null)
            {
                return 0;
            }

            uint hash = MurmurHash3((uint)key.GetHashCode());
            return (int)Math.Abs(hash % _size);
        }

        public static uint MurmurHash3(uint input)
        {
            // Реализация преобразования хеша MurmurHash3
            input ^= input >> 16;
            input *= 0x85ebca6b;
            input ^= input >> 13;
            input *= 0xc2b2ae35;
            input ^= input >> 16;
            return input;
        }

        public void Analyze()
        {
            int totalElements = 0;
            int maxChainLength = 0;
            int minChainLength = int.MaxValue;

            foreach (var chain in _table)
            {
                int chainLength = chain.Count;
                totalElements += chainLength;

                if (chainLength > maxChainLength)
                    maxChainLength = chainLength;

                if (chainLength < minChainLength && chainLength > 0)
                    minChainLength = chainLength;
            }

            double loadFactor = (double)totalElements / _size;

            Console.WriteLine($"Метод хеширования: {_hashMethod}");
            Console.WriteLine($"Коэффициент заполнения: {loadFactor}");
            Console.WriteLine($"Длина самой длинной цепочки: {maxChainLength}");
            Console.WriteLine($"Длина самой короткой цепочки: {minChainLength}\n");
        }
    }
}
