using UnityEngine;

namespace WN
{
    public interface IDB
    {
        Database Database { set; get; }
        void Read_DB();
        void Write_DB();
    }
    public class SaveLoadSystem : MonoBehaviour
    {

        private IDeviceSaveLoad deviceSaveLoad;
        private IEncryptedDeviceSaveLoad encryptedDeviceSaveLoad;

        [SerializeField] private Database database;
        [SerializeField] private AppEvent appEvent;


        private void Awake()
        {
            deviceSaveLoad = new IDeviceSaveLoad(database);
            //encryptedDeviceSaveLoad = new IEncryptedDeviceSaveLoad(database,"WE45T");
        }

        private void OnEnable()
        {
            appEvent.OnNewGameStarted += Reset;
            appEvent.OnContinueGame += Read;
        }

        private void OnDisable()
        {
            appEvent.OnNewGameStarted -= Reset;
            appEvent.OnContinueGame -= Read;
        }

        private void Reset()
        {
            database.Reset_DB();
            appEvent.OnGameStart();
        }

        private void Read()
        {
            deviceSaveLoad.Read_DB();
            appEvent.OnGameStart();
        }
    }

}