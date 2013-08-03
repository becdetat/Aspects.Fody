using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aspects.Fody;
using NUnit.Framework;
using Shouldly;
using Weavers;

namespace Tests.MethodBoundary.BeforeExecution
{
    public class SubjectClass
    {
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
            cut_point_is_inserted.BeforeExecutionCutPointExecuted = true;
        }
    }


    [TestFixture]
    public class cut_point_is_inserted : ConcernFor<SubjectClass>
    {
        public static bool BeforeExecutionCutPointExecuted;

        protected override SubjectClass Given()
        {
            return new SubjectClass();
        }

        protected override void When()
        {
            BeforeExecutionCutPointExecuted = false;
            Subject.SubjectMethod();
        }
        
        [Test]
        public void target_is_executed()
        {
            Subject.SubjectMethodExecuted.ShouldBe(true);
        }

        [Test]
        public void before_execution_cut_point_is_executed()
        {
            BeforeExecutionCutPointExecuted.ShouldBe(true);
        }
    }
}
