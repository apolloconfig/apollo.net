function FindMSBuild () {
    foreach ($program in ("C:\Program Files", "C:\Program Files (x86)")) {
        foreach ($vs in (Get-ChildItem ($program + "\Microsoft Visual Studio"))) {
            foreach ($vsv in (Get-ChildItem $vs.FullName)) {
                if (Test-Path ($vsv.FullName + "\MSBuild\Current\Bin\MSBuild.exe")) {
                    return $vsv.FullName + "\MSBuild\Current\Bin\MSBuild.exe";
                }
            }
        }
    }

    return $env:windir + "\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
}

if (-not(Test-Path .packages)) {
    mkdir .packages
}

$MSBuild = FindMSBuild
& "$MSBuild" /r /m /v:m /p:Configuration=Release

foreach ($csproj in (Get-ChildItem -r -filter *.csproj)) {
    $nupkg = Get-ChildItem "$([System.IO.Path]::GetDirectoryName($csproj.FullName))\bin\Release" |
    Where-Object { $_.Name.Endswith(".symbols.nupkg") } |
    Sort-Object -Property LastWriteTime -Descending |
    Select-Object -First 1;

    if ($null -ne $nupkg) {
        Copy-Item $nupkg.VersionInfo.FIleName (".packages\" + $nupkg.Name) -Force
    }
}
