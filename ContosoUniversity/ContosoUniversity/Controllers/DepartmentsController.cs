using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// A web server has a limited number of threads available, and in high load situations all of the available threads might be in use. When that happens, the server can't process new requests until the threads are freed up. With synchronous code, many threads may be tied up while they aren't actually doing any work because they're waiting for I/O to complete. With asynchronous code, when a process is waiting for I/O to complete, its thread is freed up for the server to use for processing other requests. As a result, asynchronous code enables server resources to be use more efficiently, and the server is enabled to handle more traffic without delays.
    /// In earlier versions of.NET, writing and testing asynchronous code was complex, error prone, and hard to debug. In.NET 4.5, writing, testing, and debugging asynchronous code is so much easier that you should generally write asynchronous code unless you have a reason not to.Asynchronous code does introduce a small amount of overhead, but for low traffic situations the performance hit is negligible, while for high traffic situations, the potential performance improvement is substantial.
    /// 
    /// Some things to be aware of when you are using asynchronous programming with the Entity Framework:
    // The async code is not thread safe.In other words, in other words, don't try to do multiple operations in parallel using the same context instance.
/// If you want to take advantage of the performance benefits of async code, make sure that any library packages that you're using (such as for paging), also use async if they call any Entity Framework methods that cause queries to be sent to the database.
    /// </summary>
    public class DepartmentsController : Controller
    {
        private SchoolDBContext db = new SchoolDBContext();

        // GET: Departments
        /// <summary>
        /// Four changes were applied to enbale the Entity Framework query to execute asynchronously:
        /// 1. The method is marked with the async keyword, which tells the compiler to generated callbacks for parts of the method body and to automatically create the Task<ActionResult></ActionResult>
        /// 2. The Task<ActionResult> represents the ongoing task of type T
        /// 3. The await keyword was applied to the web service call. When the compiler sees this keyword, behind the scenes it splits the method into two parts. The first part ends with the operation that is started asynchronously. The second part is put into a callback method that is called when the operation completes.
        /// 4. The asynchronous version of the ToList extension method was called.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            // Why is the departments.ToList statement modified but not the departments = db.Departments statement? The reason is that only statements that cause queries or commands to be sent to the database are executed asynchronously. The departments = db.Departments statement sets up a query but the query is not executed until the ToList method is called. Therefore, only the ToList method is executed asynchronously.
            var departments = db.Departments.Include(d => d.Administrator);
            return View(await departments.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // In the Details method and the HttpGet Edit and Delete methods, the Find method is the one that causes a query to be sent to the database, so that's the method that gets executed asynchronously:
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentID,Name,Budget,StartDate,InstructorID")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                // In the Create, HttpPost Edit, and DeleteConfirmed methods, it is the SaveChanges method call that causes a command to be executed, not statements such as db.Departments.Add(department) which only cause entities in memory to be modified.
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application#modify-the-department-controller
        /// <summary>
        /// If the FindAsync method returns null, the department was deleted by another user. The code shown uses the posted form values to create a department entity so that the Edit page can be redisplayed with an error message. As an alternative, you wouldn't have to re-create the department entity if you display only an error message without redisplaying the department fields.
       /// The view stores the original RowVersion value in a hidden field, and the method receives it in the rowVersion parameter.Before you call SaveChanges, you have to put that original RowVersion property value in the OriginalValues collection for the entity.Then when the Entity Framework creates a SQL UPDATE command, that command will include a WHERE clause that looks for a row that has the original RowVersion value.+
    /// If no rows are affected by the UPDATE command (no rows have the original RowVersion value), the Entity Framework throws a DbUpdateConcurrencyException exception, and the code in the catch block gets the affected Department entity from the exception object.
    /// 
    /// update edit view
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rowVersion"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Name", "Budget", "StartDate", "InstructorID", "RowVersion" };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var departmentToUpdate = await db.Departments.FindAsync(id);
            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                TryUpdateModel(deletedDepartment, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }

            if (TryUpdateModel(departmentToUpdate, fieldsToBind))
            {
                try
                {
                    db.Entry(departmentToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                // If no rows are affected by the UPDATE command (no rows have the original RowVersion value), the Entity Framework throws a DbUpdateConcurrencyException exception, and the code in the catch block gets the affected Department entity from the exception object.
                catch (DbUpdateConcurrencyException ex)
                {
                    // This object has the new values entered by the user in its Entity property, and you can get the values read from the database by calling the GetDatabaseValues method.
                    var entry = ex.Entries.Single();
                    var clientValues = (Department)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();

                    /// The GetDatabaseValues method returns null if someone has deleted the row from the database; otherwise, you have to cast the returned object to the Department class in order to access the Department properties. (Because you already checked for deletion, databaseEntry would be null only if the department was deleted after FindAsync executes and before SaveChanges executes.)
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    // Next, the code adds a custom error message for each column that has database values different from what the user entered on the Edit page:
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Current value: "
                                + databaseValues.Name);
                        if (databaseValues.Budget != clientValues.Budget)
                            ModelState.AddModelError("Budget", "Current value: "
                                + String.Format("{0:c}", databaseValues.Budget));
                        if (databaseValues.StartDate != clientValues.StartDate)
                            ModelState.AddModelError("StartDate", "Current value: "
                                + String.Format("{0:d}", databaseValues.StartDate));
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                            ModelState.AddModelError("InstructorID", "Current value: "
                                + db.Instructors.Find(databaseValues.InstructorID).FullName);
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");

                        // Finally, the code sets the RowVersion value of the Department object to the new value retrieved from the database. This new RowVersion value will be stored in the hidden field when the Edit page is redisplayed, and the next time the user clicks Save, only concurrency errors that happen since the redisplay of the Edit page will be caught.
                        departmentToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException ex)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        // GET: Departments/Delete/5
        /// <summary>
        /// For the Delete page, the Entity Framework detects concurrency conflicts caused by someone else editing the department in a similar manner. When the HttpGet Delete method displays the confirmation view, the view includes the original RowVersion value in a hidden field. That value is then available to the HttpPost Delete method that's called when the user confirms the deletion. When the Entity Framework creates the SQL DELETE command, it includes a WHERE clause with the original RowVersion value. If the command results in zero rows affected (meaning the row was changed after the Delete confirmation page was displayed), a concurrency exception is thrown, and the HttpGet Delete method is called with an error flag set to true in order to redisplay the confirmation page with an error message. It's also possible that zero rows were affected because the row was deleted by another user, so in that case a different error message is displayed.
        /// 
        /// The method accepts an optional parameter that indicates whether the page is being redisplayed after a concurrency error. If this flag is true, an error message is sent to the view using a ViewBag property.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="concurrencyError"></param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        /// <summary>
        /// In the scaffolded code that you just replaced, this method accepted only a record ID:
        /// You've changed this parameter to a Department entity instance created by the model binder. This gives you access to the RowVersion property value in addition to the record key.
        /// 
        /// You have also changed the action method name from DeleteConfirmed to Delete. The scaffolded code named the HttpPost Delete method DeleteConfirmed to give the HttpPost method a unique signature. ( The CLR requires overloaded methods to have different method parameters.) Now that the signatures are unique, you can stick with the MVC convention and use the same name for the HttpPost and HttpGet delete methods.+
        /// If a concurrency error is caught, the code redisplays the Delete confirmation page and provides a flag that indicates it should display a concurrency error message.
        /// 
        /// update delete view
                /// </summary>
                /// <param name="department"></param>
                /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Department department)
        {
            try
            {
                db.Entry(department).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentID });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
