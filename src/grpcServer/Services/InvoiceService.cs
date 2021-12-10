using Grpc.Core;
using grpcInvoiceServer;

namespace grpcServer.Services;

public class InvoiceService : Invoice.InvoiceBase
{
    private readonly ILogger<InvoiceService> _logger;
    public InvoiceService(ILogger<InvoiceService> logger)
    {
        _logger = logger;
    }

    public override async Task SendCreateInvoice(InvoiceCreateRequest request, IServerStreamWriter<InvoiceCreateResponse> responseStream, ServerCallContext context)
    {
        Console.WriteLine("************* Fatura Bilgileri *************");
        Console.WriteLine("--> Fatura Numarası: " + request.No);
        Console.WriteLine("--> Fatura Adı: " + request.Name);
        Console.WriteLine("--> Fatura Açıklaması: " + request.Description);

        for (int i = 0; i < 3; i++)
        {
            i += 33;
            await Task.Delay(1000);
            await responseStream.WriteAsync(
                new InvoiceCreateResponse
                {
                    Invoice = "Faturanız Oluşturuluyor... Aşama %" + i
                }
            );
        }
    }


}