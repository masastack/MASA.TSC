export function init(options, dotNetHelper) {
    var el = getElement(options);
    if (el.gridstack?.engine) {
        el.gridstack.destroy(false);
        initByElement(options, el, dotNetHelper);
        var els = el.querySelectorAll('.grid-stack');
        els.forEach(el => {
            try {
                el.gridstack?.destroy(false);
            }
            catch (error) {

            }
        });
        els.forEach(el => {
            initByElement(options, el, dotNetHelper);
        });
    }
    else initByElement(options, el, dotNetHelper);

    return el.gridstack;
}

export function initAll(options, dotNetHelper) {
    var grids = GridStack.initAll(options);
    grids.forEach(grid => {
        grid.on('change', function (e, items) {
            dotNetHelper.invokeMethodAsync('OnChange', {
                id: items[0].el.id,
                x: items[0].x,
                y: items[0].y,
                width: items[0].w,
                height: items[0].h,
            });
        });
    });
}

export function reload(options) {
    var el = getElement(options);
    el.gridstack.removeAll(false);
    var childs = el.querySelectorAll(':scope > .grid-stack-item');
    childs.forEach(child =>
    {
        el.gridstack.makeWidget(child);
    });
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
    grid.enableMove(state);
    grid.enableResize(state);
}

function initByElement(options, el, dotNetHelper) {
    var grid = GridStack.init(options, el);
    grid.on('change', function (e, items) {
        dotNetHelper.invokeMethodAsync('OnChange', {
            id: items[0].el.id,
            x: items[0].x,
            y: items[0].y,
            width: items[0].w,
            height: items[0].h,
        });
    });
}

function getElement(options) {
    return document.getElementById(options.id);
}
