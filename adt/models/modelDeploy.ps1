$dtname = "santacruz3203-digitaltwin"

$sitemodelid = "dtmi:percept:DigitalTwins:Site;1"
echo "Deleting Model:" 
echo $sitemodelid
az dt model delete --dt-name $dtname --dtmi $sitemodelid
$sitemodelid = $(az dt model create -n $dtname --models .\SiteInterface.json --query [].id -o tsv)

$sitefloormodelid = "dtmi:percept:DigitalTwins:SiteFloor;1"
echo "Deleting Model:"
echo  $sitefloormodelid
az dt model delete --dt-name $dtname --dtmi $sitefloormodelid
$sitefloormodelid = $(az dt model create -n $dtname --models .\SiteFloorInterface.json --query [].id -o tsv)

az dt twin delete-all --dt-name $dtname
echo "Creating twin: PerceptSite"
az dt twin create -n $dtname --dtmi $sitemodelid --twin-id "PerceptSite"
echo "Creating twin: PerceptSiteFloor"
az dt twin create -n $dtname --dtmi $sitefloormodelid --twin-id "PerceptSiteFloor"

$relname = "rel_has_floors"
echo "Creating relationships"
echo $relname
az dt twin relationship create -n $dtname --relationship $relname --twin-id "PerceptSite" --target "PerceptSiteFloor" --relationship-id "Site has floors"
