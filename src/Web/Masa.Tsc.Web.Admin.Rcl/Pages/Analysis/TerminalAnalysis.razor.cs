// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Analysis
{
    public partial class TerminalAnalysis : IDisposable
    {
        [Inject] private IPopupService PopupService { get; set; } = null!;

        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = null!;

        private object _brandOption = new { };
        private object _platformOption = new { };
        private object _modelOption = new { };
        private object _appVersionOption = new { };
        private object _deviceOption = new { };

        private const int Take = 50;

        private GraphQLHttpClient _graphClient = null!;

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
                OnFilter();
            }
        }

        private async Task RefreshBrandECharts()
        {
            var dataItems = await QueryAsync("brand");

            var data = dataItems.OrderByDescending(u => u.DeviceVisit.Qty).ToList();

            if (_brands.Count == 0)
            {
                _brands = data.Select(u => u.DeviceVisit.Brand).ToList();
            }

            var items = data
                .Select(u => new
                {
                    name = u.DeviceVisit.Brand,
                    value = u.DeviceVisit.Qty,
                    itemStyle = new
                    {
                        color = TerminalAnalysisData.KnowBrandColor.GetValueOrDefault(u.DeviceVisit.Brand)
                    }
                })
                .ToList();

            _brandOption = GetPieOption("品牌", items);

            await InvokeAsync(StateHasChanged);
        }

        private async Task<List<DeviceVisitItem>> QueryAsync(string type)
        {
            try
            {
                var query = GetQuery(type);
                var result = await _graphClient.SendQueryAsync<CubeData<DeviceVisitItem>>(query);
                return result.Data.Items;
            }
            catch (Exception e)
            {
                await PopupService.EnqueueSnackbarAsync("查询失败", e.Message, AlertTypes.Error);
                return [];
            }
        }

        private async Task RefreshPlatformECharts()
        {
            var dataItems = await QueryAsync("platform");

            var data = dataItems.OrderByDescending(u => u.DeviceVisit.Qty).ToList();

            if (_platforms.Count == 0)
            {
                _platforms = data.Select(u => u.DeviceVisit.Platform).ToList();
            }

            var items = data
                .Select(u => new
                {
                    name = u.DeviceVisit.Platform,
                    value = u.DeviceVisit.Qty,
                    itemStyle = new
                    {
                        color = TerminalAnalysisData.KnowPlatformColor.GetValueOrDefault(u.DeviceVisit.Platform)
                    }
                })
                .ToList();

            _platformOption = GetPieOption("平台", items);

            await InvokeAsync(StateHasChanged);
        }

        private async Task RefreshModelECharts()
        {
            var dataItems = await QueryAsync("modle");

            if (_models.Count == 0)
            {
                _models = dataItems.OrderByDescending(u => u.DeviceVisit.Qty)
                    .Select(u => u.DeviceVisit.Model).ToList();
            }

            var dict = dataItems.ToDictionary(u => u.DeviceVisit.Model, u => u.DeviceVisit.Qty);

            var data = dict
                .OrderByDescending(u => u.Value)
                .ToList();

            var newData = data.Take(Take).OrderBy(u => u.Value).ToList();
            var sumOfOther = data.Skip(Take).Sum(u => u.Value);
            if (sumOfOther > 0)
            {
                newData.Insert(0,
                    new KeyValuePair<string, int>($"剩余（{data.Count - Take}）", sumOfOther));
            }

            var max = Math.Max(TryGetMaxQty(data), sumOfOther / 3);

            _modelOption = GetBarOption("机型", newData.Select(u => u.Key), newData.Select(u => u.Value), max);

            await InvokeAsync(StateHasChanged);
        }

        private async Task RefreshDeviceECharts()
        {
            var dataItems = await QueryAsync("devicever");

            var dict = dataItems.ToDictionary(u => u.DeviceVisit.Device, u => u.DeviceVisit.Qty);

            var data = dict
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

            _deviceOption = GetBarOption("系统版本", newData.Select(u => u.Key).ToArray(),
                newData.Select(u => u.Value).ToArray(), max);

            await InvokeAsync(StateHasChanged);
        }

        private async Task RefreshAppVersionECharts()
        {
            var dataItems = await QueryAsync("appversion");

            var dict = dataItems.ToDictionary(u => u.DeviceVisit.AppVersion, u => u.DeviceVisit.Qty);

            if (_appVersions.Count == 0)
            {
                _appVersions = dict.OrderByDescending(u => u.Value).Select(u => u.Key).ToList();
            }

            var data = dict
                .OrderBy(u => u.Value)
                .ToList();

            var max = TryGetMaxQty(data);

            _appVersionOption = GetBarOption(
                "APP 版本",
                data.Select(u => u.Key).ToArray(),
                data.Select(u => u.Value).ToArray(),
                max);

            await InvokeAsync(StateHasChanged);
        }

        private static int TryGetMaxQty(List<KeyValuePair<string, int>> data)
        {
            return data.Count != 0 ? (int)(data.Max(u => u.Value) * 1.1) : 0;
        }

        private static object GetPieOption(string name, IEnumerable<object> data)
        {
            return new
            {
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

        private GraphQLHttpRequest GetQuery(string type)
        {
            List<string> filters = [];

            if (_selectedPlatforms.Count > 0)
            {
                filters.Add("{platform: {in: [" + string.Join(", ", _selectedPlatforms.Select(u => $"\"{u}\"")) +
                            "]}}");
            }

            if (_selectedBrands.Count > 0)
            {
                filters.Add("{brand: {in: [" + string.Join(", ", _selectedBrands.Select(u => $"\"{u}\"")) + "]}}");
            }

            if (_selectedModels.Count > 0)
            {
                filters.Add("{modle: {in: [" + string.Join(", ", _selectedModels.Select(u => $"\"{u}\"")) + "]}}");
            }

            if (_selectedDevices.Count > 0)
            {
                filters.Add("{devicever: {in: [" + string.Join(", ", _selectedDevices.Select(u => $"\"{u}\"")) + "]}}");
            }

            if (_selectedAppVersions.Count > 0)
            {
                filters.Add("{appversion: {in: [" + string.Join(", ", _selectedAppVersions.Select(u => $"\"{u}\"")) +
                            "]}}");
            }

            var filter = string.Join(", ", filters);

            return new GraphQLHttpRequest(
                $$"""
                    query {
                      cube {
                        devicevisit(where: {AND: [{{filter}}]}) {
                          qty
                          {{type}}
                        }
                      }
                    }
                  """);
        }

        private record DeviceVisitItem(
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

        public void Dispose()
        {
            _graphClient.Dispose();
        }
    }
}