using System;
using System.IO;
using System.Reflection;
using Lang.Generator;
using Lang.Parser;
using Lang.Scanner;
namespace Lang
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer(@"Testfiles\testfile2.mylang");
            var tokens = lexer.GetTokens();
            var parser = new Analyzer(tokens);
            var tree = parser.Parse();
            var codeGenerator = new Coder(tree);
            var code = codeGenerator.GenerateCode();
            System.Console.WriteLine(code);
            using (StreamWriter streamWriter = File.CreateText("result.py"))
            {
                streamWriter.Write(code);
            }

        }
    }

}