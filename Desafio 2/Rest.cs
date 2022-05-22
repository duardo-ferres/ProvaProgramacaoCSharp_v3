using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;


namespace CurrencyApplication
{
    class RestHttpServer
    {
        private Fila queue;
        public  HttpListener listener;
        public  string url = "http://localhost:8000/";
        public  int pageViews = 0;
        public  int requestCount = 0;
        public  string pageData = 
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public RestHttpServer(Fila queue)
        {
            this.queue = queue;
        }

        //defiine uma tarefa para o servidor HTTP
        private async Task HandleIncomingConnections()
        {
            bool runServer = true;

            //se Mantem ouvindo requisições
            while (runServer)
            {               
                //Espera por uma conexao do usuario
                HttpListenerContext ctx = await listener.GetContextAsync();

                // recupera dados da requisição solicitada
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                
                // temporario para debugar a requisicaoo
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if ((req.HttpMethod == "GET"))
                {
                    Console.WriteLine("Get Request Solicitada");
                    
                    // escreve resposta no cliente
                    string disableSubmit = !runServer ? "disabled" : "";
                    byte[] data = Encoding.UTF8.GetBytes(this.queue.getItem());
                    resp.ContentType = "application/json";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    //envia na porta e fecha ela
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else if ((req.HttpMethod == "POST"))
                {
                    Console.WriteLine("Post Request Solicitada");

                    //faz leitura da stream
                    System.IO.Stream body = req.InputStream;
                    System.Text.Encoding encoding = req.ContentEncoding;
                    System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
                    string bodyText = reader.ReadToEnd();
                    body.Close();
                    reader.Close();

                    //tenta adicionar registro ao banco
                    string result = this.queue.addItem(bodyText);
                    
                    // escreve resposta no cliente
                    byte[] data = Encoding.UTF8.GetBytes(result);
                    resp.ContentType = "application/json";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    //envia na porta e fecha ela
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }

            }
        }

        public void run()
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}