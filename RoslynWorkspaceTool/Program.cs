namespace RoslynWorkspaceTool
{
    using Microsoft.CodeAnalysis.CSharp;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync("../../../MainProject/MainProject.sln").Result;
            var mainProject =
                solution.Projects.First(p => p.Name == "MainProject");
            var document =
                mainProject.Documents.First(d => d.Name == "ClassDerivedFromInternalEntryBase.cs");
            var syntaxTree = document.GetSyntaxRootAsync().Result;
            var semanticModel = document.GetSemanticModelAsync().Result;
            var classDerivedFromInternalType =
                syntaxTree
                    .DescendantNodes()
                    .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>()
                    .Select(c => semanticModel.GetDeclaredSymbol(c))
                    .First();
            var internalBaseType =
                classDerivedFromInternalType.BaseType;

            if (internalBaseType.AllInterfaces.Any(i => i.Name == "IInternalInterface"))
                System.Console.WriteLine("It worked as intended, found IInternalInterface");
            else
                System.Console.WriteLine("Did not work as intended, internals are not visible despite InternalsVisibleTo.");
        }
    }
}
