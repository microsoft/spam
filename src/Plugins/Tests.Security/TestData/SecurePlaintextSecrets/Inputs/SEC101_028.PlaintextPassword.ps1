# Example from https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.security/get-credential?view=powershell-7.1
 # Implemented in PSCredentialPowershell
 $PWord = ConvertTo-SecureString -String "doodle_BLOB0000" -AsPlainText -Force
 $Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $User, $PWord

# Copied and modified from CodeAsData
ConvertTo-SecureString -String 'Doodle$111111' -AsPlainText -Force PS C:\&gt; Add-AzureRmServiceFabricNodeType -ResourceGroupName 'Group1' -Name 'someContosoCluster' -NodeType 'n2' -Capacity 5 -VmUserName 'someName' -VmPassword $pwd</dev:code>         <dev:remarks>           <maml:para>This command will add a new NodeType 'n2' w
ConvertTo-SecureString -String 'D00dle2222' -AsPlainText -Force))  #Set-TargetResource "Present" "test" "C:\Builds\vs_ultimate.exe" ""  "/quiet /noweb" "HKLM:\some\path" (New-Object System.Management.Automation.PSCredential -ArgumentList 'fareast\lmtstlab', (ConvertTo-SecureStr
ConvertTo-SecureString -String 'D$oodle3333' -AsPlainText -Force)  New-Mailbox -Organization url.com -Name "serverName1"  -WindowsLiveID "email@address.domain.com" -Password (ConvertTo-SecureString -String 'D$oodle4444' -AsPlainText -Force)  New-Mailbox -Organization url.com -Name "name
ConvertTo-SecureString -String 'Doodle$555555' -AsPlainText -Force PS C:\&gt; Add-AzureRmServiceFabricNodeType -ResourceGroupName 'Group1' -Name 'someContosoCluster' -NodeType 'n2' -Capacity 5 -VmUserName 'someName' -VmPassword $pwd</dev:code>         <dev:remarks>           <maml:para>This command will add a new NodeType 'n2' w
ConvertTo-SecureString -String "666666" -AsPlainText -Force PS c:\&gt; add-AzServiceFabricClusterCertificate -ResourceGroupName 'Group2' -Name 'someContosoCluster'  -CertificateSubjectName 'Contoso.com'  -CertificateOutputFolder 'c:\test' -CertificatePassword $pwd</dev:code>         <dev:remarks>           <maml:para>This command will
ConvertTo-SecureString -String '6ood137777' -AsPlainText -Force))  #Set-TargetResource "Present" "test" "C:\Builds\vs_ultimate.exe" ""  "/quiet /noweb" "HKLM:\SOFTWARE\Wow6432Node\Microsoft\DevDiv\vs\Servicing\12.0\ultimate1" (New-Object System.Management.Automation.PSCredential -ArgumentList 'domain\account', (ConvertTo-SecureStr"
"commandToExecute": "[concat('powershell.exe -ExecutionPolicy bypass \"& ./artifact.ps1 -StringParam ''', parameters('stringParam'), ''' -SecureStringParam (ConvertTo-SecureString ''', parameters('securestringParam'), ''' -AsPlainText -Force) -IntParam ', parameters('intParam'), ' -BoolParam:$', parameters('boolParam'), ' -FileContentsParam ''', parameters('fileContentsParam'), ''' -ExtraLogLines ', parameters('extraLogLines'), ' -ForceFail:$', parameters('forceFail'), '\"')]"

# These should not be matched
ConvertTo-SecureString -String "$($redisResourceName).redis.cache.windows.net:6380,password=$($redisKey),ssl=True,abortConnect=False" -AsPlainText -Force     .\Add-KeyVaultSecret.ps1 `         -KeyVaultName $KeyVaultName `         -SecretName ($secretName) `         -SecretValue $redisConnectionString `         -SecretExpiry $Secre
ConvertTo-SecureString -String '$(adminPassword)' -AsPlainText -Force) -adminUsername $(adminUserName)",       }     },     {       "enabled": true,       "continueOnError": true,       "timeoutInMinutes": 0,       "task": {         "id": "some-guid",         "versionSpec": "1.*",         "definitionType""
ConvertTo-SecureString -String "$(Password)" -AsPlainText -Force))) -TestResultsFile (Join-Path "$(System.DefaultWorkingDirectory)" "TestResults.xml") -test "unittests"'  - task: PublishTestResults@2 displayName: 'Publish Test Results' inputs: testResultsFormat: XUnit testResultsFiles: TestResults.xml  - task: PublishBu