/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public record YesNo
{
    private readonly bool _value;

    private YesNo(bool state)
    {
        _value = state;
    }

    public YesNo Switch => new YesNo(!_value);


    public YesNo Match(Action yes, Action no)
    {
        if (_value)
        {
            yes();
            return this;
        }

        no();
        return this;
    }

    public YesNo MatchYes(Action action)
    {
        if (_value)
            action();

        return this;
    }

    public YesNo MatchNo(Action action)
    {
        if (!_value)
            action();

        return this;
    }

    public static YesNo Return(bool value)
        => new YesNo(value);
}
