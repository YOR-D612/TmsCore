public class EnrollmentService
{
    public EnrollmentRecord ProcessRegistration(Student? student, Course? course)
    {
        // Guard clause 1 — student cannot be null
        if (student is null)
            throw new ArgumentNullException(nameof(student));

        // Guard clause 2 — course cannot be null
        if (course is null)
            throw new ArgumentNullException(nameof(course));

        // Guard clause 3 — course must not be full
        if (course.EnrolledCount >= course.Capacity)
            throw new InvalidOperationException("Course is full.");

        // Switch expression for GPA standing
        string standing = student.GPA switch
        {
            >= 3.5m => "Honors",
            >= 2.5m => "Good Standing",
            _ => "Academic Warning"
        };

        Console.WriteLine($"{student.Name} is in {standing}.");

        // Return enrollment record
        return new EnrollmentRecord(
            student.Id,
            course.Code,
            DateTime.UtcNow
        );
    }
}