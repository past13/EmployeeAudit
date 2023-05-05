
-- CREATE TABLE employees (
--     employeeid INT NOT NULL,
--     employeename VARCHAR(128) NOT NULL,
--     employeesalary INT NOT NULL,
--     existencestartutc TIMESTAMP NOT NULL,
--     existenceendutc TIMESTAMP NULL
-- );

CREATE TABLE IF NOT EXISTS public.employees
(
    "Id" serial NOT NULL,
    "EmployeeId" integer NOT NULL,
    "Name" character(128) NOT NULL,
    "Salary" integer NOT NULL,
    "ExistenceStartUtc" timestamp NOT NULL,
    "ExistenceEndUtc" timestamp 
)