using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

class Program
{
    static void Main()
    {
        // Enable the use of ExcelPackage without a license context (for non-commercial use)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        // Array sizes
        int[] arraySizes = { 5000, 10000, 20000, 40000, 80000, 160000 };

        Console.WriteLine("|  Méret  |   Futási idő (ms)   |");
        Console.WriteLine("|---------|---------------------|");

        // Define the file path
        string filePath = @"A:/Miskolci Egyetem/2023-24_tavasz/Parhuzamos eszkozok programozasa/bubble_parallel_cpu/PerformanceData.xlsx";
        FileInfo fileInfo = new FileInfo(filePath);

        using (ExcelPackage package = new ExcelPackage(fileInfo.Exists ? fileInfo : new FileInfo(filePath)))
        {
            ExcelWorksheet worksheet;

            // Check if the worksheet already exists
            if (package.Workbook.Worksheets["PerformanceData"] == null)
            {
                // Add a new worksheet to the empty workbook
                worksheet = package.Workbook.Worksheets.Add("PerformanceData");

                // Add table headers
                worksheet.Cells[1, 1].Value = "Méret";
                worksheet.Cells[1, 2].Value = "Futási idő (ms)";

                // Table header style
                using (var range = worksheet.Cells[1, 1, 1, 2])
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
                worksheet.Cells["A2:B" + worksheet.Dimension.End.Row].Clear();
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

                // Bubble Sort
                ParallelBubbleSort(data);

                // End time measurement
                DateTime actualTime = DateTime.Now;
                TimeSpan elapsedTime = actualTime - startTime;

                // Write results to console
                Console.WriteLine($"| {size.ToString().PadLeft(6)}  |  {elapsedTime.TotalMilliseconds.ToString("0.0000").PadLeft(13)}      |");

                // Write results to Excel
                worksheet.Cells[row, 1].Value = size;
                worksheet.Cells[row, 2].Value = elapsedTime.TotalMilliseconds;
                row++;
            }

            // Save the Excel file
            package.Save();
            Console.WriteLine("|---------|---------------------|");

            //Console.WriteLine($"Results saved to {filePath}");
        }
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
                    // Swap
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
