using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CurrencyApplication{
    public class Utilities{

    // Declara HttpClient
    private readonly HttpClient client = new HttpClient();

    //DadosCotacao.csv - ID_MOEDA DadosCotacao.csv - cod_cotacao
        private string[] formats = {"dd/MM/yyyy", "yyyy-MM-dd"};
        public Dictionary<string, int> deParaTable = new Dictionary<string, int>(){{"AFN",66} ,
        {"ALL",49} ,
        {"ANG",33} ,
        {"ARS",3} ,
        {"AWG",6} ,
        {"BOB",56} ,
        {"BYN",64} ,
        {"CAD",25} ,
        {"CDF",58} ,
        {"CLP",16} ,
        {"COP",37} ,
        {"CRC",52} ,
        {"CUP",8} ,
        {"CVE",51} ,
        {"CZK",29} ,
        {"DJF",36} ,
        {"DZD",54} ,
        {"EGP",12} ,
        {"EUR",20} ,
        {"FJD",38} ,
        {"GBP",22} ,
        {"GEL",48} ,
        {"GIP",18} ,
        {"HTG",63} ,
        {"ILS",40} ,
        {"IRR",17} ,
        {"ISK",11} ,
        {"JPY",9} ,
        {"KES",21} ,
        {"KMF",19} ,
        {"LBP",42} ,
        {"LSL",4} ,
        {"MGA",35} ,
        {"MGB",26} ,
        {"MMK",69} ,
        {"MRO",53} ,
        {"MRU",15} ,
        {"MUR",7} ,
        {"MXN",41} ,
        {"MZN",43} ,
        {"NIO",23} ,
        {"NOK",62} ,
        {"OMR",34} ,
        {"PEN",45} ,
        {"PGK",2} ,
        {"PHP",24} ,
        {"RON",5} ,
        {"SAR",44} ,
        {"SBD",32} ,
        {"SGD",70} ,
        {"SLL",10} ,
        {"SOS",61} ,
        {"SSP",47} ,
        {"SZL",55} ,
        {"THB",39} ,
        {"TRY",13} ,
        {"TTD",67} ,
        {"UGX",59} ,
        {"USD",1} ,
        {"UYU",46} ,
        {"VES",68} ,
        {"VUV",57} ,
        {"WST",28} ,
        {"XAF",30} ,
        {"XAU",60} ,
        {"XDR",27} ,
        {"XOF",14} ,
        {"XPF",50} ,
        {"ZAR",65} ,
        {"ZWL",31}};

        public List<string[]> getDadosMoeda(string _periodoInicio, string _periodoFim)
        {
            //define objeto de retorno
            List<string[]> returnData =  new List<string[]>();

            //verifica argumentos            
            DateTime periodoInicio;
            DateTime periodoFim;

            if((! DateTime.TryParseExact(_periodoInicio, formats, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AdjustToUniversal, out periodoInicio)) || 
                    (! DateTime.TryParseExact(_periodoFim, formats, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AdjustToUniversal, out periodoFim)))
                    {
                        Console.WriteLine("ArgumentError");
                        return returnData;
                    }


            string csvContents = File.ReadAllText("DadosMoeda.csv");
            string[] csvLines = csvContents.Split("\n");
            foreach (var csvLine in csvLines)
            {
                string moedaName, moedaDate;
                DateTime parsedDate;

                moedaName = csvLine.Split(";")[0];
                moedaDate = csvLine.Split(";")[1];

                DateTime.TryParseExact(moedaDate, formats, null,
                               System.Globalization.DateTimeStyles.AllowWhiteSpaces |
                               System.Globalization.DateTimeStyles.AdjustToUniversal,
                               out parsedDate);

                if((parsedDate > periodoInicio) && (parsedDate < periodoFim))
                {
                    string[] tmp = {moedaName, moedaDate};
                    returnData.Add(tmp);
                }
                
            }

            return returnData;
        }

        public List<object[]> getCotacao(string _moeda, string _data)
        {
            List<object[]> retData = new List<object[]>();

            DateTime dataSelecionada;

            if(! DateTime.TryParseExact(_data, formats, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AdjustToUniversal, out dataSelecionada))
                    {
                        Console.WriteLine("ArgumentError");
                        return retData;
                    }

            //faz leitura do arquivo
            string csvContents = File.ReadAllText("DadosCotacao.csv");
            string[] csvLines = csvContents.Split("\n");

            foreach (var csvLine in csvLines)
            {
                //declaracoes
                string cotacao, moeda, date;
                DateTime parsedDate;
                
                //recupera dados do csv ignorado as linhas que tiverem campos faltantes
                string[] csvFields = csvLine.Split(";");
                if(csvFields.Length != 3)   continue;
                cotacao = csvFields[0];
                moeda = csvFields[1];                
                date = csvFields[2];

                //transforma data do arquivo
                DateTime.TryParseExact(date, formats, null,
                               System.Globalization.DateTimeStyles.AllowWhiteSpaces |
                               System.Globalization.DateTimeStyles.AdjustToUniversal,
                               out parsedDate);

                //verifica moeda e data
                int tmpMoeda;
                if(int.TryParse(moeda, out tmpMoeda))
                    if(tmpMoeda == this.deParaTable[_moeda])
                        if(dataSelecionada == parsedDate)
                        {
                            object[] result = {_moeda, parsedDate, cotacao};
                            retData.Add(result);
                        }
            }

            return retData;
        }

        public void buildOutFile(List<object[]> dados)
        {
            string output = "ID_MOEDA;DATA_REF;VL_COTACAO\r\n";

            foreach(var data in dados)
            {
                output += String.Format("{0};{1:yyyy-MM-dd};{2}\r\n", (string) data[0], (DateTime) data[1], (string) data[2]);
            }

            Console.WriteLine(output);
        }

        public async Task requestMoeda()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try	
            {
                HttpResponseMessage response = await client.GetAsync("http://www.contoso.com/");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
    
    }
}