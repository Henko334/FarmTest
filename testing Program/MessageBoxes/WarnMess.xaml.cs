using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace testing_Program.MessageBoxes
{
    /// <summary>
    /// Interaction logic for WarnMess.xaml
    /// </summary>
    public partial class WarnMess : ChromelessWindow
    {
        public WarnMess(string message)
        {
            InitializeComponent();
            lblMessage.Content = message;
            SystemSounds.Asterisk.Play();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
