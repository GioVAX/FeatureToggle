namespace FeatureToggleWeb

module Models =

   type Message =
        {
            Text : string
        }

    [<CLIMutable>]
    type RazorModel = {
        Title : string
        Text : string
    }
