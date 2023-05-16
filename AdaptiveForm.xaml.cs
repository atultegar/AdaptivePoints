using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace AdaptivePoints
{
    /// <summary>
    /// Interaction logic for AdaptiveForm.xaml
    /// </summary>
    public partial class AdaptiveForm : Window
    {
        [IsVisibleInDynamoLibrary(false)]
        public List<Point> RevitPoints { get; set; }
        [IsVisibleInDynamoLibrary(false)]
        public List<Point> SharedPoints { get; set; }


        [IsVisibleInDynamoLibrary(false)]
        public AdaptiveForm()
        {
            DataContext = this;
            InitializeComponent();
            
            
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        
        private void lstRP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRvtCancel.IsEnabled = false;
            btnRvtApply.IsEnabled = false;
            btnSharedCancel.IsEnabled = false;
            btnSharedApply.IsEnabled = false;
            rvtX.IsEnabled = false;
            rvtY.IsEnabled = false;
            rvtZ.IsEnabled = false;
            sharedX.IsEnabled = false;
            sharedY.IsEnabled = false;
            sharedZ.IsEnabled = false;

            rvtX.Text = Math.Round(RevitPoints[lstRP.SelectedIndex].X,3).ToString();
            rvtY.Text = Math.Round(RevitPoints[lstRP.SelectedIndex].Y,3).ToString();
            rvtZ.Text = Math.Round(RevitPoints[lstRP.SelectedIndex].Z,3).ToString();

            sharedX.Text = Math.Round(SharedPoints[lstRP.SelectedIndex].X,3).ToString();
            sharedY.Text = Math.Round(SharedPoints[lstRP.SelectedIndex].Y,3).ToString();
            sharedZ.Text = Math.Round(SharedPoints[lstRP.SelectedIndex].Z,3).ToString();

        }

        private void btnRvtEdit_Click(object sender, RoutedEventArgs e)
        {
            rvtX.IsEnabled = true;
            rvtY.IsEnabled = true;
            rvtZ.IsEnabled = true;
            btnRvtCancel.IsEnabled = true;
            btnRvtApply.IsEnabled = true;

        }

        private void btnSharedEdit_Click(object sender, RoutedEventArgs e)
        {
            sharedX.IsEnabled = true;
            sharedY.IsEnabled = true;
            sharedZ.IsEnabled = true;
            btnSharedCancel.IsEnabled = true;
            btnSharedApply.IsEnabled = true;
        }

        private void btnRvtApply_Click(object sender, RoutedEventArgs e)
        {
            double pX = Convert.ToDouble(rvtX.Text);
            double pY = Convert.ToDouble(rvtY.Text);
            double pZ = Convert.ToDouble(rvtZ.Text);
            Point p = Point.ByCoordinates(pX, pY, pZ);
            Point sp = p.Transform(Utils.DocumentTotalTransform().Inverse()) as Point;
            
            sharedX.Text = Math.Round(sp.X, 3).ToString();
            sharedY.Text = Math.Round(sp.Y, 3).ToString();
            sharedZ.Text = Math.Round(sp.Z, 3).ToString();

            RevitPoints[lstRP.SelectedIndex] = p;
            SharedPoints[lstRP.SelectedIndex] = sp;

        }

        private void btnSharedApply_Click(object sender, RoutedEventArgs e)
        {
            double spX = Convert.ToDouble(sharedX.Text);
            double spY = Convert.ToDouble(sharedY.Text);
            double spZ = Convert.ToDouble(sharedZ.Text);
            Point sp = Point.ByCoordinates(spX, spY, spZ);
            Point p = sp.Transform(Utils.DocumentTotalTransform()) as Point;

            rvtX.Text = Math.Round(p.X, 3).ToString();
            rvtY.Text = Math.Round(p.Y, 3).ToString();
            rvtZ.Text = Math.Round(p.Z, 3).ToString();

            RevitPoints[lstRP.SelectedIndex] = p;
            SharedPoints[lstRP.SelectedIndex] = sp;

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        //private void CenterWindowOnScreen()
        //{
        //    double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        //    double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        //    double windowWidth = this.Width;
        //    double windowHeight = this.Height;
        //    this.Left = (screenWidth / 2 - windowWidth / 2);
        //    this.Top = (screenHeight / 2 - windowHeight / 2);
        //}

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }

    
}