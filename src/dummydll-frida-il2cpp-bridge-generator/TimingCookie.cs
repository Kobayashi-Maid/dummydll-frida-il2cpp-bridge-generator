using System.Diagnostics;

namespace dummydll_frida_il2cpp_bridge_generator;


public class TimingCookie : IDisposable
{
    private Stopwatch _stopwatch;
    private string _name;

    public TimingCookie(string name)
    {
        _name = name;
        _stopwatch = Stopwatch.StartNew();
        Console.WriteLine(_name);
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        Console.WriteLine($"{_name} done in [{_stopwatch.Elapsed}]");
    }
}