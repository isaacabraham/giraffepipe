open Saturn
open Giraffe
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http

let myCustomPipe shouldIDoStuff (next:HttpFunc) (ctx:HttpContext) = task {
    if not shouldIDoStuff then
        return! RequestErrors.unauthorized "SCHEME" "REALM" (text "Go away!") earlyReturn ctx
    else
        printfn "DOING STUFF!"
        return! next ctx
}

let saturnRouter = router {
    pipe_through (myCustomPipe false)
    get "/" (text "YOU GOT!")
    post "/" (text "YOU POSTED!")
}

// Giraffe pipeline
let giraffeRouter : HttpHandler =
    myCustomPipe true >=>
    choose [
        GET >=> text "YOU GOT!"
        text "SOME OTHER VERB"
    ]

let app = application {
    use_router saturnRouter
}

run app