﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using RPS.Images;
//for select directory dialog
using System.Windows.Forms;
using System.IO;
using RPS.Library.API.Utility;
using RPS.Models;
using System.Threading;
using System.Windows.Media.TextFormatting;
using System.ComponentModel;
using NLog.Targets;
using System.Drawing;
using System.Drawing.Imaging;

namespace RPS.Photoshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LogHelper lg = new LogHelper();
        //TODO bg Worker
        BackgroundWorker worker = new BackgroundWorker();

        // masukkan textbox value dlm property sebab nanti thread tak boleh baca
        public string prop_textSource { get; set; }
        public string prop_targetSource { get; set; }

        public int total_input { get; set; }
        public int total_output { get; set; }

        // for testing
        public int loop_end { get; set; }

        public MainWindow()
        {
            //test text log
            //LogHelper.ErrorLog("Application Started");
            InitializeComponent();
            //activate bgworker
            AdditionalInitialization();
        }

        public void AdditionalInitialization()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += worker_ProgressChanged;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //TODO: update progress
            pbProgress.Value = e.ProgressPercentage;
            string msgUpdate = "Progress Changed: " + e.ProgressPercentage.ToString() + "/100";
            LogHelper.TraceLog(msgUpdate);
            lblMessage.Text = msgUpdate;
        }

        // masa do work ni tak boleh panggil object dari WPF i.e. Thread lain
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            string[] files = Directory.GetFiles(prop_textSource);
            total_input = files.Length;

            //initiate report progress at 0%
            (sender as BackgroundWorker).ReportProgress(0);

            for (int i = 0; i < total_input; i++)
            {
                try
                {
                    string filePath = files[i];
                    string outputfolder = prop_targetSource + "\\";
                    FileInfo info = new FileInfo(filePath);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);
                    List<Bitmap> splitted = ImageUtils.CutImage(img);
                    List<Bitmap> images = new List<Bitmap>();
                    img.Dispose();
                    for(int y = 0; y < splitted.Count; y++) 
                    {
                        List<RectangleModel> rectangles = new List<RectangleModel>();

                        //check for Vehicle Plate
                        List<RectangleModel> rect_V = VehicleDetection.Start(splitted[y]);

                        //check for Face                            
                        List<RectangleModel> rect_F = FaceDetection.Start(splitted[y]);

                        if (rect_V.Count > 0)
                        {
                            rectangles.AddRange(rect_V);
                        }
                        if (rect_F.Count > 0)
                        {
                            rectangles.AddRange(rect_F);
                        }
                        // process images to API
                        var item = ImageProcessor.Start(splitted[y], rectangles);
                        // add process image with blurred face/plate into list
                        images.Add(item);
                    }
                    // stich images back into one piece
                    ImageUtils.CombineImgAndSave(images, outputfolder, info.Name);
                    int progressPercentage = Convert.ToInt32(((double)i / total_input) * 100);
                    (sender as BackgroundWorker).ReportProgress(progressPercentage);
                }
                catch(Exception ex)
                {
                    LogHelper.ErrorLog(ex.ToString());
                }                
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker completes work
            string jobCompleteMsg = "Job completed. 100/100.";
            LogHelper.TraceLog(jobCompleteMsg);
            lblMessage.Text = jobCompleteMsg;
            pbProgress.Visibility = Visibility.Hidden;
            btnStart.IsEnabled = true;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //lblMessage.Text = "Button is clicked";
                

                if (!IsSourceTargetDifferent())
                {
                    lblMessage.Text = "ERROR: Source and Target cannot be the same";
                    lblMessage.Foreground = System.Windows.Media.Brushes.Blue;
                }
                else
                {
                    //assign value to prop_textSource/targetSource
                    prop_textSource = txtSource.Text;
                    prop_targetSource = txtTarget.Text;
                    //UI changes
                    lblMessage.Text = string.Empty;
                    lblMessage.Foreground = System.Windows.Media.Brushes.Blue;
                    btnStart.IsEnabled = false;
                    pbProgress.Visibility = Visibility.Visible;
                    pbProgress.Value = 0;
                    pbProgress.Maximum = 100;
                    //start work
                    worker.RunWorkerAsync();                    
                }                

                lblMessage.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.ToString());
                lblMessage.Text = string.Format("the program has been interrupted. Please check the error logs.");
                lblMessage.Foreground = System.Windows.Media.Brushes.Red;
            }            
        }

        #region Khairil Testing Stuff
        //private void TestFace()
        //{
        //    string testInput = txtSource.Text + "\\" + "trafficjam3.jpg";
        //    FaceDetection.Start(testInput);
        //}

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
        

        private void Temp_AlihKejap()
        {
            

            //Test();
            //RunProgram();
            //TestFace();
            

            string[] files_input = Directory.GetFiles(prop_textSource);
            int total_input = files_input.Length;
            string[] files_output = Directory.GetFiles(prop_targetSource);
            total_output = files_output.Length;
            while (total_input > total_output)
            {
                lblMessage.Text = string.Format("Updating Task {0} out of {1}", total_input.ToString(), total_output.ToString());
                files_output = Directory.GetFiles(prop_targetSource);
                total_output = files_output.Length;
                Console.WriteLine("total output is " + total_output);
            }
            lblMessage.Text = "Task Completed";
        }

        //private void RunProgram()
        //{
        //    Console.WriteLine("Thread has started.");


        //    string[] files = Directory.GetFiles(prop_textSource);

        //    if (files.Length > 0)
        //    {
        //        Console.WriteLine(string.Format("total file(s) found: {0}", files.Length.ToString()));
        //        try
        //        {
        //            //TODO: Loop all files
        //            int counter = 0;
        //            foreach (string filePath in files)
        //            {
        //                counter++;
        //                FileInfo info = new FileInfo(filePath);
        //                List<RectangleModel> rectangles = new List<RectangleModel>();
        //                //check for Face                            
        //                List<RectangleModel> rect_F = FaceDetection.Start(info.FullName);
        //                List<RectangleModel> rect_F = new List<RectangleModel>();
        //                //check for Vehicle Plate
        //                List<RectangleModel> rect_V = VehicleDetection.Start(filePath);
        //                if (rect_V.Count > 0)
        //                {
        //                    rectangles.AddRange(rect_V);
        //                }
        //                if (rect_F.Count > 0)
        //                {
        //                    rectangles.AddRange(rect_F);
        //                }

        //                string outputfolder =  prop_targetSource + "\\";
        //                ImageProcessor.Start(info.FullName, rectangles, outputfolder + info.Name);
        //                string displayMessage = string.Format("completed task {0} out of {1}", counter.ToString(), files.Length.ToString());
        //                //trace
        //                LogHelper.TraceLog(displayMessage);
        //            }
        //        }
        //        catch (Exception ex)
        //        {   
        //            throw ex;
        //        }
        //    }
        //}

        #endregion

        //set the target folder
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

        // set the source folder
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

        // check if source folder and target folder is different
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
