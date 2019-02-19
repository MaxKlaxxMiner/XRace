﻿#region # using *.*
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

    #region # public static extern void timeBeginPeriod(int tick); // ermöglicht eine Verringerung der Wartephasen zwischen den Windows-Ticks (eine Erhöhung ist nicht möglich und wird ignoriert)
    /// <summary>
    /// ermöglicht eine Verringerung der Wartephasen zwischen den Windows-Ticks (eine Erhöhung ist nicht möglich und wird ignoriert)
    /// </summary>
    /// <param name="tick">Wartezeit in Millisekunden</param>
    [DllImport("winmm.dll", SetLastError = true)]
    [DebuggerHidden]
    public static extern void timeBeginPeriod(int tick);
    #endregion
    #region # public static extern void timeEndPeriod(int tick); // hebt die Wartezeit von timeBeginPeriod() wieder auf
    /// <summary>
    /// hebt die Wartezeit von timeBeginPeriod() wieder auf
    /// </summary>
    /// <param name="tick">Wartezeit in Millisekunden</param>
    [DllImport("winmm.dll", SetLastError = true)]
    [DebuggerHidden]
    public static extern void timeEndPeriod(int tick);
    #endregion

    Bitmap bild;

    readonly Game game = new Game();

    void Zeichne()
    {
      if (bild == null || bild.Width != pictureBox1.Width || bild.Height != pictureBox1.Height)
      {
        bild = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppRgb);
        pictureBox1.Image = bild;
      }

      float scale = Math.Min(bild.Width, bild.Height) / 2f / 1000f;
      var g = Graphics.FromImage(bild);
      g.Clear(Color.Black);
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.TranslateTransform(bild.Width / 2f, bild.Height / 2f);
      g.ScaleTransform(scale, -scale);

      var p = new Pen(Color.LightBlue, 2f / scale);
      var pg = new Pen(Color.FromArgb(0x222222 - 16777216), 2f / scale);
      var ph = new Pen(Color.FromArgb(0x444444 - 16777216), 2f / scale);

      const double Vol = 100.0;
      const double R15 = Math.PI / 12.0;
      var pl = game.player;

      // --- Debug-Lines ---
      g.DrawLine(pg, pl.pos.PlusRad(pl.posR, 3000), pl.pos.MinusRad(pl.posR, 3000));
      g.DrawLine(pg, pl.pos.PlusRad(pl.posR + R15 * 6, 3000), pl.pos.MinusRad(pl.posR + R15 * 6, 3000));

      // --- Debug-Arrows ---
      g.DrawLine(ph, pl.pos.Plus(pl.mov.Mul(1000)), pl.pos);

      // --- Player Ship ---
      var ptl = pl.pos.PlusRad(pl.posR - R15, Vol).ToP();
      var ptr = pl.pos.PlusRad(pl.posR + R15, Vol).ToP();
      var pbl = pl.pos.PlusRad(pl.posR - R15 * 9, Vol).ToP();
      var pbr = pl.pos.PlusRad(pl.posR + R15 * 9, Vol).ToP();
      g.DrawPolygon(p, new[] { ptl, ptr, pbr, pbl });

      pictureBox1.Refresh();
    }

    void Rechne()
    {
      game.player.Calc(
        pressedKeys.Contains(Keys.A), // left
        pressedKeys.Contains(Keys.D), // right
        pressedKeys.Contains(Keys.W), // up
        pressedKeys.Contains(Keys.S), // down
        pressedKeys.Contains(Keys.Q), // rotate left
        pressedKeys.Contains(Keys.E)  // rotate right
      );
    }

    bool closing;
    bool innerTimer;
    long nextTick = Stopwatch.GetTimestamp();
    readonly long waitTick = Stopwatch.Frequency / 60;
    int sleepWait = 1;

    private void timer1_Tick(object sender, EventArgs e)
    {
      timeBeginPeriod(1);

      if (innerTimer || closing) return;
      innerTimer = true;
      Thread.CurrentThread.Priority = ThreadPriority.Highest;
      for (; ; )
      {
        long tick = Stopwatch.GetTimestamp();
        if (tick > nextTick)
        {
          while (tick > nextTick + waitTick)
          {
            for (int i = 0; i < 16; i++) Rechne();
            nextTick += waitTick;
          }
          for (int i = 0; i < 16; i++) Rechne();
          Zeichne();
          Text = string.Join(", ", pressedKeys);
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
      timeEndPeriod(1);
    }

    HashSet<Keys> pressedKeys = new HashSet<Keys>();

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
