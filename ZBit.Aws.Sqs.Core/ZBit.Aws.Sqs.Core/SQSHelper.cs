using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using log4net;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace ZBit.Aws.Sqs.Core {
	public class SQSHelper : ISqsSubscriber {
		internal static readonly ILog Logger = LogManager.GetLogger(typeof(SQSHelper));
		internal static ConcurrentDictionary<string, string> g_dict = new ConcurrentDictionary<string, string>();

		internal bool m_bShouldListen;

		public static async Task<SendMessageResponse> Send<T>(string sQueueName, T payLoad) {
			using (IAmazonSQS sqs = GetClient()) {
				string sPayload = JsonConvert.SerializeObject(payLoad);
				return await sqs.SendMessageAsync(GetQueue(sqs, sQueueName), sPayload);
			}
		}

		public static SQSHelper Subscribe<T>(Func<T, bool> handler, string sQueueName, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20, Func<string, bool> errorHandler = null) {
			SQSHelper queueHelperOut = new SQSHelper();
			queueHelperOut.Subscribe(sQueueName, handler, bDeleteMsgOnSuccess, iMaxMessagesAtaTime, iWaitTimeSeconds, errorHandler);
			return queueHelperOut;
		}

		public void Subscribe<T>(string sQueueName, Func<T, bool> handler, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20, Func<string, bool> errorHandler=null) {
			IAmazonSQS sqs;
			string sQueueUrl;

			if (null == handler) {
				throw new ArgumentException("required parameter", "handler");
			}

			//Logger.DebugFormat("Subscribing to Queue {0}.", sQueueName);
			m_bShouldListen = true;
			Task.Run(() => {
				bool bHandlerRes;
				Logger.DebugFormat("Getting Queue: {0};", sQueueName);
				try {
					sqs = GetClient();
					sQueueUrl = GetQueue(sqs, sQueueName);
				} catch (Exception ex) {
					LogAndHandleError(errorHandler, "Error subscribing to Queue [{0}]: {1}", sQueueName, ex);
					//we don't care what they return in this case - we can't continue anyway
					return;
				}
				Logger.DebugFormat("Subscribing to Queue: {0}; Url: {1}", sQueueName, sQueueUrl);
				while (m_bShouldListen) {
					ReceiveMessageResponse resp;
					try {
						resp = sqs.ReceiveMessageAsync(
							new ReceiveMessageRequest(sQueueUrl) {
							WaitTimeSeconds = iWaitTimeSeconds,
								MaxNumberOfMessages = iMaxMessagesAtaTime
							}).Result;
					} catch(Exception ex1) {
						if (!LogAndHandleError(errorHandler, "Error Receiving messages from queue [{0}]: {1}", sQueueName, ex1)) {
							return;  //error hadnler told me to stop processing messages
						}
						Thread.Sleep(1000);
						continue;
					}
					if (HttpStatusCode.OK != resp.HttpStatusCode) {
						if(!LogAndHandleError(errorHandler, "Error in HTTP Response, Receiving from queue [{0}]: {1}; {2}", sQueueName, resp.HttpStatusCode, resp.ResponseMetadata)) {
							return;  //error hadnler told me to stop processing messages
						}
						Thread.Sleep(1000);
						continue;
					}
					foreach (var msg in resp.Messages) {
						T obj;
						try {
							obj = JsonConvert.DeserializeObject<T>(msg.Body);
						}catch(Exception ex2) {
							if (LogAndHandleError(errorHandler, "Error DeserializeObject from queue [{0}]: {1}", sQueueName, ex2)) {
								continue;
							} else {
								return; //error hadnler told me to stop processing messages
							} 
						}
						bHandlerRes = false;
						try {
							bHandlerRes = handler(obj);
						} catch (Exception ex3) {
							Logger.WarnFormat("Error running handler on queue [{0}]: {1}", sQueueName, ex3);
						}
						if (bHandlerRes) {
							if (bDeleteMsgOnSuccess) {
								sqs.DeleteMessageAsync(sQueueUrl, msg.ReceiptHandle).Wait();
							}
						}
					}
				}
			});
		}

		public void UnSubscribe() {
			m_bShouldListen = false;
		}

		public static string GetQueue(IAmazonSQS sqs, string sQueueName) {
			string sQueueUrl;
			if (null == sqs) throw new ArgumentException("required parameter", "sqs");
			if (!g_dict.TryGetValue(sQueueName, out sQueueUrl)) {
				var respQueCreate = sqs.CreateQueueAsync(sQueueName).Result;
				if (HttpStatusCode.OK != respQueCreate.HttpStatusCode) {
					throw new Exception("Unexpected result creating SQS: " + respQueCreate.HttpStatusCode);
				}
				sQueueUrl = respQueCreate.QueueUrl;
				g_dict[sQueueName] = sQueueUrl;
			}
			return sQueueUrl;
		}

		private static IAmazonSQS GetClient() {
			var options=ConfigMgr.Default.GetAWSOptions();
			return options.CreateServiceClient<IAmazonSQS>();
		}

		private bool LogAndHandleError(Func<string, bool> errorHandler, string sErrorFormat, params object[] args) {
			string sErr = string.Format(sErrorFormat, args);
			Logger.Error(sErr);
			if (null != errorHandler) {
				return errorHandler(sErr);
			}
			return true;
		}
	}
}
