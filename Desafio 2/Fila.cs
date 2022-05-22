using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using CurrencyApplication;
using System.IO;

namespace CurrencyApplication
{
    public class Fila{
        string currencyFile = "CurrencyQueue.txt";

        public string addItem(string item)
        {
            //busca dados no arquivo de texto
            string FileText = File.ReadAllText(this.currencyFile);
            if(!this.isCoinList(FileText))  FileText = "[]";

            List<Coin> currencyQueue = JsonSerializer.Deserialize<List<Coin>>(FileText);

            try{
                if(this.isCoin(item))
                {
                    Console.WriteLine("Is Coin");
                    Coin coin = JsonSerializer.Deserialize<Coin>(item);
                    currencyQueue.Add(coin);
                }
                else
                {
                    if(this.isCoinList(item))
                    {
                        Console.WriteLine("Is Coin List");
                        currencyQueue = JsonSerializer.Deserialize<List<Coin>>(item);
                        currencyQueue.AddRange(currencyQueue);
                    }                    
                }
                Console.WriteLine("Numero de Moedas");
                Console.WriteLine(currencyQueue.Count);

                //escreve para o arquivo de texto
                string _out = JsonSerializer.Serialize<List<Coin>>(currencyQueue);
                Console.WriteLine(_out);
                File.WriteAllText(this.currencyFile , _out);

                return "{\"status\": \"success\"}";
            }
            catch(Exception err)
            {
                Console.WriteLine(err);
                return "{\"status\": \"errorToAddObject\"}";
            }
        }

        public string getItem()
        {
            string coin = "{\"error\" : \"ObjectNotFound\"}";

            //busca dados no arquivo de texto
            string FileText = File.ReadAllText(this.currencyFile);
            if(!this.isCoinList(FileText))  FileText = "[]";
            List<Coin> currencyQueue = JsonSerializer.Deserialize<List<Coin>>(FileText);

            if(currencyQueue.Count > 0)
            {
                coin = JsonSerializer.Serialize<Coin>(currencyQueue[currencyQueue.Count-1]);
                currencyQueue.RemoveAt(currencyQueue.Count-1);

                //escreve para o arquivo de texto
                string _out = JsonSerializer.Serialize<List<Coin>>(currencyQueue);
                Console.WriteLine(_out);
                File.WriteAllText(this.currencyFile , _out);
            }
            return coin;
        }
        
        public bool isCoin(string item)
        {
            try{
                JsonSerializer.Deserialize<Coin>(item);
                return true;
            }
            catch(Exception err)
            {
                return false;
            }
        }

        public bool isCoinList(string item)
        {
            try{
                JsonSerializer.Deserialize<List<Coin>>(item);
                return true;
            }
            catch(Exception err)
            {
                return false;
            }
        }
    }
}