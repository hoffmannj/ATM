using System;

namespace ATM
{
    public interface IATMap
    {
        IATMOptions To(object instance);
        IATMOptions To(Func<object> func);
        IATMOptions To(Type type);
    }

    public interface IATMap<T> where T : class
    {
        IATMOptions To(T instance);
        IATMOptions To(Func<T> func);
        IATMOptions To<U>() where U : T;
    }
}
