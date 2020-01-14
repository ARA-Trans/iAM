# === Configuration === #
$deploymentScriptPath = Get-Location;
$iamRepoPath = (Get-Item $deploymentScriptPath).parent.FullName;
$deploymentOutputPath = "$deploymentScriptPath\Deployment_Packages";
$configurations = "$deploymentScriptPath\configurations";
$npmLogPath = "$deploymentScriptPath\npmLogs";
# === End Configuration === #

Clear-Host;

Write-Host "This script will create a deployment package for PennDOT and for iAM-deploy.";
Write-Host "The package will include fully-configured packages for all environments.`n`n";

Write-Host "The iAM repo is located at:`n    $iamRepoPath`n";
Write-Host "The deployment package will be at:`n    $deploymentOutputPath`n";
Write-Host "This script is located at`n    $deploymentScriptPath`n";
Write-Host "The logs from the latest VueJS app deployments can be found at`n    $npmLogPath`n";

Write-Host "Warning: This script does not compile the C# app! You must publish it manually from Visual Studio." -ForegroundColor Yellow -BackgroundColor Red;
Write-Host "This script packages Visual Studio's output into a fully-configured deployment package." -ForegroundColor Yellow -BackgroundColor Red;

$cSharpBuiltConfirmation = Read-Host 'Confirm that you have published the C# app by typing "confirm" here';

If ($cSharpBuiltConfirmation -ne 'confirm') {
    exit;
}

$version = Read-Host -Prompt 'Input the version name for this deployment. (Ex. "12192019")';

$startTimeSeconds = (Get-Date).Second;
$startTimeMinutes = (Get-Date).Minute;

$devPath = "$deploymentOutputPath\dev\$version\";
$systPath = "$deploymentOutputPath\syst\$version\";
$prodPath = "$deploymentOutputPath\prod\$version\";
$iamDeployPath = "$deploymentOutputPath\iam-deploy\$version\";

# === Make sure npm log folder exists === #

If (Test-Path $npmLogPath) {
    Remove-Item $npmLogPath -Recurse *>$null;
}
New-Item -Path $npmLogPath -ItemType Directory *>$null;

# === Make sure deploymentOutputPath and its subfolders exist === #

If (-Not (Test-Path "$deploymentOutputPath")) {
    New-Item -Path "$deploymentOutputPath" -ItemType Directory *>$null;
}
If (-Not (Test-Path "$deploymentOutputPath\dev")) {
    New-Item -Path "$deploymentOutputPath\dev" -ItemType Directory *>$null;
}
If (-Not (Test-Path "$deploymentOutputPath\syst")) {
    New-Item -Path "$deploymentOutputPath\syst" -ItemType Directory *>$null;
}
If (-Not (Test-Path "$deploymentOutputPath\prod")) {
    New-Item -Path "$deploymentOutputPath\prod" -ItemType Directory *>$null;
}
If (-Not (Test-Path "$deploymentOutputPath\iam-deploy")) {
    New-Item -Path "$deploymentOutputPath\iam-deploy" -ItemType Directory *>$null;
}

# === Remove any duplicate deployment, create folders for new deployment === #

If (Test-Path $devPath) {
    Remove-Item $devPath -Recurse *>$null;
}
New-Item -Path $devPath `
    -ItemType Directory *>$null;
New-Item -Path "$devPath\VueJS" `
    -ItemType Directory *>$null;
New-Item -Path "$devPath\NodeJS" `
    -ItemType Directory *>$null;
New-Item -Path "$devPath\C#" `
    -ItemType Directory *>$null;
If (Test-Path $systPath) {
    Remove-Item $systPath -Recurse *>$null;
}
New-Item -Path $systPath `
    -ItemType Directory *>$null;
New-Item -Path "$systPath\VueJS" `
    -ItemType Directory *>$null;
New-Item -Path "$systPath\NodeJS" `
    -ItemType Directory *>$null;
New-Item -Path "$systPath\C#" `
    -ItemType Directory *>$null;
If (Test-Path $prodPath) {
    Remove-Item $prodPath -Recurse *>$null;
}
New-Item -Path $prodPath `
    -ItemType Directory *>$null;
New-Item -Path "$prodPath\VueJS" `
    -ItemType Directory *>$null;
