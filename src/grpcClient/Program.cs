using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using grpcMaterialClient;
using grpcServer;

namespace grpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //İlgili server url:port adresine göre bir kanal oluşturup bunu Client a parametre olarak geçerek Client bağlantısını tamamlamış oluyoruz.
            var channel = GrpcChannel.ForAddress("https://localhost:7172");
            var greetClient = new Greeter.GreeterClient(channel);

            HelloReply msg = await greetClient.SayHelloAsync(
                 new HelloRequest()
                 { Name = "Oğuzcan Genç" }
                 );
            CalculatorReply clc = await greetClient.CalculatorAsync(
                new CalculatorRequest()
                {
                    Sayi1 = 15,
                    Sayi2 = 50
                }
            );
            Console.WriteLine(msg.Message);
            Console.WriteLine(clc.Toplam);

            var materialClient = new Material.MaterialClient(channel);

            MaterialCreateReply returnMessage = await materialClient.SendCreateMaterialAsync(
                new MaterialCreateRequest(){
                    Name="Apple Macbook Pro 15 M1 Pro",
                    Description="Apple Macbook Pro 16 gb Ram M1 CPU", 
                    Sku="MBPM1"
                }
            );

            Console.WriteLine(returnMessage.Message);
        }
    }
}
