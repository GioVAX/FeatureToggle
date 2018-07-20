Publish in this directory the applications:
	- FeatureToggle.Web --> admin
	- FeatureToggle.Api --> api

Update the appsettings.json files of both application to reference the Features.json file like:
	"FeaturesConfigurationFile" :  "..\\Features.json"
	
Finally `run `docker build .` to create the image.