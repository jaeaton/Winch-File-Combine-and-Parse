
namespace Winch_File_Combine_and_Parse
{
    public class Settings_Store : ViewModelBase
    {
        private string _directory;
        public string Directory 
        { 
            get => _directory; 
            set {_directory = value; OnPropertyChanged(nameof(Directory)); }
            
        }
        private string _cruiseName;
        public string CruiseName 
        {
            get => _cruiseName;
            set { _cruiseName = value; OnPropertyChanged(nameof(CruiseName)); }
        }
        private string _combinedFileName;
        public string CombinedFileName
        {
            get => _combinedFileName;
            set { _combinedFileName = value; OnPropertyChanged(nameof(CombinedFileName)); }
        }
        private string _processedFileName;
        public string ProcessedFileName
        {
            get => _processedFileName;
            set { _processedFileName = value; OnPropertyChanged(nameof(ProcessedFileName)); }
        }
        private string _winchSelection;
        public string WinchSelection
        {
            get => _winchSelection;
            set { _winchSelection = value; OnPropertyChanged(nameof(WinchSelection)); }
        }
       
        private float _minTension;
        public float MinTension
        {
            get => _minTension;
            set { _minTension = (float)value; OnPropertyChanged(nameof(MinTension)); }
        }
        private float _minPayout;
        public float MinPayout
        {
            get => _minPayout;
            set { _minPayout = (float)value; OnPropertyChanged(nameof(MinPayout)); }
        }
        private string _selectedWinch;
        public string SelectedWinch
        {
            get => _selectedWinch;
            set { _selectedWinch = value.ToString(); OnPropertyChanged(nameof(SelectedWinch)); SelectedWinchEnabled = true; }
        }
        private bool _selectedWinchEnabled;

        public bool SelectedWinchEnabled
        {
            get => _selectedWinchEnabled;
            set { _selectedWinchEnabled = value; OnPropertyChanged(nameof(SelectedWinchEnabled)); }
        }
        private List<string> _fileList;
        public List<string> FileList
        {
            get => _fileList;
            set { _fileList = value; OnPropertyChanged(nameof(FileList)); }
        }
        private string _readingFileName;
        public string ReadingFileName
        {
            get => _readingFileName;
            set { _readingFileName = value; OnPropertyChanged(nameof(ReadingFileName)); }
        }
        private string _readingLine;
        public string ReadingLine
        {
            get => _readingLine;
            set { _readingLine = value; OnPropertyChanged(nameof(ReadingLine)); }
        }
    }
}
