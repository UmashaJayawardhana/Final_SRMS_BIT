$c = New-Object System.Data.SqlClient.SqlConnection("Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true")
$c.Open()
$cmd = $c.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*), COUNT(CASE WHEN Status = 'Active' THEN 1 END) FROM AppMenu"
$r = $cmd.ExecuteReader()
$r.Read()
Write-Output "Total: $($r[0]), Active: $($r[1])"
$r.Close()

$cmd2 = $c.CreateCommand()
$cmd2.CommandText = "SELECT TOP 5 MenuId, MenuName, Status FROM AppMenu"
$r2 = $cmd2.ExecuteReader()
Write-Output "--- Sample Menus ---"
while ($r2.Read()) {
    Write-Output "ID: $($r2[0]) | Name: $($r2[1]) | Status: $($r2[2])"
}
$r2.Close()

$cmd3 = $c.CreateCommand()
$cmd3.CommandText = "SELECT COUNT(*) FROM RolePrivilege"
$r3 = $cmd3.ExecuteReader()
$r3.Read()
Write-Output "Total RolePrivilege records: $($r3[0])"
$r3.Close()

$c.Close()
