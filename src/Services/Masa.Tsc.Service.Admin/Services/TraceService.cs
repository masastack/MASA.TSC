// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class TraceService : ServiceBase
{
    private IServiceCollection serviceDescriptors;

    public TraceService(IServiceCollection services) : base(services, "/api/trace")
    {
        App.MapGet($"{BaseUri}/{{traceId}}", GetAsync);
        App.MapGet($"{BaseUri}/list", GetListAsync);
        App.MapGet($"{BaseUri}/attr-values", GetAttrValuesAsync);
        App.MapGet($"{BaseUri}/aggregation", GetAggegationAsync);
        serviceDescriptors = services;
    }

    private async Task<IEnumerable<object>> GetAsync([FromServices] IEventBus eventBus, [FromRoute] string traceId)
    {
        var query = new TraceDetailQuery(traceId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationDto<object>> GetListAsync([FromServices] IEventBus eventBus, RequestTraceListDto model)
    {

        ////var text = "{\"accounts\":[{\"accountID\":\"00000000-0000-0000-0000-000000000000\",\"accountNum\":\"test00001\",\"blocked\":0,\"city\":\"温州市\",\"countryRegionId\":\"CHN\",\"county\":\"乐清市\",\"creditMax\":0,\"creditRating\":\"\",\"currency\":\"CNY\",\"custClassificationId\":\"test一级代理商\",\"custGroup\":\"test国内非关联方\",\"custName\":\"test测试公司\",\"custStatus\":2,\"dlvMode\":\"\",\"dlvTerm\":\"\",\"inclTax\":1,\"invoiceAccount\":\"\",\"paymMode\":\"test支付宝\",\"paymTermId\":\"test月结\",\"priceGroup\":\"test经销公司\",\"state\":\"浙江省\",\"street\":\"北白象镇大桥工业园区\",\"taxGroup\":\"13%\",\"telephone\":\"18658198061\",\"custNumber\":\"18658198061\",\"ecsource\":0,\"custNumberNew\":\"\",\"refRecId\":0,\"salesInvoiceType\":0}]}";
        //var text = "{\"a\":1,\"b\":\"qinyouzeng\"}";

        //try
        //{
        //    var caller = serviceDescriptors.BuildServiceProvider().GetRequiredService<ICallerFactory>();
        //    var t = await caller.CreateClient("lonsdi-ax").PostAsync("http://soa-dev.lonsid.cn/api/ax/customers/upsertcusttables", JsonSerializer.Deserialize<object>(text));
        //}
        //catch(Exception e) { 

        //}
        ////
        var query = new TraceListQuery(model.Service, model.Instance, model.Endpoint, model.TraceId, model.Start, model.End, model.Page, model.PageSize);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<IEnumerable<string>> GetAttrValuesAsync([FromServices] IEventBus eventBus, [FromBody] RequestAttrDataDto model)
    {
        var query = new TraceAttrValuesQuery(model.Query, model.Name, model.Keyword, model.Start, model.End, model.Max);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ChartLineDataDto<ChartPointDto>> GetAggegationAsync([FromServices] IEventBus eventBus, [FromBody] RequestAggregationDto param)
    {
        var query = new TraceAggregationQuery(true, true, param.FieldMaps, param.Queries, param.Start, param.End, param.Interval);
        await eventBus.PublishAsync(query);
        return query.Result!;
    }
}
