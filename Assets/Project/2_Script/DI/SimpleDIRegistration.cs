using System;

public class SimpleDIRegistration
{
    public Func<SimpleDIContainer, object> Factory { get; set; }
    public bool IsSingleton { get; set; }
    public object Instance { get; set; }
}