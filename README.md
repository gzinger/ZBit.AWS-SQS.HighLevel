# ZBit.AWS-SQS.HighLevel
Higher Level Library for Amazon SQS (Simple Queue Service) with event-based retrieval for .NET v4.5 and Up.

* Allows you to Subscribe to a queue instead of polling it in a busy loop. That is, you provide a delegate function in “Subscribe” method and this callback will be invoked every time a new message is added to the queue. This library will also handle serialization and deserialization for your objects that you put in the queue.

* Also, optionally, would automatically delete messages from the queue once they processed successfully, which is not done by the AWS SDK library.

* It creates a new queue either in Send or in Subscribe if the queue with the given name doesn’t exist yet under your account on AWS.

* Revision 1.1.0

Small and Simple. Requires AWSSDK.SQS, Newtonsof.Json and log4net.

Sample Send Usage:
```csharp
		[TestMethod()]
		public void SendTest() {
			string sSuperString="my strIng;some (5) special symbols: &%_@' and a quote [\"] and a backslash <\\>!";
			SQSHelper.Send("Utest1Q", new { prop1 = 11, prop2 = sSuperString });
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = sSuperString });
		}
```

Sample Subscribe Usage:
```csharp
	public class MyType{
		public int prop1{get;set;}
		public string prop2{get;set;}
	}
	
    int m_iTotalCallCount, m_iCallCountFor2;

		[TestMethod()]
		public void SubscribeTest() {
			m_iTotalCallCount = 0;
			m_iCallCountFor2 = 0;
			var q = SQSHelper.Subscribe<MyType>(QueueMsgProcessor, "Utest1Q");
			SQSHelper.Send("Utest1Q", new { prop1 = 11, prop2 = "test2" });
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = "test3" });
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = "test2" });
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
```

Nuget Package:

[![NuGet](https://img.shields.io/nuget/v/ZBit.Aws.Sqs.HL.svg)](https://www.nuget.org/packages/ZBit.Aws.Sqs.HL/)
