namespace Speakeasy.Server.Storage.Abstractions;

public interface ITemporaryFile : IAsyncDisposable
{
    Stream GetStream();
}