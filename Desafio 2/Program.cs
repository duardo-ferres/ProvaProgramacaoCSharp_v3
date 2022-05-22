using System;
using System.Collections.Generic;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CurrencyApplication
{
    class Program
    {
        static bool processingData = false;
        static DateTime processingStart, processingEnd;
        static Log log;

        public class Coin{
            public string moeda {get; set;}
            public string data_inicio {get; set;}
            public string data_fim {get; set;}
        }
        public static bool isCoin(string item)
        {
            try{
                var confer = JsonSerializer.Deserialize<Coin>(item);
                if(confer.moeda == null)    return false;
                return true;
            }
            catch(Exception err)
            {
                return false;
            }
        }
        
        public static bool processRequestData(string requestData)
        {
            log.Write("Requisição finalizada - Iniciando Processamento de Dados");

            long andamento = 0;
            Utilities util = new Utilities();
            Moeda moeda = new Moeda();
            Cotacao cotacao = new Cotacao();
            List<object[]> cotacoes = new List<object[]>();

            //apenas retorno de sucesso
            Console.WriteLine("processado com sucesso");
            Console.WriteLine(requestData);
            
            //verifica se é moeda caso não seja cancela o evento atual e aguarda o proximo
            if(!isCoin(requestData)){
                log.Write("A requisição retornou dados inválidos - Finalizando Processamento com Codigo 0");
                processingData = false;
                return false;
            }

            //transforma em json object
            Coin coin = JsonSerializer.Deserialize<Coin>(requestData);

            //cria uma copia do arquivo em buffer
            log.Write("Recuperando dados soble Moedas");
            moeda.bufferize();
            var moedaData = moeda.getDadosMoeda(coin.data_inicio, coin.data_fim);
            moeda.unbufferize();
            
            //navega moedas para recuperar cotacao
            log.Write("Analizando cotações para a data referida");
            cotacao.bufferize();
            
            foreach(var moedas in moedaData)
            {   
                andamento += 1;
                Console.Clear();
                Console.WriteLine("{0} %", ((float) andamento/ (float)moedaData.Count)*100);
                cotacoes.AddRange(cotacao.getCotacao((string) moedas[0], (DateTime) moedas[1]));  
            }

            cotacao.unbufferize();

            //escreve arquivo de saida
            log.Write("Escrevendo arquivo de saida com moedas e cotações");
            util.buildOutFile(cotacoes, string.Format("Resultado_{0:yyyyMMdd_HHmmss}", DateTime.Now));

            log.Write("Processamento Finalizado com Sucesso");

            processingEnd = DateTime.Now;                                   //define a hora final do processamento
            TimeSpan interval = processingEnd - processingStart;

            Console.WriteLine("Tempo médio do processamento - {0: mm:ss}", new DateTime(interval.Ticks));
            log.Write(String.Format("Tempo médio do processamento - {0: mm:ss}", new DateTime(interval.Ticks)));

            processingData = false;                                         //reseta flag liberando proximo processo
            return true;
        }

        static void Main(string[] args)
        {
            Utilities util = new Utilities();                               //Declara a biblioteca de ultilidades
            log = new Log();                                                //Declara novo Logger

            while(true)
            {
                log.Write("Iniciando novo ciclo");                          //Escreve log               
                processingStart = DateTime.Now;                             //define hora de inicio do processamento
                processingData = true;                                      //seta flag para travar o processamento ate o fim do processo vigente
                log.Write("Fazendo Requisição");                            //Escreve log
                util.requestMoeda(processRequestData);                      //inicia processamento
                Thread.Sleep(120000);                                       //aguarda 120 segundos
                log.Write("Aguardando requisição vigente");                 //Escreve log
                while(processingData == true)   Thread.Sleep(1000);         //aguarda o processo remanecente
                   
            }
        }
    }
}
