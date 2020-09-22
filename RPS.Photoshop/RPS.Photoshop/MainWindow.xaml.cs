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
using RPS.Library.API.Utility;
using RPS.Models;

namespace RPS.Photoshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LogHelper lg = new LogHelper();
        public MainWindow()
        {
            //test text log
            //LogHelper.ErrorLog("Application Started");
            InitializeComponent();
            lblMessage.Text = "Program has started.";
            //lblMessage.Text = "Successfully run with no errors";
            //MessageBox.Show("Hello");
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblMessage.Text = "Button is clicked";
                //Test();
                RunProgram();
                //TestFace();
                lblMessage.Text = "All Tasks completed.";
                lblMessage.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.ToString();
                lblMessage.Foreground = Brushes.Red;
            }            
        }

        private void TestFace()
        {
            string testInput = txtSource.Text + "\\" + "trafficjam3.jpg";
            FaceDetection.Start(testInput);
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

        private void RunProgram()
        {
            //System.Windows.MessageBox.Show("Hello World");
            if (!IsSourceTargetDifferent())
            {
                lblMessage.Text = "ERROR: Source and Target cannot be the same";
                lblMessage.Foreground = Brushes.Red;
            }
            else
            {
                lblMessage.Text = string.Empty;
                lblMessage.Foreground = Brushes.Green;

                string[] files = Directory.GetFiles(txtSource.Text);
                if (files.Length > 0)
                {
                    lblMessage.Text = string.Format("total file(s) found: {0}", files.Length.ToString());
                    try
                    {
                        //TODO: Loop all files
                        int counter = 0;
                        foreach (string filePath in files)
                        {
                            counter++;
                            FileInfo info = new FileInfo(filePath);
                            List<RectangleModel> rectangles = new List<RectangleModel>();
                            //check for Face                            
                            List<RectangleModel> rect_F = 
                               FaceDetection.Start(info.FullName);
                            //check for Vehicle Plate
                            List<RectangleModel> rect_V =
                                VehicleDetection.Start(info.FullName);
                            if(rect_V.Count > 0)
                            {
                                rectangles.AddRange(rect_V);
                            }
                            if(rect_F.Count > 0)
                            {
                                rectangles.AddRange(rect_F);
                            }

                            string outputfolder = txtTarget.Text + "\\";
                            ImageProcessor.Start(info.FullName, rectangles, outputfolder + info.Name);
                            string displayMessage = string.Format("completed task {0} out of {1}", counter.ToString(), files.Length.ToString());
                            lblMessage.Text = displayMessage;
                            //trace
                            LogHelper.TraceLog(displayMessage);
                        }
                    }
                    catch (Exception ex)
                    {                        
                        LogHelper.ErrorLog(ex.ToString());
                        lblMessage.Text = string.Format("the program has been interrupted due to error. please check the error logs.");
                        //throw ex;
                    }
                }
            }
        }

        private void btnStart_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnFileDialog2_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtTarget.Text = fbd.SelectedPath;
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
                    txtSource.Text = fbd.SelectedPath;
                }
            }
        }

        private Boolean IsSourceTargetDifferent()
        {
            if (txtSource.Text == txtTarget.Text)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
