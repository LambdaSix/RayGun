using System;

namespace Raygun
{
    class Sphere : SceneObject
    {
        public Vector Center { get; set; }
        public double Radius { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            var origin = Vector.Minus(Center, ray.Start);
            double v = Vector.Dot(origin, ray.Direction);
            double dist;

            if (v < 0)
            {
                dist = 0;
            }
            else
            {
                double disc = Math.Pow(Radius, 2) - (Vector.Dot(origin, origin) - Math.Pow(v, 2));
                dist = disc < 0 ? 0 : v - Math.Sqrt(disc);
            }

            if (Math.Abs(dist - 0) < 0.001)
                return null;

            return new Intersection
            {
                    Thing = this,
                    Ray = ray,
                    Distance = dist,
            };
        }

        public override Vector Normal(Vector pos)
        {
            return Vector.Norm(Vector.Minus(pos, Center));
        }
    }
}