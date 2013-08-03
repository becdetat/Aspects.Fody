using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Tests
{
    [TestFixture]
    public class ThereAreTests
    {
        [Test]
        public void and_the_people_rejoiced()
        {
            true.ShouldBe(true);
        }
    }
}
