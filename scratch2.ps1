$connString = 'Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true'
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()

# Table columns for ExamResult
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ExamResult'"
$reader = $cmd.ExecuteReader()
$cols = @()
while ($reader.Read()) {
    $cols += $reader['COLUMN_NAME']
}
$reader.Close()
Write-Host "ExamResult cols: $($cols -join ', ')"

# Table columns for ExamAssignmentAttedance
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ExamAssignmentAttedance'"
$reader2 = $cmd2.ExecuteReader()
$cols2 = @()
while ($reader2.Read()) {
    $cols2 += $reader2['COLUMN_NAME']
}
$reader2.Close()
Write-Host "ExamAssignmentAttedance cols: $($cols2 -join ', ')"

$conn.Close()
