# NOTICE - READ THIS FIRST -
This project has been moved to a public Azure DevOps project located here: 

https://ara-cu.visualstudio.com/Infrastructure%20Asset%20Management

Please contact iam-support@ara.com for more information.

# Welcome to Infrastructure Asset Management
This application is designed to assist transportation agencies determine where best to perform maintenance, rehabilitation, and replacement of their transportation assets such as bridges and pavements.  We are currently preparing the included version which is a desktop application for use as a web based application.  If you would like to know more about this product or are looking to participate in its development, please contact the project team.

## Setup
There are three code bases at this time:  the desktop app which is stand alone and a pair of web based apps.  The API app has been developed in ASP.NET Core 2 MVC (with references to the full .Net Framework for compatibility with the desktop app) and provides the data and analysis services that are the core of the application.  The UI app has been developed using Vue.js and provides an interface to the services provided by the API app.

### Desktop
The desktop app is fully independent and can be downloaded and compiled with Visual Studio 2017.

### API
The API app has been developed in Visual Studio 2017.  However, it does require the developer or the user to have a connection.config file in the root project directory ().  That file should have the following information:

```
<connectionStrings>
  <add name="SimpleTestEntities" connectionString="metadata=res://*/SimpleTest.csdl|res://*/SimpleTest.ssdl|res://*/SimpleTest.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source={Your Database Server};initial catalog={Your Database};persist security info=True;user id={DB User Name};password={DB User Password};MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
</connectionStrings>
```

Because of security issues, this file should *never* be uploaded to the repository.

### UI
More information coming soon.
