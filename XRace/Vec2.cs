// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global
using System;
using System.Drawing;

namespace XRace
{
  /// <summary>
  /// 2D-Vector
  /// </summary>
  public struct Vec2
  {
    public readonly double x;
    public readonly double y;

    public Vec2(double x, double y)
    {
      this.x = x;
      this.y = y;
    }

    public Vec2 Mul(double k)
    {
      return new Vec2(x * k, y * k);
    }

    public Vec2 Minus(Vec2 v)
    {
      return new Vec2(x - v.x, y - v.y);
    }

    public Vec2 Minus(double x, double y)
    {
      return new Vec2(this.x - x, this.y - y);
    }

    public Vec2 MinusRad(double rad, double scale)
    {
      return new Vec2(x - Math.Sin(rad) * scale, y - Math.Cos(rad) * scale);
    }

    public Vec2 Plus(Vec2 v)
    {
      return new Vec2(x + v.x, y + v.y);
    }

    public Vec2 Plus(double x, double y)
    {
      return new Vec2(this.x + x, this.y + y);
    }

    public Vec2 PlusRad(double rad, double scale)
    {
      return new Vec2(x + Math.Sin(rad) * scale, y + Math.Cos(rad) * scale);
    }

    public double Dot(Vec2 v)
    {
      return x * v.x + y * v.y;
    }

    public double Mag()
    {
      return Math.Sqrt(x * x + y * y);
    }

    public Vec2 Norm()
    {
      double mag = Mag();
      double div = (mag == 0.0) ? double.PositiveInfinity : 1.0 / mag;
      return Mul(div);
    }

    public PointF ToP()
    {
      return new PointF((float)x, (float)y);
    }

    public static implicit operator PointF(Vec2 v)
    {
      return v.ToP();
    }

    public static implicit operator Vec2(PointF v)
    {
      return new Vec2(v.X, v.Y);
    }
  }
}
