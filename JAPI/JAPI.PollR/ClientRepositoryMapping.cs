using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.PollR
{
    class ClientRepositoryMapping<T>
    {
        //no groups can be used for this.
        private readonly Dictionary<string, T> _repositories = new Dictionary<string, T>();

        public void Add(string key, T repository)
        {
            lock (_repositories)
            {
                if (!_repositories.ContainsKey(key))
                    _repositories.Add(key, repository);
            }
        }

        public T GetClientRepository(string key)
        {
            if (_repositories.ContainsKey(key))
                return _repositories[key];
            return default(T);
        }

        public bool HasMapping(string key) 
            => (_repositories.ContainsKey(key));

        public void Remove(string key)
        {
            lock (_repositories)
            {
                if (!_repositories.ContainsKey(key))
                    return;

                _repositories.Remove(key);
            }
        }
    }
}
