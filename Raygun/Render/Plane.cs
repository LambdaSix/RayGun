namespace Raygun
{
    public class Plane : SceneObject
    {
        public Vector Norm { get; set; }
        public double Offset { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            double denom = Vector.Dot(Norm, ray.Direction);
            if (denom > 0)
                return null;

            return new Intersection
            {
                    Thing = this,
                    Ray = ray,
                    Distance = (Vector.Dot(Norm, ray.Start) + Offset) / (-denom)
            };
        }

        public override Vector Normal(Vector pos)
        {
            return Norm;
        }
    }
}