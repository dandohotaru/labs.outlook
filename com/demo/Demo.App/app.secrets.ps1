# targets
$regPath64 = "HKLM:\SOFTWARE\DEMO\Outlook\demo.com.addin"
$regPath32 = "HKLM:\SOFTWARE\WOW6432Node\DEMO\Outlook\demo.com.addin"

# variables
$openaiKeyName = "Openai:Key"
$openaiKeyValue = "..."

function Set-RegistryKey($path, $name, $value) {
    If (!(Test-Path $path)) {
        New-Item -Path $path -Force | Out-Null
    }
    Set-ItemProperty -Path $path -Name $name -Value $value -Type String
}

Set-RegistryKey $regPath64 $openaiKeyName $openaiKeyValue
Set-RegistryKey $regPath32 $openaiKeyName $openaiKeyValue
Write-Host "✅ Key stored successfully in both registry locations"
