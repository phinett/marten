using System;

namespace Marten.Services
{
    public interface IDeletion : IStorageOperation
    {
        Type DocumentType { get; }
    }
}