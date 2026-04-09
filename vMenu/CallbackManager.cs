using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace vMenuClient
{
    public class CallbackManager : BaseScript
    {
        private static int RequestsCurrentId;
        private static readonly Dictionary<int, TaskCompletionSource<object>> Requests;

        [EventHandler("vMenu:ServerCallbackResponse")]
        internal void ServerCallbackResponse(int requestId, object[] response)
        {
            if (Requests.TryGetValue(requestId, out var request)) return;
            request.SetResult(response);
            Requests.Remove(requestId);
        }
    }
}