/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;

namespace Blazr.Cadmium.Core;

public interface IRecordEditContext<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; }
    public TRecord AsRecord { get; }
    public bool IsDirty { get; }

    public Result Load(TRecord record);
    public void Reset();
    public void SetAsPersisted();
}
