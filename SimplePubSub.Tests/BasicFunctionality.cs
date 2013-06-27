using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartboyDevelopments.SimplePubSub;

namespace SimplePubSub.Tests
{
    [TestClass]
    public class BasicFunctionality
    {
        List<string> output;
        List<string> expected_output;
        IMediator mediator;

        [TestInitialize]
        public void SetupTests()
        {
            output = new List<string>();
            expected_output = new List<string>();
            mediator = new SynchronousMediator();
        }

        SubscribableListener CreateSubscriber(string id)
        {
            return ((w, x, y, z) => output.Add(id + " called by channel " + x));
        }

        void AssertListEqual<T>(IList<T> a, IList<T> b)
        {
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++)
                Assert.AreEqual(a[i], b[i]);
        }

        [TestMethod]
        public void TestSubscribe()
        {
            mediator.Subscribe("test", CreateSubscriber("Subscriber1"));
            mediator.Notify("test", null, new EventArgs());
            expected_output.Add("Subscriber1 called by channel test");
            AssertListEqual<string>(output, expected_output);
        }

        [TestMethod]
        public void TestSubscribeMultiple()
        {
            mediator.Subscribe("test", CreateSubscriber("Subscriber1"));
            mediator.Subscribe("test", CreateSubscriber("Subscriber2"));
            mediator.Subscribe("test", CreateSubscriber("Subscriber3"));
            mediator.Notify("test", null, new EventArgs());
            expected_output.Add("Subscriber1 called by channel test");
            expected_output.Add("Subscriber2 called by channel test");
            expected_output.Add("Subscriber3 called by channel test");
            AssertListEqual<string>(output, expected_output);
        }

        [TestMethod]
        public void TestUnsubscribe()
        {
            SubscribableListener subscriber3 = CreateSubscriber("Subscriber3");
            mediator.Subscribe("test", CreateSubscriber("Subscriber1"));
            mediator.Subscribe("test", CreateSubscriber("Subscriber2"));
            mediator.Subscribe("test", subscriber3);
            mediator.Notify("test", null, new EventArgs());
            mediator.Unsubscribe("test", subscriber3);
            mediator.Notify("test", null, new EventArgs());
            expected_output.Add("Subscriber1 called by channel test");
            expected_output.Add("Subscriber2 called by channel test");
            expected_output.Add("Subscriber3 called by channel test");
            expected_output.Add("Subscriber1 called by channel test");
            expected_output.Add("Subscriber2 called by channel test");
            AssertListEqual<string>(output, expected_output);
        }

        [TestMethod]
        public void TestPatternSubscribe()
        {
            mediator.PatternSubscribe("^test[0-9]?$", CreateSubscriber("Subscriber1"));
            mediator.Notify("test", null, new EventArgs());
            mediator.Notify("test1", null, new EventArgs());
            mediator.Notify("test9", null, new EventArgs());
            mediator.Notify("test2", null, new EventArgs());
            mediator.Notify("test22", null, new EventArgs());
            expected_output.Add("Subscriber1 called by channel test");
            expected_output.Add("Subscriber1 called by channel test1");
            expected_output.Add("Subscriber1 called by channel test9");
            expected_output.Add("Subscriber1 called by channel test2");
            AssertListEqual<string>(output, expected_output);
        }

        [TestMethod]
        public void TestPatternUnsubscribe()
        {
            SubscribableListener subscriber = CreateSubscriber("Subscriber1");
            mediator.Subscribe("test", subscriber);
            mediator.Subscribe("test1", subscriber);
            mediator.Subscribe("test2", subscriber);
            mediator.Subscribe("test9", subscriber);
            mediator.Subscribe("test22", subscriber);
            mediator.PatternUnsubscribe("^test[1,2]$", subscriber);
            mediator.Notify("test", null, new EventArgs());
            mediator.Notify("test1", null, new EventArgs());
            mediator.Notify("test9", null, new EventArgs());
            mediator.Notify("test2", null, new EventArgs());
            mediator.Notify("test22", null, new EventArgs());
            expected_output.Add("Subscriber1 called by channel test");
            expected_output.Add("Subscriber1 called by channel test9");
            expected_output.Add("Subscriber1 called by channel test22");
            AssertListEqual<string>(output, expected_output);
        }

        [TestMethod]
        public void TestPatternNotify()
        {
            SubscribableListener subscriber = CreateSubscriber("Subscriber1");
            mediator.Subscribe("test", subscriber);
            mediator.Subscribe("test1", subscriber);
            mediator.Subscribe("test2", subscriber);
            mediator.Subscribe("test9", subscriber);
            mediator.Subscribe("test22", subscriber);
            mediator.PatternNotify("^test(?:9|22)?$", null, new EventArgs());
            expected_output.Add("Subscriber1 called by channel test");
            expected_output.Add("Subscriber1 called by channel test9");
            expected_output.Add("Subscriber1 called by channel test22");
            AssertListEqual<string>(output, expected_output);
        }
    }
}
