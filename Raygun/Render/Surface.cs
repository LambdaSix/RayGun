using System;

namespace Raygun
{
    public class Surface
    {
        public Func<Vector, Color> Diffuse { get; set; }
        public Func<Vector, Color> Specular { get; set; }
        public Func<Vector, double> Reflect { get; set; }
        public double Roughness { get; set; }
    }
}