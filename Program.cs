using System;
using System.Collections;

namespace HashTables
{
    class Program
    {
        public static void Main()
        {
            while (true) 
            {
                Console.Clear();
                Console.WriteLine("Выберите задачу:\n" +
                    "1: Хэш-таблица с цепочками\n" +
                    "2: Хэш-таблица с открытой адресацией");
                
                string choice = Console.ReadLine()!;

                switch (choice) 
                {
                    case "1":
                        TestChainHashTables();
                        Console.WriteLine("Нажмите любую клавишу, что вернуться");
                        Console.ReadKey();
                        break;
                    case "2":
                        TestOpenAddressingTables();
                        Console.WriteLine("Нажмите любую клавишу, что вернуться");
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Такого режима нет, попробуйте ещё раз");
                        break;
                }
            }
        }

        public static void TestChainHashTables()
        {
            Console.WriteLine("== ТЕСТИРОВАНИЕ ХЭШ-ТАБЛИЦ С ЦЕПОЧКАМИ ==\n");
            int[] keys = new int[100000];
            Random random = new Random();
            for (int i = 0; i < 100000; i++)
            {
                int key = random.Next(int.MaxValue); // Генерация случайных ключей
                keys[i] = key;
            }

            foreach (HashTableChains<int, string>.HashMethod hashMethod in Enum.GetValues(typeof(HashTableChains<int, string>.HashMethod)))
            {
                HashTableChains<int, string> hashTable = new HashTableChains<int, string>(hashMethod);

                foreach (int key in keys)
                {
                    hashTable.Insert(key, $"Value {key}");
                }

                hashTable.Analyze();
            }
        }

        public static void TestOpenAddressingTables()
        {
            Console.WriteLine("== ТЕСТИРОВАНИЕ ХЭШ-ТАБЛИЦ С ОТКРЫТОЙ АДРЕСАЦИЕЙ ==\n");
            int[] keys = new int[10000];
            Random random = new Random();
            for (int i = 0; i < 10000; i++)
            {
                int key = random.Next(int.MaxValue);
                keys[i] = key;
            }

            foreach (HashTableOpenAddressing<int, string>.ProbingMethod method in Enum.GetValues(typeof(HashTableOpenAddressing<int, string>.ProbingMethod)))
            {
                Console.WriteLine($"Метод разрешения коллизий: {method}");
                HashTableOpenAddressing<int, string> hashTable = new HashTableOpenAddressing<int, string>(method);

                foreach(int key in keys)
                {
                    hashTable.Insert(key, $"Value {key}");
                }

                // Подсчет длины самого длинного кластера
                int longestCluster = hashTable.LongestCluster();
                Console.WriteLine($"Длина самого длинного кластера: {longestCluster}\n");
            }
        }
    }
}