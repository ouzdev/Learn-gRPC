using Grpc.Core;
using grpcMaterialServer;

namespace grpcServer.Services;

public class MaterialService : Material.MaterialBase
{
    private readonly ILogger<MaterialService> _logger;
    public MaterialService(ILogger<MaterialService> logger)
    {
        _logger = logger;
    }

    public override Task<MaterialCreateReply> SendCreateMaterial(MaterialCreateRequest request, ServerCallContext context)
    {

        return Task.FromResult(new MaterialCreateReply
        {
            Message = "Eklenen Malzeme Kartı Adı --> " + request.Name + "\n" +
                      "Eklenen Malzeme Kartı Açıklaması --> " + request.Description + "\n" +
                      "Eklenen Malzeme Stok Kodu -->" + request.Sku
        });
    }
}