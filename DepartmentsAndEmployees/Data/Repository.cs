using System.Collections.Generic;
using System.Data.SqlClient;
using DapperDepartments.Models;

namespace DapperDepartments.Data
{
    ///  An object to contain all database interactions:

    public class Repository
    {
        ///  Represents a connection b/w the application and the database:

        public SqlConnection Connection
        {
            get
            {
                // This is "address" of the database:
                string _connectionString = "Data Source=HNEAL-PC\\SQLEXPRESS;Initial Catalog=DepartmentsAndEmployees;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                return new SqlConnection(_connectionString);
            }
        }

        /************************************************************************************
                 * Departments
        ************************************************************************************/

        ///  Returns a list of all departments in the database:

        public List<Department> GetAllDepartments()
        {
            // We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must be careful about how we interact with it. Specifically, we Open() connections when we need to interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error. For database connections, this means the connection will be properly closed.

            using (SqlConnection conn = Connection)
            {
            // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();

                // "SqlCommand cmd" declares a command object
                            
                using (SqlCommand cmd = conn.CreateCommand())   
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, DeptName FROM Department";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the departments we retrieve from the database.
                    List<Department> departments = new List<Department>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "DeptName" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int deptNameColumnPosition = reader.GetOrdinal("DeptName");
                        string deptNameValue = reader.GetString(deptNameColumnPosition);

                        // Now let's create a new department object using the data from the database.
                        Department department = new Department
                        {
                            Id = idValue,
                            DeptName = deptNameValue
                        };

                        // ...and add that department object to our list.
                        departments.Add(department);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of departments who whomever called this method.
                    return departments;
                }
            }
        }


        ///  Returns a single department with the given id.

        public Department GetDepartmentById(int id)
            // "Get" methods require a return statement at the end because they are getting items from the database.
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // String interpolation lets us inject the id passed into this method.
                    cmd.CommandText = $"SELECT DeptName FROM Department WHERE Id = {id}";
                    // It isn't necessary to query the DeptName as well  (on line 104) because we are specifying the id, which will only be one department
                    SqlDataReader reader = cmd.ExecuteReader();

