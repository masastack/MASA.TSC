function getElement(options) {
    return document.getElementById(options.id);
}

export function init(options, dotNetHelper) {
    var el = getElement(options);
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
    return grid;
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

export function reload(options, dotNetHelper) {
    destroy(options,false);
    var els = document.getElementsByClassName('grid-stack');
    els.forEach(el =>
    {
        try {
            el.gridstack?.destroy(false);
        }
        catch (error) {

        }
    });
    els.forEach(el => {
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
    });
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
