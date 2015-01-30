using System;
using System.Linq.Expressions;

namespace ATM
{
    public static class TypeHelpers
    {
        public static dynamic CallMethod(object instance, string methodName, params Expression[] parameters)
        {
            return Expression.Lambda<Func<dynamic>>(Expression.Call(Expression.Constant(instance), methodName, null, parameters)).Compile()();
        }

        public static dynamic GetGenericInstance(Type baseType, params Type[] genericTypes)
        {
            var t = baseType.MakeGenericType(genericTypes);
            return Expression.Lambda<Func<dynamic>>(Expression.New(t)).Compile()();
        }

    }
}
