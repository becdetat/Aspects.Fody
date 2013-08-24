using System;
using System.Linq;
using Aspects.Fody.Extensions;
using Mono.Cecil;
using NUnit.Framework;
using Shouldly;

namespace AspectsTests.ServiceTests.FindDecoratedMethodsServiceTests
{
    [TestFixture]
    public class When_finding_a_subclassed_decorator : ConcernFor<Tuple<MethodDefinition, CustomAttribute>[]>
    {
        public class ParentAttribute : Attribute
        {
        }

        public class ChildAttribute : ParentAttribute
        {
        }

        [Child]
        public void Test()
        {
        }

        protected override Tuple<MethodDefinition, CustomAttribute>[] Given()
        {
            return ModuleDefinitionHelper.ThisModuleDefinition.FindDecoratedMethods<ParentAttribute>().ToArray();
        }

        [Test]
        public void It_should_be_the_test_method()
        {
            Subject[0].Item1.FullName.ShouldBe("System.Void " + GetType().FullName + "::Test()");
        }

        [Test]
        public void There_shoudl_be_one()
        {
            Subject.Length.ShouldBe(1);
        }
    }
}