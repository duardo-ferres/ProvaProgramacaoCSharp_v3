using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace CurrencyApplication{
    public class Cotacao{
        //formas para a conversao de datas
        private string[] formats = {"dd/MM/yyyy", "yyyy-MM-dd"};
        
        Dictionary<int, List<object[]>> buffer;
        string fileName = "DadosCotacao.csv";
        private Dictionary<string, int> deParaTable = (new Utilities()).deParaTable;

        public Cotacao(){
            this.buffer = new Dictionary<int, List<object[]>> ();
        }
        public Cotacao(string fName){
            this.buffer = new Dictionary<int, List<object[]>> ();
            string fileName = fName;
        }
        
        //buferiza os dados para a memoria
        public void bufferize()
        {
            DateTime parsedDate;
            string csvContents = File.ReadAllText(this.fileName);
            string[] csvLines = csvContents.Split("\n");

            //navega as linhas do csv
            foreach (var csvLine in csvLines)
            {
                //declaracoes
                string cotacao, moeda, date;

                //recupera dados do csv ignorado as linhas que tiverem campos faltantes
                string[] csvFields = csvLine.Split(";");
                if(csvFields.Length != 3)   continue;
                cotacao = csvFields[0];
                moeda = csvFields[1];                
                date = csvFields[2];

                //transforma data do arquivo
                if(DateTime.TryParseExact(date, formats, null,
                    System.Globalization.DateTimeStyles.AllowWhiteSpaces |
                        System.Globalization.DateTimeStyles.AdjustToUniversal,
                           out parsedDate)){

                    int tmpMoeda;
                    if(int.TryParse(moeda, out tmpMoeda))
                    {
                        object[] result = {parsedDate, cotacao};
                        if(!this.buffer.ContainsKey(tmpMoeda))  this.buffer[tmpMoeda] = new List<object[]>();                        
                        this.buffer[tmpMoeda].Add(result);
                    }
                }

            }

        }

        //limpa o buffer  da memoria
        public void unbufferize()
        {
            this.buffer = new Dictionary<int, List<object[]>>();
        }

        //recupera cotacoes
        public List<object[]> getCotacao(string _moeda, DateTime dataSelecionada)
        {
                List<object[]> retData = new List<object[]>();

                foreach (var bufferData in this.buffer[this.deParaTable[_moeda]])
                {
                    //declaracoes
                    string cotacao;
                    DateTime date;
             
                    date = (DateTime) bufferData[0];
                    cotacao = (string) bufferData[1];

                    if(dataSelecionada == date)
                    {
                        object[] result = {_moeda, date, cotacao};
                        retData.Add(result);
                    }
                }

            return retData;
        }
    }
}