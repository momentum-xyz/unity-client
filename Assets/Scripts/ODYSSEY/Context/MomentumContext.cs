using Odyssey.Networking;
using System.Collections.Generic;
using System;

namespace Odyssey
{
    public interface IMomentumContext
    {
        public T Get<T>();
        public void RegisterService<T>(T service, bool overwrite = false);
    }

    public class MomentumContext : IMomentumContext
    {
        Dictionary<Type, object> services = new Dictionary<Type, object>();

        public MomentumContext()
        {

        }

        public T Get<T>()
        {
            if (!services.ContainsKey(typeof(T)))
            {
                throw new Exception("Service not registered: " + typeof(T));
            }

            object service = services[typeof(T)];

            return (T)service;
        }

        public void RegisterService<T>(T service, bool overwrite = false)
        {
            if (services.ContainsKey(typeof(T)) && !overwrite)
            {
                Logging.Log("[Context] Service already registered: " + typeof(T));
                return;
            }

            if (service is IRequiresContext)
                ((IRequiresContext)service).Init(this);

            if (overwrite)
            {
                services[typeof(T)] = service;
            }
            else
            {
                services.Add(typeof(T), service);
            }
        }
    }
}
