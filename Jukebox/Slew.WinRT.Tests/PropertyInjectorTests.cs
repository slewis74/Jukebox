﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Slew.WinRT.Container;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Tests
{
    [TestClass]
    public class PropertyInjectorTests
    {
        [TestMethod]
        public void GivenASimpleClass_WhenResolved_TheObjectCanBeCreated()
        {
            var t = PropertyInjector.Resolve(() => new TestClass { Id = 13 });

            Assert.IsNotNull(t);
        }

        [TestMethod]
        public void GivenASimpleClass_WhenResolved_TheObjectIsInitialized()
        {
            var t = PropertyInjector.Resolve(() => new TestClass { Id = 13 });

            Assert.AreEqual(13, t.Id);
        }

        private class TestClass
        {
            public int Id { get; set; }
        }

        [TestMethod]
        public void GivenAClassThatPublishes_WhenResolved_ThePresentationBusPropertyIsSet()
        {
            var bus = new PresentationBus.PresentationBus();
            PropertyInjector.PresentationBus = bus;

            var t = PropertyInjector.Resolve(() => new TestClassWithBus());

            Assert.AreEqual(bus, t.PresentationBus);
        }

        private class TestClassWithBus : TestClass, IPublish
        {
            public IPresentationBus PresentationBus { get; set; }
        }
        
        [TestMethod]
        public void GivenAClassThatSubscribes_WhenTheEventIsPublished_TheHandlerGetsCalled()
        {
            var bus = new PresentationBus.PresentationBus();
            PropertyInjector.PresentationBus = bus;

            var t = PropertyInjector.Resolve(() => new TestClassWithHandler());

            bus.Publish(new TestEvent { Data =  12 });

            Assert.IsTrue(t.HandleWasCalled);
        }

        private class TestEvent : PresentationEvent<int>
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
            TestClassWithHandlerAndStaticCalledCheck.HandleWasCalled = false;
            
            var bus = new PresentationBus.PresentationBus();
            PropertyInjector.PresentationBus = bus;

            var t = PropertyInjector.Resolve(() => new TestClassWithHandlerAndStaticCalledCheck());

            bus.UnSubscribe(t);

            bus.Publish(new TestEvent { Data = 12 });

            Assert.IsFalse(TestClassWithHandlerAndStaticCalledCheck.HandleWasCalled);
        }

        private class TestEventA : PresentationEvent<int>
        {
        }

        private class TestClassWithHandlerAndStaticCalledCheck : TestClass, IHandlePresentationEvent<TestEvent>, IHandlePresentationEvent<TestEventA>
        {
            public static bool HandleWasCalled { get; set; }

            public void Handle(TestEvent e)
            {
                HandleWasCalled = true;
            }
            public void Handle(TestEventA e)
            {
                HandleWasCalled = true;
            }
        }
    }
}