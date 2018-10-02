using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace PengSW.Script
{
    public static class ScriptHelper
    {
        public static T CreateScriptObject<T>(string aScript) where T : class
        {
            if (string.IsNullOrWhiteSpace(aScript)) throw new ApplicationException("无效的脚本！");

            StringReader aStringReader = new StringReader(aScript);
            List<string> aReferences = new List<string>();
            string aLine;
            Regex aRefRegex = new Regex(@"^// import (.+)$");
            while ((aLine = aStringReader.ReadLine()) != null)
            {
                Match aMatch = aRefRegex.Match(aLine);
                if (aMatch == null || !aMatch.Success) break;
                aReferences.Add(aMatch.Groups[1].Value);
            }
            StringBuilder aCodeBuilder = new StringBuilder();
            aCodeBuilder.AppendLine(aLine);
            aCodeBuilder.Append(aStringReader.ReadToEnd());
            string aCode = aCodeBuilder.ToString();
            CSharpCodeProvider aCodeProvider = new CSharpCodeProvider();
            CompilerParameters aCompilerParameters = new CompilerParameters();
            foreach (string aReference in aReferences) aCompilerParameters.ReferencedAssemblies.Add(aReference);
            aCompilerParameters.ReferencedAssemblies.Add(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            aCompilerParameters.GenerateExecutable = false;
            aCompilerParameters.GenerateInMemory = true;

            CompilerResults aCompilerResults = aCodeProvider.CompileAssemblyFromSource(aCompilerParameters, aCode);
            if (aCompilerResults.Errors.Count > 0)
            {
                StringBuilder aErrorTextBuilder = new StringBuilder();
                foreach (CompilerError aCompilerError in aCompilerResults.Errors)
                {
                    aErrorTextBuilder.AppendLine($"行{aCompilerError.Line}：{aCompilerError.ErrorText}");
                }
                throw new ApplicationException(aErrorTextBuilder.ToString());
            }
            else
            {
                foreach (Type aType in aCompilerResults.CompiledAssembly.GetExportedTypes())
                {
                    if (aType.GetInterface(typeof(T).Name) != null)
                    {
                        return aCompilerResults.CompiledAssembly.CreateInstance(aType.FullName) as T;
                    }
                }
                throw new ApplicationException($"没有找到[{typeof(T).Name}]的实现类！");
            }
        }
    }
}
