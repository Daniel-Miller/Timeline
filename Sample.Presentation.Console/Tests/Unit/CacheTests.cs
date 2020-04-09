using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Timeline.Utilities;

namespace Sample.UnitTests.Utilities
{
    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public void Cache_Global_Working_Ok()
        {
            // Create the global Cache and check the lazy instantiation
            Assert.IsNotNull(Cache.Global);

            // Cache a new object for 1 second and test its expiry
            Cache.Global.Add("test", new object(), 1);

            Assert.IsTrue(Cache.Global.Exists("test"));

            Thread.Sleep(1050); // wait a bit more than a second

            Assert.IsFalse(Cache.Global.Exists("test"));
        }

        [TestMethod]
        public void Cache_Update_Item_Ok()
        {
            Cache c = new Cache();

            object o1 = new object();
            object o2 = new object();

            c.Add("test", o1, 1);
            Assert.AreSame(c.Get("test"), o1);
            c.Add("test", o2, 1);
            Assert.AreSame(c.Get("test"), o2);

            Thread.Sleep(1050);
            Assert.IsFalse(c.Exists("test"));

        }

        [TestMethod]
        public void Cache_Generic_Ok()
        {
            Cache<int> c = new Cache<int>();

            c.Add("test", 42, 1);
            Assert.IsTrue(c.Exists("test"));
            Assert.AreEqual(c.Get("test"), 42);

            Thread.Sleep(1050);

            Assert.IsFalse(c.Exists("test"));
        }

        [TestMethod]
        public void Cache_Restart_Timer_Ok()
        {
            Cache c = new Cache();

            object o1 = new object();

            c.Add("test", o1, 1);
            Thread.Sleep(800); // wait almost a second

            Assert.IsTrue(c.Exists("test")); // still exists

            c.Add("test", o1, 1, true); // update and refresh the timer
            Thread.Sleep(1000); // wait another second

            Assert.IsTrue(c.Exists("test")); // still exists

            c.Add("test", o1, 1, false); // default parameter 4: false - no refresh of the timer

            Thread.Sleep(500); // it should expire now

            Assert.IsNull(c.Get("test")); // no longer cached
        }

        [TestMethod]
        public void Cache_Indexer_Found_Ok()
        {
            Cache c = new Cache();

            object o1 = new object();

            c.Add("test", o1, 1);
            Assert.AreSame(c["test"], o1);
        }

        [TestMethod]
        public void Cache_Indexer_Not_Found_Ok()
        {
            Cache c = new Cache();
            Assert.IsNull(c["test2"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Cache_Timeout_Zero_Throws_Exception()
        {
            Cache c = new Cache();
            c.Add("test", new object(), 0);
        }

        [TestMethod]
        public void Cache_Clear_Ok()
        {
            Cache c = new Cache();
            c.Add("test", new object());
            Assert.IsTrue(c.Exists("test"));
            c.Clear();
            Assert.IsFalse(c.Exists("test"));
        }

        [TestMethod]
        public void Cache_Remove_By_Pattern_Ok()
        {
            Cache c = new Cache();
            c.Add("test1", new object());
            c.Add("test2", new object());
            c.Add("test3", new object());
            c.Add("Other", new object());
            Assert.IsTrue(c.Exists("test1"));
            Assert.IsTrue(c.Exists("Other"));

            c.Remove(k => k.StartsWith("test"));

            Assert.IsFalse(c.Exists("test1"));
            Assert.IsFalse(c.Exists("test2"));
            Assert.IsFalse(c.Exists("test3"));
            Assert.IsTrue(c.Exists("Other"));
        }
    }
}
