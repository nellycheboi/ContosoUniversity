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
using ContosoUniversity.ViewModels;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private SchoolDBContext db = new SchoolDBContext();

        // GET: Instructor
        /// <summary>
        /// The method accepts optional route data (id) and a query string parameter (courseID) that provide the ID values of the selected instructor and selected course, and passes all of the required data to the view. The parameters are provided by the Select hyperlinks on the page.
        /// 
        /// The code begins by creating an instance of the view model and putting in it the list of instructors. The code specifies eager loading for the Instructor.OfficeAssignment and the Instructor.Courses navigation property.
        /// 
        /// The second Include method loads Courses, and for each Course that is loaded it does eager loading for the Course.Department navigation property.
        /// 
        /// As mentioned previously, eager loading is not required but is done to improve performance. Since the view always requires the OfficeAssignment entity, it's more efficient to fetch that in the same query. Course entities are required when an instructor is selected in the web page, so eager loading is better than lazy loading only if the page is displayed more often with a course selected than without.
        /// If an instructor ID was selected, the selected instructor is retrieved from the list of instructors in the view model.The view model's Courses property is then loaded with the Course entities from that instructor's Courses navigation property.
        /// 
        /// The Where method returns a collection, but in this case the criteria passed to that method result in only a single Instructor entity being returned. The Single method converts the collection into a single Instructor entity, which gives you access to that entity's Courses property.
        /// 
        /// Next, if a course was selected, the selected course is retrieved from the list of courses in the view model. Then the view model's Enrollments property is loaded with the Enrollment entities from that course's Enrollments navigation property.
        /// 
        /// When you retrieved the list of instructors, you specified eager loading for the Courses navigation property and for the Department property of each course. Then you put the Courses collection in the view model, and now you're accessing the Enrollments navigation property from one entity in that collection. Because you didn't specify eager loading for the Course.Enrollments navigation property, the data from that property is appearing in the page as a result of lazy loading.
        /// If you disabled lazy loading without changing the code in any other way, the Enrollments property would be null regardless of how many enrollments the course actually had.In that case, to load the Enrollments property, you'd have to specify either eager loading or explicit loading. You've already seen how to do eager loading.In order to see an example of explicit loading, replace the Index method with the following code, which explicitly loads the Enrollments property. The code changed are highlighted.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public ActionResult Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department))
                .OrderBy(i => i.LastName);

            if (id != null)
            {
                ViewBag.InstructorID = id.Value;
                viewModel.Courses = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single().Courses;
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                // Lazy loading
                viewModel.Enrollments = viewModel.Courses.Where(
                    x => x.ID == courseID).Single().Enrollments;
                // Explicit loading
                // After getting the selected Course entity, the new code explicitly loads that course's Enrollments navigation property:
                // Then it explicitly loads each Enrollment entity's related Student entity:
                //var selectedCourse = viewModel.Courses.Where(x => x.ID == courseID).Single();
                //db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
                //foreach (Enrollment enrollment in selectedCourse.Enrollments)
                //{
                //    // Notice that you use the Collection method to load a collection property, but for a property that holds just one entity, you use the Reference method.
                //    db.Entry(enrollment).Reference(x => x.Student).Load();
                //}

                //viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        /// <summary>
        /// The HttpGet Create method calls the PopulateAssignedCourseData method not because there might be courses selected but in order to provide an empty collection for the foreach loop in the view (otherwise the view code would throw a null reference exception).
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<Course>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// The HttpPost Create method adds each selected course to the Courses navigation property before the template code that checks for validation errors and adds the new instructor to the database. Courses are added even if there are model errors so that when there are model errors (for an example, the user keyed an invalid date) so that when the page is redisplayed with an error message, any course selections that were made are automatically restored.+
       /// Notice that in order to be able to add courses to the Courses navigation property you have to initialize the property as an empty collection:
        /// </summary>
        /// <param name="instructor"></param>
        /// <param name="selectedCourses"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,HireDate,OfficeAssignment")]Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.Courses = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = db.Courses.Find(int.Parse(course));
                    instructor.Courses.Add(courseToAdd);
                }
            }
            
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // eager loading for the associated OfficeAssignment entity. You can't perform eager loading with the Find method, so the Where and Single methods are used instead to select the instructor.
            Instructor instructor = db.Instructors
               .Include(i => i.OfficeAssignment)
               .Include(i => i.Courses)
               .Where(i => i.ID == id)
               .Single();
            PopulateAssignedCourseData(instructor);
            if (instructor == null)
            {
                return HttpNotFound();
            }

            return View(instructor);
        }



        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// When you edit an instructor record, you want to be able to update the instructor's office assignment. The Instructor entity has a one-to-zero-or-one relationship with the OfficeAssignment entity, which means you must handle the following situations:+
        /// If the user clears the office assignment and it originally had a value, you must remove and delete the OfficeAssignment entity.
        /// If the user enters an office assignment value and it originally was empty, you must create a new OfficeAssignment entity.
        /// If the user changes the value of an office assignment, you must change the value in an existing OfficeAssignment entity.
        /// 
        /// Changes the method name to EditPost because the signature is now the same as the HttpGet method (the ActionName attribute specifies that the /Edit/ URL is still used).
        /// Gets the current Instructor entity from the database using eager loading for the OfficeAssignment navigation property.This is the same as what you did in the HttpGet Edit method.
        /// Updates the retrieved Instructor entity with values from the model binder.The TryUpdateModel overload used enables you to whitelist the properties you want to include. This prevents over-posting
        /// 
        /// The method signature is now different from the HttpGet Edit method, so the method name changes from EditPost back to Edit.
        /// Since the view doesn't have a collection of Course entities, the model binder can't automatically update the Courses navigation property.Instead of using the model binder to update the Courses navigation property, you'll do that in the new UpdateInstructorCourses method. Therefore you need to exclude the Courses property from model binding. This doesn't require any change to the code that calls TryUpdateModel because you're using the whitelisting overload and Courses isn't in the include list.
        /// </summary>
        /// <param name="instructor"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instructorToUpdate = db.Instructors
               .Include(i => i.OfficeAssignment)
               .Include(i => i.Courses)
               .Where(i => i.ID == id)
               .Single();

            if (TryUpdateModel(instructorToUpdate, "",
               new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    // If the office location is blank, sets the Instructor.OfficeAssignment property to null so that the related row in the OfficeAssignment table will be deleted.
                    if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                    {
                        instructorToUpdate.OfficeAssignment = null;
                    }

                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException ex)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
            //return View();
        }

        /// <summary>
        /// The code adds eager loading for the Courses navigation property and calls the new PopulateAssignedCourseData method to provide information for the check box array using the AssignedCourseData view model class.
        ///The code in the PopulateAssignedCourseData method reads through all Course entities in order to load a list of courses using the view model class. For each course, the code checks whether the course exists in the instructor's Courses navigation property. To create efficient lookup when checking whether a course is assigned to the instructor, the courses assigned to the instructor are put into a HashSet collection. The Assigned property is set to true for courses the instructor is assigned. The view will use this property to determine which check boxes must be displayed as selected. Finally, the list is passed to the view in a ViewBag property.
        /// </summary>
        /// <param name="instructor"></param>
        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = db.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.ID));
            var viewModel = new List<AssignedCourseData>();
            foreach (Course course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.ID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.ID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            // If no check boxes were selected, the code in UpdateInstructorCourses initializes the Courses navigation property with an empty collection:
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            // The code then loops through all courses in the database and checks each course against the ones currently assigned to the instructor versus the ones that were selected in the view. To facilitate efficient lookups, the latter two collections are stored in HashSet objects.
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.ID));

            // If the check box for a course was selected but the course isn't in the Instructor.Courses navigation property, the course is added to the collection in the navigation property.
            foreach (var course in db.Courses)
            {
                if (selectedCoursesHS.Contains(course.ID.ToString()))
                {
                    if (!instructorCourses.Contains(course.ID))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    // If the check box for a course wasn't selected, but the course is in the Instructor.Courses navigation property, the course is removed from the navigation property.
                    if (instructorCourses.Contains(course.ID))
                    {
                        instructorToUpdate.Courses.Remove(course);
                    }
                }
            }
        }
        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }
        /// <summary>
        /// If the instructor is assigned as administrator of any department, removes the instructor assignment from that department. Without this code, you would get a referential integrity error if you tried to delete an instructor who was assigned as administrator for a department.
        /// This code doesn't handle the scenario of one instructor assigned as administrator for multiple departments. In the last tutorial you'll add code that prevents that scenario from happening.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = db.Instructors
             .Include(i => i.OfficeAssignment)
             .Where(i => i.ID == id)
             .Single();

            db.Instructors.Remove(instructor);

            var department = db.Departments
                .Where(d => d.InstructorID == id)
                .SingleOrDefault();
            if (department != null)
            {
                department.InstructorID = null;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
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
