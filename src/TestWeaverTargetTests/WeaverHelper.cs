using System;
using System.IO;
using System.Reflection;
using Aspects.Fody;
using Mono.Cecil;

namespace TestWeaverTargetTests
{
    public static class WeaverHelper
    {
        public static Assembly WeaveAssembly()
        {
            string assemblyPath = GetAssemblyPath();
            string newAssemblyPath = assemblyPath.Replace(".dll", string.Format("-{0}.dll", Guid.NewGuid()));

            if (File.Exists(newAssemblyPath)) File.Delete(newAssemblyPath);

            File.Copy(assemblyPath, newAssemblyPath);

            ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(newAssemblyPath);
            var weavingTask = new ModuleWeaver
                {
                    ModuleDefinition = moduleDefinition
                };

            weavingTask.Execute();
            moduleDefinition.Write(newAssemblyPath);

            return Assembly.LoadFile(newAssemblyPath);
        }

        private static string GetAssemblyPath()
        {
            var projectPath =
                Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
                                              @"..\..\..\TestWeaverTarget\TestWeaverTarget.csproj"));
            var assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\TestWeaverTarget.dll");

#if (!DEBUG)
            assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif
            return assemblyPath;
        }
    }
}