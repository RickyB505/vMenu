using System;
using System.Collections.Generic;

using CitizenFX.Core;

namespace vMenuServer
{
    public class CallbackManager : BaseScript
    {
        private static readonly Dictionary<string, Func<Player, object[], object>> ServerCallbacks = new();

        public CallbackManager(string name, Func<Player, object[], object> listener)
        {
            ServerCallbacks[name] = listener;
        }

        [EventHandler("vMenu:ServerCallback")]
        private void CallbackHandler([FromSource] Player player, string name, int requestId, object[] args)
        {
            var callback = ServerCallbacks[name];

            if (callback == null)
                return;

            player.TriggerEvent("vMenu:ServerCallback", requestId, callback(player, args));
        }
    }
}
            //if (ServerCallbacks.TryGetValue(requestId, out var callback)) return;