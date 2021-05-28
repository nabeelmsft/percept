$rgname = "santacruz-rg"
$functionstorage = "shalimarstorage"
$telemetryfunctionname = "shalimar-telemetryfunction"
$twinupdatefunctionname = "shalimar-twinupdatefunction"
$location = "westus2"
$username = "perceptuser@nabeelfta.onmicrosoft.com"
$adthostname = "santacruz3203-digitaltwin.api.wus2.digitaltwins.azure.net"

echo "Creating injestion function app"
az functionapp create --resource-group $rgname --consumption-plan-location $location --name $telemetryfunctionname --storage-account $functionstorage --functions-version 3

echo "Getting service principal for the injestion app"
$principalID = $(az functionapp identity assign -g $rgname -n $telemetryfunctionname  --query principalId)

echo "Assign the function app's identity to the Azure Digital Twins Data Owner role for Azure Digital Twins instance"
az dt role-assignment create --dt-name $dtname --assignee $principalID --role "Azure Digital Twins Data Owner"

az functionapp config appsettings set -g $rgname -n $telemetryfunctionname --settings "ADT_SERVICE_URL=https://" + $adthostname

