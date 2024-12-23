/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class DataSetPropertyColumn<TGridItem> : TemplateColumn<TGridItem>
{
    [Parameter, EditorRequired] public string DataSetName { get; set; } = string.Empty;
}