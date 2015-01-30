using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ATM
{
    public class ATMMain : IATM, IATMapper
    {
        private Dictionary<IntPtr, List<IATMFactory>> typeMaps;

        private ATMMain()
        {
            typeMaps = new Dictionary<IntPtr, List<IATMFactory>>();
        }

        public static IATM Create()
        {
            return new ATMMain();
        }

        object IATM.Get(Type type)
        {
            var maps = GetMapsForType(type);
            if (maps == null) return null;
            AssertOnlyOneMap(maps);
            return maps[0].Get();
        }

        T IATM.Get<T>()
        {
            var type = typeof(T);
            var maps = GetMapsForType(type);
            if (maps == null) return null;
            AssertOnlyOneMap(maps);
            return (maps[0] as IATMFactory<T>).Get();
        }

        object[] IATM.GetAll(Type type)
        {
            var maps = GetMapsForType(type);
            if (maps == null) return null;
            return maps.Select(m => m.Get()).ToArray();
        }

        T[] IATM.GetAll<T>()
        {
            var type = typeof(T);
            var maps = GetMapsForType(type);
            if (maps == null) return null;
            return maps.Select(m => (m as IATMFactory<T>).Get()).ToArray();
        }

        IATMap IATMapper.Map(Type type)
        {
            return AddMapping(type, GetATMapTypeForType(type));
        }

        IATMap<T> IATMapper.Map<T>()
        {
            return AddMapping<T>(GetATMapTypeForType(typeof(T)));
        }

        void IATM.Register(Action<IATMapper> mapperAction)
        {
            mapperAction(this);
            Validate();
        }



        internal void Validate()
        {
            var maps = GetAllMaps();
            foreach (IATMValidate map in maps)
            {
                if (map.Validated) continue;
                if(!map.Validate(new List<IATMap>()))
                {
                    throw new CantResolveDependenciesException("Can't resolve dependencies");
                }
            }
        }

        internal List<IATMFactory> GetMapsForType(Type type)
        {
            var th = type.TypeHandle.Value;
            List<IATMFactory> maps;
            if (typeMaps.TryGetValue(th, out maps))
            {
                return maps;
            }
            if (type.IsPointer) return null;
            if (!type.IsClass && !type.IsInterface) return null;
            ((IATMapper)this).Map(type).To(type);
            return typeMaps[th];
        }

        internal void AssertOnlyOneMap(List<IATMFactory> maps)
        {
            if (maps.Count != 1)
            {
                throw new WrongNumberOfMappingsException("Wrong number of mappings for the type");
            }
        }

        private List<IATMFactory> GetAllMaps()
        {
            return typeMaps.SelectMany(tms => tms.Value.Where(tm => !((IATMValidate)tm).Validated)).ToList();
        }

        private IATMap AddMapping(Type type, IATMap map)
        {
            if (map == null) return null;
            var th = type.TypeHandle.Value;
            if (!typeMaps.ContainsKey(th)) typeMaps.Add(th, new List<IATMFactory>());
            typeMaps[th].Add(map as IATMFactory);
            return map;
        }

        private IATMap<T> AddMapping<T>(IATMap map) where T : class
        {
            return (IATMap<T>)AddMapping(typeof(T), map);
        }

        private IATMap GetATMapTypeForType(Type type)
        {
            if (type.IsPointer) return null;
            if (!type.IsClass && !type.IsInterface) return null;
            var t = typeof(ATMap<>).MakeGenericType(type);
            var constructor = t.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(ATMMain) }, null);
            NewExpression newExp = Expression.New(constructor, new[] { Expression.Constant(this) });
            return Expression.Lambda<Func<IATMap>>(newExp).Compile()();
        }
    }
}
