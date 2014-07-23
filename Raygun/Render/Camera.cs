namespace Raygun
{
    public class Camera
    {
        public Vector Pos { get; set; }
        public Vector Forward { get; set; }
        public Vector Up { get; set; }
        public Vector Right { get; set; }

        public static Camera Create(Vector pos, Vector lookAt)
        {
            var forward = Vector.Norm(Vector.Minus(lookAt, pos));
            var down = new Vector(0, -1, 0);
            var right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
            var up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

            return new Camera { Pos = pos, Forward = forward, Up = up, Right = right };
        }
    }
}