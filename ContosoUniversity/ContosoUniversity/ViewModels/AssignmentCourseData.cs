using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.ViewModels
{
    /// <summary>
    /// The relationship between the Course and Instructor entities is many-to-many, which means you do not have direct access to the foreign key properties which are in the join table. Instead, you add and remove entities to and from the Instructor.Courses navigation property.+
    /// The UI that enables you to change which courses an instructor is assigned to is a group of check boxes.A check box for every course in the database is displayed, and the ones that the instructor is currently assigned to are selected.The user can select or clear check boxes to change course assignments.If the number of courses were much greater, you would probably want to use a different method of presenting the data in the view, but you'd use the same method of manipulating navigation properties in order to create or delete relationships.
/// To provide data to the view for the list of check boxes, you'll use a view model class. Create AssignedCourseData.cs in the ViewModels folder and replace the existing code with the following code:
    /// </summary>
    public class AssignedCourseData
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }
    }
}