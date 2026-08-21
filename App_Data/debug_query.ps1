$c = New-Object System.Data.SqlClient.SqlConnection("Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true")
$c.Open()
$cmd = $c.CreateCommand()
$cmd.CommandText = "SELECT m.MenuId, m.MenuName, p.MenuName FROM AppMenu m LEFT JOIN AppMenu p ON m.ParentMenuId = p.MenuId"
$r = $cmd.ExecuteReader()
Write-Output "--- DB Menu Items with Parents ---"
$count = 0
while ($r.Read()) {
    $count++
    $parentId = $r[2]
    if ([string]::IsNullOrEmpty($parentId)) { $parentId = "None" }
    Write-Output "ID: $($r[0]) | Name: $($r[1]) | Parent: $parentId"
}
$r.Close()
Write-Output "Total items fetched: $count"
$c.Close()
