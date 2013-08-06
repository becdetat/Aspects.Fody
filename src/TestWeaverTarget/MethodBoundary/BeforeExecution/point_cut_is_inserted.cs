using Aspects.Fody;

namespace TestWeaverTarget.MethodBoundary.BeforeExecution.point_cut_is_inserted
{
    public class SubjectClass
    {
        public static bool BeforeExecutionPointCutExecuted;
        public bool SubjectMethodExecuted;

        [SubjectBeforeExecutionAspect]
        public void SubjectMethod()
        {
            SubjectMethodExecuted = true;
        }
    }

    public class SubjectBeforeExecutionAspect : MethodBoundaryAspect
    {
        public override void BeforeExecution()
        {
            SubjectClass.BeforeExecutionPointCutExecuted = true;
        }
    }
}