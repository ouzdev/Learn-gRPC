Protos -> proto dosyalarının bulunduğu klasordur.
Services -> Bussiness Logic (İş Süreçleri) servislerini içeren klasor.

Protos
----------------------------------------------------------------
syntax = "proto3"; --> Kullanılan syntax (sözdizimi).

option csharp_namespace = "grpcServer"; --> Oluşturulacak (code generate) servisin namespace bilgisini burada "grpcServer" olarak belirtildiği alandır.

package greet --> burada proto dosyasının ismi belirtilir.

İlgili service definationlarının belirtildiği alan
