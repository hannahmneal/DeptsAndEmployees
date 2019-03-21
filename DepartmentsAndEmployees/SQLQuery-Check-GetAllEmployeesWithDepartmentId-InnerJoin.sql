SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.DeptName FROM Employee e INNER JOIN Department d ON e.DepartmentId = d.id

-- This is the SQL version of the query in the GetAllEmployeesWithDepartmentId(int departmentId) method: 
-- cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.DeptName 
--					   FROM Employee e 
--					   INNER JOIN Department d 
--					   ON e.DepartmentId = d.id
--					   WHERE d.id = @departmentId";
				