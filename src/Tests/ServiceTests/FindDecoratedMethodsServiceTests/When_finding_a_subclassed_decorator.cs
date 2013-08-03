﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspects.Fody.Services;
using Mono.Cecil;
using NUnit.Framework;
using Shouldly;

namespace Tests.ServiceTests.FindDecoratedMethodsServiceTests
{
    [TestFixture]
    public class When_finding_a_subclassed_decorator : ConcernFor<FindDecoratedMethodsService>
    {
        private Tuple<MethodDefinition,CustomAttribute>[] _methods;

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

        protected override FindDecoratedMethodsService Given()
        {
            return new FindDecoratedMethodsService(ModuleDefinitionHelper.ThisModuleDefinition);
        }

        protected override void When()
        {
            _methods = Subject.FindDecoratedMethods<ParentAttribute>()
                              .ToArray()
                ;
        }

        [Test]
        public void There_shoudl_be_one()
        {
            _methods.Length.ShouldBe(1);
        }

        [Test]
        public void It_should_be_the_tes_tmethod()
        {
            _methods[0].Item1.FullName.ShouldBe("System.Void " + this.GetType().FullName + "::Test()");
        }
    }
}
