using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ATM
{
    public class ATMap<T> : IATMap, IATMap<T>, IATMOptions, IATMFactory, IATMFactory<T>, IATMValidate where T : class
    {
        private ATMMain mapper;
        private bool validated;
        private Type baseType;
        private Type targetType;
        private bool isSingleton;
        private T singletonInstance;
        private Dictionary<string, IGeneratorFunc> constructorParameters;
        private GeneratorFunc<T> generator;
        private IATMFactory<T> thisFactory;
        private IATMValidate thisValidate;

        private Func<T> getFunction;

        internal ATMap(ATMMain mapper)
        {
            getFunction = GetWithInitialization;
            thisFactory = this as IATMFactory<T>;
            thisValidate = this as IATMValidate;
            this.mapper = mapper;
            this.baseType = typeof(T);
            constructorParameters = new Dictionary<string, IGeneratorFunc>();
        }

        bool IATMValidate.Validated
        {
            get
            {
                return validated;
            }
        }

        IATMOptions IATMOptions.AsSingleton()
        {
            isSingleton = true;
            return this;
        }

        T IATMFactory<T>.Get()
        {
            return getFunction();
        }

        object IATMFactory.Get()
        {
            return thisFactory.Get();
        }

        IATMOptions IATMap.To(Type type)
        {
            targetType = type;
            return this;
        }

        IATMOptions IATMap<T>.To(Func<T> func)
        {
            generator = GeneratorFuncFactory.CreateFromFuncGeneric<T>(() => func());
            validated = true;
            return this;
        }

        IATMOptions IATMap<T>.To(T instance)
        {
            generator = GeneratorFuncFactory.CreateFromFuncGeneric<T>(() => instance);
            validated = true;
            return this;
        }

        IATMOptions IATMap.To(Func<object> func)
        {
            generator = GeneratorFuncFactory.CreateFromFuncGeneric<T>(() => (T)func());
            validated = true;
            return this;
        }

        IATMOptions IATMap.To(object instance)
        {
            generator = GeneratorFuncFactory.CreateFromFuncGeneric<T>(() => (T)instance);
            validated = true;
            return this;
        }

        IATMOptions IATMap<T>.To<U>()
        {
            targetType = typeof(U);
            return this;
        }

        bool IATMValidate.Validate(List<IATMap> chain)
        {
            if (validated) return true;
            if (chain.Contains(this)) throw new RecursiveDependenciesException("Recursive dependency found");
            chain.Add(this);
            return ValidateConstructors(chain);
        }

        IATMOptions IATMOptions.WithParameters(object paramObj)
        {
            constructorParameters = new ParameterExtractor(paramObj).GetParameters();
            return this;
        }




        private bool ValidateConstructors(List<IATMap> chain)
        {
            var constructors = targetType.GetConstructors();
            foreach (var constructor in constructors)
            {
                if (ValidateConstructor(constructor, chain))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ValidateConstructor(ConstructorInfo constructor, List<IATMap> chain)
        {
            var parameters = constructor.GetParameters();
            if (HasBadParameter(parameters)) return false;
            var parameterGenerators = ValidateParameters(parameters, chain);
            if (parameterGenerators == null) return false;
            NewExpression innerExpr;
            if (parameterGenerators.Length == 0)
            {
                innerExpr = Expression.New(constructor);
            }
            else
            {
                innerExpr = Expression.New(constructor, parameterGenerators.Select(p => p.GetCallExpression()).ToArray());
            }
            generator = GeneratorFuncFactory.CreateFromFuncGeneric<T>(Expression.Lambda<Func<T>>(innerExpr));
            validated = true;
            return true;
        }

        private bool HasBadParameter(ParameterInfo[] parameters)
        {
            if (parameters.Any(p => !p.ParameterType.IsClass && !p.ParameterType.IsInterface && !constructorParameters.ContainsKey(p.Name ?? ""))) return true;
            return false;
        }

        private IGeneratorFunc[] ValidateParameters(ParameterInfo[] parameters, List<IATMap> chain)
        {
            var parameterGenerators = new IGeneratorFunc[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (!ValidateParameter(parameters[i], parameterGenerators, i, chain))
                {
                    return null;
                }
            }
            return parameterGenerators;
        }

        private bool ValidateParameter(ParameterInfo parameter, IGeneratorFunc[] parameterGenerators, int index, List<IATMap> chain)
        {
            if (constructorParameters.ContainsKey(parameter.Name))
            {
                parameterGenerators[index] = constructorParameters[parameter.Name];
            }
            else
            {
                IATMFactory firstMap;
                if (!TryGetFirstMapForType(parameter.ParameterType, chain, out firstMap))
                {
                    return false;
                }
                var method = typeof(IATMFactory<>).MakeGenericType(parameter.ParameterType).GetMethod("Get");
                var lExp = Expression.Lambda(Expression.Call(Expression.Constant(firstMap), method));
                parameterGenerators[index] = GeneratorFuncFactory.CreateFromFunc(parameter.ParameterType, lExp);
            }
            return true;
        }

        private bool TryGetFirstMapForType(Type type, List<IATMap> chain, out IATMFactory map)
        {
            map = null;
            var maps = mapper.GetMapsForType(type);
            if (maps == null)
            {
                return false;
            }
            mapper.AssertOnlyOneMap(maps);
            var firstMap = maps[0];
            if (!(firstMap as IATMValidate).Validate(chain))
            {
                return false;
            }
            map = firstMap;
            return true;
        }

        private T GetWithInitialization()
        {
            if (!validated) thisValidate.Validate(new List<IATMap>());
            AssertGenerator();
            AssertSingleton();
            getFunction = GetWithoutInitialization;
            return generator.GetCallResult();
        }

        private void AssertGenerator()
        {
            if (generator == null) throw new CantResolveDependenciesException("Can't resolve dependencies");
        }

        private void AssertSingleton()
        {
            if (!isSingleton || singletonInstance != null) return;
            singletonInstance = generator.GetCallResult();
            generator = GeneratorFuncFactory.CreateFromFuncGeneric<T>(() => singletonInstance);
        }

        private T GetWithoutInitialization()
        {
            return generator.GetCallResult();
        }
    }
}
