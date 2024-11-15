let $this = {};
let _dotnetHelper = false;
function setChartEvent(module, donetHelper) {
    _dotnetHelper = donetHelper;
    let mychart = module.getOriginInstance();
    mychart.on("brushend", addEvent);
    mychart.on("brushselected", function (e) {
        console.log(e)
        if (e.batch[0].areas.length == 0) {
            callBack(true);
        }
    });
}

function addEvent(e) {
    console.log(e)
    let areas = e.areas;
    let startIndex = areas[0].coordRange[0], endIndex = areas[0].coordRange[1];
    callBack(false, startIndex, endIndex);
}

function callBack(clear, start = 0, end = 0) {
    //clearTimter($this)
    $this.clear = clear;
    $this.start = start;
    $this.end = end;
    _dotnetHelper.invokeMethodAsync("OnBrushEnd", { isClear: $this.clear, start: $this.start, end: $this.end })
}

function clearTimter(obj) {
    try {
        if (obj && !obj.timerId)
            clearTimeout(obj.timerId);
    } catch { }
}

export { setChartEvent }