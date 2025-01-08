// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Analysis
{
    public partial class TerminalAnalysis
    {
        [Inject] private IConfiguration Configuration { get; set; } = null!;

        [Inject] private IPopupService PopupService { get; set; } = null!;

        private object _brandOption = new { };
        private object _platformOption = new { };
        private object _modelOption = new { };
        private object _appVersionOption = new { };
        private object _deviceOption = new { };

        private const int Take = 50;

        private List<DataItem> _allData = [];

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
                var token = Configuration.GetValue<string>("CUBE_JWT_TOKEN");
                if (string.IsNullOrWhiteSpace(token))
                {
                    await PopupService.EnqueueSnackbarAsync("token未找到, 请检查配置", AlertTypes.Error);
                    return;
                }

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                httpClient.DefaultRequestHeaders.Add("X-Request-Type", "GraphQL");

                try
                {
                    PopupService.ShowProgressLinear();
                    var graphClient = new GraphQLHttpClient("http://10.130.0.33:4000/cubejs-api/graphql",
                        new SystemTextJsonSerializer(),
                        httpClient: httpClient);

                    var response = await graphClient.SendQueryAsync<Data>(GetQuery());
                    _allData = response.Data.Items;

                    OnFilter();
                    StateHasChanged();
                }
                catch (Exception e)
                {
                    await PopupService.EnqueueSnackbarAsync(title: "获取数据失败", content: e.Message, AlertTypes.Error);
                }
                finally
                {
                    PopupService.HideProgressLinear();
                }
            }
        }

        private IEnumerable<DataItem> ComputedData
        {
            get
            {
                return _allData
                    .Where(u => _selectedPlatforms.Count == 0 || _selectedPlatforms.Contains(u.DeviceVisit.Platform))
                    .Where(u => _selectedBrands.Count == 0 || _selectedBrands.Contains(u.DeviceVisit.Brand))
                    .Where(u => _selectedModels.Count == 0 || _selectedModels.Contains(u.DeviceVisit.Model))
                    .Where(u => _selectedDevices.Count == 0 || _selectedDevices.Contains(u.DeviceVisit.Device))
                    .Where(u => _selectedAppVersions.Count == 0 ||
                                _selectedAppVersions.Contains(u.DeviceVisit.AppVersion));
            }
        }

        private void RefreshBrandECharts()
        {
            var brandData = ComputedData
                .GroupBy(u => u.DeviceVisit.Brand)
                .ToDictionary(g => g.Key, v => v.Sum(u => u.DeviceVisit.Qty));

            if (_brands.Count == 0)
            {
                _brands = brandData.Keys.Order().ToList();
            }

            var data = brandData
                .OrderByDescending(u => u.Value)
                .Select(item => new
                {
                    name = item.Key,
                    value = item.Value,
                    itemStyle = new
                    {
                        color = TerminalAnalysisData.KnowBrandColor.GetValueOrDefault(item.Key)
                    }
                });

            _brandOption = GetPieOption("品牌", data);
        }

        private void RefreshPlatformECharts()
        {
            var platformData = ComputedData
                .GroupBy(u => u.DeviceVisit.Platform)
                .ToDictionary(g => g.Key, v => v.Sum(u => u.DeviceVisit.Qty));

            if (_platforms.Count == 0)
            {
                _platforms = platformData.Keys.Order().ToList();
            }

            var data = platformData
                .OrderByDescending(u => u.Value)
                .Select(item => new
                {
                    name = item.Key,
                    value = item.Value,
                    itemStyle = new
                    {
                        color = TerminalAnalysisData.KnowPlatformColor.GetValueOrDefault(item.Key)
                    }
                });
            _platformOption = GetPieOption("平台", data);
        }

        private void RefreshModelECharts()
        {
            var modelData = ComputedData
                .GroupBy(u => u.DeviceVisit.Model)
                .ToDictionary(g => g.Key, v => v.Sum(u => u.DeviceVisit.Qty));

            var data = modelData
                .OrderByDescending(u => u.Value)
                .ToList();

            if (_models.Count == 0)
            {
                _models = data.Select(u => u.Key).ToList();
            }

            var newData = data.Take(Take).OrderBy(u => u.Value).ToList();
            var sumOfOther = data.Skip(Take).Sum(u => u.Value);
            if (sumOfOther > 0)
            {
                newData.Insert(0, new KeyValuePair<string, int>($"剩余（{data.Count - Take}）", sumOfOther));
            }

            var max = Math.Max(TryGetMaxQty(data), sumOfOther / 3);

            _modelOption = GetBarOption(
                "机型",
                newData.Select(u => u.Key).ToArray(),
                newData.Select(u => u.Value).ToArray(),
                max);
        }

        private void RefreshDeviceECharts()
        {
            var deviceData = ComputedData
                .GroupBy(u => u.DeviceVisit.Device)
                .ToDictionary(g => g.Key, v => v.Sum(u => u.DeviceVisit.Qty));

            var data = deviceData
                .OrderByDescending(u => u.Value)
                .ToList();

            if (_devices.Count == 0)
            {
                _devices = data.Select(u => u.Key).ToList();
            }

            var newData = data.Take(Take).OrderBy(u => u.Value).ToList();
            var sumOfOther = data.Skip(Take).Sum(u => u.Value);
            if (sumOfOther > 0)
            {
                newData.Insert(0, new KeyValuePair<string, int>($"剩余（{data.Count - Take}）", sumOfOther));
            }

            var max = TryGetMaxQty(data);

            _deviceOption = GetBarOption(
                "系统版本",
                newData.Select(u => u.Key).ToArray(),
                newData.Select(u => u.Value).ToArray(),
                max);
        }

        private static int TryGetMaxQty(List<KeyValuePair<string, int>> data)
        {
            return data.Count != 0 ? (int)(data.Max(u => u.Value) * 1.1) : 0;
        }

        private void RefreshAppVersionECharts()
        {
            var appVersionData = ComputedData
                .GroupBy(u => u.DeviceVisit.AppVersion)
                .ToDictionary(g => g.Key, v => v.Sum(u => u.DeviceVisit.Qty));

            if (_appVersions.Count == 0)
            {
                _appVersions = appVersionData.OrderByDescending(u => u.Value).Select(u => u.Key).ToList();
            }

            var data = appVersionData
                .OrderBy(u => u.Value)
                .ToList();

            var max = TryGetMaxQty(data);

            _appVersionOption = GetBarOption(
                "APP 版本",
                data.Select(u => u.Key).ToArray(),
                data.Select(u => u.Value).ToArray(),
                max);
        }

        private static object GetPieOption(string name, IEnumerable<object> data)
        {
            return new
            {
                title = new
                {
                    text = name,
                },
                tooltip = new
                {
                    trigger = "item",
                    formatter = $"{name}：{{b}}<br/>数量：{{c}}<br/>占比：{{d}}%"
                },
                legend = new
                {
                    top = "bottom",
                    left = "center",
                    type = "scroll"
                },
                series = new[]
                {
                    new
                    {
                        name,
                        type = "pie",
                        radius = new[] { "40%", "70%" },
                        avoidLabelOverlap = false,
                        itemStyle = new
                        {
                            borderRadius = 4,
                            borderColor = "#fff",
                            borderWidth = 4
                        },
                        label = new
                        {
                            show = true,
                            position = "outside",
                            formatter = "{b}\n{p|{c} ({d}%)}",
                            lineHeight = 15,
                            rich = new
                            {
                                p = new
                                {
                                    color = "#999"
                                }
                            }
                        },
                        data
                    }
                }
            };
        }

        private static object GetBarOption(string name, IEnumerable<string> keys, IEnumerable<int> values, int max)
        {
            return new
            {
                title = new
                {
                    text = name
                },
                tooltip = new
                {
                    trigger = "axis",
                    formatter = $"{name}：{{b}}<br/>数量：{{c}}",
                    axisPointer = new
                    {
                        type = "shadow"
                    }
                },
                grid = new
                {
                    right = "4%",
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
                        data = values,
                        label = new
                        {
                            show = true,
                            position = "outside"
                        }
                    }
                }
            };
        }

        private static GraphQLHttpRequest GetQuery()
        {
            return new GraphQLHttpRequest(
                $$"""
                    query {
                      cube {
                        devicevisit {
                          qty
                          brand
                          platform
                          modle
                          devicever
                          appversion  
                        }
                      }
                    }
                  """);
        }

        private record Data([property: JsonPropertyName("cube")] List<DataItem> Items);

        private record DataItem(
            [property: JsonPropertyName("devicevisit")]
            DeviceVisit DeviceVisit);

        private record DeviceVisit(
            string Brand,
            string Platform,
            [property: JsonPropertyName("modle")]
            string Model,
            [property: JsonPropertyName("Devicever")]
            string Device,
            string AppVersion,
            int Qty);
    }
}