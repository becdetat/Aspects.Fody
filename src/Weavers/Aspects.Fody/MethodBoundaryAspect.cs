using System;

namespace Aspects.Fody
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodBoundaryAspect : Attribute
    {
        public virtual void BeforeExecution()
        {
        }
    }
}
