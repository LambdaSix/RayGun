using System.Collections.Generic;
using System.Linq;

namespace Raygun
{
    public class Scene
    {
        public IEnumerable<SceneObject> Things { get; set; }
        public IEnumerable<Light> Lights { get; set; }
        public Camera Camera { get; set; }

        public IEnumerable<Intersection> Intersect(Ray ray)
        {
            return Things.Select(thing => thing.Intersect(ray));
        }
    }
}