using System;

namespace Nox
{
    public interface IQueryComposer
    {
        string ComposeSelect(Type entityType);
        string ComposeInsert(Type entityType);
        string ComposeInsert(Type entityType, string primaryKeyName);
        string ComposeDelete(Type entityType, string primaryKeyName);
        string ComposeUpdate(Type entityType, string primaryKeyName);
    }
}