using System.Collections.Generic;
using System.Data.SqlClient;
using DapperDepartments.Models;

namespace DapperDepartments.Data
{

    // COMMENTS KEY:

    //(^_^) Important Comment: For Notes To/From Users other than myself (instructional staff, employers, etc.)
    //NOTE:  Notes to myself
    //? Questions
    //x Strikethrough

    public class Repository
    {

        public SqlConnection Connection
        {
            get
            {
                string _connectionString = "Data Source=HNEAL-PC\\SQLEXPRESS;Initial Catalog=DepartmentsAndEmployees;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                return new SqlConnection(_connectionString);
            }
        }

        /*****************************************************************
                                 * Departments
        ******************************************************************/

        //NOTE: This section contains 5 methods. They are (in order):

            // 1. GetAllDepartments();         
            //  SELECT - FROM       return (public)

            // 2. GetDepartmentById(int id);        
            //  SELECT - FROM - WHERE       return (public)

            // 3. AddDepartment(Department department);     
            //  INSERT INTO      cmd.ExecuteNonQuery(); (public void)

            // 4. UpdateDepartment(int id, Department department);      
            //  UPDATE - SET - WHERE        cmd.ExecuteNonQuery(); (public void)

            // 5. DeleteDepartment(int id);    
            //  DELETE FROM - WHERE    cmd.ExecuteNonQuery(); (public void)

/*=============================        GET ALL          ===================================*/
        //NOTE: Get = return statement
        //NOTE: SELECT - FROM 

        //(^_^)   Returns a list of all departments in the database:

        public List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())   
                {
                    //(^_^)  Here we setup the command with the SQL we want to execute before we execute it:
                    cmd.CommandText = "SELECT Id, DeptName FROM Department";

                    //(^_^)  Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();

                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        int deptNameColumnPosition = reader.GetOrdinal("DeptName");
                        string deptNameValue = reader.GetString(deptNameColumnPosition);

                        //(^_^)  Now let's create a new department object using the data from the database and add that department object to our list.
                        Department department = new Department
                        {
                            Id = idValue,
                            DeptName = deptNameValue
                        };
                        departments.Add(department);
                    }
                    reader.Close();

                    return departments;
                }
            }
        }
/*==========================         GET (SPECIFIC)          ===============================*/
//NOTE: Get = return statement
//NOTE: SELECT - FROM - WHERE

        //(^_^)   Returns a single department with the given id.

        public Department GetDepartmentById(int id)
        {

        //NOTE: 
        /*
                                // How would we get a single department by id? 
                                // 1. Open connection again
                                // 2. Query
                                // 3. Create variable with a value of null  called "newDepartment"
                                // 4. We only expect one record with this specific id, so there's no need to do a while loop (which would go through all records until all were checked); therefore, we use a "if" loop.
                                */

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // (^_^) String interpolation lets us inject the id passed into this method.
                    cmd.CommandText = $"SELECT DeptName FROM Department WHERE Id = {id}";
                    //(^_^)  It isn't necessary to query the DeptName as well  because we are specifying the id, which will only be one department
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

                    reader.Close();

                    return department;
                }
            }
        }
/*================================         ADD          ================================*/
//NOTE: INSERT INTO

        //(^_^)   Add a new department to the database

        public void AddDepartment(Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    //NOTE: Incorrect (no SQL params):
                    //cmd.CommandText = $"INSERT INTO Department (DeptName) Values ('{department.DeptName}')";  
                    //cmd.ExecuteNonQuery();
                    //This ^^^ uses string interpolation to insert something, but we want to use parameters instead

                    //NOTE: Correct: 
                    cmd.CommandText = @"INSERT INTO Department (DeptName) Values (@deptName)";
                    cmd.Parameters.Add(new SqlParameter("@deptName", department.DeptName));

                    cmd.ExecuteNonQuery();

                    //(^_^)  Here, we are creating an instance of our department class ("INSERT INTO"); It has passed the query checkpoint (ExecuteNonQuery --> returns how many rows are affected).
                }
            }
        }
