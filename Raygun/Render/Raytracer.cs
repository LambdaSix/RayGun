using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raygun
{
    public class Raytracer
    {
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private const int MaxDepth = 5;

        public Action<int, int, System.Drawing.Color> SetPixel;

        public Raytracer(int screenWidth, int screenHeight, Action<int,int,System.Drawing.Color> setPixel )
        {
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
            SetPixel = setPixel;
        }

        private IEnumerable<Intersection> Intersections(Ray ray, Scene scene)
        {
            return scene.Things
                    .Select(obj => obj.Intersect(ray))
                    .Where(inter => inter != null)
                    .OrderBy(inter => inter.Distance);
        }

        private double TestRay(Ray ray, Scene scene)
        {
            var intersections = Intersections(ray, scene);
            var isect = intersections.FirstOrDefault();

            return isect != null ? isect.Distance : 0;
        }

        private Color TraceRay(Ray ray, Scene scene, int depth)
        {
            var intersections = Intersections(ray, scene);
            var isect = intersections.FirstOrDefault();

            return isect != null ? Shade(isect, scene, depth) : Color.Background;
        }

        private Color GetNaturalColor(SceneObject thing, Vector pos, Vector norm, Vector rd, Scene scene)
        {
            var color = Color.Make(0, 0, 0);

            foreach (var light in scene.Lights)
            {
                var lDis = Vector.Minus(light.Pos, pos);
                var liVec = Vector.Norm(lDis);

                double neatIntersect = TestRay(new Ray { Start = pos, Direction = liVec }, scene);
                bool inShadow = !((neatIntersect > Vector.Mag(lDis)) || (Math.Abs(neatIntersect - 0) < 0.001));

                if (!inShadow)
                {
                    double illum = Vector.Dot(liVec, norm);
                    var lColor = illum > 0 ? Color.Times(illum, light.Color) : Color.Make(0, 0, 0);
                    var specular = Vector.Dot(liVec, Vector.Norm(rd));

                    var sColor = specular > 0
                                         ? Color.Times(Math.Pow(specular, thing.Surface.Roughness), light.Color)
                                         : Color.Make(0, 0, 0);

                    color = Color.Plus(color,
                                       Color.Plus(Color.Times(thing.Surface.Diffuse(pos), lColor),
                                                  Color.Times(thing.Surface.Specular(pos), sColor)));
                }
            }
            return color;
        }

        private Color GetReflectionColor(SceneObject thing, Vector pos, Vector rd, Scene scene, int depth)
        {
            return Color.Times(thing.Surface.Reflect(pos), TraceRay(new Ray { Start = pos, Direction = rd }, scene, depth + 1));
        }

        private Color Shade(Intersection isect, Scene scene, int depth)
        {
            var dir = isect.Ray.Direction;
            var pos = Vector.Plus(Vector.Times(isect.Distance, dir), isect.Ray.Start);
            var normal = isect.Thing.Normal(pos);
            var reflectDir = Vector.Minus(dir, Vector.Times(2 * Vector.Dot(normal, dir), normal));

            var color = Color.DefaultColor;

            color = Color.Plus(color, GetNaturalColor(isect.Thing, pos, normal, reflectDir, scene));

            if (depth >= MaxDepth)
            {
                return Color.Plus(color, Color.Make(.5, .5, .5));
            }

            return Color.Plus(color,
                              GetReflectionColor(isect.Thing, Vector.Plus(pos, Vector.Times(0.001, reflectDir)), reflectDir, scene, depth));
        }

        private double RecenterX(double x)
        {
            return (x - (_screenWidth / 2.0)) / (2.0 * _screenWidth);
        }

        private double RecenterY(double y)
        {
            return -(y - (_screenHeight / 2.0)) / (2.0 * _screenHeight);
        }

        private Vector GetPoint(double x, double y, Camera camera)
        {
            return Vector.Norm(Vector.Plus(camera.Forward, Vector.Plus(Vector.Times(RecenterX(x), camera.Right),
                                                                       Vector.Times(RecenterY(y), camera.Up))));
        }

        private IEnumerable<Tuple<int,int>> GetScreen()
        {
            for (int y = 0; y < _screenHeight; y++)
            {
                for (int x = 0; x < _screenWidth; x++)
                {
                    yield return new Tuple<int, int>(x, y);
                }
            }
        }

        public void Render(Scene scene)
        {
            GetScreen().AsParallel().Select(tuple => {
                                                var color = TraceRay(new Ray
                                                {
                                                        Start = scene.Camera.Pos,
                                                        Direction = GetPoint(tuple.Item1, tuple.Item2, scene.Camera)
                                                }, scene, 0);
                                                return new { X = tuple.Item1, Y = tuple.Item2, Color = color.ToDrawingColor() };
                                            }).ToList().ForEach(s => SetPixel(s.X, s.Y, s.Color));
        }

        public readonly Scene DefaultScene = new Scene {
                Things = new SceneObject[] { 
                                new Plane {
                                    Norm = Vector.Make(0,1,0),
                                    Offset = 0,
                                    Surface = Surfaces.CheckerBoard
                                },
                                new Sphere {
                                    Center = Vector.Make(0,1,0),
                                    Radius = 1,
                                    Surface = Surfaces.Shiny
                                },
                                new Sphere() {
                                    Center = Vector.Make(-1,.5,1.5),
                                    Radius = .5,
                                    Surface = Surfaces.Shiny
                                }},
                Lights = new [] { 
                                new Light {
                                    Pos = Vector.Make(-2,2.5,0),
                                    Color = Color.Make(.49,.07,.07)
                                },
                                new Light {
                                    Pos = Vector.Make(1.5,2.5,1.5),
                                    Color = Color.Make(.07,.07,.49)
                                },
                                new Light {
                                    Pos = Vector.Make(1.5,2.5,-1.5),
                                    Color = Color.Make(.07,.49,.071)
                                },
                                new Light {
                                    Pos = Vector.Make(0,3.5,0),
                                    Color = Color.Make(.21,.21,.35)
                                }},
                Camera = Camera.Create(Vector.Make(3, 1, 4), Vector.Make(-1, .5, 0))
            };
    }
}
