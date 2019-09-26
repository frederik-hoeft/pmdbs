using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Manages database access in a multithreaded environment.
    /// </summary>
    public class DataBaseThreadWatcher: IDisposable
    {
        /// <summary>
        /// Waits until the database is accessable.
        /// </summary>
        public async Task Wait()
        {
            while (GlobalVarPool.databaseIsInUse)
            {
                await Task.Delay(100);
            }
            GlobalVarPool.databaseIsInUse = true;
        }
        /// <summary>
        /// Clears the access to allow other threads to access the database.
        /// </summary>
        public void Dispose()
        {
            GlobalVarPool.databaseIsInUse = false;
        }
    }
}
