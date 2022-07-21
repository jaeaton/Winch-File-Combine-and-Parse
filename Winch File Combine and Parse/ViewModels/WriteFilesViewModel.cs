namespace Winch_File_Combine_and_Parse
{
    public class WriteFilesViewModel
    {
        public static void WriteCombined(List<string> Data)
        {

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(MainWindow._settingsStore.Directory + '\\' + MainWindow._settingsStore.CombinedFileName, true))    //Write Combined Log file
            {

                foreach(string stringData in Data)
                {
                    file.WriteLine(stringData);
                }
              

            }
        }
        public static void writeProcessed(string maxTensionString, string maxPayoutString, int cast)  //function to write log
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(MainWindow._settingsStore.Directory + '\\' + MainWindow._settingsStore.ProcessedFileName, true))    //Write Processed Log file
            {
                file.WriteLine("Cast Number " + cast);
                file.WriteLine("Winch Data, Tension, Speed, Payout, Checksum/Index, Date, Time");
                file.WriteLine(maxTensionString);
                file.WriteLine(maxPayoutString);
                file.WriteLine("\n");
            }
        }

    }
}
