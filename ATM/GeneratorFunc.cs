using System;
using System.Linq.Expressions;

namespace ATM
{
    internal interface IGeneratorFunc
    {
        Expression GetCallExpression();
    }

    internal class GeneratorFuncFactory
    {
        private static readonly Type GeneratorClassType = typeof(GeneratorFunc<>);

        public static GeneratorFunc<T> CreateFromFuncGeneric<T>(Expression<Func<T>> func)
        {
            return new GeneratorFunc<T>().Set(func);
        }

        public static IGeneratorFunc CreateFromFunc(Type targetType, LambdaExpression func)
        {
            var genF = TypeHelpers.GetGenericInstance(GeneratorClassType, targetType);
            TypeHelpers.CallMethod(genF, "Set", func);
            return genF as IGeneratorFunc;
        }
    }

    internal class GeneratorFunc<T> : IGeneratorFunc
    {
        private Func<T> TheFunc;
        private Expression callExpression;
        public Type TargetType;

        public GeneratorFunc<T> Set(Expression<Func<T>> func)
        {
            TheFunc = func.Compile();
            TargetType = typeof(T);
            callExpression = Expression.Invoke(func);
            return this;
        }

        public Expression GetCallExpression()
        {
            return callExpression;
        }

        public T GetCallResult()
        {
            return TheFunc();
        }

    }
}
