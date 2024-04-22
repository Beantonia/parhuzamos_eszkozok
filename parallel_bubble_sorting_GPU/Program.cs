using Cloo;
using System;
using System.Diagnostics;
using System.Linq;

class Program
{
    static void Main()
    {
        // Tömbök mérete
        int[] arraySizes = { 5000, 10000, 20000, 50000, 100000 };

        // Inicializálás
        ComputePlatform platform = ComputePlatform.Platforms.FirstOrDefault();
        ComputeContextPropertyList properties = new ComputeContextPropertyList(platform);
        ComputeContext context = new ComputeContext(ComputeDeviceTypes.Gpu, properties, null, IntPtr.Zero);
        ComputeCommandQueue queue = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

        // Kernel definiálása
        string kernelSource = @"
            __kernel void BubbleSort(__global int* data, const unsigned int count)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    for (int j = 0; j < count - i - 1; j++)
                    {
                        if (data[j] > data[j + 1])
                        {
                            int temp = data[j];
                            data[j] = data[j + 1];
                            data[j + 1] = temp;
                        }
                    }
                }
            }
        ";

        ComputeProgram program = new ComputeProgram(context, kernelSource);
        program.Build(null, null, null, IntPtr.Zero);
        ComputeKernel kernel = program.CreateKernel("BubbleSort");

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

            // Adatok másolása a GPU-ra
            ComputeBuffer<int> buffer = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, data);

            // Kernel argumentumok beállítása
            kernel.SetMemoryArgument(0, buffer);
            kernel.SetValueArgument(1, (uint)data.Length);

            // Kernel futtatása
            queue.ExecuteTask(kernel, null);            

            // Eredmények visszamásolása a GPU-ról
            queue.ReadFromBuffer(buffer, ref data, true, null);

            // Időmérés vége
            DateTime actualTime = DateTime.Now;
            TimeSpan elapsedTime = actualTime - startTime;

            // Táblázat feltöltése
            Console.WriteLine($"| {size,-6}  |  {elapsedTime.TotalMilliseconds, -16}   |");

            // Törlés a bufferből
            buffer.Dispose();
        }

        // Táblázat alja
        Console.WriteLine("|---------|---------------------|");
    }
}
