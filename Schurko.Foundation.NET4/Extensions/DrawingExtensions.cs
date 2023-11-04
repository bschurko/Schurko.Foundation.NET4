using System.Drawing;

namespace Schurko.Foundation.Extensions
{
    public static class DrawingExtensions
    {
        public static Rectangle ToRectangle(this RectangleF rectF) => new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
    }
}
