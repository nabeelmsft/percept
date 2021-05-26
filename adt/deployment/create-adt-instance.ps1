$rgname = "santacruz-rg"
$random = "santacruz" + $(get-random -maximum 10000)
$dtname = $random + "-digitaltwin"
$location = "westus2"
$username = "perceptuser@nabeelfta.onmicrosoft.com"
$functionstorage = $random + "storage"
$telemetryfunctionname = $random + "-telemetryfunction"
$twinupdatefunctionname = $random + "-twinupdatefunction"

#!!This command will fail for OCP Bootcamp participants since group already exists
az group create -n $rgname -l $location

$rgname
$dtname
$location
$username
$telemetryfunctionname
$twinupdatefunctionname
$functionstorage

az dt create --dt-name $dtname -g $rgname -l $location

az dt role-assignment create -n $dtname -g $rgname --role "Azure Digital Twins Data Owner" --assignee $username -o json
