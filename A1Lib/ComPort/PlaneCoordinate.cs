using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.ComPort
{
    /// <summary>物理坐标点，单位毫米</summary>
    public class PlaneCoordinate
    {
        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 0.0;

        public PlaneCoordinate() { }

        public PlaneCoordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>加法运算</summary>
        public static PlaneCoordinate operator +(PlaneCoordinate a, PlaneCoordinate b) => new PlaneCoordinate(a.X + b.X, a.Y + b.Y);

        /// <summary>减法运算</summary>
        public static PlaneCoordinate operator -(PlaneCoordinate a, PlaneCoordinate b) => new PlaneCoordinate(a.X - b.X, a.Y - b.Y);

        /// <summary>取反运算</summary>
        public static PlaneCoordinate operator -(PlaneCoordinate a) => new PlaneCoordinate(-a.X, -a.Y);

        /// <summary>数乘运算</summary>
        public static PlaneCoordinate operator *(double scalar, PlaneCoordinate a) => new PlaneCoordinate(a.X * scalar, a.Y * scalar);

        /// <summary>数除运算</summary>
        public static PlaneCoordinate operator /(PlaneCoordinate a, double scalar)
        {
            if (scalar == 0) throw new DivideByZeroException("Cannot divide PlaneCoordinate by zero");
            return new PlaneCoordinate(a.X / scalar, a.Y / scalar);
        }

        /// <summary>相等比较</summary>
        public static bool operator ==(PlaneCoordinate a, PlaneCoordinate b)
        {
            // 物理坐标，小数点后面6位即可。
            const double epsilon = 1e-6;
            return Math.Abs(a.X - b.X) < epsilon && Math.Abs(a.Y - b.Y) < epsilon;
        }

        /// <summary>不等比较</summary>
        public static bool operator !=(PlaneCoordinate a, PlaneCoordinate b) => !(a == b);

        /// <summary>重写Equals方法</summary>
        public override bool Equals(object obj) => obj is PlaneCoordinate other && this == other;

        /// <summary>转换为字符串</summary>
        public override string ToString() => $"({X:F3}, {Y:F3})";

        public override int GetHashCode() => HashCode.Combine(this.X, this.Y);
    }

    /// <summary>平面物理大小，单位毫米</summary>
    public struct PhysicalSize
    {
        public double Width;
        public double Height;

        public PhysicalSize(double w, double h)
        {
            Width = w;
            Height = h;
        }
    }
}
