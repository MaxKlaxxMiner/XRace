
using System;

namespace XRace
{
  public sealed class Player
  {
    public double posX;
    public double posY;
    double movX;
    double movY;

    /// <summary>
    /// berechnet ein Tick des Spielers
    /// </summary>
    /// <param name="l">linke Taste gedrückt</param>
    /// <param name="r">rechte Taste gedrückt</param>
    /// <param name="u">oben Taste gedrückt</param>
    /// <param name="d">unten Taste gedrückt</param>
    public void Calc(bool l, bool r, bool u, bool d)
    {
      movX = movX * 0.999;
      movY = movY * 0.999;
      if (Math.Abs(movX) < 0.00001) movX = 0;
      if (Math.Abs(movY) < 0.00001) movY = 0;
      if (l) movX -= 0.01;
      if (r) movX += 0.01;
      if (u) movY += 0.01;
      if (d) movY -= 0.01;
      posX += movX;
      posY += movY;
      if (posX < -1.0) movX += 0.001;
      if (posX > 1.0) movX -= 0.001;
      if (posY < -1.0) movY += 0.001;
      if (posY > 1.0) movY -= 0.001;
    }
  }
}
