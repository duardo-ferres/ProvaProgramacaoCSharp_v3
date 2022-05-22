using System;

namespace CurrencyApplication
{
    class Program
    {
        static string test_string = "{\"moeda\":\"USD\",\"data_inicio\":\"2010-01-01\",\"data_fim\":\"2010-01-01\"}";
        static string test_string_list = "[{\"moeda\":\"USD\", \"data_inicio\":\"2010-01-31\", \"data_fim\":\"2010-01-01\" }, { \"moeda\":\"EUR\", \"data_inicio\":\"2020-01-01\", \"data_fim\":\"2010-12-01\"}, {\"moeda\":\"JPY\", \"data_inicio\":\"2000-03-01\", \"data_fim\":\"2000-03-30\"}]";

        static void Main(string[] args)
        {
            Fila queue = new Fila();
            RestHttpServer rest = new RestHttpServer(queue);
            rest.run();
        }
    }
}
