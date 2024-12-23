﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Antimony.Core;

/// <summary>
/// The purpose of the `IKeyProvider` interface is to provide services to handle strongly typed ID keys within the data pipeline
/// </summary>
public interface IRecordIdProvider<TKey>
{
    public bool IsDefault(TKey key);
    public TKey GetKey(object key);
    public object GetValueObject(TKey key);
    public TKey GetNew();
}