using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XRace
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    bool innerTimer = false;

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (innerTimer) return;
      innerTimer = true;



      innerTimer = false;
    }
  }
}
