To use the deployment script, first you need to place the appropriate esec.config files in the 
configurations/bams, configurations/bamsdev, configurations/bamssyst, configurations/iam-deploy folders.

Then, make sure to publish the C# app in visual studio.

Finally, right click the deployment script, deploy.ps1, and select "Run with PowerShell".

The script will ask you to confirm that you have published the C# app, and will ask for a name for the
deployment. 

The script will then run for 1 - 3 minutes, and when it is done, each of the folders in the Deployment_Packages
folder (dev, syst, prod, iam-deploy) will contain a folder with the specified name, containing the deployment
package for that environment.