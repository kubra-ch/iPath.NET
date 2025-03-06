using System.Runtime.InteropServices;

namespace iPath.Data.Helper;

/*
 * https://www.linkedin.com/pulse/generating-sequential-guid-directly-c-saurabh-kumar-verma-qshfc/
 */

public class SequentialGuidUtility
{
    private static readonly object Lock1 = new object();
    private static long _lastTimestamp = DateTime.UtcNow.Ticks;

    public static Guid NewSequentialGuid()
    {
        lock (Lock1)
        {
            long timestamp = DateTime.UtcNow.Ticks;

            if (timestamp <= _lastTimestamp)
            {
                timestamp = _lastTimestamp + 1;
            }

            _lastTimestamp = timestamp;

            var guidBinary = new byte[16];
            Array.Copy(BitConverter.GetBytes(timestamp), 0, guidBinary, 0, 8);
            Array.Copy(Guid.NewGuid().ToByteArray(), 8, guidBinary, 8, 8);

            return new Guid(guidBinary);
        }
    }

    [DllImport("rpcrt4.dll", SetLastError = true)]
    static extern int UuidCreateSequential(out Guid guid);

    private static readonly object Lock2 = new object();

    public static Guid GetGuid()
    {
        lock (Lock2)
        {
            Guid guid;
            UuidCreateSequential(out guid);

            return guid;
        }
    }
}