namespace FeatureToggleWeb

module Views = 
    
    open Giraffe
    open GiraffeViewEngine
    open Models

    let private layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "FeatureToggle_Web" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let private partial () =
        h1 [] [ encodedText "FeatureToggle_Web" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
        ] |> layout


    let newView () = 
        [
            partial()
            p [] [ encodedText "This is a new view" ]
        ]  |> layout