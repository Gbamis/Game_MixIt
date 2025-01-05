using UnityEngine;

namespace WN
{
    public class IDeviceSaveLoad : IDB
    {
        public Database Database { set; get; }


        public IDeviceSaveLoad(Database db)
        {
            Database = db;
            Database.OnDatabaseChanged += Write_DB;
        }

        public void Reset_DB() => Database.Reset_DB();

        public void Read_DB()
        {
            string saved = PlayerPrefs.GetString("_userData");
            Debug.Log(saved);
            Database.SetValues(JsonUtility.FromJson<DBValues>(saved));
        }
        public void Write_DB()
        {
            string data = JsonUtility.ToJson(Database.dBValues);
            PlayerPrefs.SetString("_userData", data);
        }
    }
}
