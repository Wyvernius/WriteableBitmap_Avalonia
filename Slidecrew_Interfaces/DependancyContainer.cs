using System;
using System.Collections.Generic;
using System.Text;

namespace Slidecrew_Interfaces
{
    public static class DependancyContainer
    {
        private static Dictionary<Type, Type> _dependancy = new Dictionary<Type, Type>();

        public static void Register<T>(Type Interface)  where T : new()
        {
            if (!_dependancy.ContainsKey(Interface))
                _dependancy.Add(Interface, typeof(T));
        }

        public static T Get<T>()
        {
            if (_dependancy.ContainsKey(typeof(T)))
                return (T) Activator.CreateInstance(_dependancy[typeof(T)]);

            throw new Exception("Interface not registered!");
        }
    }
}
