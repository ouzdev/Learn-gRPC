using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using grpcInvoiceClient;
using grpcMaterialClient;
using grpcReportClient;
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
                new MaterialCreateRequest()
                {
                    Name = "Apple Macbook Pro 15 M1 Pro",
                    Description = "Apple Macbook Pro 16 gb Ram M1 CPU",
                    Sku = "MBPM1"
                }
            );

            Console.WriteLine(returnMessage.Message);

            //Server Streaming
            var invoiceClient = new Invoice.InvoiceClient(channel);
            var responseStream = invoiceClient.SendCreateInvoice(
                        new InvoiceCreateRequest()
                        {
                            Description = "Macbook Pro 15 MQ PRO Fatura Açıklaması",
                            Name = "Satış Faturası",
                            No = "EFT00000000005254"
                        }
                );
            CancellationTokenSource token = new CancellationTokenSource();
            while (await responseStream.ResponseStream.MoveNext(token.Token))
            {
                Console.WriteLine(responseStream.ResponseStream.Current.Invoice);
            }

            //Client Streaming
            var reportClient = new Report.ReportClient(channel);

            var request = reportClient.SendReport();

            for (int i = 0; i < 10; i++)
            {
                await request.RequestStream.WriteAsync(new ReportRequest
                {
                    Name = "Satış Raporu",
                    Description = "Satış Raporu Açıklaması",
                    ReportType = "13"
                });
            }

            await request.RequestStream.CompleteAsync();

            Console.WriteLine((await request.ResponseAsync).Message);
        }
    }
}
