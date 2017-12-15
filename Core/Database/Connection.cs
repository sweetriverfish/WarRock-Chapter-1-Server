using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Database
{
    class Connection : IDisposable
    {
        public uint PoolId { get; private set; }
        public bool Bussy { get; private set; }

        private DatabasePool databasePool = null;

        public Connection(uint poolId, DatabasePool pool)
        {
            if (pool == null)
                throw new Exception("The database pool can't be null");

            PoolId = poolId;
            databasePool = pool;
            Bussy = false;
        }

        public void Dispose()
        {
            // Return to the pool

            Bussy = false;
        }

        public void Destory()
        {

        }
    }
}
