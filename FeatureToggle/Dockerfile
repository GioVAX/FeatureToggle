FROM microsoft/aspnetcore:2.0
EXPOSE 80

WORKDIR /app

ADD bin/Debug/netcoreapp2.0/ .

ENTRYPOINT ["dotnet", "FeatureToggle.Web.dll"]