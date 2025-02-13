$regPath64 = "HKLM:\SOFTWARE\DEMO\Outlook\demo.com.addin"
$regPath32 = "HKLM:\SOFTWARE\WOW6432Node\DEMO\Outlook\demo.com.addin"
$openaiApiKey = "..."

function Set-RegistryKey($path, $name, $value) {
    If (!(Test-Path $path)) {
        New-Item -Path $path -Force | Out-Null
    }
    Set-ItemProperty -Path $path -Name $name -Value $value -Type String
}

Set-RegistryKey $regPath64 "OpenaiApiKey" $openaiApiKey
Set-RegistryKey $regPath32 "OpenaiApiKey" $openaiApiKey
Write-Host "✅ Key stored successfully in both registry locations"
