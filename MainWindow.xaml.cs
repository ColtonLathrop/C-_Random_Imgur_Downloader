using Microsoft.Win32;
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
using Random_Imgur_Downloader.Resources;

namespace Random_Imgur_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void changeText_test(object sender, RoutedEventArgs e)
        {
            int urlType = 5;
            if ((Boolean)lengthFive.IsChecked)
                urlType = 5;
            if ((Boolean)lengthSix.IsChecked)
                urlType = 6;
            if ((Boolean)lengthSeven.IsChecked)
                urlType = 7;
            RID test = new RID();
            string test1 = RID.GenerateString(urlType);
            this.textBox1.Text = test1;
        }

        private void StartRun(object sender, RoutedEventArgs e)
        {
            int urlType = 5;
            if ((Boolean)lengthFive.IsChecked)
                urlType = 5;
            if ((Boolean)lengthSix.IsChecked)
                urlType = 6;
            if ((Boolean)lengthSeven.IsChecked)
                urlType = 7;
            int parsedResult;
            int.TryParse(this.runLength.Text, out parsedResult);
            if (parsedResult == 0)
            {
                MessageBox.Show("Length is not a number or is zero!", "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (selectedDirectory.Text == "")
            {
                MessageBox.Show("Please select a directory!", "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            RID.KickOff(selectedDirectory.Text, urlType, parsedResult);
        }

        public void BrowseFolder(object sender, RoutedEventArgs e)
        {
            var folderbrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            if (folderbrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedDirectory.Text = folderbrowserDialog1.SelectedPath;
                }
        }
    }
}
