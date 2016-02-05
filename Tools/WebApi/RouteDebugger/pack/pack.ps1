If (Test-Path .\Areas) {
	Remove-Item -Path .\Areas -Recurse -Force
}

Copy-Item -Path ..\src\Areas -Destination .\Areas -Recurse -Force

Get-ChildItem -Path *.cs -Recurse -File | foreach {
    Rename-Item $_ ($_.FullName + ".pp") -Force
}

Get-ChildItem -Path *.cshtml -Recurse -File | foreach {
    Rename-Item $_ ($_.FullName + ".pp") -Force
}

Get-ChildItem -Path *.cs.pp -Recurse -File | foreach {
    $content = Get-Content -Path $_
    $content | foreach { $_ -replace "RouteDebugger.Areas", '$rootnamespace$.Areas' } | Set-Content $_
}

..\src\.nuget\NuGet.exe pack WebApiRouteDebugger.nuspec