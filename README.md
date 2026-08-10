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
> git clone https://github.com/CoolLikeGoose/DndCompanion.git \
> dotnet build \
> dotnet run --project .\DndCompanion.Web\DndCompanion.Web.csproj

For development purposes, database is stored locally in DndCompanion.Web/DnDCompanion.db.

### LAN startup
For windows (automatically adds firewall rule, enabling port connections from other devices on the same network):
> .\start-lan.ps1


[//]: # (Some roadmap)

## Tech stack

* .NET 8
* ASP.NET Core (Minimal API + Blazor Server)
* Entity Framework Core + SQLite
* SignalR via Blazor Server
* Bootstrap 5
* xUnit (unit tests)

## Architecture
* Clean Architecture with Vertical Slice
* Repository pattern
* Command/Result pattern (CQRS-inspired)
