using System;
using System.Collections.Generic;
using System.Linq;
using DapperDepartments.Data;
using DapperDepartments.Models;

namespace DapperDepartments
{
    class Program
    {
        /// <summary>
        ///  The Main method is the starting point for a .net application.
        /// </summary>
        static void Main(string[] args)
        {
            // We must create an instance of the Repository class in order to use it's methods to
            //  interact with the database.
            Repository repository = new Repository();

            List<Department> departments = repository.GetAllDepartments();
            // Think of "Department" (which is a type here) as if it were "string". If <string> is here (rather than <Department>), you would be able to access every property that the <string> type has. Props such as ToString, etc., are accessible by List because it is a list of the <string> type. So, every prop of <Department> is also accessible by List.

            // PrintDepartmentReport should print a department report to the console, but does it?
            //  Take a look at how it's defined below...
            PrintDepartmentReport("All Departments", departments);

            // What is this? Scroll to the bottom of the file and find out for yourself.
            Pause();            ///  ANSWER: Custom function that pauses execution of the console app until the user presses a key


            // Create a new instance of a Department, so we can save our new department to the database.
            Department accounting = new Department { DeptName = "Accounting" };
            // Pass the accounting object as an argument to the repository's AddDepartment() method.
            repository.AddDepartment(accounting);

            departments = repository.GetAllDepartments();
            PrintDepartmentReport("All Departments after adding Accounting department", departments);

            Pause();


            // Pull the object that represents the Accounting department from the list of departments that
            //  we got from the database.
            // First() is a handy LINQ method that gives us the first element in a list that matches the condition.
            Department accountingDepartmentFromDB = departments.First(d => d.DeptName == "Accounting");

            // How are the "accounting" and "accountingDepartmentFromDB" objects different?
            //  Why are they different?
            Console.WriteLine($"                accounting --> {accounting.Id}: {accounting.DeptName}");
            Console.WriteLine($"accountingDepartmentFromDB --> {accountingDepartmentFromDB.Id}: {accountingDepartmentFromDB.DeptName}");

            Pause();

            // Change the name of the Accounting department and save the change to the database.
            accountingDepartmentFromDB.DeptName = "Creative Accounting";
            repository.UpdateDepartment(accountingDepartmentFromDB.Id, accountingDepartmentFromDB);

            departments = repository.GetAllDepartments();
            PrintDepartmentReport("All Departments after updating Accounting department", departments);

            Pause();


            // Maybe we don't need an Accounting department after all...
            repository.DeleteDepartment(accountingDepartmentFromDB.Id);

            departments = repository.GetAllDepartments();
            PrintDepartmentReport("All Departments after deleting Accounting department", departments);

            Pause();

            // Create a new variable to contain a list of Employees and get that list from the database
            List<Employee> employees = repository.GetAllEmployees();

            // Does this method do what it claims to do, or does it need some work?
            PrintEmployeeReport("All Employees", employees);

            Pause();


            employees = repository.GetAllEmployeesWithDepartment();
            PrintEmployeeReport("All Employees with departments", employees);

            Pause();


            // Here we get the first department by index.
            //  We could use First() here without passing it a condition, but using the index is easy enough.
            Department firstDepartment = departments[0];
            employees = repository.GetAllEmployeesWithDepartmentByDepartmentId(firstDepartment.Id);
            PrintEmployeeReport($"Employees in {firstDepartment.DeptName}", employees);

            Pause();


            // Instantiate a new employee object.
            //  Note we are making the employee's DepartmentId refer to an existing department.
            //  This is important because if we use an invalid department id, we won't be able to save
            //  the new employee record to the database because of a foreign key constraint violation.
            Employee jane = new Employee
            {
                FirstName = "Jane",
                LastName = "Lucas",
                DepartmentId = firstDepartment.Id
            };
            repository.AddEmployee(jane);

            employees = repository.GetAllEmployeesWithDepartment();
            PrintEmployeeReport("All Employees after adding Jane", employees);

            Pause();


            // Once again, we see First() in action.
            Employee dbJane = employees.First(e => e.FirstName == "Jane");

            // Get the second department by index.
            Department secondDepartment = departments[1];

            dbJane.DepartmentId = secondDepartment.Id;
            repository.UpdateEmployee(dbJane.Id, dbJane);

            employees = repository.GetAllEmployeesWithDepartment();
            PrintEmployeeReport("All Employees after updating Jane", employees);

            Pause();


            repository.DeleteEmployee(dbJane.Id);
            employees = repository.GetAllEmployeesWithDepartment();

            PrintEmployeeReport("All Employees after updating Jane", employees);

            Pause();

        }

        /// <summary>
        ///  Prints a simple report with the given title and department information.
        /// </summary>
        /// <remarks>
        ///  Each line of the report should include the Department's ID and Name
        /// </remarks>
        /// <param name="title">Title for the report</param>
        /// <param name="departments">Department data for the report</param>
        public static void PrintDepartmentReport(string title, List<Department> departments)
        {

            Console.WriteLine(title, departments);

            /*
             * TODO: Complete this method
             *  For example a report entitled, "All Departments" should look like this:

                All Departments
                1: Marketing
                2: Engineering
                3: Design
             */

        }

        /// <summary>
        ///  Prints a simple report with the given title and employee information.
        /// </summary>
        /// <remarks>
        ///  Each line of the report should include the 
        ///   Employee's ID, First Name, Last Name, 
        ///   and department name IF AND ONLY IF the department is not null.
        /// </remarks>
        ///<param name = "title" > "Employee Report" </ param >
        ///< param name="employees">"Employee Information"</param>
        public static void PrintEmployeeReport(string title, List<Employee> employees)
        {

            Console.WriteLine(title, employees);
            /*
             * TODO: Complete this method
             *  For example a report entitled, "All Employees", should look like this:

                All Employees
                1: Margorie Klingerman
                2: Sebastian Lefebvre
                3: Jamal Ross

             *  A report entitled, "All Employees with Departments", should look like this:

                All Employees with Departments
                1: Margorie Klingerman. Dept: Marketing
                2: Sebastian Lefebvre. Dept: Engineering
                3: Jamal Ross. Dept: Design

             */
        }


        /// <summary>
        ///  Custom function that pauses execution of the console app until the user presses a key
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.Read();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
