
using System.IO;
using System;

namespace CurrencyApplication{
    public class Log{
        string logFile = "currencySystem.log";
        public Log() {}
        public Log(string fileName)
        {
            this.logFile = fileName;
        }

        public void Write(string data)
        {
            string outputFile = String.Format("{0}_{1:ddMMyyyy}", this.logFile, DateTime.Now);
            if(!File.Exists(outputFile))   File.WriteAllText(outputFile, "");
            File.AppendAllText(outputFile, string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1}\r\n", DateTime.Now, data));
        }
    }
}