using System;
using System.Diagnostics;
using System.Drawing;
using Raygun;

namespace RayGun.Example
{
    class Program
    {
        static Bitmap _bitmap;
        const int Width = 600;
        const int Height = 600;

        static void Main(string[] args)
        {
            _bitmap = new Bitmap(Width, Height);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var rayTracer = new Raytracer(Width, Height, (x, y, color) => _bitmap.SetPixel(x, y, color));
            rayTracer.Render(rayTracer.DefaultScene);

            stopWatch.Stop();
            Console.WriteLine("Took {0}ms to render", stopWatch.ElapsedMilliseconds);

            _bitmap.Save("output.png");
            Console.WriteLine("Press enter to close window");
            Console.ReadLine();
        }
    }
}
