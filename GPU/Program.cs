using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Cloo;

class Program
{
    static void Main()
    {
        // Enable the use of ExcelPackage without a license context (for non-commercial use)
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        // Array sizes
        int[] arraySizes = { 5000, 10000, 20000, 40000, 80000, 160000 };

        // Initialize OpenCL
        ComputePlatform platform = ComputePlatform.Platforms.FirstOrDefault();
        ComputeContextPropertyList properties = new ComputeContextPropertyList(platform);
        ComputeContext context = new ComputeContext(ComputeDeviceTypes.Gpu, properties, null, IntPtr.Zero);
        ComputeCommandQueue queue = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

        // Define the kernel
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

        // Table header
        Console.WriteLine("|  Méret  |   Futási idő (ms)   |");
        Console.WriteLine("|---------|---------------------|");

        // Define the file path
        string filePath = @"A:/Miskolci Egyetem/2023-24_tavasz/Parhuzamos eszkozok programozasa/bubble_parallel_cpu/PerformanceData.xlsx";
        FileInfo fileInfo = new FileInfo(filePath);

        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet;

            // Check if the worksheet already exists
            if (package.Workbook.Worksheets["PerformanceData"] == null)
            {
                // Add a new worksheet to the empty workbook
                worksheet = package.Workbook.Worksheets.Add("PerformanceData");

                // Add table headers
                worksheet.Cells[1, 1].Value = "Méret";
                worksheet.Cells[1, 3].Value = "Futási idő (ms)";

                // Table header style
                using (var range = worksheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }
            else
            {
                worksheet = package.Workbook.Worksheets["PerformanceData"];
            }

            // Clear existing data in the range to avoid overwriting issues
            if (worksheet.Dimension != null)
            {
                worksheet.Cells["C2:C" + worksheet.Dimension.End.Row].Clear();
            }

            int row = 2;

            // Iterate through array sizes
            foreach (int size in arraySizes)
            {
                // Populate array
                Random random = new Random();
                int[] data = Enumerable.Range(0, size).Select(_ => random.Next(10000)).ToArray();

                // Start time measurement
                DateTime startTime = DateTime.Now;

                // Copy data to GPU
                ComputeBuffer<int> buffer = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, data);

                // Set kernel arguments
                kernel.SetMemoryArgument(0, buffer);
                kernel.SetValueArgument(1, (uint)data.Length);

                // Execute the kernel
                queue.ExecuteTask(kernel, null);

                // Wait for the kernel to finish
                queue.Finish();

                // Copy results back from GPU
                queue.ReadFromBuffer(buffer, ref data, true, null);

                // End time measurement
                DateTime actualTime = DateTime.Now;
                TimeSpan elapsedTime = actualTime - startTime;

                // Write results to console
                Console.WriteLine($"| {size.ToString().PadLeft(6)}  |  {elapsedTime.TotalMilliseconds.ToString("0.0000").PadLeft(13)}      |");

                // Write results to Excel
                worksheet.Cells[row, 3].Value = elapsedTime.TotalMilliseconds;
                row++;

                // Dispose the buffer
                buffer.Dispose();
            }

            // Save the Excel file
            package.Save();

            //Console.WriteLine($"Results saved to {filePath}");
        }

        // Table footer
        Console.WriteLine("|---------|---------------------|");
    }
}
