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

namespace Winch_File_Combine_and_Parse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Settings_Store _settingsStore;
        public MainWindow()
        {
            InitializeComponent();
            _settingsStore = new Settings_Store();
            this.DataContext = _settingsStore;
           // _settingsStore.SelectedWinchNames.Add(new WinchModel(1, "MASH Winch", "csv"));
            
        //    ObservableCollection<WinchModel> Winches = new ObservableCollection<WinchModel>
        //{
        //    new WinchModel(1,"MASH Winch","csv"),
        //    new WinchModel(2,"SIO Traction Winch","MTN"),
        //    new WinchModel(3,"Armstrong Cast 6", "csv")
        //};
    }

        private void Button_Click_Select_Folder(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            _settingsStore.Directory = dialog.SelectedPath;
            DirectoryInfo di = new DirectoryInfo(_settingsStore.Directory);
            _settingsStore.FileList = new List<string>();

            if (_settingsStore.SelectedWinch == "SIO Traction Winch")
            {
                foreach (var fi in di.GetFiles("*.Raw"))
                {
                    _settingsStore.FileList.Add(fi.Name);
                }
                _settingsStore.FileList.Sort();                                //Sorts the List by element name
                
            }

            if (_settingsStore.SelectedWinch == "MASH Winch")
            {
                foreach (var fi in di.GetFiles("*.CSV"))
                {
                    _settingsStore.FileList.Add(fi.Name);
                }
                _settingsStore.FileList.Sort();                                //Sorts the List by element name
                
            }

            if (_settingsStore.SelectedWinch == "Armstrong CAST 6")
            {
                foreach (var fi in di.GetFiles("*.MTN_WINCH"))
                {
                    _settingsStore.FileList.Add(fi.Name);
                }
                _settingsStore.FileList.Sort();                                //Sorts the List by element name
                
            }

            if (_settingsStore.SelectedWinch == "UNOLS String")
            {
                foreach (var fi in di.GetFiles("*.log"))
                {
                    _settingsStore.FileList.Add(fi.Name);
                }
                _settingsStore.FileList.Sort();
            }

        }

        private void Button_Click_Combine_Files(object sender, RoutedEventArgs e)
        {
            ReadFilesViewModel.CombineFiles();
        }

        private void Button_Click_Parse_Files(object sender, RoutedEventArgs e)
        {
            ReadFilesViewModel.ParseFiles();
        }

        private void Button_Click_Set_Filenames(object sender, RoutedEventArgs e)
        {
            _settingsStore.CombinedFileName = $"{ _settingsStore.CruiseName }_Combined.MTN";
            _settingsStore.ProcessedFileName = $"{ _settingsStore.CruiseName }_Processed.txt";
        }
    }
}
