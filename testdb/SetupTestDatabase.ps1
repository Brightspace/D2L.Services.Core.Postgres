Param(
	[parameter( Mandatory = $false )][string] $pgBinPath = "C:\Program Files\PostgreSQL\12\bin"
)

$script:thisDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

$script:path = [Environment]::GetEnvironmentVariable( "Path" , "Process" )
$script:path += ";" + $pgBinPath
[Environment]::SetEnvironmentVariable( "Path" , $script:path, "Process" )

[Environment]::SetEnvironmentVariable( "PGPASSWORD" , "postgres", "Process" )

dropdb --if-exists -w -U postgres postgres-library-test
Write-Host
createdb -we -U postgres postgres-library-test
Write-Host

psql -weq -U postgres -d postgres-library-test -f "$script:thisDir\setup.sql" -1
Write-Host
