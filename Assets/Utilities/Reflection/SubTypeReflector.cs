using System;
using System.Collections.Generic;

public static class SubTypeReflector
{
    public static List<Type> GetSubTypes<T>() where T : class
    {
        var types = new List<Type>();
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName.StartsWith("Mono.Cecil"))
                continue;

            if (assembly.FullName.StartsWith("Boo.Lan"))
                continue;

            if (assembly.FullName.StartsWith("System"))
                continue;

            if (assembly.FullName.StartsWith("I18N"))
                continue;

            if (assembly.FullName.StartsWith("UnityEngine"))
                continue;

			if (assembly.FullName.StartsWith("UnityEditor"))
			    continue;

            if (assembly.FullName.StartsWith("mscorlib"))
                continue;
			
			if(assembly.FullName.StartsWith("Unity.PackageManager"))
				continue;
			
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (type.IsAbstract)
                    continue;
				
                if(!type.IsSubclassOf(typeof(T)))
                    continue;

                types.Add(type);
            }
        }

        return types;
    }
}
