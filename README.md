# gRPC 

![grpc-nedir](https://github.com/ouzdev/gRPC/blob/master/image/grpc-net-core.png?raw=true)
## Başlangıç
gRPC (Remote Procedure Call) Client başka bir sunucu uygulamasında ki bir fonksiyonu sanki kendi içerisindeki bir fonksiyonmuş gibi çağırıp çalıştırmasını sağlayan bir teknolojidir.

**Not: Burada ki içerikler gRPC öğrenirken aldığım notlarımın bir özeti mahiyetindedir.**


gRPC kullanılan uygulamarda iletişim Http2 protokolu kullanılarak tekbir TCP bağlantısı üzerinden sağlanır.Http2 binary serialization yöntemiyle datayı ilgili kaynağa iletir dolayısıyla data aktarımı RESTful servislerdeki text-based serialization yönteminden daha hızlı gerçekleşmektedir. Http2 protokolü çift taraflı stream sayesinde bir request karşılığında birden fazla response alınabilir. Bunun gibi değişik kombinasyonlarda request ve response senaryoları mümkündür. Bu konuya ilerleyen aşamalarda değineceğiz.


gRPC uygulamarda Client ve Server haberleşirken iletim türü ve mesaj içeriği Protocol Buffers yani protobuf adında bir kontrat (sözleşme) dosyasıyla sağlanır. Proto dosyası içerisinde ki direktifler neticesinde build aşamasında gerekli servisler oluşturulmaktadır. Uzantısı **.proto**'dur.

Örnek  .proto dosyası

    syntax = "proto3"; 
    
    option csharp_namespace = "grpcServer";
    
    package greet;
    
    service Greeter {
    
      rpc SayHello (HelloRequest) returns (HelloReply); 
    }
    
    // The request message containing the user's name.
    message HelloRequest {
      string name = 1;
    }
    
    // The response message containing the greetings.
    message HelloReply {
      string message = 1;
    }
    

## gRPC Konseptleri ve Türleri
- **Unary:** Request ve response mantığında çalışan türdür. Client server tarafına istekte bulunur ve gereken tek response alır.
- **Server Streaming:** Bu türde Client  servera istekte bulunur neticesinde birden fazla response doner ise bu Server Streaming olur.
- **Client Streaming:**  Server Streaming in tam tersidir Client birden fazla istekte bulunur neticesinde tek bir response donulen türdür.
- **Bi-Directional:**  Çift yönlü duplex streaming türüdür. Aynı anda Client ve Server ın haberleştiği modeldir.
## gRPC Yaşam Döngüsü
![grpc-workflow](https://github.com/ouzdev/gRPC/blob/master/image/gRPC-workflow.png?raw=true)
![grpc-workflow](https://github.com/ouzdev/gRPC/blob/master/image/grpc-workflow.jpeg?raw=true)

Protobuf dosyası protoc ile compile edildiğinde ilgili platforma uygun bir şekilde arayüzler sınıfları oluşacaktır. Bu arayüzler oluştuktan sonra arayüzler sayesinde Client ve Server ın haberleşmesi mümkün hale gelecektir. Haberleşmede HTTP2 protokolu kullanılır. Herhangi bir iletiim türü ile iletişimde bulunulduğunda ilk olarak meta-data adında ki yapılanmalar RPC ye gidecektir ve sonrasında veri gönderilecektir.

## Unary
![grpc-unary-type](https://github.com/ouzdev/gRPC/blob/master/image/grpc-unary-type.png?raw=true)

Unary konsepti gRPC'deki en basit iletişim türüdür. Tek bir request'e karşılık tek bir response alınan en temel modeldir.
Örnek bir kodlama üzerinden Unary türünü inceleyelim.

İlk olarak gerekli .proto uzantılı dosyamızı oluşturalım. Proto dosyamızın adı material.proto olcaktır. Bu işlemi ilk olarak Server tarafında yapıyoruz.

    syntax = "proto3";

    option csharp_namespace = "grpcMaterialServer";

    package materials;

    service Material {

     rpc SendCreateMaterial(MaterialCreateRequest) returns (MaterialCreateReply);  
    }

    message MaterialCreateRequest {
    string name = 1;
    string description=2;
    string sku=3;
    }

    message MaterialCreateReply {
    string message = 1;
    }

Proto dosyasını oluşturduktan sonra .csproj dosyasında ProtoBuf tanımlamasını yapıyoruz.

    <Project Sdk="Microsoft.NET.Sdk.Web">

    <PropertyGroup>
        <TargetFramework>net6.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>

    <ItemGroup>
        <Protobuf Include="Protos\greet.proto" GrpcServices="Server" />
        <Protobuf Include="Protos\material.proto" GrpcServices="Server" /> // İlgili proto tanımımız
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="Grpc.AspNetCore" Version="2.40.0" />
    </ItemGroup>

    </Project>

Sonrasında Client tarafında ilk olarak build işlemini yapıyoruz sonrasında Program.cs dosyasında ilgili methodu çağırıp bir request oluşturuyorum.

    var materialClient = new Material.MaterialClient(channel);

            MaterialCreateReply returnMessage = await materialClient.SendCreateMaterialAsync(
                new MaterialCreateRequest(){
                    Name="Apple Macbook Pro 15 M1 Pro",
                    Description="Apple Macbook Pro 16 gb Ram M1 CPU", 
                    Sku="MBPM1"
                }
            );

            Console.WriteLine(returnMessage.Message);

Bu işlemden sonra grpcServer'dan dönen response

    ********** Eklenen Malzeme Bilgileri **********
    Eklenen Malzeme Kartı Adı --> Apple Macbook Pro 15 M1 Pro
    Eklenen Malzeme Kartı Açıklaması --> Apple Macbook Pro 16 gb Ram M1 CPU
    Eklenen Malzeme Stok Kodu --> MBPM1

## Server Streaming
Server streming türünde giden tek bir requeste karşılık, response olarak Stream türünde veri döner.
Bunu örnek kodlama üzerinden inceleyelim. Örnek olarak fatura bilgilerini göndererek bu bilgiler ışığında bir fatura oluşturuyoruz.

İlk olarak Server tarafında ki yapılandırmalarımızı yapalım.

### Server Yapılandırması
İlk olarak Proto dosyamızı oluşturuyoruz. 

    syntax = "proto3";

    option csharp_namespace = "grpcInvoiceServer";

   package invoices;

   service Invoice {

     rpc SendCreateInvoiceParameter(MaterialCreateRequest) returns (stream InvoiceCreateResponse);  //Server stream olacağı için return tipini stream olarak belirliyoruz.
   }

    message InvoiceCreateRequest {
      string name = 1;
      string description=2;
      string no=3;
   }

   message InvoiceCreateResponse {
      string invoice = 1;
    }


  