using UnityEngine;

namespace WN
{
    public class IEncryptedDeviceSaveLoad : IDB
    {
        public Database Database { set; get; }


        public IEncryptedDeviceSaveLoad(Database db, string encryptionKey)
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

        private string EncryptData(string plainText)
        {
            //encrypt;
            string cipherText = "";
            return cipherText;
        }

        private string Descrypt(string cipherText)
        {
            string plainText = "";
            return plainText;
        }
    }

}