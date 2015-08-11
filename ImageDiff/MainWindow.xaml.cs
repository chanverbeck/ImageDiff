using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;

namespace ImageDiff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<string> imageSources;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            imageSources = new List<string>();
            imageSources.Add(@"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_01.jpg");
            imageSources.Add(@"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_02.jpg");
            imageSources.Add(@"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_03.jpg");
            imageSources.Add(@"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_04.jpg");
            imageSources.Add(@"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_05.jpg");
            imageSources.Add(@"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_06.jpg");

            for (int i = 0; i < 5; ++i)
            {
                Comparison c = new Comparison(imageSources[i], imageSources[i+1]);
               comparisons.Add(c);
            }
            // Comparison = c;
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Files to compare";
            openFileDialog.Multiselect = true;
            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                imageSources = new List<string>();
                for (int i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    imageSources.Add(openFileDialog.FileNames[i]);
                }
            }
            else
            {
                return;
            }

            if (openFileDialog.SafeFileNames.Length == 1)
            {
                openFileDialog.Title = "Right File";
                openFileDialog.Multiselect = false;
                result = openFileDialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    imageSources.Add(openFileDialog.FileName);
                }
                else
                {
                    return;
                }
            }

            comparisons.Clear();
            for (int i = 0; i < imageSources.Count - 1; ++i)
            {
                Comparison c = new Comparison(imageSources[i], imageSources[i+1]);
                comparisons.Add(c);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown(0);
        }

        private ObservableCollection<Comparison> comparisons = new ObservableCollection<Comparison>();
        public ObservableCollection<Comparison> Comparisons {
            get
            {
                return comparisons;
            }
        }
    }
}
