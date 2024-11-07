using UnityEngine;
using System.IO.Ports;
using System.Threading;
using Newtonsoft.Json.Linq;

public class ArduinoReader : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;
    private string jsonData = "";

    // Public array to store weight values
    public float[] weights = new float[8];

    // Singleton instance
    public static ArduinoReader Instance;

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Replace "COM3" with your Arduino's COM port
        serialPort = new SerialPort("COM9", 9600);
        serialPort.ReadTimeout = 1000;

        try
        {
            serialPort.Open();
            isRunning = true;
            readThread = new Thread(ReadSerialData);
            readThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(jsonData))
        {
            ProcessData(jsonData);
            jsonData = "";
        }
    }

    void ReadSerialData()
    {
        while (isRunning)
        {
            try
            {
                string line = serialPort.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    lock (this)
                    {
                        jsonData = line;
                    }
                }
            }
            catch (System.Exception)
            {
                // Timeout or other error
            }
        }
    }

    void ProcessData(string data)
    {
        try
        {
            string[] values = data.Split(',');
            for (int i = 0; i < weights.Length && i < values.Length; i++)
            {
                if (float.TryParse(values[i], out float weight))
                {
                    weights[i] = weight;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing data: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
