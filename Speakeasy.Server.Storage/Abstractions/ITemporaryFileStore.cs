namespace Speakeasy.Server.Storage.Abstractions;

public interface ITemporaryFileStore
{
    ITemporaryFile CreateTemporaryFile();
}