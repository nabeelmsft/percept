# Azure Percept with Azure Digital twins

## Overview

## Architecture

## Prerequisites

- Azure Subcription
- Admin Access to Azure AD Tenant & Azure Subscription
- Mac OS: [PowerShell for Mac](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-macos?view=powershell-6 )
- Windows OS: PowerShell is built-in
- 

## Setup

### ADT Setup

#### ADT Instance Setup

```powershell

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

```

#### Model Setup

``` PowerShell
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

```

![Azure Digital Twin - Explorer view](./images/adt-digital-twin-explorer-model-view.PNG "Azure Digital Twin - Explorer view")

### Functions Apps Setup

#### Twins App

```powershell
dotnet add package Azure.DigitalTwins.Core
dotnet add package Azure.identity
dotnet add package System.Net.Http
```
