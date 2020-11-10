
using Huh.Engine.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Huh.Engine.Test.Tasks
{
    public class TaskTest
    {
        public TaskTest()
        {

        }

        [Test]
        public void TestCopy ()
        {
            var task = new Task("Hello", new Core.Data.Record("blub", "nothing", 42));

            var copied = (Task)task.Clone();

            task.KeyWord.ShouldBe("Hello");
            copied.KeyWord.ShouldBe("Hello");
        }
    }
}