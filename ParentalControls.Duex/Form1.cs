using ParentalControls.Duex.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParentalControls.Duex
{
    public partial class Form1 : Form
    {
        NativeNotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();

            notifyIcon = new NativeNotifyIcon(ParentalControls.Duex.Properties.Resources.Image1.GetHicon());
            NativeContextMenu menu = new NativeContextMenu();
            menu.List.Add(new NativeContextMenuItem(0, "Test"));
            notifyIcon.ContextMenuStrip = menu;
            Controls.Add(notifyIcon);
            Controls.Add(menu);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
