﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Timers;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Timer = System.Timers.Timer;


namespace Masa.Tsc.Web.Admin.Rcl.Pages.Analysis
{
    public partial class AppVisitAnalysis : IDisposable
    {
        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = null!;

        private const string UvTitle = "访问人数";
        private const string UvtTitle = "打开次数";
        private const string PvTitle = "访问页面数";
        private const string Top10UvTitle = UvTitle + " Top10";
        private const string Top10UvtTitle = UvtTitle + " Top10";
        private const string Top10PvTitle = PvTitle + " Top10";

        private GraphQLHttpClient _graphClient = null!;
        private AppVisit? _uva;
        private AppVisit? _uvta;
        private AppVisit? _pva;
        private AppVisit? _aliverate;

        private object? _mapOption;
        private object? _uvOption;
        private object? _uvtOption;
        private object? _pvOption;
        private object? _uvPageOption;
        private object? _uvtPageOption;
        private object? _pvPageOption;

        private bool _loading;
        private StringNumber _interval = 5;
        private DateTime _lastRefreshTime = DateTime.UtcNow;
        private Timer? _timer;

        private bool ignoreLogin;
        private bool ignoreHome;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var httpClient = HttpClientFactory.CreateClient("analysis");
            _graphClient = new GraphQLHttpClient("http://10.130.0.33:4000/cubejs-api/graphql",
                new SystemTextJsonSerializer(),
                httpClient: httpClient);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ReloadAll();

                _timer = new Timer(TimeSpan.FromMinutes(_interval.ToInt32()));
                _timer.AutoReset = true;
                _timer.Elapsed += TimerOnElapsed;
                _timer.Start();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            InvokeAsync(ReloadAll);
        }

        private async Task ReloadAll()
        {
            _lastRefreshTime = DateTime.UtcNow;

            _loading = true;
            StateHasChanged();

            await RefreshAppVisitDataAsync();
            _loading = false;

            _ = RefreshMapDataAsync();
            _ = RefreshAppVisitHourAsync();
            _ = RefreshAppVisitPageAsync2();
        }

        private void OnIntervalChanged(StringNumber val)
        {
            if (_interval == val)
            {
                return;
            }

            if (_timer is null)
            {
                throw new InvalidOperationException("Timer is not initialized");
            }

            _interval = val;
            _lastRefreshTime = DateTime.UtcNow;

            var minutes = TimeSpan.FromMinutes(val.ToInt32());
            _timer.Interval = minutes.TotalMilliseconds;
        }

        private void OnIgnoreLoginChanged(bool val)
        {
            ignoreLogin = val;
            _ = RefreshAppVisitPageAsync2();
        }

        private void OnIgnoreHomeChanged(bool val)
        {
            ignoreHome = val;
            _ = RefreshAppVisitPageAsync2();
        }

        private async Task RefreshMapDataAsync()
        {
            var response = await _graphClient.SendQueryAsync<Data<AreaVisitItem>>(GetMapDataQuery());
            var data = response.Data.Items.Select(x => new
            {
                name = ADCodeName[x.AreaVisit.ADCode],
                value = x.AreaVisit.Pv,
                uv = x.AreaVisit.Uv,
                uvt = x.AreaVisit.Uvt
            }).ToArray();

            var max = RoundUpToNearest(data.Max(x => x.value));
            UpdateMapOption(data, max);
        }

        private async Task RefreshAppVisitHourAsync()
        {
            var response = await _graphClient.SendQueryAsync<Data<AppVisitHourItem>>(GetAppVisitHourQuery());

            var x = response.Data.Items.Select(u => u.AppVisitHour.TimeKey).ToArray();
            var tuv = response.Data.Items.Select(u => u.AppVisitHour.Tuv).ToArray();
            var yuv = response.Data.Items.Select(u => u.AppVisitHour.Yuv).ToArray();
            var suv = response.Data.Items.Select(u => u.AppVisitHour.Suv).ToArray();
            var tuvt = response.Data.Items.Select(u => u.AppVisitHour.Tuvt).ToArray();
            var yuvt = response.Data.Items.Select(u => u.AppVisitHour.Yuvt).ToArray();
            var suvt = response.Data.Items.Select(u => u.AppVisitHour.Suvt).ToArray();
            var tpv = response.Data.Items.Select(u => u.AppVisitHour.Tpv).ToArray();
            var ypv = response.Data.Items.Select(u => u.AppVisitHour.Ypv).ToArray();
            var spv = response.Data.Items.Select(u => u.AppVisitHour.Spv).ToArray();

            _uvOption = GetSharedHourOption(UvTitle, x, tuv, yuv, suv);
            _uvtOption = GetSharedHourOption(UvtTitle, x, tuvt, yuvt, suvt);
            _pvOption = GetSharedHourOption(PvTitle, x, tpv, ypv, spv);
            StateHasChanged();
        }

        private async Task RefreshAppVisitPageAsync2()
        {
            _uvPageOption = await RefreshAppVisitPageAsync(AppVisitType.Uv);
            _uvtPageOption = await RefreshAppVisitPageAsync(AppVisitType.Uvt);
            _pvPageOption = await RefreshAppVisitPageAsync(AppVisitType.Pv);
            StateHasChanged();
        }

