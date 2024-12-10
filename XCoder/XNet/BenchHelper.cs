using NewLife.Data;
using NewLife.Net;

namespace XCoder.XNet;

static class BenchHelper
{
    /// <summary>异步多次发送数据</summary>
    /// <param name="session">会话</param>
    /// <param name="pk">数据包</param>
    /// <param name="times">次数</param>
    /// <param name="msInterval">间隔</param>
    /// <returns></returns>
    public static Task SendConcurrency(this ISocketRemote session, IPacket pk, Int32 times, Int32 msInterval)
    {
        var task = Task.Run(async () =>
        {
            for (var i = 0; i < times; i++)
            {
                session.Send(pk);

                await Task.Delay(msInterval);
            }
        });

        return task;
    }
}