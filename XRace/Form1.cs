#region # using *.*
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

    Bitmap bild;

    void Zeichne()
    {
      if (bild == null || bild.Width != pictureBox1.Width || bild.Height != pictureBox1.Height)
      {
        bild = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppRgb);
        pictureBox1.Image = bild;
      }

      var g = Graphics.FromImage(bild);
      g.Clear(Color.Black);
      g.SmoothingMode = SmoothingMode.HighQuality;

      float scale = Math.Min(bild.Width, bild.Height) / 2f / 1000f;
      g.TranslateTransform(bild.Width / 2f, bild.Height / 2f);
      g.ScaleTransform(scale, -scale);
      var p = new Pen(Color.LightBlue, 2f / scale);

      g.DrawLine(p, (int)(Math.Sin(Math.PI / 1800.0 * grad) * 900),
                    (int)(Math.Cos(Math.PI / 1800.0 * grad) * 900),
                    (int)(Math.Sin(Math.PI / 1800.0 * (grad + 1800)) * 900),
                    (int)(Math.Cos(Math.PI / 1800.0 * (grad + 1800)) * 900));
      g.DrawEllipse(p, -800, -800, 1600, 1600);

      pictureBox1.Refresh();
    }

    int fpsCount = 0;
    int grad = 0;
    void Rechne()
    {
      grad++;
      if (grad >= 3600) grad -= 3600;
    }

    bool innerTimer = false;
    long nextTick = Stopwatch.GetTimestamp();
    long waitTick = Stopwatch.Frequency / 60;

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (innerTimer) return;
      innerTimer = true;

      for (; ; )
      {
        if (Stopwatch.GetTimestamp() > nextTick)
        {
          for (int i = 0; i < 16; i++) Rechne();
          Zeichne();
          Application.DoEvents();
          nextTick += waitTick;
          while (Stopwatch.GetTimestamp() > nextTick)
          {
          for (int i = 0; i < 16; i++) Rechne();
          nextTick += waitTick;
          }
        }
        Thread.Sleep(0);
      }

      innerTimer = false;
    }
  }
}
