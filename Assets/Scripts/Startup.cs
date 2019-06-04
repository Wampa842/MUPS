using System.Globalization;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class Startup : MonoBehaviour
{
    private Process _console;

    private void StartConsole()
    {
        try
        {
            _console = new Process();
            _console.StartInfo.FileName = Path.Combine(Application.streamingAssetsPath, "OtherConsole.exe");
            _console.StartInfo.RedirectStandardInput = true;
            _console.StartInfo.UseShellExecute = false;
            MUPS.Log.LogConsole = _console;
            _console.Start();
        }
        catch(System.Exception ex)
        {
            MUPS.Log.Warning("Could not start console process.");
        }
    }

    private void Awake()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        StartConsole();
    }

    private void OnApplicationQuit()
    {
        _console.Kill();
        _console.Dispose();
        MUPS.Log.LogConsole = null;
    }
}
