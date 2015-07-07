namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharp.TPLDataflow")>]
[<assembly: AssemblyProductAttribute("FSharp.TPLDataflow")>]
[<assembly: AssemblyDescriptionAttribute("An FSharp wrapper libray for TPL Dataflow")>]
[<assembly: AssemblyVersionAttribute("0.1.1")>]
[<assembly: AssemblyFileVersionAttribute("0.1.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.1.1"
