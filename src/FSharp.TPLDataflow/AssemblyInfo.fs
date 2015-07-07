namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharp.TPLDataflow")>]
[<assembly: AssemblyProductAttribute("FSharp.TPLDataflow")>]
[<assembly: AssemblyDescriptionAttribute("An FSharp wrapper libray for TPL Dataflow")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
