#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

#endregion

namespace scrshtr
{
  class Program
  {
    static readonly int WaitMs = 5000; // warte 5 Sekunden

    static void Main()
    {
      Console.WriteLine();
      Console.WriteLine("  * * * * * * * * * * * *");
      Console.WriteLine("  *                     *");
      Console.WriteLine("  *  Hello Screenshot!  *");
      Console.WriteLine("  *                     *");
      Console.WriteLine("  * * * * * * * * * * * *");
      Console.WriteLine();

      var screens = Screen.AllScreens;
      if (screens == null || screens.Length == 0) throw new Exception("no screens found!");

      var capScreen = screens.Last();
      var capArea = capScreen.WorkingArea;

      string path = "../../cap/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
      Directory.CreateDirectory(path);

      //RunScreens(capArea, path);
      RunDifScreens(capArea, path);
    }

    static void RunScreens(Rectangle capArea, string path)
    {
      var bitmap = new Bitmap(capArea.Width, capArea.Height, PixelFormat.Format32bppRgb);
      var graphics = Graphics.FromImage(bitmap);

      for (;;)
      {
        graphics.CopyFromScreen(capArea.Location, Point.Empty, capArea.Size);
        string filename = "cap_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".png";
        bitmap.Save(path + filename, ImageFormat.Png);
        Console.WriteLine("{0} - {1} x {2}, {3:N2} kByte", filename, capArea.Width, capArea.Height, new FileInfo(path + filename).Length / 1024.0);
        Thread.Sleep(WaitMs);
      }
    }

    static void DifBitmap(Bitmap lastBitmap, Bitmap nextBitmap)
    {
      if (lastBitmap == null) throw new NullReferenceException("lastBitmap");
      if (nextBitmap == null) throw new NullReferenceException("nextBitmap");
      if (lastBitmap.Size != nextBitmap.Size || lastBitmap.PixelFormat != nextBitmap.PixelFormat) throw new FormatException();

      var lastBits = lastBitmap.LockBits(new Rectangle(Point.Empty, lastBitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
      var nextBits = nextBitmap.LockBits(new Rectangle(Point.Empty, nextBitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

      var lastBuf = new uint[lastBitmap.Width * lastBitmap.Height];
      var nextBuf = new uint[nextBitmap.Width * nextBitmap.Height];
      if (lastBuf.Length != nextBuf.Length) throw new IndexOutOfRangeException();

      Marshal.Copy(lastBits.Scan0, (int[])(object)lastBuf, 0, lastBuf.Length);
      Marshal.Copy(nextBits.Scan0, (int[])(object)nextBuf, 0, nextBuf.Length);

      for (int i = 0; i < lastBuf.Length; i++)
      {
        uint last = lastBuf[i];
        uint next = nextBuf[i];
        nextBuf[i] = 0xff000000 | (next - last);
        lastBuf[i] = next;
      }

      int difPixels = 0;
      for (int i = 1; i < nextBuf.Length; i++)
      {
        if (nextBuf[i - 1] != nextBuf[i]) difPixels++;
      }
      Console.WriteLine("pixels: " + difPixels);

      Marshal.Copy((int[])(object)lastBuf, 0, lastBits.Scan0, lastBuf.Length);
      Marshal.Copy((int[])(object)nextBuf, 0, nextBits.Scan0, nextBuf.Length);

      lastBitmap.UnlockBits(lastBits);
      nextBitmap.UnlockBits(nextBits);
    }

    static void RunDifScreens(Rectangle capArea, string path)
    {
      var bitmap = new Bitmap(capArea.Width, capArea.Height, PixelFormat.Format32bppRgb);
      var graphics = Graphics.FromImage(bitmap);

      graphics.CopyFromScreen(capArea.Location, Point.Empty, capArea.Size);
      string filenameFirst = "cap_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + "_start.png";
      bitmap.Save(path + filenameFirst, ImageFormat.Png);
      Console.WriteLine("{0} - {1} x {2}, {3:N2} kByte", filenameFirst, capArea.Width, capArea.Height, new FileInfo(path + filenameFirst).Length / 1024.0);
      var lastBitmap = bitmap.Clone() as Bitmap;
      Thread.Sleep(WaitMs); // warte 5 Sekunden

      for (; ; )
      {
        graphics.CopyFromScreen(capArea.Location, Point.Empty, capArea.Size);
        string filenameDif = "cap_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + "_dif.png.gz";
        DifBitmap(lastBitmap, bitmap);
        using (var fileStream =  File.Create(path + filenameDif))
        using (var gzip = new GZipStream(fileStream, CompressionLevel.Optimal))
        {
          bitmap.Save(gzip, ImageFormat.Png);
        }
        Console.WriteLine("{0} - {1} x {2}, {3:N2} kByte", filenameDif, capArea.Width, capArea.Height, new FileInfo(path + filenameDif).Length / 1024.0);
        Thread.Sleep(WaitMs); // warte 5 Sekunden
      }
    }
  }
}
