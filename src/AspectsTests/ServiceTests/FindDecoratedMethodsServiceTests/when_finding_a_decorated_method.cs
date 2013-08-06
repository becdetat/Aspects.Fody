using System;
using System.Linq;
using Aspects.Fody.Services;
using Mono.Cecil;
using NUnit.Framework;
using Shouldly;

namespace AspectsTests.ServiceTests.FindDecoratedMethodsServiceTests
{
    [TestFixture]
    public class when_finding_a_decorated_method : ConcernFor<FindDecoratedMethodsService>
    {
        private Tuple<MethodDefinition, CustomAttribute>[] _methods;

        public class SomeTestAttribute : Attribute
        {
        }

        [SomeTest]
        public void Test()
        {
        }

        protected override FindDecoratedMethodsService Given()
        {
            return new FindDecoratedMethodsService(ModuleDefinitionHelper.ThisModuleDefinition);
        }

        protected override void When()
        {
            _methods = Subject.FindDecoratedMethods<SomeTestAttribute>()
                              .ToArray()
                ;
        }

        [Test]
        public void It_should_be_the_test_method()
        {
            _methods[0].Item1.FullName.ShouldBe("System.Void " + GetType().FullName + "::Test()");
        }

        [Test]
        public void There_should_be_one()
        {
            _methods.Length.ShouldBe(1);
        }
    }
}