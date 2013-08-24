using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspects.Fody;

namespace TestWeaverTarget.MethodBoundary.OnSuccess.point_cut_is_inserted
{
    public class SubjectClass
    {
        public static bool OnSuccessPointCutExecuted;
        public bool SubjectMethodExecuted;


        [SubjectOnSuccessAspect]
        public void SubjectMethod()
        {
            SubjectMethodExecuted = true;
        }
    }

    public class SubjectOnSuccessAspect : MethodBoundaryAspect
    {
        public override void OnSuccess()
        {
            SubjectClass.OnSuccessPointCutExecuted = true;
        }
    }

    public class Example
    {
        public void SubjectMethod()
        {
            var aspect = new SubjectOnSuccessAspect();
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
