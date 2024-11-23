using Microsoft.Data.Sqlite;
using Microsoft.Win32;

Console.WriteLine("按任意键开始执行，右上角直接关闭");
Console.ReadKey();

Console.WriteLine("开始执行");

var path = GetInstallPath();
if (path == null)
{
    Console.WriteLine("获取游戏路径失败");
    Console.ReadKey();
    return;
}

Console.WriteLine($"获取游戏安装路径成功：{path}");
var storagePath = Path.Combine(path, "Wuthering Waves Game", "Client", "Saved", "LocalStorage", "LocalStorage.db");

Console.WriteLine("-----------------------");

Console.WriteLine("修改游戏帧率...");
ChangeValue(storagePath, "CustomFrameRate", "3");
Console.WriteLine("关闭垂直同步...");
ChangeValue(storagePath, "PcVsync", "0");
Console.WriteLine("-----------------------");
Console.WriteLine("修改后游戏设置内帧数会显示为30FPS，不影响游戏帧数，可以用微星小飞机等工具查看实际帧数");
Console.WriteLine("-----------------------");


Console.WriteLine("请按任意键退出...");
Console.ReadKey();



/// <summary>
/// 通过注册表获取游戏安装路径
/// </summary>
/// <returns></returns>
static string? GetInstallPath()
{
    string registryKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
    using var key = Registry.LocalMachine.OpenSubKey(registryKey);
    if (key == null)
    {
        return null;
    }
    using var subkey = key.OpenSubKey("KRInstall Wuthering Waves");
    if (subkey == null)
    {
        return null;
    }
    return subkey.GetValue("InstallPath")?.ToString();
}

/// <summary>
/// 修改Sqlite配置
/// </summary>
/// <param name="path"></param>
/// <param name="key"></param>
/// <param name="value"></param>
/// <returns></returns>
static int ChangeValue(string path, string key, string value)
{
    using var connection = new SqliteConnection($"Data Source={path}");
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText =
    @"
        UPDATE LocalStorage
        SET value = $value
        WHERE key = $key
    ";
    command.Parameters.AddWithValue("$key", key);
    command.Parameters.AddWithValue("$value", value);

    var rows = command.ExecuteNonQuery();
    if (rows > 0)
    {
        Console.WriteLine("修改成功");
    }
    else
    {
        Console.WriteLine("修改失败");
    }
    return rows;
}