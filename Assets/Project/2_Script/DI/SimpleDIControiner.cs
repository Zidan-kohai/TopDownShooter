using System;
using System.Collections.Generic;

public class SimpleDIContainer
{
    private SimpleDIContainer parentContainer;
    private Dictionary<(string, Type), SimpleDIRegistration> registration;


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
