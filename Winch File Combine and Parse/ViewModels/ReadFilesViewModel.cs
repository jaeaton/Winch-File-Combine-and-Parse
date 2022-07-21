namespace Winch_File_Combine_and_Parse
{
    public class ReadFilesViewModel
    {
        public static async void CombineFiles()
        {
            Settings_Store _settingsStore = MainWindow._settingsStore;
            List<string> fileList = new List<string>();
            fileList = _settingsStore.FileList;
            string filePath = _settingsStore.Directory;
            foreach (var fin in fileList)
            {
                var fileRead = filePath + "\\" + fin;
                MainWindow._settingsStore.ReadingFileName = fileRead;
                //labelOpenFile.Update();
                System.IO.StreamReader file = new System.IO.StreamReader(fileRead);
                string line;
                bool flag = false;
                string stringData = null;
                List<string> Data = new List<string>();
                await Task.Run(() =>
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        line = line.Replace("\n", String.Empty); //remove EOL Characters
                        line = line.Replace("\r", String.Empty);
                        string[] data = line.Split(',');

                        if (_settingsStore.SelectedWinch == "SIO Traction Winch")
                        {
                            if (data[0] == "RD")
                            {
                                foreach (var dat in data)
                                {
                                    stringData += dat + ',';
                                }
                                flag = true;
                            }
                            else if (flag == true && data[0].Contains('/'))
                            {
                                data = data.Take(data.Count() - 1).ToArray();
                                foreach (var dat in data)
                                {
                                    stringData += dat + ',';
                                }
                                //writeCombined();
                                MainWindow._settingsStore.ReadingLine = stringData;
                                flag = false;
                                //stringData = null;
                            }
                            else
                            {
                                stringData = null;
                            }
                        }
                        else if (_settingsStore.SelectedWinch == "MASH Winch")
                        {
                            if (data[0] == "[LOGGING]" ||
                                data[0] == "DATETIME[YYYY/MM/DD hh:mm:ss.s]" ||
                                data[0] == "TIME")
                            {
                                stringData = null;
                            }
                            else
                            {
                                string dataDateAndTime = data[0];
                                string[] dataDate = dataDateAndTime.Split(' ');
                                stringData = "RD," + data[3] + "," + data[2] + "," + data[4] + "," + data[1] + "," + dataDate[0] + "," + dataDate[1];
                                /*foreach (var dat in data)
                                {
                                    stringData += dat + ',';
                                }*/
                                //writeCombined();
                                MainWindow._settingsStore.ReadingLine = stringData;
                                //stringData = null;
                            }

                        }
                        else if (_settingsStore.SelectedWinch == "Armstrong CAST 6")
                        {

                            //string dataDateAndTime = data[0];
                            //string[] dataDate = dataDateAndTime.Split(' ');
                            stringData = line;//"RD," + data[3] + "," + data[2] + "," + data[4] + "," + data[1] + "," + dataDate[0] + "," + dataDate[1];
                            /*foreach (var dat in data)
                            {
                                stringData += dat + ',';
                            }*/
                            //writeCombined();
                            MainWindow._settingsStore.ReadingLine = stringData;
                            //stringData = null;


                        }
                        if (stringData != null)
                        {
                            Data.Add(stringData);
                            stringData = null;
                        }

                    }
                    //Write data
                    WriteFilesViewModel.WriteCombined(Data);
                });

                file.Close();
            }
            MainWindow._settingsStore.ReadingLine = "Done!";
        }
        public static async void ParseFiles()
        {
            // Read threshold values
            float minPayout = MainWindow._settingsStore.MinPayout;
            float minTension = MainWindow._settingsStore.MinTension;
            int x = 1;
            int y = 3;
            MainWindow._settingsStore.ReadingFileName = MainWindow._settingsStore.CombinedFileName;
            MainWindow._settingsStore.ReadingLine = "Starting!";
            //Read in collected file and determine maximum values of casts
            using (System.IO.StreamReader sr = new System.IO.StreamReader(MainWindow._settingsStore.Directory + '\\' + MainWindow._settingsStore.CombinedFileName, true))
            {
                float maxTensionCurrent = 0;
                float maxPayoutCurrent = 0;
                string maxTensionString = null;
                string maxPayoutString = null;
                int cast = 1;
                bool castActive = false;
                float temp;
                string input;
                await Task.Run(() =>
                {
                    while ((input = sr.ReadLine()) != null)
                    {
                        input = input.Replace("\n", String.Empty); //remove EOL Characters
                        input = input.Replace("\r", String.Empty);
                        string[] values = input.Split(',');
                        object[] valueObject = new object[values.Length];
                        int i = 0;

                        if (MainWindow._settingsStore.SelectedWinch == "SIO Traction Winch")
                        {
                            foreach (var ob in values)
                            {
                                var test = float.TryParse(ob, out temp);
                                if (test == true) { valueObject[i] = temp; }
                                else { valueObject[i] = ob; }
                                i++;
                            }
                            //detect start of cast (values above threshold with positive slope)
                            if ((float)valueObject[x] > minTension && (float)valueObject[y] < minPayout)
                            {
                                castActive = true;
                                //check for new maximum values (tension and payout) and store
                                if ((float)valueObject[x] > maxTensionCurrent)
                                {
                                    maxTensionCurrent = (float)valueObject[x];
                                    maxTensionString = input;
                                }
                                if ((float)valueObject[y] < maxPayoutCurrent)
                                {
                                    maxPayoutCurrent = (float)valueObject[y];
                                    maxPayoutString = input;
                                }

                            }
                            //detect end of cast (values below threshold with negative slope)
                            if ((float)valueObject[x] < minTension && (float)valueObject[y] > minPayout && castActive == true)
                            {
                                WriteFilesViewModel.writeProcessed(maxTensionString, maxPayoutString, cast); //end cast, increment cast number, write processed data
                                MainWindow._settingsStore.ReadingLine = maxTensionString;

                                cast++;
                                castActive = false;
                                maxPayoutCurrent = 0;
                                maxTensionCurrent = 0;

                            }
                        }
                        if (MainWindow._settingsStore.SelectedWinch == "MASH Winch")
                        {
                            foreach (var ob in values)
                            {
                                var test = float.TryParse(ob, out temp);
                                if (test == true) { valueObject[i] = temp; }
                                else { valueObject[i] = ob; }
                                i++;
                            }
                            //detect start of cast (values above threshold with positive slope)
                            if ((float)valueObject[x] > minTension && (float)valueObject[y] > minPayout)
                            {
                                castActive = true;
                                //check for new maximum values (tension and payout) and store
                                if ((float)valueObject[x] > maxTensionCurrent)
                                {
                                    maxTensionCurrent = (float)valueObject[x];
                                    maxTensionString = input;
                                }
                                if ((float)valueObject[y] > maxPayoutCurrent)
                                {
                                    maxPayoutCurrent = (float)valueObject[y];
                                    maxPayoutString = input;
                                }

                            }
                            //detect end of cast (values below threshold with negative slope)
                            if (/*(float)valueObject[x] < minTension &&*/ (float)valueObject[y] < minPayout && castActive == true)
                            {
                                WriteFilesViewModel.writeProcessed(maxTensionString, maxPayoutString, cast); //end cast, increment cast number, write processed data
                                MainWindow._settingsStore.ReadingLine = maxTensionString;

                                cast++;
                                castActive = false;
                                maxPayoutCurrent = 0;
                                maxTensionCurrent = 0;

                            }
                        }
                    }
                });
            }
            MainWindow._settingsStore.ReadingLine = "Done!";
        }
    }
    
}