/*===============================         UPDATE          ===============================*/
//NOTE: UPDATE - SET - WHERE

        public void UpdateDepartment(int id, Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Department
                                           SET DeptName = @deptName
                                         WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@deptName", department.DeptName));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
/*==============================         DELETE          ================================*/
//NOTE: DELETE FROM - WHERE

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

/****************************************************************************
                                 * Employees
                            ******************************************************************************/
        //NOTE :This section contains 7 methods. They are (in order):
        //  1. GetAllEmployees(); 
        // 2. GetEmployeeById(int id);
        // 3.  GetAllEmployeesWithDepartment();
        // 4. GetAllEmployeesWithDepartmentByDepartmentId(int departmentId);
        // 5. AddEmployee(Employee employee);
        // 6. UpdateEmployee(int id, Employee employee);
        // 7. DeleteEmployee(int id);

/******************************************************************************/
        //NOTE: Here they are again with more details:
        //  1. GetAllEmployees(); 
        // SELECT - FROM        return (public)

        // 2. GetEmployeeById(int id);
        // SELECT - FROM - WHERE    return (public)

        // 3.  GetAllEmployeesWithDepartment();
        // SELECT - FROM --- INNER JOIN/JOIN - ON       return (public)

        // 4. GetAllEmployeesWithDepartmentByDepartmentId(int departmentId);
        // SELECT - FROM - WHERE    return (public)

        // 5. AddEmployee(Employee employee);
        // INSERT INTO     cmd.ExecuteNonQuery();  (public void)

        // 6. UpdateEmployee(int id, Employee employee);
        // UPDATE - SET - WHERE     cmd.ExecuteNonQuery();  (public void)

        // 7. DeleteEmployee(int id);
        // DELETE - FROM - WHERE    cmd.ExecuteNonQuery();      (public void)

/*=============================         GET ALL         ===================================*/
//NOTE: Get = return statement
//NOTE: SELECT - FROM

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

                            //? Could the value of the text in the firstName column also be accessed with bracket notation, as with lists and dictionaries?
                        };

                        employees.Add(employee);
                    }

                    reader.Close();
                    return employees;
                }
            }
        }

/*==========================         GET (SPECIFIC)          ===============================*/
//NOTE: Get = return statement
//NOTE: SELECT - FROM - WHERE
        
        //(^_^)   Returns a single employee with a given id

        public Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    /*
                     * TODO: Complete this method with SQL parameters
                     */

                    cmd.CommandText = $"SELECT FirstName, LastName FROM Employee WHERE Id = {id}";
                    
                    //NOTE: It is not necessary to include the DepartmentId in this GET because we are following the SOLID principles; This method only retrieves an employee by their id. The employee and their department are called in the "ADD" method  (GetAllEmployeesByDepartment) right after this GET.
                
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
                    //? When am I supposed to return null??
                }
            }
        }

 /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!         GET WITH DEPT          !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
//NOTE: Get = return statement
//NOTE: SELECT - FROM --- INNER JOIN/JOIN - ON

        //(^_^)   Get all employees along with their departments
        //Note: This is really important!!!This shows how to display information for one object that is in an independent dataset/table from another object, 
        //(^_^) A list of employees in which each employee object contains their department object.

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

            // NOTE: Code from GetAllEmployeesWithDepartmentByDepartmentId

                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId,
                                                d.DeptName
                                           FROM Employee e INNER JOIN Department d ON e.DepartmentId = d.id";

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

/*=======================       GET BY SPECIFIC DEPARTMENT         =====================*/
//NOTE: SELECT - FROM - WHERE

        //(^_^)  Get employees who are in the given department. Include the employee's department object.
        //(^_^) <param name="departmentId">Only include employees in this department</param>
        //(^_^)A list of employees in which each employee object contains their department object.


        public List<Employee> GetAllEmployeesWithDepartmentByDepartmentId(int departmentId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //NOTE: The equivalent SQL syntax for the query below is:
                    /*
                                        SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.DeptName FROM Employee e INNER JOIN Department d ON e.DepartmentId = d.id
                                        */

                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.DeptName
                                        FROM Employee e 
                                        INNER JOIN Department d 
                                        ON e.DepartmentId = d.id
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

/*===================================         ADD          ==================================*/
//NOTE: INSERT INTO

        //(^_^)   Add a new employee to the database
        //(^_^)    NOTE: This method sends data to the database; it does not get anything from the database, so there is nothing to return.

        public void AddEmployee(Employee employee)
    
    //NOTE: The only thing this method (AddEmployee in Repository.cs) cares about is adding an Employee object to the database; whatever file calls this method is the one that should be concerned with what is info is inserted into this Employee object. Here, we just have to ensure allowances are made for the properties the Employee object has.

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

                    cmd.CommandText = $"INSERT INTO Employee (FirstName, LastName, DepartmentId) Values (@FirstName, @LastName, @DepartmentId)";

                    cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));

                    cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));

                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));

                    cmd.ExecuteNonQuery();
                }
            }
        }

/*===============================         UPDATE          ===============================*/
//NOTE: UPDATE - SET - WHERE

        //(^_^)   Updates the employee of the given id

        public void UpdateEmployee(int id, Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Employee
                                        SET DepartmentId = @DepartmentId
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }

        }

/*==============================         DELETE          ================================*/
//NOTE: DELETE FROM - WHERE

        //(^_^)   Delete the employee with the given id

        public void DeleteEmployee(int id)
        {
            /*
             * TODO: Complete this method using a DELETE statement with SQL
             *  Remember to use SqlParameters!
             */
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}