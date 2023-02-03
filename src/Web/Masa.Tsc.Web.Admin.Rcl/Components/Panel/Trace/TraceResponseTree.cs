// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class TraceResponseTree : TraceResponseDto
{
    public TraceResponseTree(TraceResponseDto dto, int level)
    {
        Timestamp = dto.Timestamp;
        EndTimestamp = dto.EndTimestamp;
        TraceId = dto.TraceId;
        SpanId = dto.SpanId;
        ParentSpanId = dto.ParentSpanId;
        Kind = dto.Kind;
        TraceStatus = dto.TraceStatus;
        Name = dto.Name;
        Attributes = dto.Attributes;
        Resource = dto.Resource;

        Level = level;
    }

    public TraceResponseTree(TraceResponseDto dto, int level, List<TraceResponseTree> children) : this(dto, level)
    {
        Children = children;
    }

    public List<TraceResponseTree>? Children { get; set; }

    public int Level { get; set; }

    public List<TraceResponseTimeline> Timelines { get; set; } = new();

    public double DoubleDuration => (this.EndTimestamp - this.Timestamp).TotalMilliseconds;
}

public class TraceResponseTimeline
{
    public TraceResponseTimeline(bool Render, double Percent, double marginLeft = 0)
    {
        this.Render = Render;
        this.Percent = Percent;
        this.marginLeft = marginLeft;
    }

    public bool Render { get; set; }
    public double Percent { get; set; }
    public double marginLeft { get; set; }

    public void Deconstruct(out bool Render, out double Percent, out double marginLeft)
    {
        Render = this.Render;
        Percent = this.Percent;
        marginLeft = this.marginLeft;
    }
}