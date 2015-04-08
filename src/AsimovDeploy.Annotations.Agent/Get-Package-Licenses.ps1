@( Get-Project -All |
  ? { $_.ProjectName } |
  % { Get-Package -ProjectName $_.ProjectName } ) |
  Sort -Unique |
  % { $pkg = $_ ; Try { (New-Object System.Net.WebClient).DownloadFile($pkg.LicenseUrl, 'c:\dev\licenses\' + $pkg.Id + ".txt") } Catch [system.exception] { Write-Host "Could not download license for $pkg" } }