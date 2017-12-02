using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoUniversity;
using ContosoUniversity.DAL;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Reflection;
using ContosoUniversity.Logging;
using System.Data.Common;

namespace ContosoUniversity.Tests.DAL
{
    class SchoolInterceptorTransientErrorsTest : DbCommandInterceptor
    {
        private int _counter = 0;
        private ILogger _logger = new Logger();

        /// <summary>
        /// overrides the ReaderExecuting method, which is called for queries that can return multiple rows of data.
        /// The value you enter in the Search box will be in command.Parameters[0] and command.Parameters[1] (one is used for the first name and one for the last name). When the value "%Throw%" is found, "Throw" is replaced in those parameters by "an" so that some students will be found and returned.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            bool throwTransientErrors = false;
            if (command.Parameters.Count > 0 && command.Parameters[0].Value.ToString() == "%Throw%")
            {
                throwTransientErrors = true;
                command.Parameters[0].Value = "%an%";
                command.Parameters[1].Value = "%an%";
            }

            if (throwTransientErrors && _counter < 4)
            {
                _logger.Information("Returning transient error for command: {0}", command.CommandText);
                _counter++;
                interceptionContext.Exception = CreateDummySqlException();
            }
        }


        /// <summary>
        ///  Other error numbers currently recognized as transient are 64, 233, 10053, 10054, 10060, 10928, 10929, 40197, 40501, and 40613, but these are subject to change in new versions of SQL Database.
        ///  The code returns the exception to Entity Framework instead of running the query and passing back query results. The transient exception is returned four times, and then the code reverts to the normal procedure of passing the query to the database.
       /// Because everything is logged, you'll be able to see that Entity Framework tries to execute the query four times before finally succeeding, and the only difference in the application is that it takes longer to render a page with query results.
        /// </summary>
        /// <returns></returns>
        private SqlException CreateDummySqlException()
        {
            // The instance of SQL Server you attempted to connect to does not support encryption
            int sqlErrorNumber = 20;

            var sqlErrorCtor = typeof(SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 7).Single();
            var sqlError = sqlErrorCtor.Invoke(new object[] { sqlErrorNumber, (byte)0, (byte)0, "", "", "", 1 });

            var errorCollection = Activator.CreateInstance(typeof(SqlErrorCollection), true);
            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
            addMethod.Invoke(errorCollection, new[] { sqlError });

            var sqlExceptionCtor = typeof(SqlException).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 4).Single();
            var sqlException = (SqlException)sqlExceptionCtor.Invoke(new object[] { "Dummy", errorCollection, null, Guid.NewGuid() });

            return sqlException;
        }
    }
}
