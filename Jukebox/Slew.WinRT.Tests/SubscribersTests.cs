using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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

        private class TestRequest : IPresentationRequest
        {
            public bool IsHandled { get; set; }
            public int Value { get; set; }
        }

        private class TestClassAWithRequestSubscriber : IHandlePresentationRequest<TestRequest>
        {
            public bool HandleWasCalled { get; set; }
            public void Handle(TestRequest request)
            {
                HandleWasCalled = true;
                request.IsHandled = true;
                request.Value = 23;
            }
        }
        private class TestClassBWithRequestSubscriber : IHandlePresentationRequest<TestRequest>
        {
            public bool HandleWasCalled { get; set; }
            public void Handle(TestRequest request)
            {
                HandleWasCalled = true;
            }
        }

        [TestMethod]
        public void GivenTwoClassesThatHandleARequest_WhenTheRequestIsPublish_TheFirstClassHandleIsCalled()
        {
            var bus = new PresentationBus.PresentationBus();

            var t1 = new TestClassAWithRequestSubscriber();
            var t2 = new TestClassAWithRequestSubscriber();

            bus.Subscribe(t1);
            bus.Subscribe(t2);

            bus.Publish(new TestRequest());

            Assert.IsTrue(t1.HandleWasCalled);
        }

        [TestMethod]
        public void GivenTwoClassesThatHandleARequest_WhenTheRequestIsPublish_TheFirstClassHandlesTheRequest()
        {
            var bus = new PresentationBus.PresentationBus();

            var t1 = new TestClassAWithRequestSubscriber();
            var t2 = new TestClassAWithRequestSubscriber();

            bus.Subscribe(t1);
            bus.Subscribe(t2);

            var request = new TestRequest();

            bus.Publish(request);

            Assert.AreEqual(23, request.Value);
        }

        [TestMethod]
        public void GivenTwoClassesThatHandleARequest_WhenTheRequestIsPublish_TheSecondClassHandleIsNotCalled()
        {
            var bus = new PresentationBus.PresentationBus();

            var t1 = new TestClassAWithRequestSubscriber();
            var t2 = new TestClassAWithRequestSubscriber();

            bus.Subscribe(t1);
            bus.Subscribe(t2);

            bus.Publish(new TestRequest());

            Assert.IsFalse(t2.HandleWasCalled);
        }
    }
}
