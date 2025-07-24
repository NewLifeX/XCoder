
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NewLife;
using NewLife.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XCoder
{
    public class AudioDevice
    {
        /// <summary>设备名称</summary>
        public string Name { get; set; }

        /// <summary>ContainerID</summary>
        public string ContainerId { get; set; }

        /// <summary>播放设备</summary>
        public MMDevice PlaybackDevice;

        /// <summary>录音设备</summary>
        public MMDevice RecordingDevice;

        public override string ToString()
        {
            var sb = Pool.StringBuilder.Get();

            sb.AppendLine($"设备：{Name}");
            sb.AppendLine($"ContainerID：{ContainerId}");

            sb.Append("播放:");
            if (PlaybackDevice != null)
            {
                sb.Append($"  声道：{PlaybackDevice.AudioClient.MixFormat.Channels}");
                sb.Append($"  采样率：{PlaybackDevice.AudioClient.MixFormat.SampleRate}");
            }
            else
            {
                sb.Append("无");
            }
            sb.AppendLine();

            sb.Append("录音:");
            if (RecordingDevice != null)
            {
                sb.Append($"  声道：{RecordingDevice.AudioClient.MixFormat.Channels}");
                sb.Append($"  采样率：{RecordingDevice.AudioClient.MixFormat.SampleRate}");
            }
            else
            {
                sb.Append("无");
            }
            sb.AppendLine();

            return sb.Return(true);
        }

        /// <summary>字符串截取括号内内容</summary>
        /// <remarks>FriendlyName 中截取括号内声卡名称</remarks>
        /// <param name="str"></param>
        /// <returns></returns>
        private string getCardName(string str)
        {
            if (str == null) return null;

            int start = str.IndexOf('(') + 1;
            int end = str.IndexOf(')', start);
            if (start > 0 && end > start)
            {
                return str.Substring(start, end - start);
            }

            return null;
        }

        /// <summary>获取声卡名称</summary>
        /// <returns></returns>
        public string GetCardName()
        {
            if (PlaybackDevice != null)
            {
                return PlaybackDevice.DeviceFriendlyName;
                //return getCardName(PlaybackDevice.DeviceFriendlyName);
            }
            if (RecordingDevice != null)
            {
                return RecordingDevice.DeviceFriendlyName;
                //return getCardName(RecordingDevice.DeviceFriendlyName);
            }
            return null;
        }

    }

    public static class AudioHelper
    {
        /// <summary>从注册表获取音频设备ContainerID</summary>
        /// <param name="deviceId"></param>
        /// <param name="basepath"></param>
        /// <returns></returns>
        public static string GetAudioContainerID(string deviceId, string basepath = "SYSTEM\\CurrentControlSet\\Enum\\SWD\\MMDEVAPI\\")
        {
            string registryPath = basepath + deviceId;

            using (RegistryKey deviceKey = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (deviceKey == null) return null;

                string containerIdStr = deviceKey.GetValue("ContainerID") as string;
                if (!string.IsNullOrEmpty(containerIdStr))
                {
                    return containerIdStr;
                    // Guid.TryParse(containerIdStr, out Guid containerId);
                    // return containerId;
                }
            }

            return null;
        }

        /// <summary>获取所有声卡</summary>
        /// <returns></returns>
        public static Dictionary<string, AudioDevice> GetAudioDevices()
        {
            var dic = new Dictionary<string, AudioDevice>();
            var enumerator = new MMDeviceEnumerator();
            var playbackDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            var recordingDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var dev in playbackDevices)
            {
                var containerId = GetAudioContainerID(dev.ID);
                if (containerId != null)
                {
                    if (!dic.TryGetValue(containerId, out var audioDevice))
                    {
                        audioDevice = new AudioDevice { ContainerId = containerId };
                        dic[containerId] = audioDevice;
                    }
                    audioDevice.PlaybackDevice = dev;
                }
            }
            foreach (var dev in recordingDevices)
            {
                var containerId = GetAudioContainerID(dev.ID);
                if (containerId != null)
                {
                    if (!dic.TryGetValue(containerId, out var audioDevice))
                    {
                        audioDevice = new AudioDevice { ContainerId = containerId };
                        dic[containerId] = audioDevice;
                    }
                    audioDevice.RecordingDevice = dev;
                }
            }

            foreach (var item in dic)
            {
                var dev = item.Value;
                // 尝试从ContainerID获取设备名称
                dev.Name = dev.ContainerId;
                var name = dev.GetCardName();
                if (!name.IsNullOrEmpty()) dev.Name = name;
            }

            return dic;
        }

        /// <summary>从USB设备获取声卡</summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public static AudioDevice GetAudioDevice(this UsbDevice dev)
        {
            var devs = AudioHelper.GetAudioDevices();
            if (devs.ContainsKey(dev.ContainerId)) { return devs[dev.ContainerId]; }

            return null;
        }
    }
}
