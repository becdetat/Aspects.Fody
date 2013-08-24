using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestWeaverTargetTests.MethodBoundary.OnSuccess
{
    [TestFixture]
    public class point_cut_is_inserted
    {
        [Test]
        public void target_is_executed()
        {
            var assembly = WeaverHelper.WeaveAssembly();
            var type = assembly.GetType("TestWeaverTarget.MethodBoundary.OnSuccess.point_cut_is_inserted.SubjectClass");
            var instance = (dynamic) Activator.CreateInstance(type);

            instance.SubjectMethodExecuted = false;
            instance.SubjectMethod();

            Assert.AreEqual(true, instance.SubjectMethodExecuted);
        }
    }
}
