using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;


namespace ContosoUniversity.DAL
{
    // https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/connection-resiliency-and-command-interception-with-the-entity-framework-in-an-asp-net-mvc-application
    // Way to go Azure!!
    // I am just going to live this here. Way to go Azure
    // Enable connection resiliency
    // When you deploy the application to Windows Azure, you'll deploy the database to Windows Azure SQL Database, a cloud database service. Transient connection errors are typically more frequent when you connect to a cloud database service than when your web server and your database server are directly connected together in the same data center. Even if a cloud web server and a cloud database service are hosted in the same data center, there are more network connections between them that can have problems, such as load balancers.1
    // Also a cloud service is typically shared by other users, which means its responsiveness can be affected by them.And your access to the database might be subject to throttling.Throttling means the database service throws exceptions when you try to access it more frequently than is allowed in your Service Level Agreement(SLA).
    // Many or most connection problems when you're accessing a cloud service are transient, that is, they resolve themselves in a short period of time. So when you try a database operation and get a type of error that is typically transient, you could try the operation again after a short wait, and the operation might be successful. You can provide a much better experience for your users if you handle transient errors by automatically trying again, making most of them invisible to the customer. The connection resiliency feature in Entity Framework 6 automates that process of retrying failed SQL queries.
    /// The connection resiliency feature must be configured appropriately for a particular database service:+
    /// It has to know which exceptions are likely to be transient.You want to retry errors caused by a temporary loss in network connectivity, not errors caused by program bugs, for example.
    // It has to wait an appropriate amount of time between retries of a failed operation.You can wait longer between retries for a batch process than you can for an online web page where a user is waiting for a response.
    /// It has to retry an appropriate number of times before it gives up. You might want to retry more times in a batch process that you would in an online application.
    ///You can configure these settings manually for any database environment supported by an Entity Framework provider, but default values that typically work well for an online application that uses Windows Azure SQL Database have already been configured for you
    public class SchoolDBConfiguration : DbConfiguration
    {
        public SchoolDBConfiguration()
        {
            // This class sets up Entity Framework to use the SQL Azure execution strategy - to automatically retry failed database operations
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            DbInterception.Add(new SchoolInterceptorLogging());
        }
    }
}