using Mono.Cecil;

namespace AspectsTests
{
    public static class ModuleDefinitionHelper
    {
        public static ModuleDefinition ThisModuleDefinition
        {
            get { return ModuleDefinition.ReadModule(typeof (ModuleDefinitionHelper).Module.FullyQualifiedName); }
        }
    }
}