Param(
	[parameter( Mandatory = $false )][string] $pgBinPath = "C:\Program Files\PostgreSQL\9.5\bin"
)

$script:thisDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

$script:path = [Environment]::GetEnvironmentVariable( "Path" , "Process" )
$script:path += ";" + $pgBinPath
[Environment]::SetEnvironmentVariable( "Path" , $script:path, "Process" )

[Environment]::SetEnvironmentVariable( "PGPASSWORD" , "postgres", "Process" )

dropdb --if-exists -w -U postgres postgres-library-test
Write-Host
