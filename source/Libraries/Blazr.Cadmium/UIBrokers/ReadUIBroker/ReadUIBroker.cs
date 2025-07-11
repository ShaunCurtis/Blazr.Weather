/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Blazr.Gallium;

namespace Blazr.Cadmium.Presentation;

public class ReadUIBroker<TRecord, TKey> : IReadUIBroker<TRecord, TKey>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;
    private readonly IMessageBus _messageBus;
    private TKey _key = default!;

    public TRecord Item { get; protected set; } = new TRecord();
    public event EventHandler? RecordChanged;
    public Result LastResult { get; protected set; } = Result.Success();

    public ReadUIBroker(IEntityProvider<TRecord, TKey> entityProvider, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _entityProvider = entityProvider;

        _messageBus.Subscribe<TKey>(OnRecordChanged);
    }

    public async ValueTask LoadAsync(TKey id)
        => await GetRecordItemAsync(id);

    private async ValueTask GetRecordItemAsync(TKey id)
    {
        await GetRecordItemAsync(Result<TKey>.Create(id));
    }

    private async ValueTask GetRecordItemAsync(Result<TKey> id)
    {
        LastResult = await id
            .ResultSideEffect((recordId) => _key = recordId)
            .MapResultAsync(_entityProvider.RecordRequest)
            .TaskSideEffectAsync(success: (record) => this.Item = record ?? _entityProvider.NewRecord)
            .MapTaskAsync();
    }

    private void OnRecordChanged(object? obj)
    {
        // test to see if we have a key of the same type
        // if so and it doesn't match the current key, we invoke the RecordChanged event
        var result = _entityProvider.GetKey(obj)
            .MapResult<TKey>(key => _key.Equals(key) ? Result<TKey>.NoValueFailure() : Result<TKey>.Success(key))
            .ResultSideEffect(
                success: async (key) =>
                {
                    await this.GetRecordItemAsync(key);
                    this.RecordChanged?.Invoke(this, EventArgs.Empty);
                });
    }

    public void Dispose()
    {
        _messageBus.UnSubscribe<TKey>(OnRecordChanged);
    }
}