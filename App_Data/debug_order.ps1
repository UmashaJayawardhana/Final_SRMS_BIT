$c = New-Object System.Data.SqlClient.SqlConnection("Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true")
$c.Open()
$cmd = $c.CreateCommand()
$cmd.CommandText = @"
    SELECT m.MenuId, m.MenuName, p.MenuName AS ParentMenuName, m.DisplayOrder
    FROM AppMenu m
    LEFT JOIN AppMenu p ON m.ParentMenuId = p.MenuId
    ORDER BY m.DisplayOrder ASC
"@
$r = $cmd.ExecuteReader()
Write-Output "--- Query Result ordered by DisplayOrder ---"
$index = 0
while ($r.Read()) {
    $index = $index + 1
    $parent = if ($r[2] -eq [DBNull]::Value) { "None" } else { $r[2].ToString() }
    Write-Output "Row $($index) : ID=$($r[0]) | Name=$($r[1]) | Parent=$($parent) | DisplayOrder=$($r[3])"
}
$r.Close()
$c.Close()
