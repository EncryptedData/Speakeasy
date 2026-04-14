using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IGifRepository : IRepository<Gif>
{
    IAsyncEnumerable<Gif> GetAsyncEnumerable(
        int skip,
        int take,
        Group? group = null, 
        IEnumerable<string>? tags = null,
        bool trackEntities = false);
}