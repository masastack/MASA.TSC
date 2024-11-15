let $this = {};
let _dotnetHelper = false;
function setChartEvent(module, donetHelper) {
    _dotnetHelper = donetHelper;
    let mychart = module.getOriginInstance();
    console.log("mychart")
    console.log(mychart)
    mychart.on("brushselected", function (e) {
        var areas = e.batch[0].areas;
        if (areas.length == 0) {
            //清除选择
            callBack(true);
        } else {
            let startIndex = areas[0].coordRange[0], endIndex = areas[0].coordRange[1];
            callBack(false, startIndex, endIndex);
        }
    });
}

function callBack(clear, start = 0, end = 0) {
    clearTimter($this)
    $this.clear = clear;
    $this.start = start;
    $this.end = end;
    $this.timerId = setTimeout(() => {
        if (!_dotnetHelper)
            return;
        _dotnetHelper.invokeMethodAsync("OnBrushEnd", { isClear: $this.clear, start: $this.start, end: $this.end }).then(() => { clearTimter($this); })
    }, 500);
}

function clearTimter(obj) {
    try {
        if (obj && !obj.timerId)
            clearTimeout(obj.timerId);
    } catch { }
}

export { setChartEvent }