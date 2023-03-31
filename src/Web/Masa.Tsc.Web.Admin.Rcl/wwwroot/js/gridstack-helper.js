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

    if (options.id === 'grid-stack') {
        var grid = el.gridstack;
        grid.cellHeight = cellHeight;
        var val = grid.cellWidth() * 0.98;
        grid.cellHeight(val, false);
        window.addEventListener('resize', () => {
            window.setTimeout(() => {
                var val = grid.cellWidth() * 0.98;
                grid.cellHeight(val, true);
            }, 100);
        });
    }

    return grid;
}

function cellHeight(val, update = true){
    this.opts.cellHeight = val;

    if (update) {
        this._updateStyles(true); // true = force re-create for current # of rows
    }
    return this;
}

export function initAll(options, dotNetHelper) {
    var grids = GridStack.initAll(options);
    grids.forEach(grid => {
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
    });
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
    dotNetHelper.invokeMethodAsync('OnChange', datas.map(item =>
    {
        return {
            id: item.id,
            x: item.x,
            y: item.y,
            width: item.w,
            height: item.h,
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
    grid.enableMove(state);
    grid.enableResize(state);
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
