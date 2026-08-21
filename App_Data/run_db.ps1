$connString = "Data Source=LAPTOP-5979LPPL\SQLEXPRESS;Initial Catalog=SRMS_DB;Integrated Security=true"
$sqlFile = "d:\BIT\MvcApplication1\MvcApplication1\App_Data\db_setup.sql"
$sqlText = Get-Content -Raw -Path $sqlFile

# Split by GO keyword on its own line
$batches = [System.Text.RegularExpressions.Regex]::Split($sqlText, "(?im)^\s*GO\s*\r?\n")

$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

foreach ($batch in $batches) {
    $trimmed = $batch.Trim()
    if ($trimmed.Length -gt 0) {
        $command = New-Object System.Data.SqlClient.SqlCommand($trimmed, $connection)
        try {
            $command.ExecuteNonQuery() > $null
        } catch {
            Write-Error "Failed to execute batch: $_"
            throw
        }
    }
}

$connection.Close()
Write-Output "SQL executed successfully!"
