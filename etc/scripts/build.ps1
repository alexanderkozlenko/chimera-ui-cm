[string]$workspace = "$PSScriptRoot\..\.."
[string]$configuration = "Release"

& dotnet restore "$workspace\src\"

If ($LASTEXITCODE -ne 0) {
    Throw "Restore command failed with code $LASTEXITCODE"
}

& dotnet build "$workspace\src\" --configuration $configuration

If ($LASTEXITCODE -ne 0) {
    Throw "Build command failed with code $LASTEXITCODE"
}

& dotnet test "$workspace\src\Chimera.UI.ComponentModel.Tests\Chimera.UI.ComponentModel.Tests.csproj" --configuration $configuration

If ($LASTEXITCODE -ne 0) {
    Throw "Test command failed with code $LASTEXITCODE"
}

& dotnet pack "$workspace\src\Chimera.UI.ComponentModel\Chimera.UI.ComponentModel.csproj" --no-build --configuration $configuration

If ($LASTEXITCODE -ne 0) {
    Throw "Pack command failed with code $LASTEXITCODE"
}