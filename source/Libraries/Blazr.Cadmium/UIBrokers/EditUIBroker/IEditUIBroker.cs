/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.Cadmium.Presentation;

public interface IEditUIBroker<TRecordEditContext, TKey>
        where TKey : notnull, IEntityId
{
    public Result LastResult { get; }
    public TRecordEditContext EditMutator { get; }
    public EditContext EditContext { get; }
    public EditState State { get; }

    public ValueTask LoadAsync(TKey id);
    public ValueTask ResetItemAsync();
    public ValueTask SaveItemAsync(bool refreshOnNew = true);
    public ValueTask DeleteItemAsync();
}
