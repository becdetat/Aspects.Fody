using System;

namespace Aspects.Fody
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodBoundaryAspect : Attribute
    {
        public virtual void OnEntry()
        {
        }

        public virtual void OnSuccess()
        {
        }

        public virtual void OnException()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}
