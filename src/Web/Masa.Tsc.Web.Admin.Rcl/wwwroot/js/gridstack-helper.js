export function init(options, dotNetHelper) {
    var topEl = document.getElementById('grid-stack');
    var el = getElement(options);
    if (topEl.loadComplete) {
        if (!el.gridstack) {
            initByElement(options, el, dotNetHelper);
            el.gridstack.cellHeight(el.gridstack.cellWidth() * 1);
            return el.gridstack;
        }
    }
    var timer = setInterval(function () {
        if (topEl.loadComplete) {
            clearInterval(timer);
            el.gridstack.cellHeight(el.gridstack.cellWidth() * 1.2);
            el.gridstack.on('change', function (e, items) {
                dotNetHelper.invokeMethodAsync('OnChange', items.map(item => {
                    return {
                        id: item.el.id,
                        x: item.x,
                        y: item.y,
                        width: item.w,
                        height: item.h,
                    }
                }))
            });
        }
    }, 100);
    return el.gridstack;
}

export function initAll(options, dotNetHelper) {
    var grids = GridStack.initAll(options);
    grids[0].on('change', function (e, items) {
        dotNetHelper.invokeMethodAsync('OnChange', items.map(item => {
            return {
                id: item.el.id,
                x: item.x,
                y: item.y,
                width: item.w,
                height: item.h,
            }
        }))
    });
    getElement(options).loadComplete = true;
}

export function reload(options) {
    var el = getElement(options);
    el.gridstack.removeAll(false);
    var childs = el.querySelectorAll(':scope > .grid-stack-item');
    el.gridstack.batchUpdate(true);
    childs.forEach(child => {
        el.gridstack.makeWidget(child);
    });
    el.gridstack.batchUpdate(false);
}

export function removeAll(options, destroyDom) {
    var grid = getElement(options).gridstack;
    grid.removeAll(destroyDom);
}

export function makeWidgets(options, elementIds) {
    var grid = getElement(options).gridstack;
    elementIds.forEach(id => {
        grid.makeWidget('#' + id);
    });
}

export function save(options, dotNetHelper) {
    var grid = getElement(options).gridstack;
    var datas = grid.save();
    dotNetHelper.invokeMethodAsync('OnChange', datas.map(item => {
        return {
            id: item.id,
            x: item.x,
            y: item.y,
            width: item.w || item.minW,
            height: item.h || item.minH,
        }
    }));
}

export function compact(options) {
    let grid = document.getElementById(options.id).gridstack;
    grid.compact();
}

export function destroy(options, destroyDom) {
    var grid = getElement(options).gridstack;
    grid.destroy(destroyDom);
}

export function switchState(options, state) {
    var grid = getElement(options).gridstack;
    if (grid) {
        grid.enableMove(state);
        grid.enableResize(state);
    }
    else {
        debugger
    }
}

export function movable(options, elId, state) {
    var grid = getElement(options).gridstack;
    var el = document.getElementById(elId);
    if (grid && el) {
        grid.movable(el, state);
    }
}

export function resizable(options, elId, state) {
    var grid = getElement(options).gridstack;
    var el = document.getElementById(elId);
    if (grid && el) {
        grid.resizable(el, state);
    }
}

function initByElement(options, el, dotNetHelper) {
    var grid = GridStack.init(options, el);
    grid.on('change', function (e, items) {
        dotNetHelper.invokeMethodAsync('OnChange', items.map(item => {
            return {
                id: item.el.id,
                x: item.x,
                y: item.y,
                width: item.w,
                height: item.h,
            }
        }))
    });
    //grid.on('added', function (e, items) {
    //    dotNetHelper.invokeMethodAsync('OnAdd', items.map(item => {
    //        return {
    //            id: item.el.id,
    //        }
    //    }))
    //});
    //grid.on('removed', function (e, items) {
    //    window.setTimeout(() => {
    //        dotNetHelper.invokeMethodAsync('OnRemove',[]);
    //    }, 500);
    //});
}

function getElement(options) {
    return document.getElementById(options.id);
}
