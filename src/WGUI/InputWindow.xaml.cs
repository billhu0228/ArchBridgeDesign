using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WGUI
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
            paras = new List<double>();
        }

        public DialogResult InputDialogRes { get; set; }

        public List<double> paras;

        //public DialogResult ShowInput()
        //{
        //    Show();

        //}

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            InputDialogRes = System.Windows.Forms.DialogResult.OK;
        }
    }
}
