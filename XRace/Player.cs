
using System;
// ReSharper disable MemberCanBePrivate.Global

namespace XRace
{
  public sealed class Player
  {
    /// <summary>
    /// absolute Position
    /// </summary>
    public Vec2 pos;
    /// <summary>
    /// Drehrichtung (in Rad: 2 * PI == 360°)
    /// </summary>
    public double posR;
    /// <summary>
    /// Bewegungsgeschwindigkeit
    /// </summary>
    public Vec2 mov;
    /// <summary>
    /// Rotationsgeschwindigkeit in Rad
    /// </summary>
    public double movR;

    /// <summary>
    /// letzte links/rechts Steuerung (-1 bis 1)
    /// </summary>
    public double lrLast;
    /// <summary>
    /// letzte oben/unten Steuerung (1 bis -1)
    /// </summary>
    public double udLast;
    /// <summary>
    /// letzte links/rechts Drehung (-1 bis 1)
    /// </summary>
    public double rtLast;

    /// <summary>
    /// max. Seitwärtsbeschleunigung
    /// </summary>
    const double AccS = 0.0001;
    /// <summary>
    /// max. Vorwärtsbeschleunigung
    /// </summary>
    const double AccU = 0.0005;
    /// <summary>
    /// max. Rückwärtsbremsung
    /// </summary>
    const double AccD = 0.0003;
    /// <summary>
    /// max. Rotationsbeschleunigung
    /// </summary>
    const double RotS = 0.000002;

    /// <summary>
    /// berechnet ein Tick des Spielers
    /// </summary>
    /// <param name="lr">links/rechts bewegen (-1 bis 1)</param>
    /// <param name="ud">oben/unten bewegen (1 bis -1)</param>
    /// <param name="rt">links/rechts rotieren (-1 bis 1)</param>
    public void Calc(double lr, double ud, double rt)
    {
      if (lr < -1) lr = -1; else if (lr > 1) lr = 1; lrLast = lr;
      if (ud < -1) ud = -1; else if (ud > 1) ud = 1; udLast = ud;
      if (rt < -1) rt = -1; else if (rt > 1) rt = 1; rtLast = rt;

      mov = mov.PlusRad(posR + Math.PI / 2, lr * AccS);
      mov = ud > 0 ? mov.PlusRad(posR, ud * AccU) : mov.PlusRad(posR, ud * AccD);

      movR += rt * RotS;

      if (movR < -Math.PI) movR += Math.PI * 2;
      if (movR > Math.PI) movR -= Math.PI * 2;

      pos = pos.Plus(mov);
      posR += movR;
    }
  }
}
