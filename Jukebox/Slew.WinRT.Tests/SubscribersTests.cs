﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Tests
{
    [TestClass]
    public class SubscribersTests
    {
        private class TestClass
        {
            public int Id { get; set; }
        }

        [TestMethod]
        public void GivenAClassThatSubscribes_WhenTheEventIsPublished_TheHandlerGetsCalled()
        {
            var bus = new PresentationBus.PresentationBus();

            var t = new TestClassWithHandler();
            bus.Subscribe(t);
            bus.Publish(new TestEvent { Data = 12 });

            Assert.IsTrue(t.HandleWasCalled);
        }

        private class TestEvent : IPresentationEvent<int>
        {
            public int Data { get; set; }
        }

        private class TestClassWithHandler : TestClass, IHandlePresentationEvent<TestEvent>
        {
            public bool HandleWasCalled { get; private set; }

            public void Handle(TestEvent e)
            {
                HandleWasCalled = true;
            }
        }

        [TestMethod]
        public void GivenAClassThatSubscribes_WhenTheObjectIsUnsubscribedBeforeTheEventIsPublished_TheHandlerDoesntGetCalled()
        {
            TestClassWithHandlerAndStaticCalledCheck.Reset();

            var bus = new PresentationBus.PresentationBus();

            var t = new TestClassWithHandlerAndStaticCalledCheck();

            bus.Subscribe(t);
            bus.UnSubscribe(t);

            bus.Publish(new TestEvent { Data = 12 });

            Assert.IsFalse(TestClassWithHandlerAndStaticCalledCheck.HandleWasCalled);
        }

        private class TestEventA : IPresentationEvent<int>
        {
            public int Data { get; set; }
        }

        private class TestClassWithHandlerAndStaticCalledCheck : TestClass, IHandlePresentationEvent<TestEvent>, IHandlePresentationEvent<TestEventA>
        {
            public static bool HandleWasCalled { get; private set; }
            public static bool HandleAWasCalled { get; private set; }

            public static void Reset()
            {
                HandleWasCalled = false;
                HandleAWasCalled = false;
            }

            public void Handle(TestEvent e)
            {
                HandleWasCalled = true;
            }
            public void Handle(TestEventA e)
            {
                HandleAWasCalled = true;
            }
        }

        [TestMethod]
        public void GivenAClassThatSubscribesToMultipleEvents_WhenTheOneEventIsPublished_ItsHandlerGetsCalled()
        {
            TestClassWithHandlerAndStaticCalledCheck.Reset();

            var bus = new PresentationBus.PresentationBus();

            var t = new TestClassWithHandlerAndStaticCalledCheck();

            bus.Subscribe(t);

            bus.Publish(new TestEvent { Data = 12 });

            Assert.IsTrue(TestClassWithHandlerAndStaticCalledCheck.HandleWasCalled);
        }

        [TestMethod]
        public void GivenAClassThatSubscribesToMultipleEvents_WhenTheOneEventIsPublished_TheOtherHandlerDoesntGetCalled()
        {
            TestClassWithHandlerAndStaticCalledCheck.Reset();

            var bus = new PresentationBus.PresentationBus();

            var t = new TestClassWithHandlerAndStaticCalledCheck();

            bus.Subscribe(t);

            bus.Publish(new TestEvent { Data = 12 });

            Assert.IsFalse(TestClassWithHandlerAndStaticCalledCheck.HandleAWasCalled);
        }
    }
}
