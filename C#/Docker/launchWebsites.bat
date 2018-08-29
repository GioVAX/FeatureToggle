start /b cmd /c "cd admin && dotnet FeatureToggle.Web.dll --server.urls ""http://0.0.0.0:5101"""
(cd api && dotnet FeatureToggle.API.dll)