        private async Task<object> RefreshAppVisitPageAsync(AppVisitType type)
        {
            var response = await _graphClient.SendQueryAsync<Data<AppVisitPageItem>>(GetAppVisitPageQuery(type));
            var data = response.Data.Items.Select(x => new
            {
                x.AppVisitPage.Path,
                value = x.AppVisitPage.Count,
                x.AppVisitPage.Rate
            }).OrderBy(u => u.value).ToArray();

            var keys = data.Select(x => x.Path).ToArray();
            var max = RoundUpToNearest(data.Max(x => x.value));

            return GetSharedAppVisitPageOption(keys, data, max);
        }


        private async Task RefreshAppVisitDataAsync()
        {
            _uva = await GetAppVisitAsync(AppVisitType.Uv);
            _uvta = await GetAppVisitAsync(AppVisitType.Uvt);
            _pva = await GetAppVisitAsync(AppVisitType.Pv);
            _aliverate = await GetAppVisitAsync(AppVisitType.Aliverate);
            StateHasChanged();
        }

        private async Task<AppVisit> GetAppVisitAsync(AppVisitType type)
        {
            var query = GetAppVisitQuery(type);
            var response = await _graphClient.SendQueryAsync<Data<AppVisitItem>>(query);
            return response.Data.Items.First().AppVisit;
        }

        private void UpdateMapOption(object data, int max)
        {
            _mapOption = new
            {
                title = new
                {
                    text = "访问地图",
                    left = "left"
                },
                tooltip = new
                {
                    trigger = "item",
                    formatter = """
                                function (params) {
                                  var { name, value, uv, uvt } = params.data;
                                  return `省份：<b>${name}</b><br>页面访问：<b>${value}</b><br>访问用户数：<b>${uv}</b><br>用户访问次数：<b>${uvt}</b>`;
                                }
                                """
                },
                visualMap = new
                {
                    min = 0,
                    max = max,
                    left = "24px",
                    bottom = "24px",
                    text = new[] { "高", "低" },
                    calculable = true,
                    inRange = new
                    {
                        color = new[] { "#d9ebfa", "#125998" }
                    }
                },
                series = new
                {
                    type = "map",
                    map = "china",
                    roam = true,
                    data
                }
            };

            StateHasChanged();
        }

        private static object GetSharedHourOption(string title, string[] x, int[] t, int[] y, int[] s)
        {
            return new
            {
                title = new
                {
                    text = title
                },
                tooltip = new
                {
                    trigger = "axis"
                },
                legend = new
                {
                    data = new[] { "今日", "昨天", "7天前" }
                },
                grid = new
                {
                    left = "3%",
                    right = "4%",
                    bottom = "3%",
                    containLabel = true
                },
                toolbox = new
                {
                    feature = new
                    {
                        saveAsImage = new { }
                    }
                },
                xAxis = new
                {
                    type = "category",
                    boundaryGap = false,
                    data = x
                },
                yAxis = new
                {
                    type = "value"
                },
                series = new[]
                {
                    new
                    {
                        name = "今日",
                        type = "line",
                        smooth = true,
                        data = t
                    },
                    new
                    {
                        name = "昨天",
                        type = "line",
                        smooth = true,
                        data = y
                    },
                    new
                    {
                        name = "7天前",
                        type = "line",
                        smooth = true,
                        data = s
                    }
                }
            };
        }

        private static object GetSharedAppVisitPageOption(string[] keys, object[] data, int max)
        {
            return new
            {
                tooltip = new
                {
                    trigger = "axis",
                    formatter = """
                                function (params) {
                                  var { path, value, rate } = params[0].data;
                                  return `页面：<b>${path}</b><br>访问次数：<b>${value}</b><br>占比：<b>${(rate * 100).toFixed(2)}%</b>`;
                                }
                                """,
                    axisPointer = new
                    {
                        type = "shadow"
                    }
                },
                grid = new
                {
                    right = 16,
                    top = 0,
                    bottom = 0,
                    containLabel = true
                },
                xAxis = new
                {
                    type = "value",
                    max = max
                },
                yAxis = new
                {
                    type = "category",
                    data = keys,
                },
                series = new[]
                {
                    new
                    {
                        name = "数量",
                        type = "bar",
                        data = data,
                        label = new
                        {
                            show = true,
                            position = "outside"
                        }
                    }
                }
            };
        }

