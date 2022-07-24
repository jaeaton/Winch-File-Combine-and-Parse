namespace Winch_File_Combine_and_Parse
{
    public class ReadFilesViewModel
    {
        public static async void CombineFiles()
        {
            Settings_Store _settingsStore = MainWindow._settingsStore;
            List<string> fileList = new(); //List<string>();
            //fileList = _settingsStore.FileList;
            string filePath = _settingsStore.Directory;
            foreach (var fin in _settingsStore.FileList)
            {
                var fileRead = filePath + "\\" + fin;
                MainWindow._settingsStore.ReadingFileName = fileRead;
                System.IO.StreamReader file = new System.IO.StreamReader(fileRead); //Setup stream reader to read file
                string line;
                bool flag = false;
                bool dataLine = false;
                //string stringData = null;
                Line_Data_Model lineData = new();// Line_Data_Model();
                List<string> Data = new();// List<string>();
                List<Line_Data_Model> DataModels = new();// List<Line_Data_Model>();
                //Makes reading the file Asynchronous leaving the UI responsive
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
                                lineData.StringID = data[0];
                                lineData.Tension = float.Parse(data[1]);
                                lineData.Speed = float.Parse(data[2]);
                                lineData.Payout = float.Parse(data[3]);
                                lineData.Checksum = data[4];
                                lineData.TMAlarms = "00000000";
                                lineData.TMWarnings = "00000000";
                                //foreach (var dat in data)
                                //{
                                //    stringData += dat + ',';

                                //}
                                flag = false;
                            }
                            else if (flag == true && data[0].Contains('/'))
                            {
                                data = data.Take(data.Length - 1).ToArray();
                                lineData.DateAndTime = DateTime.Parse(data[0] + "T" + data[1]);
                                //foreach (var dat in data)
                                //{
                                //    stringData += dat + ',';
                                //}

                                //MainWindow._settingsStore.ReadingLine = stringData; //Updates Line being written to UI
                                flag = false;
                                dataLine = true;
                            }
                            else
                            {
                                //stringData = null;
                                dataLine = false;
                                //lineData = new Line_Data_Model();
                            }
                        }
                        else if (_settingsStore.SelectedWinch == "MASH Winch")
                        {
                            if (data[0] == "[LOGGING]" ||
                                data[0] == "DATETIME[YYYY/MM/DD hh:mm:ss.s]" ||
                                data[0] == "TIME")
                            {
                                dataLine = false;
                                //stringData = null; //Ingnore line that starts with above
                                //lineData = new Line_Data_Model();
                            }
                            else
                            {
                                string dataDateAndTime = data[0];
                                string[] dataDate = dataDateAndTime.Split(' ');
                                //stringData = "RD," + data[3] + "," + data[2] + "," + data[4] + "," + data[1] + "," + dataDate[0] + "," + dataDate[1];
                                lineData.StringID = "RD";
                                lineData.Tension = float.Parse(data[3]);
                                lineData.Speed = float.Parse(data[2]);
                                lineData.Payout = float.Parse(data[4]);
                                lineData.Checksum = data[1];
                                lineData.DateAndTime = DateTime.Parse(dataDate[0] + "T" + dataDate[1]);
                                lineData.TMAlarms = "00000000";
                                lineData.TMWarnings = "00000000";
                                //MainWindow._settingsStore.ReadingLine = stringData; //Updates Line being written to UI
                                dataLine = true;

                            }

                        }
                        else if (_settingsStore.SelectedWinch == "Armstrong CAST 6")
                        {

                            //string dataDateAndTime = data[0];
                            //string[] dataDate = dataDateAndTime.Split(' ');
                            //stringData = line;//"RD," + data[3] + "," + data[2] + "," + data[4] + "," + data[1] + "," + dataDate[0] + "," + dataDate[1];
                            /*foreach (var dat in data)
                            {
                                stringData += dat + ',';
                            }*/
                            //writeCombined();
                            //MainWindow._settingsStore.ReadingLine = stringData; //Updates Line being written to UI
                            //stringData = null;
                            dataLine = true;

                        }
                        else if (_settingsStore.SelectedWinch == "UNOLS String")
                        {
                            if (data[0] == "$WIR")
                            {
                                lineData.StringID = data[0];
                                lineData.Tension = float.Parse(data[3]);
                                lineData.Speed = float.Parse(data[4]);
                                lineData.Payout = float.Parse(data[5]);
                                lineData.Checksum = data[9];
                                lineData.DateAndTime = DateTime.Parse($"{data[1]}T{data[2]}");
                                lineData.TMAlarms = data[7];
                                lineData.TMWarnings = data[8];
                                dataLine = true;
                            }
                        }

                        if (dataLine == true)
                        {
                            //Data.Add(stringData); //add the string to Data List
                            DataModels.Add(lineData);
                            //MainWindow._settingsStore.ReadingLine = stringData; //Updates Line being written to UI
                            MainWindow._settingsStore.ReadingLine = lineData.StringID + "," + lineData.Tension + "," + lineData.Speed + "," + lineData.Payout + "," + lineData.DateAndTime;
                            //stringData = null; //Clear stringData for next loop
                            lineData = new Line_Data_Model();
                            dataLine = false;
                        }

                }
                    //Write data
                    WriteFilesViewModel.WriteCombined(DataModels); //Write Data list
                });

                file.Close(); //Close the file
            }
            MainWindow._settingsStore.ReadingLine = "Done!"; //Update UI with done
        }
        public static async void ParseFiles()
        {
            // Read threshold values
            float minPayout = MainWindow._settingsStore.MinPayout;
            float minTension = MainWindow._settingsStore.MinTension;
            //int x = 1;
            //int y = 3;
            MainWindow._settingsStore.ReadingFileName = MainWindow._settingsStore.CombinedFileName;
            MainWindow._settingsStore.ReadingLine = "Starting!";
            //Read in collected file and determine maximum values of casts
            using (System.IO.StreamReader sr = new System.IO.StreamReader(MainWindow._settingsStore.Directory + '\\' + MainWindow._settingsStore.CombinedFileName, true))
            {
                float maxTensionCurrent = 0;
                float maxPayoutCurrent = 0;
                string? maxTensionString = null;
                string? maxPayoutString = null;
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
                        Line_Data_Model lineData = new();// Line_Data_Model();
                        string[] values = input.Split(',');
                        //object[] valueObject = new object[values.Length];
                        //int i = 0;

                        //lineData.StringID = values[0];
                        lineData.Tension = float.Parse(values[3]);
                        //lineData.Speed = float.Parse(values[4]);
                        lineData.Payout = float.Parse(values[5]);
                        //lineData.Checksum = values[8];
                        //lineData.DateAndTime = DateTime.Parse($"{values[1]}T{values[2]}");
                        //lineData.TMAlarms = values[7];
                        //lineData.TMWarnings = values[6];

                        //if (MainWindow._settingsStore.SelectedWinch == "SIO Traction Winch")
                        {
                            //foreach (var ob in values)
                            //{
                            //    var test = float.TryParse(ob, out temp);
                            //    if (test == true) { valueObject[i] = temp; }
                            //    else { valueObject[i] = ob; }
                            //    i++;
                            //}
                            //detect start of cast (values above threshold with positive slope)
                            if (lineData.Tension > minTension && Math.Abs(lineData.Payout) > minPayout)
                            {
                                castActive = true;
                                //check for new maximum values (tension and payout) and store
                                if (lineData.Tension> maxTensionCurrent)
                                {
                                    maxTensionCurrent = lineData.Tension;
                                    maxTensionString = input;
                                }
                                if (Math.Abs(lineData.Payout) > maxPayoutCurrent)
                                {
                                    maxPayoutCurrent = Math.Abs(lineData.Payout);
                                    maxPayoutString = input;
                                }

                            }
                            //detect end of cast (values below threshold with negative slope)
                            
                                if (/*lineData.Tension < minTension &&*/ Math.Abs(lineData.Payout) < minPayout && castActive == true)
                                {
                                    WriteFilesViewModel.writeProcessed(maxTensionString, maxPayoutString, cast); //end cast, increment cast number, write processed data
                                    MainWindow._settingsStore.ReadingLine = maxTensionString;

                                    cast++;
                                    castActive = false;
                                    maxPayoutCurrent = 0;
                                    maxTensionCurrent = 0;

                                }
                            
                        }
                        //if (MainWindow._settingsStore.SelectedWinch == "MASH Winch")
                        //{
                        //    foreach (var ob in values)
                        //    {
                        //        var test = float.TryParse(ob, out temp);
                        //        if (test == true) { valueObject[i] = temp; }
                        //        else { valueObject[i] = ob; }
                        //        i++;
                        //    }
                        //    //detect start of cast (values above threshold with positive slope)
                        //    if ((float)valueObject[x] > minTension && (float)valueObject[y] > minPayout)
                        //    {
                        //        castActive = true;
                        //        //check for new maximum values (tension and payout) and store
                        //        if ((float)valueObject[x] > maxTensionCurrent)
                        //        {
                        //            maxTensionCurrent = (float)valueObject[x];
                        //            maxTensionString = input;
                        //        }
                        //        if ((float)valueObject[y] > maxPayoutCurrent)
                        //        {
                        //            maxPayoutCurrent = (float)valueObject[y];
                        //            maxPayoutString = input;
                        //        }

                        //    }
                        //    //detect end of cast (values below threshold with negative slope)
                        //    if (/*(float)valueObject[x] < minTension &&*/ (float)valueObject[y] < minPayout && castActive == true)
                        //    {
                        //        WriteFilesViewModel.writeProcessed(maxTensionString, maxPayoutString, cast); //end cast, increment cast number, write processed data
                        //        MainWindow._settingsStore.ReadingLine = maxTensionString;

                        //        cast++;
                        //        castActive = false;
                        //        maxPayoutCurrent = 0;
                        //        maxTensionCurrent = 0;

                        //    }
                        //}
                    }
                });
            }
            MainWindow._settingsStore.ReadingLine = "Done!";
        }
    }
    
}
