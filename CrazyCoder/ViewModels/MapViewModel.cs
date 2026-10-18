using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Map;
using NewLife.Model;
using NewLife.Reflection;
using NewLife.Serialization;

namespace CrazyCoder.ViewModels;

/// <summary>地图接口 ViewModel</summary>
public partial class MapViewModel : ObservableObject
{
    #region 属性
    /// <summary>地图类型列表</summary>
    public ObservableCollection<String> MapTypes { get; } = [];

    /// <summary>选中的地图类型</summary>
    [ObservableProperty]
    private String _selectedMap = "";

    /// <summary>方法列表</summary>
    public ObservableCollection<String> Methods { get; } = [];

    /// <summary>选中的方法</summary>
    [ObservableProperty]
    private String _selectedMethod = "";

    /// <summary>坐标类型</summary>
    public ObservableCollection<String> CoordTypes { get; } = ["wgs84", "gcj02", "bd09ll"];

    /// <summary>选中的坐标类型</summary>
    [ObservableProperty]
    private String _selectedCoordType = "wgs84";

    /// <summary>地址</summary>
    [ObservableProperty]
    private String _address = "";

    /// <summary>城市</summary>
    [ObservableProperty]
    private String _city = "";

    /// <summary>经纬度</summary>
    [ObservableProperty]
    private String _location = "";

    /// <summary>经纬度2</summary>
    [ObservableProperty]
    private String _location2 = "";

    /// <summary>结果日志</summary>
    [ObservableProperty]
    private String _resultLog = "";

    /// <summary>格式化地址</summary>
    [ObservableProperty]
    private Boolean _formatAddress;

    private Dictionary<String, Map> _cache = [];
    #endregion

    #region 构造
    /// <summary>实例化地图接口 ViewModel</summary>
    public MapViewModel()
    {
        LoadMaps();
    }

    private void LoadMaps()
    {
        MapTypes.Clear();
        foreach (var item in typeof(IMap).GetAllSubclasses())
        {
            MapTypes.Add(item.Name);
        }
        if (MapTypes.Count > 0) SelectedMap = MapTypes[0];
    }

    partial void OnSelectedMapChanged(String value)
    {
        Methods.Clear();
        if (value.IsNullOrEmpty()) return;

        var type = typeof(IMap).GetAllSubclasses().FirstOrDefault(e => e.Name == value);
        if (type == null) return;

        var methods = type.GetMethods()
            .Where(m => m.IsPublic && !m.IsStatic && m.DeclaringType != typeof(Object))
            .Select(m => m.Name);
        foreach (var m in methods)
        {
            Methods.Add(m);
        }
        if (Methods.Count > 0) SelectedMethod = Methods[0];
    }
    #endregion

    #region 调用
    /// <summary>调用地图接口</summary>
    [RelayCommand]
    private async void Invoke()
    {
        var typeName = SelectedMap;
        var methodName = SelectedMethod;
        if (typeName.IsNullOrEmpty() || methodName.IsNullOrEmpty()) return;

        var type = typeof(IMap).GetAllSubclasses().FirstOrDefault(e => e.Name == typeName);
        if (type == null) return;

        try
        {
            if (!_cache.TryGetValue(typeName, out var map))
            {
                var provider = ObjectContainer.Provider;
                map = type.GetConstructor(new[] { typeof(IServiceProvider) }) != null
                    ? type.CreateInstance(provider) as Map
                    : type.CreateInstance() as Map;
                if (map != null) map.Log = XTrace.Log;
                _cache[typeName] = map!;
            }

            if (map == null) return;

            // 设置 AppKey
            if (map is BaiduMap bmap)
                bmap.AppKey = "C73357a276668f8b0563d3f936475007";
            else if (map is AMap amap)
                amap.AppKey = "038a84bf20e8306fdd2203110739110c";
            else if (map is WeMap wmap)
                wmap.AppKey = "YGEBZ-BDCCX-AJG4X-ZUH6W-MESMV-P2BFF";

            var im = map as IMap;
            var addr = Address;
            var city = City;
            if (!GeoPoint.TryParse(Location, out var point) || !GeoPoint.TryParse(Location2, out var point2))
            {
                ResultLog = "经纬度格式非法，应为 \"经度,纬度\"，如：116.4,39.9";
                return;
            }

            Object result = null;

            if (methodName == nameof(im.GetGeoAsync))
            {
                result = await im.GetGeoAsync(addr, city, SelectedCoordType, FormatAddress);
            }
            else if (methodName == nameof(im.GetReverseGeoAsync))
            {
                result = await im.GetReverseGeoAsync(point, SelectedCoordType);
            }
            else if (methodName == nameof(im.GetDistanceAsync))
            {
                result = await im.GetDistanceAsync(point, point2, SelectedCoordType);
            }
            else if (map is BaiduMap bd)
            {
                if (methodName == nameof(bd.PlaceSearchAsync))
                    result = await bd.PlaceSearchAsync(addr, null, city, SelectedCoordType, FormatAddress);
                else if (methodName == nameof(bd.ConvertAsync))
                    result = await bd.ConvertAsync(new[] { point }, SelectedCoordType, "bd09ll");
                else if (methodName == nameof(bd.IpLocationAsync))
                    result = await bd.IpLocationAsync(addr, SelectedCoordType);
            }
            else if (map is AMap am && methodName == nameof(am.GetAreaAsync))
            {
                result = (await am.GetAreaAsync(city))?.ToArray();
            }
            else
            {
                var ps = new Dictionary<String, Object>();
                if (methodName.Contains("address", StringComparison.OrdinalIgnoreCase)) ps["address"] = addr;
                if (methodName.Contains("city", StringComparison.OrdinalIgnoreCase)) ps["city"] = city;
                if (methodName.Contains("point", StringComparison.OrdinalIgnoreCase)) ps["point"] = point;

                var task = map.InvokeWithParams(type.GetMethod(methodName), ps) as Task;
                if (task != null)
                {
                    await task;
                    result = task.GetValue("Result");
                }
            }

            ResultLog = result?.ToJson(true) ?? "无结果";
        }
        catch (Exception ex)
        {
            ex = ex.GetTrue();
            ResultLog = ex.ToString();
        }
    }

    /// <summary>清空日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        ResultLog = "";
    }
    #endregion
}
