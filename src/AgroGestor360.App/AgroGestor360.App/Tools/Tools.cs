
namespace AgroGestor360.App.Tools;

public class CurrentDevicePlatform
{
    public static string GetDevicePlatform()
    {
        IDeviceInfo device = DeviceInfo.Current;

        var platformNames = new Dictionary<DevicePlatform, string>{ { DevicePlatform.iOS, "iOS" }, { DevicePlatform.Android, "Android" },
        { DevicePlatform.WinUI, "Windows" },
        { DevicePlatform.macOS, "macOS" }};

        platformNames.TryGetValue(device.Platform, out string? platform);

        return platform ?? "Unknown";
    }

}
