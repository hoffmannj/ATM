using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ATM
{
    internal class ParameterExtractor
    {
        private const BindingFlags FlagsForMembers = BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding;
        private object theObj;

        public ParameterExtractor(object parameterObj)
        {
            theObj = parameterObj;
        }

        public Dictionary<string, IGeneratorFunc> GetParameters()
        {
            return ObjectToDictionary(theObj);
        }

        private static List<MemberInfo> GetTypeMembers(Type type)
        {
            return type.GetMembers(FlagsForMembers).Where(IsMemberFieldOrProperty).ToList();
        }

        private static Type GetMemberType(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)member).FieldType;
            }
            else
            {
                return ((PropertyInfo)member).PropertyType;
            }
        }

        private static void HandleMember(Dictionary<string, IGeneratorFunc> dict, MemberInfo member, object obj)
        {
            if (dict.ContainsKey(member.Name)) return;

            var mType = GetMemberType(member);

            var getExp = Expression.Lambda(Expression.PropertyOrField(Expression.Constant(obj), member.Name)).Compile();
            var lExp = Expression.Lambda(Expression.Convert(Expression.Constant(getExp.DynamicInvoke()), mType));

            dict.Add(member.Name, GeneratorFuncFactory.CreateFromFunc(mType, lExp));
        }

        private static bool IsMemberFieldOrProperty(MemberInfo member)
        {
            return member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property;
        }

        public static Dictionary<string, IGeneratorFunc> ObjectToDictionary(object obj)
        {
            var result = new Dictionary<string, IGeneratorFunc>();
            var pmembers = GetTypeMembers(obj.GetType());
            pmembers.ForEach(member => HandleMember(result, member, obj));
            return result;
        }
    }
}