                    Department department = null;
                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = id,
                            DeptName = reader.GetString(reader.GetOrdinal("DeptName"))
                        };
                    }
                    // How would we get a single department by id? 
                    // 1. Open connection again
                    // 2. Query
                    // 3. Create variable with a value of null  called "newDepartment"
                    // 4. We only expect one record with this specific id, so there's no need to do a while loop (which would go through all records until all were checked); therefore, we use a "if" loop.

                    reader.Close();

                    return department;  
                    // Return statement because we got something from the database
                }
            }
        }


        ///  Add a new department to the database
        ///   NOTE: This method SENDS data to the database (it does not get anything from the database, so there is nothing to return).

        public void AddDepartment(Department department)
        {
            // HN: Refactor your code to include parameters here in the "Department Add" section.

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation

                    // If this line were added here, it would open up the following lines to a SQL injection attack: //department.DeptName="foo'); DROP TABLE Department; --";

                    //cmd.CommandText = $"INSERT INTO Department (DeptName) Values ('{department.DeptName}')";  
                    //HN: This ^^^ uses string interpolation to insert something, but we want to use parameters instead
                    //cmd.ExecuteNonQuery();

                    // Incorrect Example:
                    //cmd.CommandText = @"INSERT INTO Department
                    //                        SET DeptName = @deptName";
                    //// HN: SET is only for "U" (UPDATE)!!! We are Adding!
                    //cmd.Parameters.Add(new SqlParameter("@deptName", department.DeptName));
                    ///////////   
                    /// Correct: 
                    cmd.CommandText = @"INSERT INTO Department (DeptName) Values (@deptName)";
                    cmd.Parameters.Add(new SqlParameter("@deptName", department.DeptName));

                    //cmd.ExecuteNonQuery();

                    // Here, we are creating an instance of our department class ("INSERT INTO"); It has passed the query checkpoint (ExecuteNonQuery --> returns how many rows are affected).
                }
            }

            // when this method is finished we can look in the database and see the new department.
        }

        /// <summary>
        ///  Updates the department with the given id
        /// </summary>
        public void UpdateDepartment(int id, Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // SQL Parameters: protect against SQL Injection attacks:

                    //  First, we add variable names with @ signs in our SQL.
                    //  Then, we add SqlParamters for each of those variables.

            // Using parameterized queries is a three-step process:
            //1. Construct the SqlCommand command string with parameters
            //2. Declare a SqlParameter object, assigning values as needed
            //3. Assign the SqlParameter object to the SqlCommand object's Parameters property

                    cmd.CommandText = @"UPDATE Department
                                           SET DeptName = @deptName
                                         WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@deptName", department.DeptName));
                    // "When you execute this command, pass over these values into the database as placeholders"
                    // This will protect against SQL Injection attack.
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    // HN: Add parameters to the "DepartmentAdd" method

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        ///  Delete the department with the given id
        /// </summary>
        public void DeleteDepartment(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Department WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /************************************************************************************
         * Employees
         ************************************************************************************/

        public List<Employee> GetAllEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, DepartmentId FROM Employee";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return employees;
                }
            }
        }

        /// <summary>
        ///  Get an individual employee by id
        /// </summary>
        /// <param name="id">The employee's id</param>
        /// <returns>The employee that with the given id</returns>
        public Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    /*
                     * TODO: Complete this method
                     */

                    cmd.CommandText = $"SELECT FirstName, LastName FROM Employee WHERE Id = {id}";
                    // HN: How to include the DepartmentId in this query?
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };
                    }

                    reader.Close();

                    return employee;

                    //return null;
                }
            }
        }


        /// <summary>
        ///  Get all employees along with their departments
        /// </summary>
        /// <returns>A list of employees in which each employee object contains their department object.</returns>
        public List<Employee> GetAllEmployeesWithDepartment()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    /*
                     * TODO: Complete this method
                     *  Look at GetAllEmployeesWithDepartmentByDepartmentId(int departmentId) for inspiration.
                     */

                    // HN: Code from GetAllEmployeesWithDepartmentByDepartmentId

                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId,
                                                d.DeptName
                                           FROM Employee e INNER JOIN Department d ON e.DepartmentId = d.id";

                    // HN: Is an INNER JOIN necessary here? Since the tables were joined in the previous method?

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                DeptName = reader.GetString(reader.GetOrdinal("DeptName"))
                            }
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return null;
                }
            }
        }

        /// <summary>
        ///  Get employees who are in the given department. Include the employee's department object.
        /// </summary>
        /// <param name="departmentId">Only include employees in this department</param>
        /// <returns>A list of employees in which each employee object contains their department object.</returns>
        public List<Employee> GetAllEmployeesWithDepartmentByDepartmentId(int departmentId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId,
                                                d.DeptName
                                           FROM Employee e INNER JOIN Department d ON e.DepartmentID = d.id
                                          WHERE d.id = @departmentId";
                    cmd.Parameters.Add(new SqlParameter("@departmentId", departmentId));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                DeptName = reader.GetString(reader.GetOrdinal("DeptName"))
                            }
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return employees;
                }
            }
        }


        /// <summary>
        ///  Add a new employee to the database
        ///   NOTE: This method sends data to the database, 
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void AddEmployee(Employee employee)
        /*
            HN: The only thing this method (AddEmployee in Repository.cs) cares about is adding an Employee object to the database; whatever file calls this method is the one that should be concerned with what is info is inserted into this Employee object. Here, we just have to ensure allowances are made for the properties the Employee object should have.
         */
        {
            /*
             * TODO: Complete this method by using an INSERT statement with SQL
             *  Remember to use SqlParameters!
             */

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
  //                  employee.FirstName = "foo'); DROP TABLE Department; --";
                    cmd.CommandText = $"INSERT INTO Employee (FirstName) Values ('{employee.FirstName}')";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///  Updates the employee with the given id
        /// </summary>
        public void UpdateEmployee(int id, Employee employee)
        {
            /*
             * TODO: Complete this method using an UPDATE statement with SQL
             *  Remember to use SqlParameters!
             */
        }


        /// <summary>
        ///  Delete the employee with the given id
        /// </summary>
        public void DeleteEmployee(int id)
        {
            /*
             * TODO: Complete this method using a DELETE statement with SQL
             *  Remember to use SqlParameters!
             */
        }
    }
}