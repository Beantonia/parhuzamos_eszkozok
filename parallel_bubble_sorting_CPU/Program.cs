using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        // Tömbök mérete
        int[] arraySizes = { 5000, 10000, 20000, 50000, 100000 };

        // Táblázat fejléce
        Console.WriteLine("|  Méret  |   Futási idő (ms)   |");
        Console.WriteLine("|---------|---------------------|");

        // Tömbök méretének iterálása
        foreach (int size in arraySizes)
        {
            // Tömb feltöltése
            Random random = new Random();
            int[] data = Enumerable.Range(0, size).Select(_ => random.Next(10000)).ToArray();

            // Időmérés kezdete
            DateTime startTime = DateTime.Now;

            // Bubble Sort
            ParallelBubbleSort(data);

            // Időmérés vége
            DateTime actualTime = DateTime.Now;
            TimeSpan elapsedTime = actualTime - startTime;

            // Táblázat feltöltése
            Console.WriteLine($"| {size,-6}  |  {elapsedTime.TotalMilliseconds, -16}   |");
        }

        // Táblázat alja
        Console.WriteLine("|---------|---------------------|");
    }

    static void ParallelBubbleSort(int[] data)
    {
        bool swapped;
        int n = data.Length;
        do
        {
            swapped = false;
            Parallel.For(0, n - 1, i =>
            {
                if (data[i] > data[i + 1])
                {
                    // Csere
                    int temp = data[i];
                    data[i] = data[i + 1];
                    data[i + 1] = temp;
                    swapped = true;
                }
            });
            n--;
        } while (swapped);
    }
}
