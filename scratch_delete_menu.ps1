$connString = 'Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true'
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
$cmd = $conn.CreateCommand()
# Only delete the top-level one (which has ParentMenuId IS NULL), leaving their manually added child one intact
$cmd.CommandText = "DELETE FROM AppMenu WHERE ActionName = 'ManageNotifications' AND ParentMenuId IS NULL"
$deleted = $cmd.ExecuteNonQuery()
Write-Host "Deleted $deleted rows."
$conn.Close()
