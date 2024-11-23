using NeuroSDK;
using SignalMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI; // Для работы с UI



public class ImGenius : MonoBehaviour
{
    
    private BrainBitController brainBitController;
    private BrainBitInfo sen;
    public Text text1; // Текст для отображения статусов
    public Text o1Text; // Текст для O1
    public Text o2Text; // Текст для O2
    public Text t3Text; // Текст для T3
    public Text t4Text; // Текст для T4
    public Text AttText;
    public Text RelText;
    public Button stopCalibrationButton; // Кнопка для остановки калибровки
    private Dictionary<string, BrainBitAdditional> connectedDevices = new();
    bool isConn = false;
    public GameObject bt1;
    public GameObject bt2;

    public float att;
    public float rel;
    

    void Awake()
    {
        brainBitController = GameObject.FindWithTag("NeuroController").GetComponent<BrainBitController>();
    }

    public async void gg()
    {
        bt1.SetActive(false);
        text1.text = "Loading..."; // Установка статуса загрузки
        var sensors = await brainBitController.SearchWithResult(5, new List<string>());
        var senso = sensors.ToList();
        foreach (var sens in senso)
        {
            Debug.Log("123");
            if (sens.Address == "FB:91:D4:A7:CC:24")
            {
                sen = sens;
                text1.text = "Connecting"; // Установка статуса завершения
                isConn = true;
                await brainBitController.ConnectTo(sen, true);
                text1.text = "Done";
                connectedDevices = (Dictionary<string, BrainBitAdditional>)brainBitController.asd();
                Debug.Log(connectedDevices);
                Debug.Log("CONNECTED");
                break;
            }
            else
            {
                text1.text = "Not Done"; // Установка статуса, если устройство не найдено
            }
        }
        if (!isConn)
        {
            text1.text = "No Devices Found"; // Установка статуса, если устройства не найдены
        }
        bt1.SetActive(!isConn);
        bt2.SetActive(isConn);
        stopCalibrationButton.gameObject.SetActive(isConn);
      }












    public async void StartCalibration()
    {

            var address = sen.Address;
            var tmp = connectedDevices[address].bb;
            connectedDevices[address].calibrationStarted = true;
            tmp.EventBrainBitResistDataRecived += eventBrainBitResistDataRecived; // Подписка на событие получения данных сопротивления
            text1.text = "Loading Calibration..."; // Установка статуса загрузки калибровки
            await brainBitController.StartResist(address); // Запуск калибровки
            text1.text = "Calibration Done"; // Установка статуса завершения калибровки
            stopCalibrationButton.gameObject.SetActive(true); // Показать кнопку остановки калибровки
        
    }

    public void StopCalibration()
    {
      
            var address = sen.Address;
            var tmp = connectedDevices[address].bb;
            tmp.EventBrainBitResistDataRecived -= eventBrainBitResistDataRecived; // Отписка от события
            brainBitController.StopResist(address); // Остановка калибровки
            text1.text = "Calibration Stopped"; // Установка статуса остановки калибровки
            stopCalibrationButton.gameObject.SetActive(false); // Скрыть кнопку остановки калибровки
            StartCalculations(address); // Запуск расчетов после завершения калибровки
        
    }

    public async void StartCalculations(string address = "FB:91:D4:A7:CC:24")
    {
        await Task.Run(() =>
        {
            var tmp = connectedDevices[address].bb;
            connectedDevices[address].calibrationStarted = true;
            connectedDevices[address].emotionalMath.StartCalibration();
            tmp.EventBrainBitSignalDataRecived += eventBrainBitSignalDataRecived; // Подписка на событие получения данных сигнала
            tmp.ExecCommand(SensorCommand.CommandStartSignal); // Запуск получения сигнала
            connectedDevices[address].isSignal = true;
            UnityMainThreadDispatcher.Enqueue(() =>
        {
            text1.text = "Loading Signal Data..."; // Установка статуса загрузки данных сигнала
            });
        });
    }

    private void eventBrainBitResistDataRecived(ISensor sensor, BrainBitResistData data)
{
    // Обработка полученных данных сопротивления
    if (connectedDevices.TryGetValue(sensor.Address, out var device))
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            o1Text.text = data.O1 < 2500000 ? "O1: Normal" : "O1: Bad";
            o2Text.text = data.O2 < 2500000 ? "O2: Normal" : "O2: Bad";
            t3Text.text = data.T3 < 2500000 ? "T3: Normal" : "T3: Bad";
            t4Text.text = data.T4 < 2500000 ? "T4: Normal" : "T4: Bad";
        });
    }
}

    private void eventBrainBitSignalDataRecived(ISensor sensor, BrainBitSignalData[] data)
    {
        // Обработка полученных данных сигнала
        Debug.Log($"Received signal data from {sensor.Address} with {data.Length} samples.");
        // Здесь можно добавить логику обработки данных для получения Attention и Relaxation
        var samples = new RawChannels[data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            
            samples[i].LeftBipolar = data[i].T3 - data[i].O1;
            samples[i].RightBipolar = data[i].T4 - data[i].O2;
        }
        var math = connectedDevices[sensor.Address].emotionalMath;
        math.PushData(samples);
        math.ProcessDataArr();
        MindData[] mindData = math.ReadMentalDataArr();
        if (mindData.Length > 0)
        {
            MindData md = mindData.Last();
            att = (float)md.RelAttention;
            rel = (float)md.RelRelaxation;
            UnityMainThreadDispatcher.Enqueue(() =>
        {
            AttText.text = "Atention:"+att.ToString();
            RelText.text = "Relaxation:"+rel.ToString();
            });
        }
    }
    public void disc()
    {
        brainBitController.DisconnectFrom("FB:91:D4:A7:CC:24");
        text1.text = "disconnected";
    }
}