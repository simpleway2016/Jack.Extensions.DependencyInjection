using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core.Adapter.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    class SocketFilterAdapter : IConnectionAdapter
    {
        public bool IsHttps => false;

        public Task<IAdaptedConnection> OnConnectionAsync(ConnectionAdapterContext context)
        {
            var connectionTransportFeature = context.Features.Get<IConnectionTransportFeature>();
            var f = connectionTransportFeature.GetType().GetField("_receiver", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var receiver = f.GetValue(connectionTransportFeature) as SocketReceiver;
            f = receiver.GetType().GetField("_socket", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var socket = (Socket)f.GetValue(receiver);

            return Task.Run<IAdaptedConnection>(()=> {
                //这里可以判断是否合格
                //byte[] bs = new byte[1024];
                //int readed = socket.Receive(bs,bs.Length, SocketFlags.Peek);
                //var text = Encoding.UTF8.GetString(bs);
                //socket.Shutdown(SocketShutdown.Both);
                //socket.Close();
                //socket.Dispose();
                //return null;
                return new AdaptedConnection(context.ConnectionStream);
            });
            
        }

        private class RewritingStream : Stream
        {
            private readonly Stream _innerStream;

            public RewritingStream(Stream innerStream)
            {
                _innerStream = innerStream;
            }

            public override bool CanRead => _innerStream.CanRead;

            public override bool CanSeek => _innerStream.CanSeek;

            public override bool CanWrite => _innerStream.CanWrite;

            public override long Length => _innerStream.Length;

            public override long Position
            {
                get
                {
                    return _innerStream.Position;
                }
                set
                {
                    _innerStream.Position = value;
                }
            }

            public override void Flush()
            {
                _innerStream.Flush();
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return _innerStream.FlushAsync(cancellationToken);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _innerStream.Read(buffer, offset, count);
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
               return _innerStream.ReadAsync(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _innerStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _innerStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _innerStream.Write(buffer, offset, count);
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
            }
        }

        private class AdaptedConnection : IAdaptedConnection
        {
            public AdaptedConnection(Stream adaptedStream)
            {
                ConnectionStream = new RewritingStream(adaptedStream);
            }

            public Stream ConnectionStream { get; }

            public void Dispose()
            {
            }
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
           .UseKestrel(options =>
           {
               var tocomparedata = "3082010A0282010100BB5A3812BF6BE5127456F2C46FAC1E6F85B3BFE19DF50C1530019466F5333B1CFA1A3FDE6DCDE867EF4DA5065DACB34BB4F3104ACB188837CCBF73E3C50CC292BA47BBD378D586CFBBDA108FD47A4780B3C24BE27CFA53B65EA3DE1080DD2C320D93093DA5F88A36342D097A7BF0EDED0ABDDC59055CFE669652620BF73304FC41FDC0CDFC292645CC935E958CFB3660F7E2A56A1AF78EC7DD635622C19136E00D556F464428F5314EF7967879D421B953EAB1F0AA0ACD201DA1B9660478E62FDAB61A9A8EDC91F9658DB4661F51842A525E0BAD1239A23C87F9AAEBD088AF60AD9B7BCE2E1A3FF820D15FFA0820334FE9D49E876E8A8166A0B269B52998D91B0203010001";
               //启用https，443端口
               options.Listen(IPAddress.Any, 5000, listenOptions =>
                                {
                                    //var serverCertificate = new X509Certificate2("D:/nodetls/server.pfx", "123456");
                                    //var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                                    //{ 

                                    //    ClientCertificateMode = ClientCertificateMode.RequireCertificate,
                                    //    SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                                    //    //用chain.Build验证客户端证书
                                    //    ClientCertificateValidation = (cer, chain, error) =>
                                    //     {
                                    //         return true;
                                    //         if (cer.GetPublicKeyString() != tocomparedata)
                                    //             return false;

                                    //         //var ret = chain.Build(cer);
                                    //         return true;
                                    //     },
                                    //    ServerCertificate = serverCertificate
                                    //};
                                    //listenOptions.UseHttps(httpsConnectionAdapterOptions);

                                    listenOptions.ConnectionAdapters.Add(new SocketFilterAdapter());


                                });
           })
                .UseStartup<Startup>();
    }
}
