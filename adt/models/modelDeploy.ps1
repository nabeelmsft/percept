$dtname = "santacruz3203-digitaltwin"
$sitemodelid = $(az dt model create -n $dtname --models .\SiteInterface.json --query [].id -o tsv)
az dt twin create -n $dtname --dtmi $sitemodelid --twin-id "PerceptSite"

