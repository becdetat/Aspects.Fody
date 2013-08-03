using System;
using System.Collections.Generic;
using System.Linq;
using Aspects.Fody.Extensions;
using Mono.Cecil;

namespace Aspects.Fody.Services
{
    public class FindDecoratedMethodsService
    {
        private readonly ModuleDefinition _moduleDefinition;

        public FindDecoratedMethodsService(ModuleDefinition moduleDefinition)
        {
            _moduleDefinition = moduleDefinition;
        }

        public IEnumerable<Tuple<MethodDefinition, CustomAttribute>> FindDecoratedMethods<T>()
        {
            var decoratorFullName = typeof (T).FullName
                .Replace('+', '/');

            return from type in _moduleDefinition.Types
                   from method in type.Methods
                   from attribute in method.CustomAttributes
                   where attribute.Constructor.DeclaringType.DerivesFrom(decoratorFullName)
                   select new Tuple<MethodDefinition, CustomAttribute>(method, attribute)
                ;
        }
    }
}