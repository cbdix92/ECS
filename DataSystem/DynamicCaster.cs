using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CMDR.DataSystem
{
    internal static class DynamicCaster
    {
        internal static Dictionary<Type, ArrayList> CachedTypeCasterMethods;

        static DynamicCaster()
        {
            CachedTypeCasterMethods = new Dictionary<Type, ArrayList>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.GetInterfaces().Contains(typeof(IComponent))).ToArray();

            foreach (Type tComp in types)
            {
                if (tComp.Name == typeof(IComponent<>).Name)
                    continue;

                CachedTypeCasterMethods.Add(tComp, new ArrayList());
            }

            CacheTypeCaster<Transform>();
        }

        /*
        static void CacheTypeCaster<T>()
        {
            Type tComp = typeof(T);

            MethodInfo genericCasterMethod = typeof(DynamicCaster).GetMethod("GenericCaster", BindingFlags.Static | BindingFlags.NonPublic);

            MethodInfo genericCasterConverted = genericCasterMethod.MakeGenericMethod(typeof(IComponent), tComp);

            CachedTypeCasterMethods[tComp].Add(genericCasterConverted.Invoke(null, new object[] { tComp }));
        }

        static Func<TInterface, TComponent> GenericCaster<TInterface, TComponent>(Type type)
        {
            ParameterExpression inputObject = Expression.Parameter(typeof(TInterface));

            return Expression.Lambda<Func<TInterface, TComponent>>(Expression.Convert(inputObject, type), inputObject).Compile();
        }
        */

        static void CacheTypeCaster<TComp>()
        {
            MethodInfo casterMethod = typeof(DynamicCaster).GetMethod("CastHelper", BindingFlags.Static | BindingFlags.NonPublic);

            MethodInfo casterMethodConverted = casterMethod.MakeGenericMethod(typeof(TComp));

            CachedTypeCasterMethods[typeof(TComp)].Add(Delegate.CreateDelegate(typeof(Func<IComponent, TComp>), casterMethodConverted));
        }

        static TComp CastHelper<TComp>(IComponent component)
        {
            return (TComp)component;
        }
    }
}
