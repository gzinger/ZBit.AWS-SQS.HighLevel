using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBit.Aws.Sqs.Core;
using System.Threading;

namespace ZBit.Aws.Sqs.CoreTests
{
    [TestClass]
    public class SQSHelperTests
    {
		[TestMethod()]
		public void SendTest() {
			string sSuperString = "my strIng;some (5) special symbols: &%_@' and a quote [\"] and a backslash <\\>!";
			SQSHelper.Send("Utest1Q", new { prop1 = 11, prop2 = sSuperString }).Wait();
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = sSuperString }).Wait();
		}

		int m_iTotalCallCount, m_iCallCountFor2;

		[TestMethod()]
		public void SubscribeTest() {
			m_iTotalCallCount = 0;
			m_iCallCountFor2 = 0;
			var q = SQSHelper.Subscribe<MyType>(QueueMsgProcessor, "Utest1Q");
			SQSHelper.Send("Utest1Q", new { prop1 = 11, prop2 = "test2" }).Wait();
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = "test3" }).Wait();
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = "test2" }).Wait();
			Thread.Sleep(1000);
			Assert.IsTrue(m_iTotalCallCount > 2, "total: " + m_iTotalCallCount);
			Assert.AreEqual(2, m_iCallCountFor2);
			q.UnSubscribe();
		}

		private bool QueueMsgProcessor(MyType obj) {
			m_iTotalCallCount++;
			if (obj.prop2 == "test2") m_iCallCountFor2++;
			return true;
		}
	}
}
