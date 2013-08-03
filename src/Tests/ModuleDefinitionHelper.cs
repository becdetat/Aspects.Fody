using Mono.Cecil;

namespace Tests
{
    public static class ModuleDefinitionHelper
    {
        public static ModuleDefinition ThisModuleDefinition
        {
            get { return ModuleDefinition.ReadModule(typeof (ModuleDefinitionHelper).Module.FullyQualifiedName); }
        }
    }
}