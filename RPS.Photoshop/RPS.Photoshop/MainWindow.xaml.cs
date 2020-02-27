using System;
using System.Collections.Generic;
using System.Linq;
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
using MahApps.Metro.Controls;
using RPS.Images;

namespace RPS.Photoshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            lblMessage.Text = "Successfully run with no errors";
            //MessageBox.Show("Hello");
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            
            lblMessage.Text = "Button is clicked";
            Test();
        }

        private void Test()
        {
            //TODO
            try
            {
                ImageHelper.TestMasking();
                lblMessage.Text = "Completed";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                lblMessage.Text = ex.ToString();
            }
            
        }

        private void btnStart_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello World");
        }
    }
}
