migrate:
	dotnet ef migrations add InitialCreate --project DndCompanion.Infrastructure --startup-project DndCompanion.Web

drop-migrations:
    powershell -Command "Remove-Item -Force DndCompanion.Infrastructure/Migrations/*.cs -ErrorAction SilentlyContinue"

drop-db:
    powershell -Command "Remove-Item -Force DndCompanion.Web/DndCompanion.db -ErrorAction SilentlyContinue"

reset: drop-db drop-migrations migrate
