// using System;
// using System.Security.Authentication;
// using System.Text;
// using System.Threading;
// using Unity.WebRTC;
// using UnityEngine;
// using WebSocketSharp;
// using WebSocketSharp.Server;

// namespace Unity.RenderStreaming
// {
//   public class WebSocketServer : MonoBehaviour
//   {
//     // protected override void OnMessage (MessageEventArgs e)
//     // {
//     //   var name = Context.QueryString["name"];
//     //   Send (!name.IsNullOrEmpty () ? String.Format ("\"{0}\" to {1}", e.Data, name) : e.Data);
//     // }
  

// //   public class Program
// //   {
//     // public static void Main (string[] args)
//     public void Awake()    
//     {
//       Console.WriteLine("This is C#");
//       var wssv = new WebSocketServer ("ws://localhost:4649");
//     //   wssv.AddWebSocketService<OnMessage> ("/OnMessage");
//       wssv.AddWebSocketService<Echo> ("/Echo");
//       wssv.Start ();
//       Console.ReadKey (true);
//       wssv.Stop ();
//     }
//   }

// }


// namespace Unity.RenderStreaming.Signaling
// {
//     public class WebSocketSignaling : ISignaling
//     {
//         private string m_url;
//         private float m_timeout;
//         private SynchronizationContext m_mainThreadContext;
//         private bool m_running;
//         private Thread m_signalingThread;
//         private AutoResetEvent m_wsCloseEvent;
//         private WebSocket m_webSocket;

//         public WebSocketSignaling(string url, float timeout, SynchronizationContext mainThreadContext)
//         {
//             m_url = url;
//             m_timeout = timeout;
//             m_mainThreadContext = mainThreadContext;
//             m_wsCloseEvent = new AutoResetEvent(false);
//         }

//         public void Start()
//         {
//             m_running = true;
//             m_signalingThread = new Thread(WSManage);
//             m_signalingThread.Start();
//         }


//         public void Stop()
//         {
//             if (m_running)
//             {
//                 m_running = false;
//                 m_webSocket?.Close();
//                 m_signalingThread.Abort();
//             }
//         }

//         public event OnStartHandler OnStart;
//         public event OnConnectHandler OnCreateConnection;
//         public event OnOfferHandler OnOffer;
//         #pragma warning disable 0067
//         // this event is never used in this class
//         public event OnAnswerHandler OnAnswer;
//         #pragma warning restore 0067
//         public event OnIceCandidateHandler OnIceCandidate;

//         public void SendOffer(string connectionId, RTCSessionDescription offer)
//         {
//             DescData data = new DescData();
//             data.connectionId = connectionId;
//             data.sdp = offer.sdp;
//             data.type = "offer";

//             RoutedMessage<DescData> routedMessage = new RoutedMessage<DescData>();
//             routedMessage.from = connectionId;
//             routedMessage.data = data;
//             routedMessage.type = "offer";

//             WSSend(routedMessage);
//         }

//         public void SendAnswer(string connectionId, RTCSessionDescription answer)
//         {
//             DescData data = new DescData();
//             data.connectionId = connectionId;
//             data.sdp = answer.sdp;
//             data.type = "answer";

//             RoutedMessage<DescData> routedMessage = new RoutedMessage<DescData>();
//             routedMessage.from = connectionId;
//             routedMessage.data = data;
//             routedMessage.type = "answer";

//             WSSend(routedMessage);
//         }

//         public void SendCandidate(string connectionId, RTCIceCandidate candidate)
//         {
//             CandidateData data = new CandidateData();
//             data.connectionId = connectionId;
//             data.candidate = candidate.candidate;
//             data.sdpMLineIndex = candidate.sdpMLineIndex;
//             data.sdpMid = candidate.sdpMid;

//             RoutedMessage<CandidateData> routedMessage = new RoutedMessage<CandidateData>();
//             routedMessage.from = connectionId;
//             routedMessage.data = data;
//             routedMessage.type = "candidate";

//             WSSend(routedMessage);
//         }

//         public void CreateConnection()
//         {
//             this.WSSend("{\"type\":\"connect\"}");
//         }

//         private void WSManage()
//         {
//             while (m_running)
//             {
//                 WSCreate();

