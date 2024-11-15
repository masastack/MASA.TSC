let $this = {};
let _dotnetHelper = false;
function setChartEvent(module, donetHelper) {
    _dotnetHelper = donetHelper;
    let mychart = module.getOriginInstance();   
    mychart.on("brushend", function (e) {       
        let areas = e.areas;
        if (areas.length == 0) {            
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