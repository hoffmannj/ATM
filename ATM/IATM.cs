using System;

namespace ATM
{
    public interface IATM
    {
        void Register(Action<IATMapper> mapperAction);
        object Get(Type type);
        T Get<T>() where T : class;

        object[] GetAll(Type type);
        T[] GetAll<T>() where T : class;
    }
}
