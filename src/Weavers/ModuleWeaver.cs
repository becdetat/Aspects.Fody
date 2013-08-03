using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aspects.Fody;
using Aspects.Fody.Extensions;
using Aspects.Fody.Services;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Weavers
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

            var methods = _findDecoratedMethodsService.FindDecoratedMethods<MethodBoundaryAspect>();

            LogWarning("Found " + methods.Count().ToString());

            foreach (var method in methods)
            {
                //Decorate(method.Item1, method.Item2);
            }
        }

        private void Decorate(MethodDefinition method, CustomAttribute attribute)
        {
            var beforeExecutionMethodReference = GetMethodReference(attribute.AttributeType, md => md.Name == "BeforeExecution");

            var processor = method.Body.GetILProcessor();
            var firstInstruction = method.IsConstructor
                                       ? method.Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next
                                       : method.Body.Instructions.First();

            var methodBaseTypeRef = ModuleDefinition.Import(typeof (MethodBase));

            var attributeVariableDefinition = AddVariableDefinition(method, "__fody$attribute", attribute.AttributeType);
            var methodVariableDefinition = AddVariableDefinition(method, "__fody$method", methodBaseTypeRef);

            var beforeExecutionInstructions = GetBeforeExecutionInstructions(processor, attributeVariableDefinition, methodVariableDefinition, beforeExecutionMethodReference);
            
            processor.InsertBefore(firstInstruction, beforeExecutionInstructions);
        }
        
        private static VariableDefinition AddVariableDefinition(MethodDefinition method, string variableName, TypeReference variableType)
        {
            var variableDefinition = new VariableDefinition(variableName, variableType);
            method.Body.Variables.Add(variableDefinition);
            return variableDefinition;
        }

        private static IEnumerable<Instruction> GetBeforeExecutionInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, VariableDefinition methodVariableDefinition, MethodReference beforeExecutionMethodReference)
        {
            // Call __fody$attribute.BeforeExecution("{methodName}")
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                       processor.Create(OpCodes.Ldloc_S, methodVariableDefinition),
                       processor.Create(OpCodes.Callvirt, beforeExecutionMethodReference)
                   };
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
