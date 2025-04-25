// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Analysis
{
    public partial class CoreAnalysis : TscComponentBase
    {
        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = null!;

        private static readonly Dictionary<string, string> UserVisitDisplay = new()
        {
            { "uv", "日访问人数" },
            { "uvt", "日打开次数" },
            { "pv", "日访问页面数" }
        };

        private static readonly Dictionary<string, string> NewUserVisitDisplay = new()
        {
            { "newuser", "日新增用户" },
            { "newvisit", "新增用户当日打开次数" }
        };

        private GraphQLHttpClient _graphClient = null!;
        private CoreMainData? _mainData;
        private object? _permonOption;
        private object? _monVisitOption;
        private object? _monUserOption;
        private object? _monUserVisitOption;
        private object? _userLoseByDayOption;
        private object? _userLoseByWeekOption;
        private object? _userLoseByMonOption;

        private bool _dateRangeMenu;
        private List<DateOnly> _dateRange = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var now = DateTime.UtcNow.Add(CurrentTimeZone.BaseUtcOffset);
            var nowDate = DateOnly.FromDateTime(now);

            _dateRange = [nowDate.AddMonths(-1), nowDate];

            var httpClient = HttpClientFactory.CreateClient("analysis");
            _graphClient = new GraphQLHttpClient("http://10.130.0.33:4000/cubejs-api/graphql",
                new SystemTextJsonSerializer(),
                httpClient: httpClient);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _ = GetMainDataAsync();
                _ = GetPermonDataAsync();
                _ = GetMonVisitDataAsync();
                _ = GetMonUserDataAsync();
                _ = GetUserLoseByDayDataAsync();
                _ = GetUserLoseByWeekDataAsync();
                _ = GetUserLoseByMonDataAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task OnDateRangeUpdate()
        {
            if (_dateRange.Count != 2)
            {
                return;
            }

            _dateRangeMenu = false;
            await GetMonUserDataAsync();
        }

        private async Task GetMainDataAsync()
        {
            var response = await _graphClient.SendQueryAsync<CubeData<CoreMainItem>>(GetMainDataQuery());
            _mainData = response.Data.Items.FirstOrDefault()?.CoreMain;
            StateHasChanged();
        }

        private async Task GetPermonDataAsync()
        {
            var response = await _graphClient.SendQueryAsync<CubeData<PerMonItem>>(GetPermonDataQuery());
            var permonDatas = response.Data.Items.Select(x => x.PerMon).ToArray();
            var legendData = permonDatas.Select(x => x.Category).ToArray();
            var data = permonDatas.Select(x => new { name = x.Category, value = x.Count }).ToArray();
            _permonOption = GetFunnelOption(legendData, data);
            StateHasChanged();
        }

        private async Task GetMonVisitDataAsync()
        {
            var response =
                await _graphClient.SendQueryAsync<CubeData<MonVisitItem>>(GenGraphQLQuery("coremonvisit", "datekey",
                    "cnt"));
            var monVisitDatas = response.Data.Items.Select(x => x.MonVisit).ToArray();
            var x = monVisitDatas.Select(u => u.DateKey[..10]).ToArray();
            var t = monVisitDatas.Select(u => u.Count).ToArray();
            var series = new object[]
            {
                new
                {
                    data = t,
                    type = "line",
                }
            };
            _monVisitOption = GetSharedLineOption(x, series);
            StateHasChanged();
        }

        private async Task GetMonUserDataAsync()
        {
            var response = await _graphClient.SendQueryAsync<CubeData<MonUserItem>>(
                GenGraphQLQuery("coremonuser", _dateRange[0], _dateRange[1], "uv", "uvt", "pv", "newuser",
                    "newvisit"));
            var monUserDatas = response.Data.Items.Select(x => x.MonUser).ToArray();
            var x = monUserDatas.Select(u => u.DateKey.Day[..10]).ToArray();
            var uv = monUserDatas.Select(u => u.Uv).ToArray();
            var uvt = monUserDatas.Select(u => u.Uvt).ToArray();
            var pv = monUserDatas.Select(u => u.Pv).ToArray();

            var series = new object[]
            {
                new
                {
                    data = uv,
                    type = "line",
                    name = UserVisitDisplay["uv"],
                    yAxisIndex = 0
                },
                new
                {
                    data = uvt,
                    type = "line",
                    name = UserVisitDisplay["uvt"],
                    yAxisIndex = 0
                },
                new
                {
                    data = pv,
                    type = "line",
                    name = UserVisitDisplay["pv"],
                    yAxisIndex = 1
                }
            };

            string[] legendData = UserVisitDisplay.Values.ToArray();

            object yAxis = new[]
            {
                new
                {
                    type = "value",
                    position = "left",
                },
                new
                {
                    type = "value",
                    position = "right",
                }
            };

            _monUserOption = GetSharedLineOption(x, series, legendData, yAxis);

            var series2 = new object[]
            {
                new
                {
                    data = monUserDatas.Select(u => u.NewUser ?? 0).ToArray(),
                    type = "line",
                    name = NewUserVisitDisplay["newuser"]
                },
                new
                {
                    data = monUserDatas.Select(u => u.NewVisit ?? 0).ToArray(),
                    type = "line",
                    name = NewUserVisitDisplay["newvisit"]
                }
            };

            _monUserVisitOption = GetSharedLineOption(x, series2, NewUserVisitDisplay.Values.ToArray());
            StateHasChanged();
        }

        private async Task GetUserLoseByDayDataAsync()
        {
            var response = await _graphClient.SendQueryAsync<CubeData<UserLoseByDayItem>>(
                GenGraphQLQuery("coreulosebyday", "datekey", "cnt", "alive", "rate"));

            var items = response.Data.Items.Select(x => x.UserLoseByDay).ToArray();
            items = items.Select(u => u with { DateKey = u.DateKey[..10] }).ToArray();
            var x = items.Select(u => u.DateKey).ToArray();
            var series = new[]
            {
                new
                {
                    encode = new
                    {
                        x = "datekey",
                        y = "rate"
                    },
                    type = "line",
                    smooth = true
                }
            };
            _userLoseByDayOption = GetSharedLoseUserOption(x, series, items, "datekey");
            StateHasChanged();
        }

        private async Task GetUserLoseByWeekDataAsync()
        {
            var response = await _graphClient.SendQueryAsync<CubeData<UserLoseByWeekItem>>(
                GenGraphQLQuery("coreulosebyweek(orderBy:{yearweek:asc})", "weekname", "yearweek", "cnt", "alive",
                    "rate"));

            var items = response.Data.Items.Select(x => x.UserLoseByWeek).ToArray();
            var x = items.Select(u => u.WeekName).ToArray();
            var series = new[]
            {
                new
                {
                    encode = new
                    {
                        x = "weekname",
                        y = "rate"
                    },
                    type = "line",
                    smooth = true
                }
            };
            _userLoseByWeekOption = GetSharedLoseUserOption(x, series, items, "weekname");
            StateHasChanged();
        }

        private async Task GetUserLoseByMonDataAsync()
        {
            var response = await _graphClient.SendQueryAsync<CubeData<UserLoseByMonItem>>(
                GenGraphQLQuery("coreulosebymon(orderBy:{monnum:asc})", "mon", "monnum", "cnt", "alive", "rate"));

            var items = response.Data.Items.Select(x => x.UserLoseByMon).ToArray();
            var x = items.Select(u => u.Mon).ToArray();
            var series = new[]
            {
                new
                {
                    encode = new
                    {
                        x = "mon",
                        y = "rate"
                    },
                    type = "line",
                    smooth = true
                }
            };
            _userLoseByMonOption = GetSharedLoseUserOption(x, series, items, "mon");
            StateHasChanged();
        }

        private static object GetFunnelOption(string[] legendData, object data)
        {
            return new
            {
                legend = new
                {
                    data = legendData
                },
                series = new[]
                {
                    new
                    {
                        type = "funnel",
                        orient = "horizontal",
                        sort = "none",
                        label = new
                        {
                            show = true,
                            formatter = "{b}：{c}",
                            fontSize = 16
                        },
                        emphasis = new
                        {
                            label = new
                            {
                                fontSize = 20
                            }
                        },
                        data
                    }
                }
            };
        }

        private static object GetSharedLineOption(string[] xData, object series, string[]? legendData = null,
            object? yAxis = null)
        {
            return new
            {
                tooltip = new
                {
                    trigger = "axis"
                },
                legend = new
                {
                    data = legendData
                },
                xAxis = new
                {
                    type = "category",
                    boundaryGap = false,
                    data = xData
                },
                yAxis = yAxis ?? new
                {
                    type = "value"
                },
                series,
            };
        }

        private static object GetSharedLoseUserOption(string[] xData, object series, object dataset, string xName)
        {
            var xLabel = xName switch
            {
                "datekey" => "日期",
                "weekname" => "周",
                "mon" => "月",
            };

            return new
            {
                dataset = new
                {
                    source = dataset,
                },
                tooltip = new
                {
                    trigger = "axis",
                    formatter = $$"""
                                  function (params) {
                                    var { {{xName}}, cnt, alive, rate} = params[0].data;
                                    return `{{xLabel}}：${{{xName}}} <br />
                                            流失用户：${cnt} <br />
                                            存活用户：${alive} <br />
                                            流失率：${(rate*100).toFixed(2)}%`;
                                  }
                                  """,
                },
                xAxis = new
                {
                    type = "category",
                    boundaryGap = false,
                    data = xData
                },
                yAxis = new
                {
                    type = "value",
                    axisLabel = new
                    {
                        formatter = """
                                    function (value) {
                                      return (value * 100).toFixed(2) + "%";
                                    }
                                    """
                    }
                },
                series,
            };
        }

        private static GraphQLHttpRequest GetMainDataQuery() => GenGraphQLQuery("coremain", "datekey", "startuv",
            "finaluv", "aliverate", "usersign", "toyesterday", "rate");

        private static GraphQLHttpRequest GetPermonDataQuery() =>
            GenGraphQLQuery("corepermon(orderBy:{st:asc})", "category", "cnt", "st");

        private static GraphQLHttpRequest GenGraphQLQuery(string dataName, params string[] fields)
        {
            return new GraphQLHttpRequest(
                $$"""
                  query {
                    cube {
                      {{dataName}} {
                        {{string.Join("\n      ", fields)}}
                      }
                    }
                  }
                  """);
        }

        private static GraphQLHttpRequest GenGraphQLQuery(string dataName, DateOnly startDate, DateOnly endDate,
            params string[] fields)
        {
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");
            var condition = $"(orderBy:{{datekey:asc}},where:{{datekey:{{inDateRange:[\"{start}\", \"{end}\"]}}}})";

            return new GraphQLHttpRequest(
                $$"""
                  query {
                    cube {
                      {{dataName}}{{condition}} {
                        datekey {
                          day
                        }
                        {{string.Join("\n      ", fields)}}
                      }
                    }
                  }
                  """);
        }

        private record CoreMainItem(
            [property: JsonPropertyName("coremain")]
            CoreMainData CoreMain);

        private record CoreMainData(
            [property: JsonPropertyName("datekey")]
            string DateKey,
            [property: JsonPropertyName("startuv")]
            int StartUv,
            [property: JsonPropertyName("finaluv")]
            int FinalUv,
            [property: JsonPropertyName("aliverate")]
            double AliveRate,
            [property: JsonPropertyName("usersign")]
            double UserSign,
            [property: JsonPropertyName("toyesterday")]
            double ToYesterday,
            double Rate);

        private record PerMonItem(
            [property: JsonPropertyName("corepermon")]
            PerMonData PerMon);

        private record PerMonData(
            string Category,
            [property: JsonPropertyName("cnt")]
            int Count);

        private record MonVisitItem(
            [property: JsonPropertyName("coremonvisit")]
            MonVisitData MonVisit);

        private record MonVisitData(
            [property: JsonPropertyName("datekey")]
            string DateKey,
            [property: JsonPropertyName("cnt")]
            int Count);

        private record MonUserItem(
            [property: JsonPropertyName("coremonuser")]
            MonUserData MonUser);

        private record MonUserData(
            [property: JsonPropertyName("datekey")]
            DateKey DateKey,
            int Uv,
            int Uvt,
            int Pv,
            [property: JsonPropertyName("newuser")]
            int? NewUser,
            [property: JsonPropertyName("newvisit")]
            int? NewVisit);

        private record DateKey(string Day);

        private record UserLoseByDayItem(
            [property: JsonPropertyName("coreulosebyday")]
            UserLoseByDayData UserLoseByDay);

        private record UserLoseByDayData(
            [property: JsonPropertyName("datekey")]
            string DateKey,
            [property: JsonPropertyName("cnt")]
            int Count,
            int Alive,
            [property: JsonPropertyName("rate")]
            double Value);

        private record UserLoseByWeekItem(
            [property: JsonPropertyName("coreulosebyweek")]
            UserLoseByWeekData UserLoseByWeek);

        private record UserLoseByWeekData(
            [property: JsonPropertyName("weekname")]
            string WeekName,
            [property: JsonPropertyName("cnt")]
            int Count,
            int Alive,
            [property: JsonPropertyName("rate")]
            double Value);

        private record UserLoseByMonItem(
            [property: JsonPropertyName("coreulosebymon")]
            UserLoseByMonData UserLoseByMon);

        private record UserLoseByMonData(
            string Mon,
            [property: JsonPropertyName("monnum")]
            int MonNum,
            [property: JsonPropertyName("cnt")]
            int Count,
            int Alive,
            [property: JsonPropertyName("rate")]
            double Value);
    }
}