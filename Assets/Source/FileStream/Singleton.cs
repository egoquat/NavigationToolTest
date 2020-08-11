using System;
using System.Reflection;

public static class Singleton<T> where T : class
{
    static volatile T _instance;
    static object _lock = new object();

    static Singleton()
    {
    }

    public static T GetInstance
    {
        get
        {
            if (_instance == null)
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        ConstructorInfo constructor = null;

                        try
                        {
                            // Binding flags exclude public constructors.
                            constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
                        }
                        catch (Exception /*exception*/)
                        {
                            // throw new SingletonException(exception);
                            return null;
                        }

                        if (constructor == null || constructor.IsAssembly)
                        {
                            // Also exclude internal constructors.
                            //throw new SingletonException(string.Format("A private or " + "protected constructor is missing for '{0}'.", typeof(T).Name));
                            return null;
                        }

                        _instance = (T)constructor.Invoke(null);
                    }
                }

            return _instance;
        }
    }
}
