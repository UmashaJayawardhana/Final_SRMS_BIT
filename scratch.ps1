$connString = 'Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true'
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()

# Table info for ExamResult
$cmd = $conn.CreateCommand()
$cmd.CommandText = 'SELECT TOP 1 * FROM ExamResult'
$reader = $cmd.ExecuteReader()
$dt = New-Object System.Data.DataTable
$dt.Load($reader)
$dt | Format-Table -AutoSize

# Table info for ExamAssignmentAttedance
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = 'SELECT TOP 1 * FROM ExamAssignmentAttedance'
$reader2 = $cmd2.ExecuteReader()
$dt2 = New-Object System.Data.DataTable
$dt2.Load($reader2)
$dt2 | Format-Table -AutoSize

$conn.Close()
