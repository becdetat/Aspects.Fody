using System;
using Aspects.Fody;

namespace TestWeaverTarget.MethodBoundary.OnEntry.point_cut_is_inserted
{
    public class SubjectClass
    {
        public static bool OnEntryPointCutExecuted;
        public bool SubjectMethodExecuted;

        [SubjectOnEntryAspect]
        public void SubjectMethod()
        {
            SubjectMethodExecuted = true;
        }
    }

    public class SubjectOnEntryAspect : MethodBoundaryAspect
    {
        public override void OnEntry()
        {
            SubjectClass.OnEntryPointCutExecuted = true;
        }
    }

    public class Example
    {
        public void SubjectMethod()
        {
            var aspect = new SubjectOnEntryAspect();
            aspect.OnEntry();
            try
            {
                Payload();
                aspect.OnSuccess();
            }
            catch (Exception)
            {
                aspect.OnException();
                throw;
            }
            finally
            {
                aspect.OnExit();
            }
        }

        void Payload()
        {
            Console.WriteLine("Payload");
        }
    }
}