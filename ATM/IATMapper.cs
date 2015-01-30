using System;

namespace ATM
{
    public interface IATMapper
    {
        IATMap Map(Type type);
        IATMap<T> Map<T>() where T : class;
    }
}
