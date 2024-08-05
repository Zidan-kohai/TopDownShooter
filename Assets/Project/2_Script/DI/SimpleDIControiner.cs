using System;
using System.Collections.Generic;

public class SimpleDIContainer
{
    private SimpleDIContainer parentContainer;
    private Dictionary<(string, Type), SimpleDIRegistration> registration;
    private HashSet<(string, Type)> resolutuons;

    public SimpleDIContainer(SimpleDIContainer parentContainer = null)
    {
        this.parentContainer = parentContainer;

    }

    public void RegisterAsSingleton<T>(Func<SimpleDIContainer, T> factory)
    {
        RegisterSingleton(null, factory);
    }

    public void RegisterSingleton<T>(string tag, Func<SimpleDIContainer, T> factory)
    {
        (string, Type) key = (tag, typeof(T));

        Register<T>(key, factory, true);
    }

    public void RegisterTransient<T>(Func<SimpleDIContainer, T> factory)
    {
        RegisterTransient(null, factory);
    }

    public void RegisterTransient<T>(string tag, Func<SimpleDIContainer, T> factory)
    {
        (string, Type) key = (tag, typeof(T));

        Register<T>(key, factory, false);
    }

    public void RegisterInstance<T>(T instance)
    {
        RegisterInstance(null, instance);
    }

    public void RegisterInstance<T>(string tag, T instance)
    {
        (string, Type) key = (tag, typeof(T));

        if (registration.ContainsKey(key))
        {
            throw new Exception($"SimpleDI: Factory with tag {key.Item1} and type {key.Item2} already exist");
        }


        registration[key] = new SimpleDIRegistration()
        {
            Instance = instance,
            IsSingleton = true
        };
    }

    public T Resolve<T>(string tag = null)
    {
        (string, Type) key = (tag, typeof(T));

        if (resolutuons.Contains(key))
        {
            throw new Exception($"Cycle dependency for tag {key.Item1} and type {key.Item2.FullName}");
        }

        resolutuons.Add(key);

        try
        {
            if (registration.TryGetValue(key, out SimpleDIRegistration register))
            {
                if (register.IsSingleton)
                {
                    if (register.Instance == null)
                    {
                        register.Instance = register.Factory(this);
                    }

                    return (T)register.Instance;
                }

                return (T)register.Factory(this);
            }

            if (parentContainer != null)
            {
                parentContainer.Resolve<T>(tag);
            }
        }
        finally
        {
            resolutuons.Clear();
        }


        throw new Exception($"Coudn`t find dependency for tag {key.Item1} and type {key.Item2.FullName}");

    }
    private void Register<T>((string, Type) key, Func<SimpleDIContainer, T> factory, bool isSingleton)
    {
        if (registration.ContainsKey(key))
        {
            throw new Exception($"SimpleDI: Factory with tag {key.Item1} and type {key.Item2} already exist");
        }


        registration[key] = new SimpleDIRegistration()
        {
            Factory = c => factory(c),
            IsSingleton = isSingleton
        };
    }
}