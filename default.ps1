properties {
	$base_dir  = resolve-path .
	$lib_dir = "$base_dir\libs"
	$build_dir = "$base_dir\build_artifacts"
	$buildartifacts_dir = "$build_dir\"
	$sln_file = "$base_dir\AsimovAnnotations.sln"
	$tools_dir = "$base_dir\tools"
	$configuration = "Release"
	$drop_folder = "$base_dir\build_artifacts\drop"
	$version = "1.0"
	$commit = "1234567"
	$branch = "master"
	$build = "10"
    $nuget = "$base_dir\tools\nuget\nuget.exe"
    $nugetConfig = "$base_dir\tools\nuget\nuget.config"
    $nugetPublic = "https://nuget.org/api/v2/"
}

task default -depends Release

task Clean {
	Remove-Item -force -recurse $build_dir -ErrorAction SilentlyContinue
}

task Init -depends Clean {
	$script:version = "$version.$build"
	$script:commit = $commit.substring(0,7)

	Write-Host "##teamcity[buildNumber '$script:version']"

	exec { git update-index --assume-unchanged "$base_dir\src\SharedAssemblyInfo.cs" }
	(Get-Content "$base_dir\src\SharedAssemblyInfo.cs") |
		Foreach-Object { $_ -replace "{version}", $script:version } |
		Set-Content "$base_dir\src\SharedAssemblyInfo.cs" -Encoding UTF8

	New-Item $build_dir -itemType directory -ErrorAction SilentlyContinue | Out-Null
}

function CreateZipFile([string] $name, [string] $folder) {
	$zipFile = "$base_dir\build_artifacts\packages\$name-v$script:version-[$branch]-[$script:commit].zip"
	$folderToZip = "$base_dir\build_artifacts\packages\$name\*"

	exec {
		& $tools_dir\7zip\7z.exe a -tzip `
			$zipFile `
			$folderToZip
	}
}

function Create-DirectoryIfMissing($path) {
        if (-not (Test-Path $path)) {
            mkdir $path | Quiet
        }
        return $path
}

function Quiet {
    [CmdletBinding()]
param(
    [Parameter(ValueFromPipeline=$True)]
    $output
)
    Process {
        if ($Verbose) {
            Write-Host $output
        }
    }
}
function Get-Directories {
    Get-ChildItem -path .\*\*,.\* | ?{ $_.PSIsContainer }
}

function Get-PackageConfigs {
    Get-Directories | Get-ChildItem -Filter packages.config
}

Task ChocolateyPackage {
    $chocolatey_output_dir = Create-DirectoryIfMissing "$base_dir\build_artifacts\Chocolatey"

    $old = pwd
    cd "$base_dir\build_artifacts"
    Get-ChildItem -Filter *.Chocolatey.nuspec | ForEach-Object {
        $nuspecContent =  [xml](Get-Content -Path $_.FullName)
        $packageName = $nuspecContent.package.metadata.id
        $versionChoco = $nuspecContent.package.metadata.version

        $dir = Create-DirectoryIfMissing "$chocolatey_output_dir\$packageName"

        Write-Host "Packaging chocolatey package: " $packagename $version -ForegroundColor Green
        & $nuget Pack $_.FullName -OutputDirectory "$dir" | Out-Null
        $package = Resolve-Path "$dir\$packagename*.nupkg"
    }
    cd $old
}

Task NugetRestore -depends Init {
    $packagesDir = "$base_dir\packages"
    Create-DirectoryIfMissing $packagesDir
    $old = pwd
    Get-PackageConfigs | ForEach-Object {
        & $nuget restore $($_.FullName) -Source "$nugetPublic" -ConfigFile $nugetConfig -PackagesDirectory $packagesDir
    }
    cd $old
}

task CopyAsimovDeployAnnotations {

    $include = @("*.exe", "*.dll", "*.config")
	Copy-Item "$build_dir\*" "$build_dir\packages\AsimovDeploy.Annotations" -Recurse -Force -include $include

	CreateZipFile("AsimovDeploy.Annotations")
}

task CreateDistributionPackage {
	New-Item $build_dir\packages\AsimovAnnotations -Type directory -ErrorAction SilentlyContinue | Out-Null
	Copy-Item "$build_dir\packages\*.zip" "$build_dir\packages\AsimovAnnotations" -Force -ErrorAction SilentlyContinue

	$licenseFiles = @('LICENSE', "NOTICE", "library-licenses")
	Copy-Item "$base_dir\*" "$build_dir\packages\AsimovAnnotations" -Recurse -Force -include $licenseFiles

	CreateZipFile("AsimovAnnotations")
}

task Compile -depends Init, NugetRestore {

	$v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name

	try {
		Write-Host "Compiling with '$configuration' configuration"
		exec { & msbuild "$sln_file" /p:OutDir="$buildartifacts_dir\" /p:Configuration=$configuration }
	} catch {
		Throw
	} finally {
		exec { git checkout "$base_dir\src\SharedAssemblyInfo.cs" }
	}
}

task Test -depends Compile {

}

task Release -depends DoRelease

task CreateOutputDirectories {
	New-Item $build_dir\packages -Type directory -ErrorAction SilentlyContinue | Out-Null
	New-Item $build_dir\packages\AsimovDeploy.Annotations -Type directory | Out-Null
	New-Item $build_dir\drop -Type directory | Out-Null
}

task CopyToDropFolder {
	Write-Host "Copying to drop folder $drop_folder"

	Create-DirectoryIfMissing "$drop_folder"

	Copy-Item "$build_dir\packages\AsimovDeploy.Annotations-*.zip" "$drop_folder" -Force -ErrorAction SilentlyContinue
}

task DoRelease -depends Compile, `
	CreateOutputDirectories, `
	CopyAsimovDeployAnnotations, `
	#CreateDistributionPackage, `
	ChocolateyPackage, `
	CopyToDropFolder {
	Write-Host "Done building AsimovDeploy"
}
