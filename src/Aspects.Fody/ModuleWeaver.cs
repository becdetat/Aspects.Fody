using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aspects.Fody.Services;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Aspects.Fody.Extensions;

namespace Aspects.Fody
{
    public class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }
        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        FindDecoratedMethodsService _findDecoratedMethodsService;

        public ModuleWeaver()
        {
            LogInfo = s => { };
            LogWarning = s => { };
        }

        public void Execute()
        {
            _findDecoratedMethodsService = new FindDecoratedMethodsService(ModuleDefinition);

            LogWarning("Aspects.Fody.ModuleWeaver.Execute");

            var methods = _findDecoratedMethodsService
                .FindDecoratedMethods<MethodBoundaryAspect>()
                .ToArray()
                ;

            LogWarning("Found " + methods.Count().ToString());

            foreach (var method in methods)
            {
                Decorate(method.Item1, method.Item2);
            }
        }

        private void Decorate(MethodDefinition method, CustomAttribute aspectAttribute)
        {
            var processor = method.Body.GetILProcessor();
            var firstInstruction = method.IsConstructor
                                       ? method.Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next
                                       : method.Body.Instructions.First();

            // MethodBoundaryAspectImplementation __fody$aspect;
            // >>>> .locals init (
            // >>>>   [0] class MethodBoundaryAspectImplementation __fody$aspect
            // >>>> )
            var aspectVariableDefinition = new VariableDefinition("__fody$aspect", aspectAttribute.AttributeType);
            method.Body.Variables.Add(aspectVariableDefinition);
            processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Ldloc_S, aspectVariableDefinition));
            
            // __fody$aspect = new MethodBoundaryAspectImplementation();
            // >>>> newobj instance void MethodBoundaryAspectImplementation::.ctor()
            processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Newobj, aspectAttribute.Constructor));
            // >>>> stloc.0
            processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Stloc_0));
            // >>>> ldloc.0
            processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Ldloc_0));

            // __fody$aspect.OnEntry();
            // >>>> callvirt instance void MethodBoundaryAspectImplementation::OnEntry()
            processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Callvirt, GetMethodReference(aspectAttribute.AttributeType, x => x.Name == "OnEntry")));
            
            // try {
            // >>>> .try {
            //   >>>> .try {
            // **** This causes a NRE in cecil, going to have to mess around with this a bit.
            // See:
            // http://stackoverflow.com/questions/11074518/add-a-try-catch-with-mono-cecil/11074910#11074910
            // http://stackoverflow.com/questions/12769699/mono-cecil-injecting-try-finally
            //method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
            //    {
            //        TryStart = firstInstruction
            //    });

            //   **PAYLOAD**

            processor.InsertAfter(
                GetLastInstruction(method), 
                processor.BuildMethodCall(GetMethodReference(aspectAttribute.AttributeType, x => x.Name == "OnSuccess")));

            // } catch (Exception ex) {
            //   __fody$aspect.OnException();
            // } finally {
            //   __fody$aspect.OnExit();
            // }
        }

        


        private static Instruction GetLastInstruction(MethodDefinition method)
        {
            return method.Body.Instructions.Skip(method.Body.Instructions.Count - 2).FirstOrDefault();
            //var lastInstruction = method.Body.Instructions.Last();
            //return lastInstruction;
        }

        public MethodReference GetMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate)
        {
            var typeDefinition = typeReference.Resolve();

            MethodDefinition methodDefinition;
            do
            {
                methodDefinition = typeDefinition.Methods.FirstOrDefault(predicate);
                typeDefinition = typeDefinition.BaseType == null ? null : typeDefinition.BaseType.Resolve();
            } while (methodDefinition == null && typeDefinition != null);

            return ModuleDefinition.Import(methodDefinition);
        }
    }
}
