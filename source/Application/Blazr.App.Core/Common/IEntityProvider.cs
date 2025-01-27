/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

/// <summary>
/// All Concrete implementations should be registered as Singletons to ensure unique new record Ids
/// are generated for volatile records in composite objects
/// </summary>
/// <typeparam name="TRecord"></typeparam>
public interface IEntityProvider<TRecord, TKey>
    where TRecord : new()
    where TKey : IEntityId
{
    public TRecord NewRecord();

    public TRecord DefaultRecord();
}
