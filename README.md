# Speakeasy
Self hosted text chat

# Building
For frontend instructions see [this readme](Speakeasy.WebUI/README.md).

1) Open the solution in Visual Studio or Jetbrains Rider
2) The provided docker-compose.yml file hosts the Postgres database. Bring it up.
   1) A convient Run Configuration has been created to start the Docker compose in Rider.
3) In the Speakeasy.Server folder, copy `appsettings.Development.json` and paste it as `appsettings.local.json`.
   1) If you changed any database information in step 2, reflect it in the `appsettings.local.json` file.
4) Launch the Speakeasy.Server program

# Database Setup

- Ensure dotnet efcore tools are installed. `dotnet tool install --global dotnet-ef`
- Add dotnet ef tools to the path. `export PATH=$PATH:$HOME/.dotnet/tools`
- Update the database.
  - From project root, run `dotnet ef database update --project Speakeasy.Server.Models/Speakeasy.Server.Models.csproj --startup-project Speakeasy.Server.Models/Speakeasy.Server.Models.csproj --context Speakeasy.Server.Models.Database.SpeakeaseDbContext --configuration Debug --connection "<postgresql connection string>" <target migration name (latest in Speakeasy.Server.Models.Database.Migrations)>`