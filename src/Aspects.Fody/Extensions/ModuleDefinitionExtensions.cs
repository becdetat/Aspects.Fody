using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Aspects.Fody.Extensions
{
    public static class ModuleDefinitionExtensions
    {
        public static IEnumerable<Tuple<MethodDefinition, CustomAttribute>> FindDecoratedMethods<T>(this ModuleDefinition moduleDefinition)
        {
            var decoratorFullName = typeof (T).FullName
                .Replace('+', '/');

            return from type in moduleDefinition.Types
                   from method in type.Methods
                   from attribute in method.CustomAttributes
                   where attribute.Constructor.DeclaringType.DerivesFrom(decoratorFullName)
                   select new Tuple<MethodDefinition, CustomAttribute>(method, attribute)
                ;
        }
    }
}