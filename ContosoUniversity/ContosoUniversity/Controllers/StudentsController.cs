using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using System.Diagnostics;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private SchoolDBContext db = new SchoolDBContext();

        // GET: Students
        // This code adds a page parameter, a current sort order parameter, and a current filter parameter to the method signature:
        // The first time the page is displayed, or if the user hasn't clicked a paging or sorting link, all the parameters will be null. If a paging link is clicked, the page variable will contain the page number to display.
        // A ViewBag property provides the view with the current sort order, because this must be included in the paging links in order to keep the sort order the same while paging:
        // Another property, ViewBag.CurrentFilter, provides the view with the current filter string. This value must be included in the paging links in order to maintain the filter settings during paging, and it must be restored to the text box when the page is redisplayed. If the search string is changed during paging, the page has to be reset to 1, because the new filter can result in different data to display. The search string is changed when a value is entered in the text box and the submit button is pressed. In that case, the searchString parameter is not null.

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            IQueryable<Student> students = from s in db.Students
                                           select s;

            // For example, the .NET Framework implementation of the Contains method returns all rows when you pass an empty string to it, but the Entity Framework provider for SQL Server Compact 4.0 returns zero rows for empty strings. Therefore the code in the example (putting the Where statement inside an if statement) makes sure that you get the same results for all versions of SQL Server. Also, the .NET Framework implementation of the Contains method performs a case-sensitive comparison by default, but Entity Framework SQL Server providers perform case-insensitive comparisons by default. Therefore, calling the ToUpper method to make the test explicitly case-insensitive ensures that results do not change when you change the code later to use a repository, which will return an IEnumerable collection instead of an IQueryable object. (When you call the Contains method on an IEnumerable collection, you get the .NET Framework implementation; when you call it on an IQueryable object, you get the database provider implementation.)
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            int pageSize = 3;
            // The ToPagedList method takes a page number. The two question marks represent the null-coalescing operator. The null-coalescing operator defines a default value for a nullable type; the expression (page ?? 1) means return the value of page if it has a value, or return 1 if page is null.
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // This code adds the Student entity created by the ASP.NET MVC model binder to the Students entity set and then saves the changes to the database. (Model binder refers to the ASP.NET MVC functionality that makes it easier for you to work with data submitted by a form; a model binder converts posted form values to CLR types and passes them to the action method in parameters. In this case, the model binder instantiates a Student entity for you using property values from the Form collection.)
        // https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application
        // ID from the bind property is removed because it is the primary key value whic SQL server will set automatically whe the row is inserted. Input form the user does not set the ID Value
        // The Bind attribute is one way to protect against over-posting in create scenarios. For example, suppose the Student entity includes a Secret property that you don't want this web page to set.
        // Even if you don't have a Secret field on the web page, a hacker could use a tool such as fiddler, or write some JavaScript, to post a Secret form value. Without the Bind attribute limiting the fields that the model binder uses when it creates a Student instance, the model binder would pick up that Secret form value and use it to create the Student entity instance. 
        // It's a security best practice to use the Include parameter with the Bind attribute to whitelist fields. It's also possible to use the Exclude parameter to blacklist fields you want to exclude. The reason Include is more secure is that when you add a new property to the entity, the new field is not automatically protected by an Exclude list.
        // You can prevent overposting in edit scenarios is by reading the entity from the database first and then calling TryUpdateModel, passing in an explicit allowed properties list. That is the method used in these tutorials.
        // An alternative way to prevent overposting that is preferred by many developers is to use view models rather than entity classes with model binding. Include only the properties you want to update in the view model. Once the MVC model binder has finished, copy the view model properties to the entity instance, optionally using a tool such as AutoMapper. Use db.Entry on the entity instance to set its state to Unchanged, and then set Property("PropertyName").IsModified to true on each entity property that is included in the view model. This method works in both edit and create scenarios.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }


            }
            //  Log to Azure
            // DataException exceptions are sometimes caused by something external to the application rather than a programming error, so the user is advised to try again.
            catch (RetryLimitExceededException ex)
            {
                // TODO: https://docs.microsoft.com/en-us/aspnet/aspnet/overview/developing-apps-with-windows-azure/building-real-world-cloud-apps-with-windows-azure/monitoring-and-telemetry#log
                Trace.TraceError(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if problems persists see you system administrator.");
            }
           
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(student).State = EntityState.Modified;

                    // Read on Entity states and the attach and save changes methods
                    // The database context keeps track of whether entities in memory are in sync with their corresponding rows in the database, and this information determines what happens when you call the SaveChanges method. For example, when you pass a new entity to the Add method, that entity's state is set to Added. Then when you call the SaveChanges method, the database context issues a SQL INSERT command.
                    // In a desktop application, state changes are typically set automatically. In a desktop type of application, you read an entity and make changes to some of its property values. This causes its entity state to automatically be changed to Modified. Then when you call SaveChanges, the Entity Framework generates a SQL UPDATE statement that updates only the actual properties that you changed.+
                   // The disconnected nature of web apps doesn't allow for this continuous sequence. The DbContext that reads an entity is disposed after a page is rendered. When the HttpPost Edit action method is called, a new request is made and you have a new instance of the DbContext, so you have to manually set the entity state to Modified. Then when you call SaveChanges, the Entity Framework updates all columns of the database row, because the context has no way to know which properties you changed.
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            // You were using DataException to try to identify errors that might be transient in order to give a friendly "try again" message. But now that you've turned on a retry policy, the only errors likely to be transient will already have been tried and failed several times and the actual exception returned will be wrapped in the RetryLimitExceededException exception.
            catch (RetryLimitExceededException ex)
            {
                // TODO: Read up on https://docs.microsoft.com/en-us/aspnet/aspnet/overview/developing-apps-with-windows-azure/building-real-world-cloud-apps-with-windows-azure/monitoring-and-telemetry#log
                Trace.TraceError(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if problems persists see you system administrator.");
            }
            return View(student);
        }

        // GET: Students/Delete/5
      //  You'll add a try-catch block to the HttpPost Delete method to handle any errors that might occur when the database is updated. If an error occurs, the HttpPost Delete method calls the HttpGet Delete method, passing it a parameter that indicates that an error has occurred. The HttpGet Delete method then redisplays the confirmation page along with the error message, giving the user an opportunity to cancel or try again.
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);

                //if improving performance in a high-volume application is a priority, you could avoid an unnecessary SQL query to retrieve the row by replacing the lines of code that call the Find and Remove methods with the following code:
                // This code instantiates a Student entity using only the primary key value and then sets the entity state to Deleted. That's all that the Entity Framework needs in order to delete the entity.
                //Student studentToDelete = new Student() { ID = id };
                //db.Entry(studentToDelete).State = EntityState.Deleted;

                db.SaveChanges();
            }
            catch(DataException ex)
            {
                Trace.TraceError(ex.Message);
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            
            return RedirectToAction("Index");
        }
        // To close database connections and free up the resources they hold as soon as possible, dispose the context instance when you are done with it. That is why the scaffolded code provides a Dispose method at the end of the StudentController class in StudentController.cs, as shown in the following example
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
