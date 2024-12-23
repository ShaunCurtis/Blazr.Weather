/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public abstract class ReadPresenter<TRecord, TKey> : IReadPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    protected IMediator _dataBroker;
    private readonly INewRecordProvider<TRecord> _newRecordProvider;

    public TRecord Item { get; protected set; } = new TRecord();

    public ItemQueryResult<TRecord> LastResult { get; protected set; } = ItemQueryResult<TRecord>.Success(new());

    public ReadPresenter(IMediator dataBroker, INewRecordProvider<TRecord> newRecordProvider)
    {
        _dataBroker = dataBroker;
        _newRecordProvider = newRecordProvider;
    }

    public async ValueTask LoadAsync(TKey id)
        => await GetRecordItemAsync(id);

    protected abstract Task<ItemQueryResult<TRecord>> GetItemAsync(TKey id);

    private async ValueTask GetRecordItemAsync(TKey id)
    {
        LastResult = await this.GetItemAsync(id);

        if (LastResult.Successful)
            this.Item = this.LastResult.Item ?? _newRecordProvider.NewRecord(); 
    }

}