        private static GraphQLHttpRequest GetAppVisitQuery(AppVisitType type)
        {
            var props = type switch
            {
                AppVisitType.Uv => """
                                   data1: uva
                                   data2: yesterdayuvrate
                                   data3: seventhuvrate
                                   """,
                AppVisitType.Uvt => """
                                    data1: uvta
                                    data2: yesterdayuvrate
                                    data3: seventhuvrate
                                    """,
                AppVisitType.Pv => """
                                   data1: pva
                                   data2: yesterdaypvrate
                                   data3: seventhpvrate
                                   """,
                AppVisitType.Aliverate => """
                                          data1: aliverate
                                          data2: todaynewuser
                                          data3: todayvist
                                          """,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            return new GraphQLHttpRequest(
                $$"""
                    query {
                      cube {
                        appinitvisit {
                          {{props}}
                        }
                      }
                    }
                  """);
        }

        private static GraphQLHttpRequest GetMapDataQuery()
        {
            return new GraphQLHttpRequest(
                $$"""
                    query {
                      cube {
                        areavisit {
                          pv
                          uv
                          uvt
                          adcode
                        }
                      }
                    }
                  """);
        }

        private static GraphQLHttpRequest GetAppVisitHourQuery()
        {
            return new GraphQLHttpRequest(
                $$"""
                  query {
                    cube {
                      inithourtotal {
                        timenum
                        timekey
                        tuv
                        yuv
                        suv
                        tuvt
                        yuvt
                        suvt
                        tpv
                        ypv
                        spv
                      }
                    }
                  }
                  """);
        }

        private GraphQLHttpRequest GetAppVisitPageQuery(AppVisitType type)
        {
            var countName = type switch
            {
                AppVisitType.Uv => "uv",
                AppVisitType.Uvt => "uvt",
                AppVisitType.Pv => "pv",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var rateName = type switch
            {
                AppVisitType.Uv => "uvrate",
                AppVisitType.Uvt => "uvtrate",
                AppVisitType.Pv => "pvrate",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var filter = string.Empty;
            List<string> _urls = [];

            if (ignoreLogin)
            {
                _urls.Add("/home/index/{IsLogin:bool}");
            }

            if (ignoreHome)
            {
                _urls.Add("/");
            }

            if (_urls.Count > 0)
            {
                filter = $"path:{{notIn:[{string.Join(",", _urls.Select(x => $"\"{x}\""))}]}}";
            }

            var r = new GraphQLHttpRequest(
                $$"""
                    query {
                      cube(limit:10) {
                        initpage(orderBy:{{{countName}}:desc}, where:{{{filter}}}){
                          path
                          count: {{countName}}
                          rate: {{rateName}}
                        }
                      }
                    }
                  """);

            Console.Out.WriteLine("query: " + r.Query);

            return r;
        }

        // TODO: Test this method, like value 0 or there has more effective way to implement this method
        private static int RoundUpToNearest(int value)
        {
            var str = value.ToString();
            var length = str.Length;
            var first = int.Parse(str[0].ToString());
            return (first + 1) * (int)Math.Pow(10, length - 1);
        }

        private enum AppVisitType
        {
            /// <summary>
            /// 访问人数
            /// </summary>
            Uv,

            /// <summary>
            /// 打开次数
            /// </summary>
            Uvt,

            /// <summary>
            /// 访问页面数
            /// </summary>
            Pv,

            /// <summary>
            /// 留存率
            /// </summary>
            Aliverate
        }

        private record Data<T>([property: JsonPropertyName("cube")] List<T> Items) where T : class;

        private record AppVisitItem(
            [property: JsonPropertyName("appinitvisit")]
            AppVisit AppVisit);

        private record AppVisit(double Data1, double Data2, double Data3);

        private record AreaVisitItem(
            [property: JsonPropertyName("areavisit")]
            AreaVisit AreaVisit);

        private record AreaVisit(int Pv, int Uv, int Uvt, string ADCode);

        private record AppVisitHourItem(
            [property: JsonPropertyName("inithourtotal")]
            AppVisitHour AppVisitHour);

        private record AppVisitHour(
            int TimeNum,
            string TimeKey,
            int Tuv,
            int Yuv,
            int Suv,
            int Tuvt,
            int Yuvt,
            int Suvt,
            int Tpv,
            int Ypv,
            int Spv);

        private record AppVisitPageItem(
            [property: JsonPropertyName("initpage")]
            AppVisitPage AppVisitPage);

        private record AppVisitPage(string Path, int Count, double Rate);

        private static IReadOnlyDictionary<string, string> ADCodeName = new Dictionary<string, string>()
        {
            { "110000", "北京" },
            { "120000", "天津" },
            { "130000", "河北" },
            { "140000", "山西" },
            { "150000", "内蒙古" },
            { "210000", "辽宁" },
            { "220000", "吉林" },
            { "230000", "黑龙江" },
            { "310000", "上海" },
            { "320000", "江苏" },
            { "330000", "浙江" },
            { "340000", "安徽" },
            { "350000", "福建" },
            { "360000", "江西" },
            { "370000", "山东" },
            { "410000", "河南" },
            { "420000", "湖北" },
            { "430000", "湖南" },
            { "440000", "广东" },
            { "450000", "广西" },
            { "460000", "海南" },
            { "500000", "重庆" },
            { "510000", "四川" },
            { "520000", "贵州" },
            { "530000", "云南" },
            { "540000", "西藏" },
            { "610000", "陕西" },
            { "620000", "甘肃" },
            { "630000", "青海" },
            { "640000", "宁夏" },
            { "650000", "新疆" },
            { "710000", "台湾" },
            { "810000", "香港" },
            { "820000", "澳门" }
        };

        public void Dispose()
        {
            _graphClient.Dispose();

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= TimerOnElapsed;
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}