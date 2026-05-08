// Common/ServiceResult.cs
// ✅ Both ServiceResult and ServiceResult<T> live here together
// WHY: Having them in two different namespaces (SchoolApp.Common vs
//      SchoolApp.Models.Common) causes CS0308 because GlobalUsings.cs
//      imports SchoolApp.Models.Common which has ServiceResult<T>,
//      but ServiceResult (non-generic) is in SchoolApp.Common —
//      so the compiler finds one but not the other depending on context.
//
//      Putting both in SchoolApp.Common and importing that one namespace
//      globally means ALL files can use both without any using statements.

namespace SchoolApp.Common
{
    // Non-generic version — for operations that return no data
    // Example: AssignCoursesToTeacher() — just succeeds or fails, no payload
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ServiceResult Ok(string message = "Operation completed successfully.")
        {
            return new ServiceResult { Success = true, Message = message };
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { Success = false, Message = message };
        }
    }

    // Generic version — for operations that return data
    // Example: StudentService.Add() returns the created Student
    //          CourseService.GetById() returns a Course
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T? data, string message = "Operation completed successfully.")
        {
            return new ServiceResult<T> { Success = true, Message = message, Data = data };
        }

        public static ServiceResult<T> Fail(string message)
        {
            return new ServiceResult<T> { Success = false, Message = message, Data = default };
        }
    }


}