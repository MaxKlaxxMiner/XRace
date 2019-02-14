#region # using *.*
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
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

      fpsCount++;
      if (tickTime > fpsTick)
      {
        Text = tickCount.ToString("N0") + ", fps: " + fpsCount;
        fpsTick += 1000;
        fpsCount = 0;
      }
    }

    int tickCount = 0;
    int fpsTick = Environment.TickCount;
    int fpsCount = 0;
    int grad = 0;
    void Rechne()
    {
      tickCount++;
      grad++;
      if (grad >= 3600) grad -= 3600;
    }

    bool innerTimer = false;
    int tickTime = Environment.TickCount;

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (innerTimer) return;
      innerTimer = true;

      int tim = Environment.TickCount;
      while (tim > tickTime)
      {
        if (tickTime + 1000 < tim) tickTime = tim;
        Rechne();
        tickTime++;
      }

      Zeichne();

      innerTimer = false;
    }
  }
}
