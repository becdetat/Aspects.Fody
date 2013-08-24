using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Aspects.Fody.Extensions
{
    public static class ILProcessorExtensions
    {
        public static void InsertBefore(this ILProcessor processor, Instruction target,
                                        IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                processor.InsertBefore(target, instruction);
            }
        }

        public static void InsertAfter(this ILProcessor processor, Instruction target,
                                       IEnumerable<Instruction> instructions)
        {
            var current = target;
            foreach (var instruction in instructions)
            {
                processor.InsertAfter(current, instruction);
                current = instruction;
            }
        }

        public static IEnumerable<Instruction> BuildMethodCall(this ILProcessor processor, MethodReference methodReference)
        {
            yield return processor.Create(OpCodes.Nop);
            yield return processor.Create(OpCodes.Ldloc_0);
            yield return processor.Create(OpCodes.Callvirt, methodReference);
            yield return processor.Create(OpCodes.Nop);
            yield return processor.Create(OpCodes.Nop);
        }

    }
}
