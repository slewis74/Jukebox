using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Tests
{
    [TestClass]
    public class CommandTests
    {
        private class TestCommand : Command<string>
        {
            public override bool CanExecute(string parameter)
            {
                return false;
            }

            public override void Execute(string parameter)
            {
                ExecuteGotCalled = true;
            }

            public bool ExecuteGotCalled { get; set; }
        }

        [TestMethod]
        public void IfCommandSaysItCantExecuteThenCallingExecuteDoesNothing()
        {
            var command = new TestCommand();
            command.Execute("test");
            Assert.IsFalse(command.ExecuteGotCalled);
        }
    }
}