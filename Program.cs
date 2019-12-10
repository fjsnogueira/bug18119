using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace hello
{
public class GenericNameFetcher : CSharpSyntaxWalker
    {
        public override void VisitGenericName(GenericNameSyntax node)
        {
            GenericName = node;
            base.VisitGenericName(node);
        }

        public GenericNameSyntax GenericName { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
			var tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(@"using System.Collections.Generic;
    public class Dictionaries
    {
        public void Method1()
        {
            var dic = typeof(Dictionary<string, int>);
        }
    }");
            var compilation = CSharpCompilation
                .Create("TempComp")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree);
            var root = tree.GetRoot();
            var cv = new GenericNameFetcher();
            cv.Visit(root);
            tree = (CSharpSyntaxTree)root.SyntaxTree;
            var model = compilation.GetSemanticModel(tree);
            var s = model.GetSymbolInfo(cv.GenericName).Symbol;
        }
	}
}
