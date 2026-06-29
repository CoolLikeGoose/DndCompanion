# DnD Companion (WIP)

Web companion tool for tabletop DnD sessions. Designed to make it easier for both the game master and the players themselves to manage characters and their attributes.

## Features
* Authentication (optional, for saving characters)
* Cookies for storing authentication between sessions
* Character management
* Attribute management

[//]: # (## Architecture)

## Getting started
### Development
> git clone https://github.com/josh-k-johnson/DnDCompanion.git \
> dotnet build \
> dotnet run --project .\DndCompanion.Web\DndCompanion.Web.csproj

For development purposes, database is stored locally in DndCompanion.Web/DnDCompanion.db.

### LAN startup
For windows (automatically adds firewall rule, enabling port connections from other devices on the same network):
> .\start-lan.ps1


[//]: # (Some roadmap)

## Tech stack

* .NET 8
* ASP.NET Core
* EF Core
* SQLite
* Blazor Server