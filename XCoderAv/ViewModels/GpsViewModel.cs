using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;

namespace XCoderAv.ViewModels;

/// <summary>GPS 辅助工具 ViewModel</summary>
public partial class GpsViewModel : ObservableObject
{
    #region 属性
    /// <summary>HEX 纬度（8字符）</summary>
    [ObservableProperty]
    private String _hexLat = "";

    /// <summary>HEX 经度（8字符）</summary>
    [ObservableProperty]
    private String _hexLong = "";

    /// <summary>HEX 合并输入（16字符）</summary>
    [ObservableProperty]
    private String _hexCombined = "";

    /// <summary>转换后纬度</summary>
    [ObservableProperty]
    private String _latitude = "";

    /// <summary>转换后经度</summary>
    [ObservableProperty]
    private String _longitude = "";

    /// <summary>合并坐标</summary>
    [ObservableProperty]
    private String _latLong = "";
    #endregion

    #region 方法
    /// <summary>分离转换</summary>
    [RelayCommand]
    private void Convert()
    {
        var lat = HexLat?.Trim();
        var lng = HexLong?.Trim();

        if (lat.IsNullOrEmpty() || lng.IsNullOrEmpty()) return;

        var vLat = BitConverter.ToSingle(lat.ToHex(), 0);
        var vLng = BitConverter.ToSingle(lng.ToHex(), 0);

        Latitude = vLat.ToString();
        Longitude = vLng.ToString();
        LatLong = $"{vLat},{vLng}";
    }

    /// <summary>合并转换</summary>
    [RelayCommand]
    private void ConvertCombined()
    {
        var temp = HexCombined?.Trim();
        if (temp.IsNullOrEmpty() || temp.Length < 16) return;

        var lat = temp[..8];
        var lng = temp[8..];

        HexLat = lat;
        HexLong = lng;

        var vLat = BitConverter.ToSingle(lat.ToHex(), 0);
        var vLng = BitConverter.ToSingle(lng.ToHex(), 0);

        Latitude = vLat.ToString();
        Longitude = vLng.ToString();
        LatLong = $"{vLat},{vLng}";
    }

    /// <summary>清空</summary>
    [RelayCommand]
    private void Clear()
    {
        HexLat = "";
        HexLong = "";
        HexCombined = "";
        Latitude = "";
        Longitude = "";
        LatLong = "";
    }
    #endregion
}
