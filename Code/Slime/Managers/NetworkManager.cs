using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{
    public static void SendPacket()
    {

    }

    public static void SendContentsPacket()
    {

    }

    public static async UniTask<Dictionary<string, string>> PostAsync(ePacketType packetType, Enum contentsType)
    {
        string URL = string.Empty;
        Dictionary<string, string> Form = null;
        using (UnityWebRequest Request = UnityWebRequest.Post(URL, Form))
        {
            await Request.SendWebRequest();

            if (Request.result == UnityWebRequest.Result.Success)
            {
                var Response = Util.ToObject<Response>(Request.downloadHandler.text);
                if (Response.StateCode == eStateCode.Success.GetHashCode())
                {
                    User.Update();
                    return Response.Data;
                }
                else
                {
                    Debug.LogError($"StateCode : {Response.StateCode}");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"{Request.result}");
                return null;
            }
        }
    }
}
