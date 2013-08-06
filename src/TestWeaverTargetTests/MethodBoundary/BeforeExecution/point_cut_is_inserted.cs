using System;
using NUnit.Framework;
using Shouldly;
using TestWeaverTarget.MethodBoundary.BeforeExecution;

namespace TestWeaverTargetTests.MethodBoundary.BeforeExecution
{
    [TestFixture]
    public class point_cut_is_inserted
    {
        [Test]
        public void target_is_executed()
        {
            var assembly = WeaverHelper.WeaveAssembly();
            var type = assembly.GetType("TestWeaverTarget.MethodBoundary.BeforeExecution.point_cut_is_inserted.SubjectClass");
            var instance = (dynamic) Activator.CreateInstance(type);

            instance.SubjectMethodExecuted = false;
            instance.SubjectMethod();

            Assert.AreEqual(true, instance.SubjectMethodExecuted);
        }
    }
}