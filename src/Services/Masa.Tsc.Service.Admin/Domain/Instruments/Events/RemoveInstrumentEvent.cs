﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Instruments.Events;

public record RemoveInstrumentEvent(Instrument[] Instruments) : Event;