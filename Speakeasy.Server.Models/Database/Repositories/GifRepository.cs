using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class GifRepository : BaseRepository<Gif>, IGifRepository
{
    public GifRepository(SpeakeasyDbContext context) : base(context.Gifs)
    {
    }

    protected override IQueryable<Gif> ApplyIncludes(IQueryable<Gif> query)
    {
        return query.Include(e => e.StoredFile)
            .Include(e => e.Author)
            .Include(e => e.Group);
    }

    public IAsyncEnumerable<Gif> GetAsyncEnumerable(
        int skip, 
        int take,
        Group? group = null,
        IEnumerable<string>? tags = null,
        bool trackEntities = false)
    {
        IQueryable<Gif> query = _db;

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = group is not null
            ? query.Where(e => e.Group == null || e.Group.Id == group.Id)
            : query.Where(e => e.Group == null);

        if (tags is not null)
        {
            tags = tags.Select(e => e.ToLower());
            
            query = query.Where(e => e.Tags.Any(e => tags.Contains(e)));
        }

        query = ApplyIncludes(query);

        return query
            .Skip(skip)
            .Take(take)
            .AsAsyncEnumerable();
    }
}