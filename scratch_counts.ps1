$connString = 'Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true'
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM Student"
$studentCount = $cmd.ExecuteScalar()

$cmd.CommandText = "SELECT COUNT(*) FROM Teacher"
$teacherCount = $cmd.ExecuteScalar()

$cmd.CommandText = "SELECT COUNT(*) FROM Exam"
$examCount = $cmd.ExecuteScalar()

$cmd.CommandText = "SELECT COUNT(*) FROM Subject"
$subjectCount = $cmd.ExecuteScalar()

$cmd.CommandText = "SELECT COUNT(*) FROM Class"
$classCount = $cmd.ExecuteScalar()

Write-Host "Students: $studentCount, Teachers: $teacherCount, Exams: $examCount, Subjects: $subjectCount, Classes: $classCount"

$conn.Close()
