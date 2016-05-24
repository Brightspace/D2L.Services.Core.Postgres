$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
cd $projectDir

$solutionDir = (Get-Item ..).FullName
$project = (Get-Item .).Name

# Cleanup old packages
rm *.nupkg 2> $nul

# Get version number from AssemblyInfo.cs
$assemblyInfo = Get-Content .\Properties\AssemblyInfo.cs

$regex = [Regex]"AssemblyVersion\(\s*""(\d+\.\d+\.\d+).*""\s*\)"
$match = $regex.Match( $assemblyInfo )

if( !$match.Success ) {
	Write-Host
	Write-Host "Could not find version number in AssemblyInfo.cs" -ForegroundColor "red"
	cmd /C "pause"
	return
}

$version = $match.Groups[1].Value

# Make NuGet package
nuget pack "$project.csproj" -Version $version -Properties "Configuration=Release" -Build -NonInteractive

if( !$? ) {
	Write-Host
	Write-Host "Build failed" -ForegroundColor "red"
	cmd /C "pause"
	return
}

# Wait for user acknowledgement
Write-Host
Write-Host "Package built successfully." -ForegroundColor "green"
cmd /C "pause"
