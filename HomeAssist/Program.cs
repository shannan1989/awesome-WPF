using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiHome.Net.Dto;
using MiHome.Net.Middleware;
using MiHome.Net.Service;

var hostBuilder = Host.CreateDefaultBuilder();
//添加小米米家的驱动服务，需要小米账号和密码
hostBuilder.ConfigureServices(it => it.AddMiHomeDriver(x =>
{
    x.UserName = "手机号";
    x.Password = "密码";
}));
var miHomeDriver = hostBuilder.Build().Services.GetService<IMiHomeDriver>();
if (miHomeDriver == null)
{
}
else
{
    var deviceList = await miHomeDriver.Cloud.GetDeviceListAsync();
    foreach (XiaoMiDeviceInfo item in deviceList)
    {
        Console.WriteLine("设备ID：" + item.Did);
        Console.WriteLine("Token：" + item.Token);
        Console.WriteLine("Model：" + item.Model);
        Console.WriteLine("名称：" + item.Name);
        Console.WriteLine("描述：" + item.Desc);
        Console.WriteLine("是否在线：" + item.IsOnline);
        Console.WriteLine("WIFI名称：" + item.Ssid);
        Console.WriteLine("RSSI：" + item.Rssi);
        Console.WriteLine("局域网IP：" + item.LocalIp);
        Console.WriteLine("MAC地址：" + item.Mac);

        Console.WriteLine("设备规格");
        MiotSpec spec = await miHomeDriver.Cloud.GetDeviceSpec(item.Model);//通过设备型号获取设备规格
        Console.WriteLine("  类型：" + spec.Type);
        Console.WriteLine("  描述：" + spec.Description);
        Console.WriteLine("  服务：");
        foreach (MiotService service in spec.Services)
        {
            Console.WriteLine("    ID：" + service.Iid);
            Console.WriteLine("    类型：" + service.Type);
            Console.WriteLine("    描述：" + service.Description);
            if (service.Actions != null)
            {
                Console.WriteLine("    动作：");
                foreach (MiotAction action in service.Actions)
                {
                    Console.WriteLine("      Iid：" + action.Iid);
                    Console.WriteLine("      类型：" + action.Type);
                    Console.WriteLine("      描述：" + action.Description);
                    Console.WriteLine("      In：" + string.Join("/", action.In));
                    Console.WriteLine("      Out：" + string.Join("/", action.Out));
                    Console.WriteLine("      -----------------------------");
                }
            }
            if (service.Properties != null)
            {
                Console.WriteLine("    属性：");
                foreach (MiotProperty property in service.Properties)
                {
                    Console.WriteLine("      Iid：" + property.Iid);
                    Console.WriteLine("      类型：" + property.Type);
                    Console.WriteLine("      描述：" + property.Description);
                    Console.WriteLine("      格式：" + property.Format);
                    Console.WriteLine("      单位：" + property.Unit);
                    Console.WriteLine("      Access：" + string.Join("/", property.Access));
                    Console.WriteLine("      -----------------------------");
                }
            }
        }

        Console.WriteLine(item.Longitude);
        Console.WriteLine(item.Latitude);
        Console.WriteLine(item.Pid);
        Console.WriteLine("BSSID：" + item.Bssid);
        Console.WriteLine(item.Parent_Id);
        Console.WriteLine(item.Parent_Model);
        Console.WriteLine(item.Show_Mode);
        Console.WriteLine(item.AdminFlag);
        Console.WriteLine(item.ShareFlag);
        Console.WriteLine(item.PermitLevel);
        Console.WriteLine("UID：" + item.Uid);
        Console.WriteLine(item.Pd_id);
        Console.WriteLine(item.P2p_id);
        Console.WriteLine(item.Family_Id);
        Console.WriteLine(item.Reset_Flag);
        Console.WriteLine(item.Password);
        Console.WriteLine("--------------------------------");
    }

    // 通过米家app里设置的智能家居名称找出要操作的智能设备
    // XiaoMiDeviceInfo? device = deviceList.FirstOrDefault(it => it.Name == "客厅加湿器");
}
