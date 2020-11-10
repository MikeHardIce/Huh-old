
using NUnit.Framework;
using Shouldly;

namespace Huh.Engine.Test.Data
{
    public class RecordTest 
    {
        public RecordTest() {}

        [Test]
        public void TestCopy ()
        {
            var rec = new Core.Data.Record("bla", "muh", 100);

            var copy = rec.Copy();
            
            Assert.That(copy.Content == 100);

            copy.Key.ShouldBe("bla");
            copy.ContentHint.ShouldBe("muh");
        }
    }
}