New-Item -Path "$prodPath\NodeJS" `
    -ItemType Directory *>$null;
New-Item -Path "$prodPath\C#" `
    -ItemType Directory *>$null;
If (Test-Path $iamDeployPath) {
    Remove-Item $iamDeployPath -Recurse *>$null;
}
New-Item -Path $iamDeployPath `
    -ItemType Directory *>$null;
New-Item -Path "$iamDeployPath\VueJS" `
    -ItemType Directory *>$null;
New-Item -Path "$iamDeployPath\NodeJS" `
    -ItemType Directory *>$null;
New-Item -Path "$iamDeployPath\C#" `
    -ItemType Directory *>$null;

Clear-Host

Write-Host "Beginning deployment: $version`n"

# ======================================= C# ======================================= #

Write-Host '===== Beginning C# Deployments =====' -ForegroundColor White;

Set-Location "$iamRepoPath\BridgeCareApp\BridgeCare\bin\release";

Write-Host 'Removing PCI Files...' -ForegroundColor Yellow;

If (Test-Path ".\publish\bin\PCI.dll") {
    Remove-Item ".\publish\bin\PCI.dll";
}
If (Test-Path ".\publish\bin\PCI.pdb") {
    Remove-Item ".\publish\bin\PCI.pdb";
}

$cSharpExclusions = "connection.config", "esec.config";

Write-Host 'Copying C# App...' -ForegroundColor Yellow;

Copy-Item -Path `
    (Get-Item -Path ".\publish\*" -Exclude $cSharpExclusions).FullName `
    -Destination "$devPath\C#" -Recurse -Force;

Copy-Item -Path `
    (Get-Item -Path ".\publish\*" -Exclude $cSharpExclusions).FullName `
    -Destination "$systPath\C#" -Recurse -Force;

Copy-Item -Path `
    (Get-Item -Path ".\publish\*" -Exclude $cSharpExclusions).FullName `
    -Destination "$prodPath\C#" -Recurse -Force;

Copy-Item -Path `
    (Get-Item -Path ".\publish\*" -Exclude $cSharpExclusions).FullName `
    -Destination "$iamDeployPath\C#" -Recurse -Force;

Write-Host 'Setting ESEC Configurations...' -ForegroundColor Yellow;

Copy-Item "$configurations\bamsdev\esec.config" `
    -Destination "$devPath\C#\esec.config";

Copy-Item "$configurations\bamssyst\esec.config" `
    -Destination "$systPath\C#\esec.config";

Copy-Item "$configurations\bams\esec.config" `
    -Destination "$prodPath\C#\esec.config";

Copy-Item "$configurations\iam-deploy\esec.config" `
    -Destination "$iamDeployPath\C#\esec.config";

Write-Host "C# Deployments Completed.`n" -ForegroundColor Green;

# ======================================= NODE ======================================= #

Write-Host '===== Beginning Node Deployments =====' -ForegroundColor White;

Set-Location "$iamRepoPath\BridgeCareApp";

$nodeExclusions = ".vscode", "node_modules";

Write-Host 'Beginning "bamsdev" NodeJS Deployment...' -ForegroundColor Yellow;

Copy-Item -Path `
    (Get-Item -Path ".\BridgeCareNodeApp\*" -Exclude $nodeExclusions).FullName `
    -Destination "$devPath\NodeJS" -Recurse -Force;

Write-Host 'Completed "bamsdev" NodeJS Deployment.' -ForegroundColor Green;

Write-Host 'Beginning "bamssyst" NodeJS Deployment...' -ForegroundColor Yellow;

Copy-Item -Path `
    (Get-Item -Path ".\BridgeCareNodeApp\*" -Exclude $nodeExclusions).FullName `
    -Destination "$systPath\NodeJS" -Recurse -Force;

Write-Host 'Completed "bamssyst" NodeJS Deployment.' -ForegroundColor Green;

Write-Host 'Beginning "bams" [PROD] NodeJS Deployment...' -ForegroundColor Yellow;

Copy-Item -Path `
    (Get-Item -Path ".\BridgeCareNodeApp\*" -Exclude $nodeExclusions).FullName `
    -Destination "$prodPath\NodeJS" -Recurse -Force;

Write-Host 'Configuring "bams" [PROD] NodeJS Deployment...' -ForegroundColor Yellow;

Remove-Item "$prodPath\NodeJS\authorization\authorizationConfig.js";

