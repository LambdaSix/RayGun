namespace Raygun
{
    public abstract class SceneObject
    {
        public Surface Surface { get; set; }

        public abstract Intersection Intersect(Ray ray);
        public abstract Vector Normal(Vector pos);
    }
}