//                 m_wsCloseEvent.WaitOne();

//                 Thread.Sleep((int)(m_timeout * 1000));
//             }

//             Debug.Log("Signaling: WS managing thread ended");
//         }

//         private void WSCreate()
//         {
//             m_webSocket = new WebSocket(m_url);
//             if (m_url.StartsWith("wss"))
//             {
//                 m_webSocket.SslConfiguration.EnabledSslProtocols =
//                     SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
//             }

//             m_webSocket.OnOpen += WSConnected;
//             m_webSocket.OnMessage += WSProcessMessage;
//             m_webSocket.OnError += WSError;
//             m_webSocket.OnClose += WSClosed;

//             Monitor.Enter(m_webSocket);

//             Debug.Log($"Signaling: Connecting WS {m_url}");
//             m_webSocket.ConnectAsync();
//         }

//         private void WSProcessMessage(object sender, MessageEventArgs e)
//         {
//             var content = Encoding.UTF8.GetString(e.RawData);
//             Debug.Log($"Signaling: Receiving message: {content}");

//             try
//             {
//                 var routedMessage = JsonUtility.FromJson<RoutedMessage<SignalingMessage>>(content);

//                 SignalingMessage msg;
//                 if (!string.IsNullOrEmpty(routedMessage.type))
//                 {
//                     msg = routedMessage.data;
//                 }
//                 else
//                 {
//                     msg = JsonUtility.FromJson<SignalingMessage>(content);
//                 }

//                 if (!string.IsNullOrEmpty(routedMessage.type))
//                 {
//                     if (routedMessage.type == "connect")
//                     {
//                         string connectionId = JsonUtility.FromJson<SignalingMessage>(content).connectionId;
//                         m_mainThreadContext.Post(d => OnCreateConnection?.Invoke(this, connectionId), null);
//                     }
//                     else if (routedMessage.type == "offer")
//                     {
//                         DescData offer = new DescData();
//                         offer.connectionId = routedMessage.from;
//                         offer.sdp = msg.sdp;
//                         m_mainThreadContext.Post(d => OnOffer?.Invoke(this, offer), null);
//                     }
//                     else if (routedMessage.type == "answer")
//                     {
//                         DescData answer = new DescData
//                         {
//                             connectionId = routedMessage.from,
//                             sdp = msg.sdp
//                         };
//                         m_mainThreadContext.Post(d => OnAnswer?.Invoke(this, answer), null);
//                     }
//                     else if (routedMessage.type == "candidate")
//                     {
//                         CandidateData candidate = new CandidateData
//                         {
//                             connectionId = routedMessage.@from,
//                             candidate = msg.candidate,
//                             sdpMLineIndex = msg.sdpMLineIndex,
//                             sdpMid = msg.sdpMid
//                         };
//                         m_mainThreadContext.Post(d => OnIceCandidate?.Invoke(this, candidate), null);
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError("Signaling: Failed to parse message: " + ex);
//             }
//         }

//         private void WSConnected(object sender, EventArgs e)
//         {
//             Debug.Log("Signaling: WS connected.");
//             m_mainThreadContext.Post(d => OnStart?.Invoke(this), null);
//         }


//         private void WSError(object sender, ErrorEventArgs e)
//         {
//             Debug.LogError($"Signaling: WS connection error: {e.Message}");
//         }

//         private void WSClosed(object sender, CloseEventArgs e)
//         {
//             Debug.Log($"Signaling: WS connection closed, code: {e.Code}");

//             m_wsCloseEvent.Set();
//             m_webSocket = null;
//         }

//         private void WSSend(object data)
//         {
//             if (m_webSocket == null || m_webSocket.ReadyState != WebSocketState.Open)
//             {
//                 Debug.LogError("Signaling: WS is not connected. Unable to send message");
//                 return;
//             }

//             if (data is string s)
//             {
//                 Debug.Log("Signaling: Sending WS data: " + s);
//                 m_webSocket.Send(s);
//             }
//             else
//             {
//                 string str = JsonUtility.ToJson(data);
//                 Debug.Log("Signaling: Sending WS data: " + str);
//                 m_webSocket.Send(str);
//             }
//         }
//     }
// }