Copy-Item -Path "$configurations\bams\authorizationConfig.js" `
    -Destination "$prodPath\NodeJS\authorization";

Write-Host 'Completed "bams" [PROD] NodeJS Deployment.' -ForegroundColor Green;

Write-Host 'Beginning "iAM-deploy" NodeJS Deployment...' -ForegroundColor Yellow;

Copy-Item -Path `
    (Get-Item -Path ".\BridgeCareNodeApp\*" -Exclude $nodeExclusions).FullName `
    -Destination "$iamDeployPath\NodeJS" -Recurse -Force;

Write-Host 'Completed "iAM-deploy" NodeJS Deployment.' -ForegroundColor Green;

Write-Host "NodeJS Deployments Completed.`n" -ForegroundColor Green;

# ======================================= VUE ======================================= #

Write-Host '===== Beginning VueJS Deployments =====' -ForegroundColor White;

Set-Location "$iamRepoPath\BridgeCareApp\VuejsApp";

# === bamsdev ===
Write-Host 'Beginning "bamsdev" VueJS Deployment...' -ForegroundColor Yellow;

Copy-Item "$configurations\bamsdev\.env.production" `
    -Destination ".\.env.production";
Copy-Item "$configurations\bamsdev\oidc-config.ts" `
    -Destination ".\src\oidc-config.ts";

Write-Host "Building app... Build log can be found at`n$npmLogPath\bamsdev.txt" -ForegroundColor Yellow;
npm run build-prod *> $npmLogPath\bamsdev.txt;

Copy-Item -Path ".\dist\*" `
    -Destination "$devPath\VueJS" `
    -Recurse;

Write-Host 'Completed "bamsdev" VueJS Deployment...' -ForegroundColor Green;

# === bamssyst ===
Write-Host 'Beginning "bamssyst" VueJS Deployment...' -ForegroundColor Yellow;

Copy-Item "$configurations\bamssyst\.env.production" `
    -Destination ".\.env.production";
Copy-Item "$configurations\bamssyst\oidc-config.ts" `
    -Destination ".\src\oidc-config.ts";

Write-Host "Building app... Build log can be found at`n$npmLogPath\bamssyst.txt" -ForegroundColor Yellow;
npm run build-prod *> $npmLogPath\bamssyst.txt;

Copy-Item -Path ".\dist\*" `
    -Destination "$systPath\VueJS" `
    -Recurse;

Write-Host 'Completed "bamssyst" VueJS Deployment...' -ForegroundColor Green;

# === bams [PROD] ===
Write-Host 'Beginning "bams" [PROD] VueJS Deployment...' -ForegroundColor Yellow;

Copy-Item "$configurations\bams\.env.production" `
    -Destination ".\.env.production";
Copy-Item "$configurations\bams\oidc-config.ts" `
    -Destination ".\src\oidc-config.ts";

Write-Host "Building app... Build log can be found at`n$npmLogPath\bams.txt" -ForegroundColor Yellow;
npm run build-prod *> $npmLogPath\bams.txt;

Copy-Item -Path ".\dist\*" `
    -Destination "$prodPath\VueJS" `
    -Recurse;

Write-Host 'Completed "bams" [PROD] VueJS Deployment...' -ForegroundColor Green;

# === iAM-deploy ===
Write-Host 'Beginning "iAM-deploy" VueJS Deployment...' -ForegroundColor Yellow;

# iam-deploy build does not require a change to .env.production, as it uses .env.staging
Copy-Item "$configurations\iam-deploy\oidc-config.ts" `
    -Destination ".\src\oidc-config.ts";

Write-Host "Building app... Build log can be found at`n$npmLogPath\iam-deploy.txt" -ForegroundColor Yellow;
npm run stage *> $npmLogPath\iam-deploy.txt;

Copy-Item -Path ".\dist\*" `
    -Destination "$iamDeployPath\VueJS" `
    -Recurse;

Write-Host 'Completed "iAM-deploy" VueJS Deployment...' -ForegroundColor Green;

Write-Host "VueJS Deployments Completed`n" -ForegroundColor Green;

$endTimeSeconds = (Get-Date).Second;
$endTimeMinutes = (Get-Date).Minute;

Read-Host -Prompt "Finished in $($endTimeMinutes - $startTimeMinutes) minutes $($endTimeSeconds - $startTimeSeconds) seconds.";