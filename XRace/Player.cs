
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


    const double AccS = 0.0001;
    const double AccU = 0.0003;
    const double AccD = 0.0002;
    const double RotL = 0.000002;
    const double RotR = 0.000002;

    /// <summary>
    /// berechnet ein Tick des Spielers
    /// </summary>
    /// <param name="ml">nach links bewegen</param>
    /// <param name="mr">nach rechts bewegen</param>
    /// <param name="mu">nach oben bewegen</param>
    /// <param name="md">nach unten bewegen</param>
    /// <param name="rl">nach links drehen</param>
    /// <param name="rr">nach rechts drehen</param>
    public void Calc(bool ml, bool mr, bool mu, bool md, bool rl, bool rr)
    {
      if (ml) mov = mov.PlusRad(posR - Math.PI / 2, AccS);
      if (mr) mov = mov.PlusRad(posR + Math.PI / 2, AccS);
      if (mu) mov = mov.PlusRad(posR, AccU);
      if (md) mov = mov.MinusRad(posR, AccD);

      if (rl) movR -= RotL;
      if (rr) movR += RotR;
      if (movR < -Math.PI) movR += Math.PI * 2;
      if (movR > Math.PI) movR -= Math.PI * 2;

      pos = pos.Plus(mov);
      posR += movR;
    }
  }
}
