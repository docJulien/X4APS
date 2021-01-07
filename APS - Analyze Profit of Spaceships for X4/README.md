# Google-Timeline-Analyser
HUUUUGE CREDIT TO magictrip https://github.com/magictripgames/X4MagicTripLogAnalyzer

## Installation Requirements:
 - MySQL, create a database and make sure the connection string in appsettings.json are valid. I created a bd using mysql workbench with shema name x4aps and user aps
 - Kendo MVC Core 2019.3 or later. Import the kendo mvc wrapper in the project and include kendo in the wwwroot/lib folder. It is configured in the libman.json already.
 - Initial data is seeded, an admin account and roles are created on application start. 
   admin:Secret1234!
 - .net core entity framework tools for CLI
     run the command in Package Manager Console: 
     dotnet tool install --global dotnet-ef
     dotnet ef database update --project APS
     
   
## Features:
 - microsoft identity login and user management with roles
 - data separated by user
 - log management
 - report page showing a graph for activities

## Usage:
 - start it and upload a file

## Notes:
 - beta version

## TODO features:
 - easy installation package for windows and linux
 - add more statistics
-use db instead of json files for data and wares.json
-remember last used path
