/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public readonly record struct  ListResult<TRecord>(IEnumerable<TRecord> Items, int TotalCount);
