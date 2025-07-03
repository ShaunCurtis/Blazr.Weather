/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class TripSwitch
{
    private bool _tripped;

    private TripSwitch() {}

    public void Trip()
        => _tripped = true;

    public void Reset()
        => _tripped = false;

    public TripSwitch Map(Func<bool> func)
    {
        if (func())
            Trip();
        return this;
    }

    public TripSwitch Match(Action Tripped, Action NotTripped)
    {
        if (_tripped)
        {
            Tripped();
            return this;
        }

        NotTripped();
        return this;
    }

    public TripSwitch MatchYes(Action action)
    {
        if (_tripped)
            action();

        return this;
    }

    public TripSwitch MatchNo(Action action)
    {
        if (!_tripped)
            action();

        return this;
    }
}
