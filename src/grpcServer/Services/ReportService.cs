
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
            Console.WriteLine("Rapor Yükleniyor Lütfen Bekleyiniz... | Aşama %" + count);
            count++;

        }

        return new ReportResponse
        {
            Message = "Rapor başarıyla yüklendi... Aşama %100"
        };
    }
}