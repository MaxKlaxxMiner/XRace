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

      int w = bild.Width;
      int h = bild.Height;

      g.DrawLine(new Pen(Color.LightBlue), 0, 0, w, h);

      pictureBox1.Refresh();

      Text = tickCount.ToString("N0");
    }

    int tickCount = 0;
    void Rechne()
    {
      tickCount++;
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
        tickTime += 10;
      }

      Zeichne();

      innerTimer = false;
    }
  }
}
