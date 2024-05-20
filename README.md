# Comparison of parallel Bubble sorting on CPU and GPU.

## Overview
The goal of this task is to compare the parallelized bubble sorting method on CPU and on GPU



## CPU
#### Description:
This C# program demonstrates parallel bubble sort algorithm along with a program to measure its execution time on arrays of various sizes.

#### Requirements:
- .NET Framework or .NET Core SDK installed on your machine.

#### Setup:
- Step 1:  Clone this repository to your local machine using Git.
- Step 2:  Navigate to the directory containing the Program.cs file and compile the code using the C# compiler. Then run the compiled executable.
- Step 3:  After running the program, you will see a table displaying the size of the array and its corresponding execution time in milliseconds for each array size tested.

#### Usage:
- Execute the compiled program.
- After running the program, a table will be displayed showing the size of the array and its corresponding execution time in milliseconds for each array size tested.

#### Credits:
- The parallel bubble sort algorithm implementation in this project is based on the classic bubble sort algorithm.
- The Parallel.For construct available in the .NET Framework is utilized for parallelizing the sorting process.


## GPU
#### Description:
This C# program showcases parallel sorting using the Bubble Sort algorithm on a GPU (Graphics Processing Unit) leveraging the Cloo library, which provides .NET bindings for OpenCL.

#### Requirements:
- .NET Framework or .NET Core SDK installed
- OpenCL runtime installed (typically provided by GPU drivers)

#### Setup:
- Step 1:  Clone or download the repository containing the source code.
- Step 2:  Ensure that the Cloo library is referenced in the project.
- Step 3:  Build the project using Visual Studio or the command line.

#### Usage:
- Run the compiled executable.
- The program will generate an array of random integers and sort them using the Bubble Sort algorithm on the GPU.
- The sorted array will be displayed in the console output along with the execution time of the sorting process.

#### Credits:
- Cloo library: https://github.com/clSharp/cloo
- OpenCL: https://www.khronos.org/opencl/

## Results
### CPU:
![CPU results](https://github.com/Beantonia/parhuzamos_eszkozok/blob/main/CPU/CPU_results.png)

### GPU:
![GPU results](https://github.com/Beantonia/parhuzamos_eszkozok/blob/main/GPU/GPU_results.png)

### Plot to compare:
![Plot](https://github.com/Beantonia/parhuzamos_eszkozok/blob/main/plot_CPU_GPU.png)

## Conclusion
Based on the results observed, it can be concluded that the GPU-based solution is more favorable than the CPU-based one.
