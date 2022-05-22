using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

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


        public void buildOutFile(List<object[]> dados, string fileName)
        {
            Console.WriteLine("Escrevendo saida no HD");
            
            string output = "ID_MOEDA;DATA_REF;VL_COTACAO\r\n";
            File.WriteAllText(fileName+".csv", output);

            foreach(var data in dados)
            {
                output = String.Format("{0};{1:yyyy-MM-dd};{2}\r\n", (string) data[0], (DateTime) data[1], (string) data[2]);
                File.AppendAllText(fileName+".csv", output);
            }
        }

        public async Task requestMoeda(Func<string, bool> myMethodName)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try	
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:8000/");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                myMethodName(responseBody);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
    
    }
}