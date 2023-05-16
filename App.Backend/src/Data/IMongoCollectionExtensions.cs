using MongoDB.Driver;

namespace App.Backend.Data;

static class IMongoCollectionExtensions
{
    public static async Task<TDocument> FindOrCreateAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filter,
        UpdateDefinition<TDocument> updateOnCreate,
        CancellationToken cancellationToken = default)
    {
        return await FindOrCreateAsync(collection, filter, updateOnCreate, null, cancellationToken);
    }

    public static async Task<TDocument> FindOrCreateAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filter,
        UpdateDefinition<TDocument> updateOnCreate,
        FindOneAndUpdateOptions<TDocument, TDocument>? options = null,
        CancellationToken cancellationToken = default)
    {
        if (options == null)
        {
            options = new FindOneAndUpdateOptions<TDocument, TDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
        }
        else
        {
            options.IsUpsert = true;
            options.ReturnDocument = ReturnDocument.After;
        }

        return await collection.FindOneAndUpdateAsync(filter, updateOnCreate, options, cancellationToken);
    }
}