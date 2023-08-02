using Containerd.Services.Containers.V1;
using Containerd.Services.Images.V1;
using Containerd.Services.Introspection.V1;
using ContainerdLib;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.Threading;
using static Containerd.Services.Containers.V1.Containers;
using static Containerd.Services.Images.V1.Images;
using static Containerd.Services.Introspection.V1.Introspection;
using static Containerd.Services.Version.V1.Version;

NamedPipesConnectionFactory npipecFactory = new NamedPipesConnectionFactory(".","containerd-containerd");

var socketsHttpHandler = new SocketsHttpHandler
{
    ConnectCallback = npipecFactory.ConnectAsync
};

var grpcChannel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
{
    HttpHandler = socketsHttpHandler
});

Metadata headers = new()
{
    { "containerd-namespace", "default" }
};

var callOpts = new CallOptions()
    .WithCancellationToken(CancellationToken.None)
    .WithDeadline(DateTime.UtcNow.AddMinutes(2));

var versionClient = new VersionClient(grpcChannel);
var emt = new Empty();
var verResponse = versionClient.Version(emt, callOpts);
Console.WriteLine(verResponse.ToString() + Environment.NewLine);

var imagesClient = new ImagesClient(grpcChannel);
var imagesReq = new ListImagesRequest();
var imagesResponse = imagesClient.List(imagesReq, headers);
Console.WriteLine(imagesResponse.ToString() + Environment.NewLine);

var containersClient = new ContainersClient(grpcChannel);
var containersRequest = new ListContainersRequest();
var containersResponse = containersClient.List(containersRequest, headers);
Console.WriteLine(containersResponse.ToString() + Environment.NewLine);

var introspectionClient = new IntrospectionClient(grpcChannel);
var pluginRequest = new PluginsRequest();
var serverResponse = introspectionClient.Server(emt);
Console.WriteLine(serverResponse.ToString() + Environment.NewLine); 
var pluginResponse = introspectionClient.Plugins(pluginRequest);
Console.WriteLine(pluginResponse.ToString() + Environment.NewLine);

