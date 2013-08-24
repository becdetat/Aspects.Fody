using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public ModuleWeaver()
        {
            LogInfo = s => { };
            LogWarning = s => { };
        }

        public void Execute()
        {
            LogWarning("Aspects.Fody.ModuleWeaver.Execute");

            var methods = ModuleDefinition
                .FindDecoratedMethods<MethodBoundaryAspect>()
                .ToArray()
                ;

            LogWarning(string.Format("Found {0}", methods.Count()));

            foreach (var method in methods)
            {
                Decorate(method.Item1, method.Item2);
            }
        }

        private void Decorate(MethodDefinition method, CustomAttribute aspectAttribute)
        {
            var processor = method.Body.GetILProcessor();
            FixReturns(method);


            var firstInstruction = GetFirstInstruction(method);

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

            processor.InsertBefore(
                firstInstruction,
                processor.BuildMethodCall(GetMethodReference(aspectAttribute.AttributeType, x => x.Name == "OnEntry")));

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


            // aspect.OnSuccess()
            var onSuccessInstructions =
                processor.BuildMethodCall(GetMethodReference(aspectAttribute.AttributeType, x => x.Name == "OnSuccess"))
                         .ToArray()
                ;
            processor.InsertAfter(
                GetLastInstruction(method),
                onSuccessInstructions);

            // aspect.OnException()
            var onExceptionInstructions =
                processor.BuildMethodCall(GetMethodReference(aspectAttribute.AttributeType, x => x.Name == "OnException"))
                         .ToArray()
                ;
            processor.InsertAfter(
                method.Body.Instructions.Skip(method.Body.Instructions.Count - 1).FirstOrDefault(), 
                onExceptionInstructions);
            // Wrap a handler around { Payload(); aspect.OnSuccess(); }:
            //var innerHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
            //{
            //    TryStart = firstInstruction,
            //    TryEnd = onSuccessInstructions.Last(),
            //    HandlerStart = onExceptionInstructions.First(),
            //    HandlerEnd = onExceptionInstructions.Last()
            //};
            //method.Body.ExceptionHandlers.Add(innerHandler);

            // } catch (Exception ex) {
            //   __fody$aspect.OnException();
            // } finally {
            //   __fody$aspect.OnExit();
            // }
            
        }

        private void FixReturns(MethodDefinition method)
        {
            // TODO handle non-void method return types

            var instructions = method.Body.Instructions;

            if (method.ReturnType == ModuleDefinition.TypeSystem.Void)
            {
                var lastReturnInstruction = Instruction.Create(OpCodes.Ret);

                foreach (var instruction in instructions.Where(x => x.OpCode == OpCodes.Ret))
                {
                    var index = instructions.IndexOf(instruction);
                    instructions[index] = Instruction.Create(OpCodes.Leave, lastReturnInstruction);
                }

                instructions.Add(lastReturnInstruction);
            }
        }

        private static Instruction GetFirstInstruction(MethodDefinition method)
        {
            var firstInstruction = method.IsConstructor
                                       ? method.Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next
                                       : method.Body.Instructions.First();
            return firstInstruction;
        }

        private static Instruction GetLastInstruction(MethodDefinition method)
        {
            //if (method.Body.Instructions.Last().OpCode == OpCodes.Ret) return method.Body.Instructions.Last();
            return method.Body.Instructions.Skip(method.Body.Instructions.Count - 3).FirstOrDefault();
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
