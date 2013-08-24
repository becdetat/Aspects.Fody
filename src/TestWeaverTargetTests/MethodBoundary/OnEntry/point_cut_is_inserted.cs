using System;
using System.Reflection;
using NUnit.Framework;

namespace TestWeaverTargetTests.MethodBoundary.OnEntry
{
    [TestFixture]
    public class point_cut_is_inserted
    {
        [Test]
        public void target_is_executed()
        {
            Assembly assembly = WeaverHelper.WeaveAssembly();
            Type type = assembly.GetType("TestWeaverTarget.MethodBoundary.OnEntry.point_cut_is_inserted.SubjectClass");
            var instance = (dynamic) Activator.CreateInstance(type);

            instance.SubjectMethodExecuted = false;
            instance.SubjectMethod();

            Assert.AreEqual(true, instance.SubjectMethodExecuted);
        }
    }
}