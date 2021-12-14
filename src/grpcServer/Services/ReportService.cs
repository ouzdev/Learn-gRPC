
using Grpc.Core;
using grpcReportServer;

namespace grpcServer.Services;

public class ReportService : Report.ReportBase
{
    private readonly ILogger<ReportService> _logger;

    public ReportService(ILogger<ReportService> logger)
    {
        _logger = logger;
    }

    public override async Task<ReportResponse> SendReport(IAsyncStreamReader<ReportRequest> requestStream, ServerCallContext context)
    {
        int count = 0;
        while (await requestStream.MoveNext(context.CancellationToken))
        {
            await Task.Delay(1000);
            Console.WriteLine("Rapor Yükleniyor Lütfen Bekleyiniz... | Aşama %" + count*10);
            count++;

        }

        return new ReportResponse
        {
            Message = "Rapor başarıyla yüklendi... Aşama %100"
        };
    }
}