migrate:
	dotnet ef migrations add InitialCreate --project DndCompanion.Infrastructure --startup-project DndCompanion.Web

drop:
	powershell -Command "Remove-Item -Force DndCompanion.Web/DndCompanion.db -ErrorAction SilentlyContinue; Remove-Item -Force DndCompanion.Infrastructure/Migrations/*.cs -ErrorAction SilentlyContinue"

reset: drop migrate