using System.Diagnostics;
// Simulate 5 database calls

var sw = Stopwatch.StartNew();

// WRONG WAY - Blocking
for (int i = 0; i < 5; i++)
{
    Thread.Sleep(300);
}

Console.WriteLine($"Blocking sequential: {sw.ElapsedMilliseconds}ms");


// ASYNC BUT SEQUENTIAL
sw.Restart();

for (int i = 0; i < 5; i++)
{
    await Task.Delay(300);
}

Console.WriteLine($"Async sequential: {sw.ElapsedMilliseconds}ms");


// RIGHT WAY - Async Parallel
sw.Restart();
var tasks = Enumerable.Range(0, 5)
    .Select(_ => Task.Delay(300));

await Task.WhenAll(tasks);

Console.WriteLine($"Async parallel: {sw.ElapsedMilliseconds}ms");
async Task<Student> FetchStudentAsync(string id)
{
    Console.WriteLine($"Fetching {id}...");

    await Task.Delay(300);

    return new Student
    {
        Id = id,
        Name = $"Student-{id}",
        Age = 20,

        GPA = id switch
        {
            "S1" => 3.8m,
            "S2" => 2.4m,
            "S3" => 3.5m,
            "S4" => 1.9m,
            "S5" => 3.2m,
            _ => 2.5m
        }
    };
}
async Task<Course> FetchCourseAsync(string code)
{
    Console.WriteLine($"Fetching course {code}...");

    await Task.Delay(200);

    return new Course
    {
        Code = code,
        Title = $"Course-{code}",

        Capacity = code switch
        {
            "CRS-101" => 2,
            "CRS-201" => 30,
            "CRS-301" => 15,
            _ => 25
        }
    };
}
sw.Restart();

string[] studentIds = ["S1", "S2", "S3", "S4", "S5"];

string[] courseCodes = ["CRS-101", "CRS-201", "CRS-301"];


// Start all tasks together
var studentTasks = studentIds.Select(id => FetchStudentAsync(id));

var courseTasks = courseCodes.Select(code => FetchCourseAsync(code));


// Wait for all tasks
Student[] students = await Task.WhenAll(studentTasks);

Course[] courses = await Task.WhenAll(courseTasks);


Console.WriteLine($"\nLoaded {students.Length} students and {courses.Length} courses in {sw.ElapsedMilliseconds}ms");

foreach (var s in students)
{
    Console.WriteLine($"{s.Name} GPA: {s.GPA}");
}
var enrollCourse = new Course
{
    Code = "CRS-101",
    Title = "C# Mastery",
    Capacity = 2
};

var enrollService = new EnrollmentService();

var enrollments = new List<EnrollmentRecord>();

var failures = new List<string>();


sw.Restart();

foreach (var student in students)
{
    try
    {
        var record = enrollService.ProcessRegistration(student, enrollCourse);

        enrollCourse.EnrolledCount++;

        enrollments.Add(record);

        Console.WriteLine($"Enrolled: {student.Name}");
        _ = SendConfirmationAsync(student);
    }
    catch (InvalidOperationException ex)
    {
        failures.Add($"{student.Name}: {ex.Message}");

        Console.WriteLine($"Rejected: {student.Name} {ex.Message}");
    }
}
async Task SendConfirmationAsync(Student student)
{
    try
    {
        await Task.Delay(100);

        Console.WriteLine($"Email sent to {student.Name}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Email failed for {student.Name}: {ex.Message}");
    }
}
try
{
    var overflowCourse = new Course
    {
        Code = "CRS-999",
        Title = "Overflow Test",
        Capacity = 1
    };

    overflowCourse.EnrolledCount = 1;

    enrollService.ProcessRegistration(
        new Student
        {
            Id = "S99",
            Name = "Test",
            Age = 20,
            GPA = 3.0m
        },
        overflowCourse
    );
}
catch (CapacityReachedException ex)
{
    Console.WriteLine("\nDomain exception caught:");

    Console.WriteLine($"Course: {ex.CourseCode}");

    Console.WriteLine($"Message: {ex.Message}");
}
// Stop timer
sw.Stop();


// Calculate GPA average
decimal classAverage = students.Length > 0
    ? students.Average(s => s.GPA)
    : 0m;


// Print report
Console.WriteLine("\n========== ENROLLMENT SUMMARY ==========");

Console.WriteLine($"Total students loaded: {students.Length}");

Console.WriteLine($"Successful enrollments: {enrollments.Count}");

Console.WriteLine($"Failed enrollments: {failures.Count}");

Console.WriteLine($"Class average GPA: {classAverage:F2}");

Console.WriteLine($"Total elapsed time: {sw.ElapsedMilliseconds}ms");


if (failures.Count > 0)
{
    Console.WriteLine("\n--- Failure Details ---");

    foreach (var failure in failures)
    {
        Console.WriteLine(failure);
    }
}

Console.WriteLine("========================================");