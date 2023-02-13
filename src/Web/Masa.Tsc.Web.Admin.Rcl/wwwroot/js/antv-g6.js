G6.registerEdge(
    'circle-running',
    {
        afterDraw(cfg, group) {
            // 获得当前边的第一个图形，这里是边本身的 path
            const shape = group.get('children')[0];
            // 边 path 的起点位置
            const startPoint = shape.getPoint(0);

            // 添加红色 circle 图形
            const circle = group.addShape('circle', {
                attrs: {
                    x: startPoint.x,
                    y: startPoint.y,
                    fill: 'blue',
                    r: 3,                 
                },
                // 在 G6 3.3 及之后的版本中，必须指定 name，可以是任意字符串，但需要在同一个自定义元素类型中保持唯一性
                name: 'circle-shape',
            });

            // 对红色圆点添加动画
            circle.animate(
                (ratio) => {
                    // 每一帧的操作，入参 ratio：这一帧的比例值（Number）。返回值：这一帧需要变化的参数集（Object）。
                    // 根据比例值，获得在边 path 上对应比例的位置。
                    const tmpPoint = shape.getPoint(ratio);
                    // 返回需要变化的参数集，这里返回了位置 x 和 y
                    return {
                        x: tmpPoint.x,
                        y: tmpPoint.y,
                    };
                },
                {
                    repeat: true, // 动画重复
                    duration: 2000,
                },
            ); // 一次动画的时间长度
        },
    },
    'cubic',
); // 该自定义边继承内置三阶贝塞尔曲线 cubic
// lineDash 的差值，可以在后面提供 util 方法自动计算
const lineDash = [4, 2, 1, 2];
G6.registerEdge(
    'line-dash',
    {
        afterDraw(cfg, group) {
            // 获得该边的第一个图形，这里是边的 path
            const shape = group.get('children')[0];
            shape.attrs.lineAppendWidth = 40;
            debugger;
            let index = 0;
            // 边 path 图形的动画
            shape.animate(
                () => {
                    index++;
                    if (index > 9) {
                        index = 0;
                    }
                    const res = {
                        lineDash,
                        lineDashOffset: -index
                    };
                    // 返回需要修改的参数集，这里修改了 lineDash,lineDashOffset
                    return res;
                },
                {
                    repeat: true, // 动画重复
                    duration: 3000, // 一次动画的时长为 3000
                },
            );           
        },
    },
    'quadratic',
); // 该自定义边继承了内置三阶贝塞尔曲线边 cubic

export function init(elementRef, option, data, pluginOption) {
    if (pluginOption.useNodeTooltip) {
        option.modes.default.push({
            type: 'tooltip', // 提示框
            formatText(model) {
                return eval(`(${pluginOption.nodeTooltipFormatText})(model)`);
            },
        });
    }
    if (pluginOption.useEdgeTooltip) {
        option.modes.default.push({
            type: 'edge-tooltip', // 边提示框
            formatText(model) {
                return eval(`(${pluginOption.edgeTooltipFormatText})(model)`);
            }
        });
    }
    option.container = elementRef;
    const graph = new G6.Graph(option);
    elementRef.graph = graph;
    graph.on('edge:mouseenter', (ev) => {
        const edge = ev.item;
        graph.setItemState(edge, 'active', true);
    });

    graph.on('edge:mouseleave', (ev) => {
        const edge = ev.item;
        graph.setItemState(edge, 'active', false);
    });
    graph.data(data); // 加载数据
    graph.render(); // 渲染
};

export function render(elementRef, data) {
    const graph = elementRef.graph;
    graph.data(data); // 加载数据
    graph.render(); // 渲染
};
