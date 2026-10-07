using NAudio.CoreAudioApi;
using NewLife.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Audio
{
    public class AudioDevice
    {
        /// <summary>设备名称 ContainerId</summary>
        public string Name { get; set; }

        /// <summary>ContainerID</summary>
        public string ContainerId { get; set; }

        /// <summary>播放设备</summary>
        public MMDevice PlaybackDevice;

        /// <summary>录音设备</summary>
        public MMDevice RecordingDevice;


        #region

        /// <summary>创建录音器,wav</summary>
        /// <returns></returns>
        public AudioRecord CreateWavRecord()
        {
            if (RecordingDevice == null) return null;
            if (RecordingDevice.AudioClient.MixFormat.Channels < 1) return null;

            return new AudioRecord(this);
        }

        #endregion

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
}
