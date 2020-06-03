Write-Output "Starting PowerShell module build..."

$project = Get-Content -Path ".\projectConfig.json" -Raw -ErrorAction "Stop" | ConvertFrom-Json

switch ((Test-Path -Path ".\build\")) {
    $false {
        $null = New-Item -Path ".\build\" -ItemType "Directory" -ErrorAction "Stop"
        break
    }
}

$buildDir = Join-Path -Path ".\build\" -ChildPath $project.ModuleName

switch ((Test-Path -Path $buildDir)) {
    $true {
        Write-Output "Cleaning the build directory..."
        Remove-Item -Path $buildDir -Force -Recurse
        break
    }

    Default {
        Write-Output "Build directory already clear of project files..."
        break
    }
}

Write-Output "Building..."

$null = New-Item -Path $buildDir -ItemType "Directory" -ErrorAction Stop
Start-Process -FilePath "dotnet" -ArgumentList @("clean") -Wait
Start-Process -FilePath "dotnet" -ArgumentList @("publish", "/property:PublishWithAspNetCoreTargetManifest=false") -Wait

Write-Output "Copying compiled files to the build directory..."
Copy-Item -Path "./bin/Debug/netstandard2.0/publish/pwsh_graph_connect.dll" -Destination $buildDir
Copy-Item -Path "./bin/Debug/netstandard2.0/publish/Microsoft.Identity.Client.dll" -Destination $buildDir

<#
Because of the differences in PowerShell 6 and higher for `ConvertFrom-Json`, we have to specify two ways to import the project config into a hashtable:
## PowerShell 5.1
`ConvertFrom-Json` only supports converting the JSON input into a PSObject, so we have to import it in normally and then convert each 'NoteProperty' member into a Hashtable.
## PowerShell 6 and higher
`ConvertFrom-Json` natively supports converting the JSON input into a Hashtable, so no further conversion is necessary.
#>

Write-Output "Importing module manifest settings..."
$ConfigContentSplat = @{
    "Path"        = ".\buildConfig.json";
    "Raw"         = $true;
    "ErrorAction" = "Stop";
}
switch ($PSVersionTable.PSVersion.Major -le 5) {
    $true {
        $baseProjectConfig = Get-Content @ConfigContentSplat | ConvertFrom-Json
        
        $projectConfig = [hashtable]::new()
        foreach ($member in ($baseProjectConfig.PSObject.Members | Where-Object { $PSItem.MemberType -eq "NoteProperty" })) {
            $projectConfig.Add($member.Name, $member.Value)
        }
        break
    }

    Default {
        $projectConfig = Get-Content @ConfigContentSplat | ConvertFrom-Json -AsHashtable
        break
    }
}

Write-Output "Creating module manifest..."
New-ModuleManifest @projectConfig

Write-Output "Module is located in: '$($buildDir)'"
Write-Output "Done."