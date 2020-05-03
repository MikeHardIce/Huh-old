
using Shouldly;
using Xunit;

namespace Huh.Engine.Test.Data
{
    public class RecordTest 
    {
        public RecordTest() {}

        [Fact]
        public void TestCopy ()
        {
            var rec = new Core.Data.Record("bla", "muh", 100);

            var copy = rec.Copy();
            
            Assert.IsType<long>(copy.Content);

            copy.Key.ShouldBe("bla");
            copy.ContentHint.ShouldBe("muh");
        }
    }
}