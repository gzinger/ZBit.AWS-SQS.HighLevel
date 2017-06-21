using System;
using System.Collections.Generic;
using System.Text;

namespace ZBit.Aws.Sqs.Core
{
	interface ISqsSubscriber {
		//static void Send<T>(string sQueueName, T payLoad);
		//static SQSHelper Subscribe<T>(Func<T, bool> handler, string sQueueName, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20);

		void Subscribe<T>(string sQueueName, Func<T, bool> handler, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20, Func<string, bool> errorHandler = null);
		void UnSubscribe();
	}
}
