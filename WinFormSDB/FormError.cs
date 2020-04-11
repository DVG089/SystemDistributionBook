using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormSDB
{
    public partial class FormError : Form
    {
        public FormError(string error)
        {
            InitializeComponent();

            lblError.Text = error;
            btnOk.Click += (sender, e) => this.Close();
        }
    }
}
