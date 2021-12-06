# gRPC 
## Başlangıç
gRPC (Remote Procedure Call) Client başka bir sunucu uygulamasında ki bir fonksiyonu sanki kendi içerisindeki bir fonksiyonmuş gibi çağırıp çalıştırmasını sağlayan bir teknolojidir.

**Not: Burada ki içerikler gRPC öğrenirken aldığım notlarımın bir özeti mahiyetindedir.**

[![](grpc)](http://https://www.google.com/url?sa=i&url=https%3A%2F%2Fmedium.com%2F%40sddkal%2Fgrpc-api-rehberi-6dc561070c03&psig=AOvVaw0dX5UGRRq0hu4jpzzYgvx5&ust=1638906138049000&source=images&cd=vfe&ved=0CAsQjRxqFwoTCJDo2aT3z_QCFQAAAAAdAAAAABAD)
gRPC kullanılan uygulamarda iletişim Http2 protokolu kullanılarak tekbir TCP bağlantısı üzerinden sağlanır.Http2 binary serialization yöntemiyle datayı ilgili kaynağa iletir dolayısıyla RESTful servislerdeki text-based serialization yönteminden daha hızlı gerçekleşmektedir. Http2 protokolü çift taraflı stream sayesinde bir request karşılığında birden fazla response alınabilir. Bunun gibi değişik kombinasyonlarda request ve response senaryoları mümkündür. Bu konuya ilerleyen aşamalarda değineceğiz.


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
Protobuf dosyası protoc ile compile edildiğinde ilgili platforma uygun bir şekilde arayüzler sınıfları oluşacaktır. Bu arayüzler oluştuktan sonra arayüzler sayesinde Client ve Server ın haberleşmesi mümkün hale gelecektir. Haberleşmede HTTP2 protokolu kullanılır. Herhangi bir iletiim türü ile iletişimde bulunulduğunda ilk olarak meta-data adında ki yapılanmalar RPC ye gidecektir ve sonrasında veri gönderilecektir.
