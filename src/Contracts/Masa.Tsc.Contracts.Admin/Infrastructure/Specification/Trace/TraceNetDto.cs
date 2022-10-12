// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Tsc.Contracts.Admin;

public class TraceNetDto
{
    public string Transport { get; set; }

    public string PeerIp { get; set; }

    public int PeerPort { get; set; }

    public string PeerName { get; set; }

    public string HostIp { get; set; }

    public int HostPort { get; set; }

    public string HostName { get; set; }

    public string HostConnectType { get; set; }

    public string HostConnectSubtype { get; set; }

    public string CarrierName { get; set; }

    public string CarrierMCC { get; set; }

    public string CarrierMNC { get; set; }

    public string CarrierICC { get; set; }

    public string PeerService { get; set; }
}

public class PeerDto { 
    public string Ip { get; set; }

    public string Port { get; set; }

    public string Name { get; set; }
}
