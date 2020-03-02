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
//for select directory dialog
using System.Windows.Forms;
using System.IO;

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
                System.Windows.MessageBox.Show(ex.Message);
                lblMessage.Text = ex.ToString();
            }
            
        }

        private void btnStart_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Hello World");
        }

        private void btnFileDialog2_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtTarget.Text = fbd.SelectedPath;
                    CheckIfSourceAndTargetIsSame();
                }
            }
        }

        private void btnFileDialog1_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                    txtSource.Text = fbd.SelectedPath;
                    CheckIfSourceAndTargetIsSame();
                }
            }
        }

        private void CheckIfSourceAndTargetIsSame()
        {
            if(txtSource.Text == txtTarget.Text)
            {
                lblMessage.Text = "Source and Target cannot be the same";
                lblMessage.Foreground = Brushes.Red;
            }
            else
            {
                lblMessage.Text = string.Empty;
                lblMessage.Foreground = Brushes.Black;
            }

        }
    }
}
