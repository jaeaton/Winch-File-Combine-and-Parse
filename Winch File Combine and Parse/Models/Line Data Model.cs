
namespace Winch_File_Combine_and_Parse
{
    public class Line_Data_Model
    {
        public string StringID { get; set; }
        public DateTime DateAndTime { get; set; }
        public float Tension { get; set; }
        public float Speed { get; set; }
        public float Payout { get; set; }
        public string Checksum { get; set; }
        public string TMWarnings { get; set; }
        public string TMAlarms { get; set; }

        public Line_Data_Model()
        {

        }
        public Line_Data_Model(string SID, DateTime DandT, float Ten, float Sp, float Pay, string check, string TMWarn, string TMAlarm)
        {
            StringID = SID;
            DateAndTime = DandT;
            Tension = Ten;  
            Speed = Sp; 
            Payout = Pay;
            Checksum = check;
            TMWarnings = TMWarn;
            TMAlarms = TMAlarm;
        }
    }
}
