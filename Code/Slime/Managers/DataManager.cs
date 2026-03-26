using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region Member Property
    private Dictionary<eTableType, object> m_GameTable = null;
    #endregion

    #region Override Method
    protected override void Init()
    {
        m_GameTable = new Dictionary<eTableType, object>();
    }
    #endregion

    #region Member Method
    public async UniTask Load()
    {
        JArray Arr = new JArray();

        for (eTableType Type = 0; Type < eTableType.End; Type++)
        {
            TextAsset TableBytes = await ResourceManager.Instance.LoadResourceAsync<TextAsset>($"Data/Table/{Type.ToString()}", false);
            var TableDecrypt = Util.Decrypt(TableBytes.bytes);
            var TableDeCompress = Util.DeCompress(TableDecrypt);

            JObject Obj = new JObject();
            Obj.Add("Key", Type.ToString());
            Obj.Add("Value", TableDeCompress);

            Arr.Add(Obj);
        }

        m_GameTable = GameTable.Parse(Arr);
    }

    public List<T> GetTable<T>(eTableType gameTable) where T : GameTable
    {
        if (m_GameTable.ContainsKey(gameTable))
        {
                return m_GameTable[gameTable] as List<T>;
        }
        else
        {
            Debug.LogError($"TableType {gameTable} is not exist");
            return null;
        }
    }

    public List<JObject> GetTable(string tableName)
    {
        eTableType type = Enum.Parse<eTableType>(tableName);

        if (m_GameTable.ContainsKey(type))
        {
            return Util.ToObject<List<JObject>>(Util.ToJson(m_GameTable[type]));
        }
        else
        {
            return new List<JObject>();
        }
    }
    #endregion
}
