
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CurrencyApplication{
    
    public class Moeda{
        private string[] formats = {"dd/MM/yyyy", "yyyy-MM-dd"};
        List<object[]> buffer;
        string fileName = "DadosMoeda.csv";
        public Moeda()
        {
            this.buffer = new List<object[]>();
        }
        public Moeda(string fName)
        {
            this.buffer = new List<object[]>();
            this.fileName = fName;
        }

        public void bufferize()
        {
            DateTime parsedDate;
            string csvContents = File.ReadAllText(this.fileName);
            string[] csvLines = csvContents.Split("\n");
            foreach (var csvLine in csvLines)
            {
                string moedaName, moedaDate;

                moedaName = csvLine.Split(";")[0];
                moedaDate = csvLine.Split(";")[1];

                if(DateTime.TryParseExact(moedaDate, formats, null,
                    System.Globalization.DateTimeStyles.AllowWhiteSpaces |
                        System.Globalization.DateTimeStyles.AdjustToUniversal,
                            out parsedDate)){

                    object[] tmp = {moedaName, parsedDate};
                    this.buffer.Add(tmp);

                }

            }

        }
        
        public void unbufferize()
        {
            this.buffer = new List<object[]>();
        }
    
        public List<object[]> getDadosMoeda(string _periodoInicio, string _periodoFim)
        {
            //define objeto de retorno
            List<object[]> returnData =  new List<object[]>();

            //verifica argumentos            
            DateTime periodoInicio;
            DateTime periodoFim;

            if((! DateTime.TryParseExact(_periodoInicio, formats, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AdjustToUniversal, out periodoInicio)) || 
                    (! DateTime.TryParseExact(_periodoFim, formats, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AdjustToUniversal, out periodoFim)))
                    {
                        Console.WriteLine("ArgumentError");
                        return returnData;
                    }

            foreach (var fileData in this.buffer)
            {
                string moedaName = (string) fileData[0];
                DateTime moedaDate = (DateTime) fileData[1];

                if((moedaDate < periodoInicio) && (moedaDate > periodoFim))
                {
                    object[] tmp = {moedaName, moedaDate};
                    returnData.Add(tmp);
                }
                
            }

            return returnData;
        }
    }
}