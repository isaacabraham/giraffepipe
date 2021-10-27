open Saturn
open Giraffe
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http

// Custom http handler / pipeline stage
let myCustomPipe shouldIDoStuff (next:HttpFunc) (ctx:HttpContext) = task {
    if not shouldIDoStuff then
        return! RequestErrors.unauthorized "SCHEME" "REALM" (text "Go away!") earlyReturn ctx
    else
        printfn "DOING STUFF!"
        return! next ctx
}

// Saturn pipeline
let saturnRouter = router {
    pipe_through (myCustomPipe false) // compose your custom handler with "the rest of your app"
    get "/" (text "YOU GOT!")
    post "/" (text "YOU POSTED!")
}

// Giraffe pipeline
let giraffeRouter : HttpHandler =
    myCustomPipe true >=> // compose your custom handler with "the rest of your app"
    choose [
        GET >=> text "YOU GOT!"
        text "SOME OTHER VERB"
    ]

let appRouter = router {
    forward "/giraffe" giraffeRouter
    forward "/saturn" saturnRouter
}

let app = application {
    use_router appRouter
}

run app