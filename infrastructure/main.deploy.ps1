az bicep build --file .\main.bicep

az deployment group create --name "Main-$(Get-Date -Format "MMddyyyy-HHmmss")" --resource-group rlitest --template-file main.bicep --parameters @parameters\main.parameters.json