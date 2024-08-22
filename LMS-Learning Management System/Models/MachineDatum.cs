using System;
using System.Collections.Generic;

namespace LMS_Learning_Management_System.Models;

public partial class MachineDatum
{
    public int Id { get; set; }

    public string DeviceUserAgent { get; set; }

    public string DevicePlatformName { get; set; }

    public string DevicePlatformProcessor { get; set; }

    public string DeviceEngine { get; set; }

    public string DeviceBrowser { get; set; }

    public string DeviceType { get; set; }

    public string IpAddress { get; set; }

    public string Macaddress { get; set; }

    public string Ip2 { get; set; }
    public string Uname { get; set; }
    public string pass { get; set; }
}
