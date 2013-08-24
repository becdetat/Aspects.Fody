using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspects.Fody;

namespace TestWeaverTarget.MethodBoundary.OnException.point_cut_is_inserted
{
    public class SubjectClass
    {
        public static bool OnExceptionPointCutExecuted;
        public bool SubjectMethodExecuted;

        [SubjectOnExceptionAspect]
        public void SubjectMethod(bool throwException)
        {
            SubjectMethodExecuted = true;
            if (throwException) throw new Exception();
        }
    }

    public class SubjectOnExceptionAspect : MethodBoundaryAspect
    {
        public override void OnException()
        {
            SubjectClass.OnExceptionPointCutExecuted = true;
        }
    }
}
