﻿namespace Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
public interface ISessionService
{
    void Clear();
    void Set(string key, object value);
    void Remove(string key);
    T? Get<T>(string key);
    bool Exists(string key);
}
