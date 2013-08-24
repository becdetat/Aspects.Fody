using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspects.Fody.Extensions;
using NUnit.Framework;

namespace TestWeaverTargetTests.MethodBoundary.OnException
{
    [TestFixture]
    public class point_cut_is_inserted
    {
        [Test]
        public void target_is_executed_when_exception_is_not_thrown()
        {
            var assembly = WeaverHelper.WeaveAssembly();
            var type = assembly.GetType("TestWeaverTarget.MethodBoundary.OnException.point_cut_is_inserted.SubjectClass");
            var instance = (dynamic)Activator.CreateInstance(type);

            instance.SubjectMethodExecuted = false;
            instance.SubjectMethod(false);

            Assert.AreEqual(true, instance.SubjectMethodExecuted);
        }

        [Test]
        public void target_is_executed_when_exception_is_thrown()
        {
            var assembly = WeaverHelper.WeaveAssembly();
            var type = assembly.GetType("TestWeaverTarget.MethodBoundary.OnException.point_cut_is_inserted.SubjectClass");
            var instance = (dynamic)Activator.CreateInstance(type);

            instance.SubjectMethodExecuted = false;
            instance.SubjectMethod(true);

            Assert.AreEqual(true, instance.SubjectMethodExecuted);
        }

        [Test]
        public void point_cut_is_not_executed_when_exception_is_not_thrown()
        {
            var assembly = WeaverHelper.WeaveAssembly();
            var type = assembly.GetType("TestWeaverTarget.MethodBoundary.OnException.point_cut_is_inserted.SubjectClass");
            var instance = (dynamic)Activator.CreateInstance(type);

            instance.SubjectMethodExecuted = false;
            instance.SubjectMethod(false);

            Assert.AreEqual(false, type.GetStaticFieldValue<bool>("OnExceptionPointCutExecuted"));
        }

        [Test]
        public void point_cut_is_executed_when_exception_is_thrown()
        {
            var assembly = WeaverHelper.WeaveAssembly();
            var type = assembly.GetType("TestWeaverTarget.MethodBoundary.OnException.point_cut_is_inserted.SubjectClass");
            var instance = (dynamic)Activator.CreateInstance(type);

            instance.SubjectMethodExecuted = false;
            instance.SubjectMethod(true);

            Assert.AreEqual(true, type.GetStaticFieldValue<bool>("OnExceptionPointCutExecuted"));
        }
    }
}
