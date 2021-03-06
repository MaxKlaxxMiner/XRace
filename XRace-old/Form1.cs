﻿#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
#endregion

namespace XRace
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    #region # timeBeginPeriod / timeEndPeriod
    /// <summary>
    /// ermöglicht eine Verringerung der Wartephasen zwischen den Windows-Ticks (eine Erhöhung ist nicht möglich und wird ignoriert)
    /// </summary>
    /// <param name="tick">Wartezeit in Millisekunden</param>
    [DllImport("winmm.dll", SetLastError = true)]
    [DebuggerHidden]
    static extern void timeBeginPeriod(int tick);
    /// <summary>
    /// hebt die Wartezeit von timeBeginPeriod() wieder auf
    /// </summary>
    /// <param name="tick">Wartezeit in Millisekunden</param>
    [DllImport("winmm.dll", SetLastError = true)]
    [DebuggerHidden]
    static extern void timeEndPeriod(int tick);
    #endregion

    Bitmap bild;

    readonly Game game = new Game();
    readonly Random rnd = new Random();

    readonly Vec2 autoStart = new Vec2(-1100, -900);
    readonly Vec2 autoEnd = new Vec2(-300, 700);

    void Zeichne()
    {
      if (bild == null || bild.Width != pictureBox1.Width || bild.Height != pictureBox1.Height)
      {
        if (pictureBox1.Width * pictureBox1.Height == 0) return;
        bild = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppRgb);
        pictureBox1.Image = bild;
      }

      float scale = Math.Min(bild.Width, bild.Height) / 2f / 1000f;
      var g = Graphics.FromImage(bild);
      g.Clear(Color.Black);
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.TranslateTransform(bild.Width / 2f, bild.Height / 2f);
      g.ScaleTransform(scale, -scale);
      var penSize = 2f / scale;

      var p = new Pen(Color.LightBlue, penSize);
      var pg = new Pen(Color.FromArgb(0x222222 - 16777216), penSize);
      var ph = new Pen(Color.FromArgb(0x555555 - 16777216), penSize);
      var pr = new Pen(Color.FromArgb(0x0080ff - 16777216), penSize);

      const double Vol = 100.0;
      const double R15 = Math.PI / 12.0;
      var pl = game.player;

      // --- Debug-Ratation ---
      g.DrawLine(pg, pl.pos.PlusRad(pl.posR, 30000), pl.pos.MinusRad(pl.posR, 30000));
      g.DrawLine(pg, pl.pos.PlusRad(pl.posR + R15 * 6, 30000), pl.pos.MinusRad(pl.posR + R15 * 6, 30000));
      for (int r = 0; r < 1000; r += 50)
      {
        g.DrawLine(pg, pl.pos.PlusRad(pl.posR + pl.movR * r, 300), pl.pos.PlusRad(pl.posR + pl.movR * (r + 50), 300));
      }
      if (pl.movR < 0)
      {
        g.DrawLine(ph, pl.pos.PlusRad(pl.posR + pl.movR * 790, 300 + pl.movR * 20000), pl.pos.PlusRad(pl.posR + pl.movR * 1000, 300));
        g.DrawLine(ph, pl.pos.PlusRad(pl.posR + pl.movR * 810, 300 - pl.movR * 20000), pl.pos.PlusRad(pl.posR + pl.movR * 1000, 300));
      }
      else
      {
        g.DrawLine(ph, pl.pos.PlusRad(pl.posR + pl.movR * 810, 300 + pl.movR * 20000), pl.pos.PlusRad(pl.posR + pl.movR * 1000, 300));
        g.DrawLine(ph, pl.pos.PlusRad(pl.posR + pl.movR * 790, 300 - pl.movR * 20000), pl.pos.PlusRad(pl.posR + pl.movR * 1000, 300));
      }

      // --- Debug-Speed ---
      double speed = pl.mov.Mag() * 500;
      if (speed >= 1)
      {
        double r = pl.mov.Rad();
        var pdir = pl.pos.PlusRad(r, speed);
        var pdir2 = pl.pos.PlusRad(r + Math.PI, speed * 0.7);
        g.DrawLine(pg, pdir, pdir2);
        g.DrawLine(ph, pdir, pl.pos.PlusRad(r - R15, speed * 0.7));
        g.DrawLine(ph, pdir, pl.pos.PlusRad(r + R15, speed * 0.7));
        g.DrawLine(ph, pdir2, pl.pos.PlusRad(r - R15 * 0.7 + Math.PI, speed));
        g.DrawLine(ph, pdir2, pl.pos.PlusRad(r + R15 * 0.7 + Math.PI, speed));
      }

      // --- Troll-Face ---
      //var m = g.Transform;
      //g.TranslateTransform((float)pl.pos.x, (float)pl.pos.y);
      //g.RotateTransform((float)(pl.posR / Math.PI * -180));
      //g.TranslateTransform(-128f * 2, 117f * 2 - 300);
      //g.ScaleTransform(2f, -2f);
      //g.DrawImage(troll, 0, 0);
      //g.Transform = m;

      // --- Autopilot Lines ---
      g.DrawLine(p, autoStart, autoEnd);
      var dir = autoEnd.Minus(autoStart).Norm();
      double dist = pl.pos.Minus(autoStart).Cross(dir);
      var pp2 = pl.pos.PlusRad(autoEnd.Minus(autoStart).Rad() - R15 * 6, dist);
      g.DrawLine(p, pl.pos, pp2);

      // --- Player Ship ---
      var ptl = pl.pos.PlusRad(pl.posR - R15, Vol).ToP();
      var ptr = pl.pos.PlusRad(pl.posR + R15, Vol).ToP();
      var pbl = pl.pos.PlusRad(pl.posR - R15 * 9, Vol).ToP();
      var pbr = pl.pos.PlusRad(pl.posR + R15 * 9, Vol).ToP();

      double boost = (0.5 + 0.5 * rnd.NextDouble()) * pl.udLast;
      var pthr1 = pl.pos.PlusRad(pl.posR - R15 * 10, Vol * 0.8 + boost * Vol * 0.7).ToP();
      var pthr2 = pl.pos.PlusRad(pl.posR - R15 * 11, Vol * 0.74 + boost * Vol * 0.2).ToP();
      var pthr3 = pl.pos.PlusRad(pl.posR - R15 * 12, Vol * 0.7 + boost * Vol).ToP();
      var pthr4 = pl.pos.PlusRad(pl.posR + R15 * 11, Vol * 0.74 + boost * Vol * 0.2).ToP();
      var pthr5 = pl.pos.PlusRad(pl.posR + R15 * 10, Vol * 0.8 + boost * Vol * 0.7).ToP();
      g.DrawLine(pr, pbl, pthr1);
      g.DrawLine(pr, pthr1, pthr2);
      g.DrawLine(pr, pthr2, pthr3);
      g.DrawLine(pr, pthr3, pthr4);
      g.DrawLine(pr, pthr4, pthr5);
      g.DrawLine(pr, pthr5, pbr);

      double drift = (0.5 + 0.5 * rnd.NextDouble()) * pl.lrLast;
      var pdr1 = pl.pos.PlusRad(pl.posR + R15 * 6, drift * Vol * 0.5).ToP();
      g.DrawLine(pr, pl.pos.PlusRad(pl.posR, Vol * 0.5), pdr1);
      g.DrawLine(pr, pl.pos.PlusRad(pl.posR + R15 * 12, Vol * 0.2), pdr1);

      g.DrawPolygon(p, new[] { ptl, ptr, pbr, pbl });

      pictureBox1.Refresh();
    }

    void Rechne()
    {
      var pl = game.player;

      if (pressedKeys.Contains(Keys.Return)) // Autopilot active?
      {
        double drift = 0.0;
        double acc = 0.0;
        double rotate = 0.0;

        var dirCurrent = new Vec2().PlusRad(pl.posR, 1);
        var radCurrent = dirCurrent.Rad();
        var radLast = new Vec2().PlusRad(pl.posR - pl.movR, 1).Rad();
        var dirTarget = autoEnd.Minus(autoStart).Norm();
        var radTarget = dirTarget.Rad();
        rotate = (radTarget - radCurrent - (radCurrent - radLast) * 1000) * 1000;

        double lenX = new Vec2().PlusRad(pl.posR, 1).Cross(pl.pos.Minus(autoStart));
        if (!pressedKeys.Contains(Keys.ShiftKey))
        {
          double spd = new Vec2().PlusRad(pl.posR, 1).Cross(pl.mov);
          double brk = (spd * spd) / (Player.AccS + Player.AccS - Player.AccS * 0.1);
          if (lenX < -0.001 && spd < -Player.AccS) drift = -1;
          else if (lenX > 0.001 && spd > Player.AccS) drift = +1;
          else
          {
            drift = lenX * 0.1;
            if (brk > Math.Abs(lenX)) drift = -drift;
          }
        }


        double lenY = new Vec2().PlusRad(pl.posR - Math.PI / 2, 1).Cross(pl.pos.Minus(autoStart))
                      + Math.Abs(pl.movR * 100000.0) // Sicherheitsabstand bei unkontrollierten Drehungen
                      + 75.0 // Extra Boden-Abstand
                      + Math.Abs(lenX * 0.3); // Extra, wenn auch Seitenabstand zu hoch

        if (!pressedKeys.Contains(Keys.ControlKey))
        {
          double spd = new Vec2().PlusRad(pl.posR - Math.PI / 2, 1).Cross(pl.mov);
          double brk = (spd * spd) / (Player.AccU + Player.AccU - Player.AccU * 0.05);
          Text = lenY.ToString("N5") + " - " + spd.ToString("N5") + " - " + brk.ToString("N5");
          if (lenY < -0.001 && spd < -Player.AccD) acc = -1;
          else if (lenY > 0.001 && spd > Player.AccD) acc = +1;
          else
          {
            acc = lenY * 0.1;
            if (brk > Math.Abs(lenY)) acc = -acc;
          }
        }

        game.player.Calc(drift, acc, rotate);
      }
      else
      {
        int rotate = (pressedKeys.Contains(Keys.A) ? -1 : 0) + // rotate left
                     (pressedKeys.Contains(Keys.D) ? +1 : 0);  // rotate right

        // auto-rotation
        if (rotate == 0)
        {
          if (game.player.movR < 0) rotate = 1;
          if (game.player.movR > 0) rotate = -1;
        }

        double drift = (pressedKeys.Contains(Keys.Q) ? -1 : 0) + // left
                       (pressedKeys.Contains(Keys.E) ? +1 : 0);  // right

        if (drift == 0)
        {
          var d = game.player.mov.Mag();
          if (d > 0.00001)
          {
            var v = new Vec2().PlusRad(game.player.mov.Rad() - game.player.posR, d);
            drift = v.x * -1000.0;
          }
        }
        game.player.Calc(drift,
                         (pressedKeys.Contains(Keys.W) ? +1 : 0) + // up
                         (pressedKeys.Contains(Keys.S) ? -1 : 0),  // down
                         rotate);
      }
    }

    bool closing;
    bool innerTimer;
    long nextTick = Stopwatch.GetTimestamp();
    readonly long waitTick = Stopwatch.Frequency / 60;
    int sleepWait = 1;

    private void timer1_Tick(object sender, EventArgs e)
    {
      try { timeBeginPeriod(1); }
      catch { }

      if (innerTimer || closing) return;
      innerTimer = true;
      Thread.CurrentThread.Priority = ThreadPriority.Highest;
      for (; ; )
      {
        long tick = Stopwatch.GetTimestamp();
        if (tick > nextTick)
        {
          int ti = pressedKeys.Contains(Keys.F) ? 160 : 16;
          while (tick > nextTick + waitTick)
          {
            for (int i = 0; i < ti; i++) Rechne();
            nextTick += waitTick;
          }
          for (int i = 0; i < ti; i++) Rechne();
          Zeichne();
          //Text = string.Join(", ", pressedKeys);
          Application.DoEvents();
          nextTick += waitTick;
        }
        Thread.Sleep(sleepWait);
        if (closing) return;
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      closing = true;
      try { timeEndPeriod(1); }
      catch { }
    }

    readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
      pressedKeys.Add(e.KeyCode);
      if (e.KeyCode == Keys.Escape) Close();
    }

    private void Form1_KeyUp(object sender, KeyEventArgs e)
    {
      pressedKeys.Remove(e.KeyCode);

      if (e.KeyCode == Keys.F12)
      {
        sleepWait = sleepWait == 0 ? 1 : 0;
      }
    }
  }
}
