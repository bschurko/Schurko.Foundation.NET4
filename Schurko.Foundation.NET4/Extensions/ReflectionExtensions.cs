using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class ReflectionExtensions
    {
        private const string NullAsString = "<null />";
        private static readonly object TypeDefLock = new object();
        private static readonly Dictionary<Type, FieldInfo[]> TypeDefinitions = new Dictionary<Type, FieldInfo[]>();

        public static T GetAttribute<T>(this MemberInfo property) where T : Attribute => GetCustomAttribute(typeof(T), property) as T;

        private static object GetCustomAttribute(Type attributeType, MemberInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(true);
            for (int index = 0; index < customAttributes.Length; ++index)
            {
                if (customAttributes[index].GetType() == attributeType || customAttributes[index].GetType().GetInterface(attributeType.Name) != null)
                    return customAttributes[index];
            }
            return null;
        }

        public static string ReflectToString(this object target) => target.ReflectToString(0);

        public static string ReflectToString(this object target, int recursion) => target.ReflectToString(recursion, ", ");

        public static string ReflectToString(this object target, int recursion, string delimiter)
        {
            if (target == null)
                return "<null />";
            Type type = target.GetType();
            if (type == typeof(string))
                return (string)target;
            MethodInfo method = type.GetMethod("ToString", Type.EmptyTypes);
            if (method.DeclaringType != typeof(object) && method.GetAttribute<ReflectToStringIgnoreAttribute>() == null)
                return target.ToString();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(type.FullName);
            stringBuilder.Append("[");
            int length = stringBuilder.Length;
            if (target is IEnumerable)
            {
                foreach (object target1 in (IEnumerable)target)
                    stringBuilder.Append(target1.ReflectToString());
            }
            else
            {
                foreach (FieldInfo fieldInfo in GetFieldInfo(type))
                {
                    if (stringBuilder.Length > length)
                        stringBuilder.Append(delimiter);
                    stringBuilder.Append(fieldInfo.Name);
                    stringBuilder.Append("=");
                    object target2 = fieldInfo.GetValue(target);
                    if (target2 == null)
                        stringBuilder.Append("<null />");
                    else if (recursion > 0)
                        stringBuilder.Append(target2.ReflectToString(recursion - 1, delimiter));
                    else
                        stringBuilder.Append(target2.ToString());
                }
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        private static IEnumerable<FieldInfo> GetFieldInfo(Type objectType)
        {
            lock (TypeDefLock)
            {
                FieldInfo[] fieldInfo;
                if (TypeDefinitions.TryGetValue(objectType, out fieldInfo))
                    return fieldInfo;
            }
            FieldInfo[] array = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(fi => fi.GetAttribute<ReflectToStringIgnoreAttribute>() == null).ToArray();
            lock (TypeDefLock)
            {
                if (!TypeDefinitions.ContainsKey(objectType))
                    TypeDefinitions.Add(objectType, array);
            }
            return array;
        }
    }
}
