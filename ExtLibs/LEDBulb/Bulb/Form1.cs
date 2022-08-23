using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bulb {
	public partial class Form1 : Form {

		private int _blink = 0;
		
		public Form1() {
			InitializeComponent();
		}

		// Turn the bulb On or Off
		private void ledBulb_Click(object sender, EventArgs e) {
			((LedBulb)sender).On = !((LedBulb)sender).On;
		}

		private void ledBulb7_Click(object sender, EventArgs e) {
			if (_blink == 0) _blink = 500;
			else _blink = 0;
			((LedBulb)sender).Blink(_blink);
		}
	}
}
