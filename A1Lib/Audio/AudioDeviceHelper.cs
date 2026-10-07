using ALib.UsbHub;
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

namespace ALib.Audio
{
    public static class AudioDeviceHelper
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
            var devs = AudioDeviceHelper.GetAudioDevices();
            if (devs.ContainsKey(dev.ContainerId)) { return devs[dev.ContainerId]; }

            return null;
        }

        /// <summary>从HUB端口获取声卡</summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public static AudioDevice GetAudioDevice(this UsbHubPort dev)
        {
            if (!dev.Exists()) return null;
            return GetAudioDevice(dev.GetUsbDevice());
        }
    }
}
