﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Manina.Windows.Forms
{
    /// <summary>
    /// Contains extension methods used in the assembly.
    /// </summary>
    internal static class ExtensionMethods
    {
        #region Rectangle
        public static Point GetBottomLeft(this Rectangle r) {
            return new Point(r.Left, r.Bottom);
        }

        public static Point GetTopLeft(this Rectangle r) {
            return new Point(r.Left, r.Top);
        }

        public static Point GetBottomRight(this Rectangle r) {
            return new Point(r.Right, r.Bottom);
        }

        public static Point GetTopRight(this Rectangle r) {
            return new Point(r.Right, r.Top);
        }

        public static Rectangle GetOffset(this Rectangle rec, int dx, int dy) {
            return new Rectangle(rec.Left + dx, rec.Top + dy, rec.Width, rec.Height);
        }

        public static Rectangle GetInflated(this Rectangle rec, int dx, int dy) {
            return new Rectangle(rec.Left - dx, rec.Top - dy, rec.Width + 2 * dx, rec.Height + 2 * dy);
        }

        public static Rectangle GetInflated(this Rectangle rec, Size size) {
            return rec.GetInflated(size.Width, size.Height);
        }

        public static Rectangle GetInflated(this Rectangle rec, Padding padding) {
            return new Rectangle(rec.Left - padding.Left, rec.Top - padding.Top, rec.Width + padding.Horizontal, rec.Height + padding.Vertical);
        }

        public static Rectangle GetDeflated(this Rectangle rec, int dx, int dy) {
            return new Rectangle(rec.Left + dx, rec.Top + dy, rec.Width - 2 * dx, rec.Height - 2 * dy);
        }

        public static Rectangle GetDeflated(this Rectangle rec, Size size) {
            return rec.GetDeflated(size.Width, size.Height);
        }

        public static Rectangle GetDeflated(this Rectangle rec, Padding padding) {
            return new Rectangle(rec.Left + padding.Left, rec.Top + padding.Top, rec.Width - padding.Horizontal, rec.Height - padding.Vertical);
        }

        public static Rectangle GetRotated(this Rectangle rec, Rectangle fitInside, TextDirection direction) {
            var rotatedRec = rec;

            if (direction == TextDirection.Down) {
                rotatedRec = new Rectangle(fitInside.Width - rec.Height - rec.Top, rec.Left, rec.Height, rec.Width);
            }
            else if (direction == TextDirection.Up) {
                rotatedRec = new Rectangle(rec.Top, fitInside.Height - rec.Width - rec.Left, rec.Height, rec.Width);
            }

            rotatedRec.Offset(fitInside.Left, fitInside.Top);

            return rotatedRec;
        }

        public static Rectangle GetCenteredInside(this Rectangle rec, Rectangle inside) {
            return new Rectangle(inside.Left + (inside.Width - rec.Width) / 2, inside.Top + (inside.Height - rec.Height) / 2, rec.Width, rec.Height);
        }

        public static Rectangle EnsureMinSize(this Rectangle rec, Size size) {
            return rec.EnsureMinSize(size.Width, size.Height);
        }

        public static Rectangle EnsureMinSize(this Rectangle rec, int width, int height) {
            return new Rectangle(rec.Left, rec.Top, Math.Max(rec.Width, width), Math.Max(rec.Height, height));
        }

        public static Rectangle EnsureMaxSize(this Rectangle rec, Size size) {
            return rec.EnsureMaxSize(size.Width, size.Height);
        }

        public static Rectangle EnsureMaxSize(this Rectangle rec, int width, int height) {
            return new Rectangle(rec.Left, rec.Top, Math.Min(rec.Width, width), Math.Min(rec.Height, height));
        }
        #endregion

        #region Size
        public static Size Max(this Size thisSize, Size size) {
            return new Size(Math.Max(thisSize.Width, size.Width), Math.Max(thisSize.Height, size.Height));
        }
        #endregion

        #region Point
        public static Point GetOffset(this Point pt, int dx, int dy) {
            return new Point(pt.X + dx, pt.Y + dy);
        }
        #endregion

        #region Graphics
        public static void DrawRectangleFixed(this Graphics g, Pen pen, int x, int y, int width, int height) {
            g.DrawRectangle(pen, x, y, width - 1, height - 1);
        }

        public static void DrawRectangleFixed(this Graphics g, Pen pen, Rectangle rec) {
            g.DrawRectangleFixed(pen, rec.X, rec.Y, rec.Width, rec.Height);
        }

        public static void DrawVerticalTextDown(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags) {
            if (bounds.Height <= 0 || bounds.Width <= 0)
                return;

            var imageBounds = new Rectangle(0, 0, bounds.Height, bounds.Width);

            using (var image = new Bitmap(imageBounds.Width, imageBounds.Height, PixelFormat.Format32bppArgb))
            using (Graphics imageGraphics = Graphics.FromImage(image)) {
                TextRenderer.DrawText(imageGraphics, text, font, imageBounds, foreColor, backColor, flags);
                // Rotate, translate and draw the image
                Point[] ptMap = new Point[] {
                    bounds.GetTopRight(),    // upper-left
                    bounds.GetBottomRight(), // upper-right
                    bounds.GetTopLeft(),     // lower-left
                };
                g.DrawImage(image, ptMap);
            }
        }

        public static void DrawVerticalTextUp(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags) {
            if (bounds.Height <= 0 || bounds.Width <= 0)
                return;

            var imageBounds = new Rectangle(0, 0, bounds.Height, bounds.Width);

            using (var image = new Bitmap(imageBounds.Width, imageBounds.Height, PixelFormat.Format32bppArgb))
            using (Graphics imageGraphics = Graphics.FromImage(image)) {
                TextRenderer.DrawText(imageGraphics, text, font, imageBounds, foreColor, backColor, flags);
                // Rotate, translate and draw the image
                Point[] ptMap = new Point[] {
                    bounds.GetBottomLeft(),  // upper-left
                    bounds.GetTopLeft(),     // upper-right
                    bounds.GetBottomRight(), // lower-left
                };
                g.DrawImage(image, ptMap);
            }
        }

        public static void DrawImageRotatedDown(this Graphics g, Image image, Rectangle bounds) {
            // Rotate, translate and draw the image
            Point[] ptMap = new Point[] {
                bounds.GetTopRight(),    // upper-left
                bounds.GetBottomRight(), // upper-right
                bounds.GetTopLeft(),     // lower-left
            };
            g.DrawImage(image, ptMap);
        }

        public static void DrawImageRotatedUp(this Graphics g, Image image, Rectangle bounds) {
            // Rotate, translate and draw the image
            Point[] ptMap = new Point[] {
                bounds.GetBottomLeft(),  // upper-left
                bounds.GetTopLeft(),     // upper-right
                bounds.GetBottomRight(), // lower-left
            };
            g.DrawImage(image, ptMap);
        }
        #endregion

        #region Color
        public static Color Lighten(this Color color, float amount) {
            float h = color.GetHue() / 360f;
            float s = color.GetSaturation();
            float l = color.GetBrightness();

            l = l + (1.0f - l) * amount;

            return ColorFromHSL(color.A / 255f, h, s, l);
        }

        public static Color Darken(this Color color, float amount) {
            float h = color.GetHue() / 360f;
            float s = color.GetSaturation();
            float l = color.GetBrightness();

            l = l - (l - 0f) * amount;

            return ColorFromHSL(color.A / 255f, h, s, l);
        }

        public static Color ColorFromHSL(float a, float h, float s, float l) {
            float m2 = (l <= 0.5f ? l * (s + 1f) : (l + s - l * s));
            float m1 = l * 2f - m2;
            float r = HueToRgb(m1, m2, h + 1f / 3f);
            float g = HueToRgb(m1, m2, h);
            float b = HueToRgb(m1, m2, h - 1f / 3f);

            return Color.FromArgb((int)(a * 255f), (int)(r * 255f), (int)(g * 255f), (int)(b * 255f));
        }

        private static float HueToRgb(float m1, float m2, float h) {
            if (h < 0f)
                h = h + 1f;
            if (h > 1f)
                h = h - 1f;


            if (h * 6f < 1f)
                return m1 + (m2 - m1) * h * 6f;
            else if (h * 2f < 1f)
                return m2;
            else if (h * 3f < 2f)
                return m1 + (m2 - m1) * (2f / 3f - h) * 6f;
            else
                return m1;
        }
        #endregion

        /// <summary>
        /// Filters the elements of an <see cref="IEnumerable"/> based on a specified type.
        /// </summary>

        public static IEnumerable<TResult> OfType<TResult>(
            this IEnumerable source) {
            if (source == null)
                throw new ArgumentNullException("source");

            return OfTypeYield<TResult>(source);
        }

        private static IEnumerable<TResult> OfTypeYield<TResult>(
            IEnumerable source) {
            foreach (var item in source)
                if (item is TResult)
                    yield return (TResult)item;
        }
        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>

        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector) {
            if (selector == null)
                throw new ArgumentNullException("selector");

            return source.Select((item, i) => selector(item));
        }
        /// <summary>
        /// Projects each element of a sequence into a new form by 
        /// incorporating the element's index.
        /// </summary>

        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("selector");

            return SelectYield(source, selector);
        }
        private static IEnumerable<TResult> SelectYield<TSource, TResult>(
         IEnumerable<TSource> source,
         Func<TSource, int, TResult> selector) {
            var i = 0;
            foreach (var item in source)
                yield return selector(item, i++);
        }
        /// <summary>
        /// Creates an array from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static TSource[] ToArray<TSource>(
            this IEnumerable<TSource> source) {
            return source.ToList().ToArray();
        }
        /// <summary>
        /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>

        public static List<TSource> ToList<TSource>(
            this IEnumerable<TSource> source) {
            if (source == null)
                throw new ArgumentNullException("source");

            return new List<TSource>(source);
        }
        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static int Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int> selector) {
            return source.Select(selector).Sum();
        }
        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> values.
        /// </summary>

        public static int Sum(
            this IEnumerable<int> source) {
            if (source == null)
                throw new ArgumentNullException("source");

            int sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }
    }

    //public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T arg);
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    //public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    //public delegate TResult Func<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

}


namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute
    {
    }
}
namespace System.Linq
{

}
namespace Manina.Windows.Forms.Properties
{
}
namespace WinTabControls.Properties
{

}