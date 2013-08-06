using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
