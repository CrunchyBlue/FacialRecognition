build: run_dashboard run_mvcfrontend run_ordersapi run_facesapi run_notificationsapi

run_dashboard:
	dapr dashboard
run_mvcfrontend:
	dapr run --app-id mvcfront --app-port 5002 --dapr-http-port 50002 --components-path ./components -- dotnet run --project ./MvcFrontend/MvcFrontend.csproj
run_ordersapi:
	dapr run --app-id ordersapi --app-port 5003 --dapr-http-port 50003 --components-path ./components -- dotnet run --project ./OrdersApi/OrdersApi.csproj
run_facesapi:
	dapr run --app-id facesapi --app-port 5004 --dapr-http-port 50004 --components-path ./components -- dotnet run --project ./FacesApi/FacesApi.csproj
run_notificationsapi:
	dapr run --app-id facesapi --app-port 5005 --dapr-http-port 50005 --components-path ./components -- dotnet run --project ./NotificationsApi/NotificationsApi.csproj