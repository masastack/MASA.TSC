const colorList = ['rgba(54, 153, 255, 1)', 'rgba(56, 213, 48, 1)']

const tooltip = new G6.Tooltip({
    offsetX: 10,
    offsetY: 10,
    trigger: 'click',
    itemTypes: ['node', 'edge'],
    getContent: (e) => {
        const outDiv = document.createElement('div');
        outDiv.style.width = 'fit-content';
        outDiv.innerHTML = `
        <p>name:</p>
        <p>load:</p>
        <p>Success Rate:</p>
        <p>Latency:</p>
        <p><a href="#">跳转详情</a></p>`;
        return outDiv;
    },
});
const defaultNodeStyle = {
    fill: "white",
    stroke: "#0500FF",
    lineWidth: 4,
    lineDash: [5, 5]
};
const getNodeStroke = (state) => {
    switch (state) {
        case 1: {
            return "#4318FF";
        }
        case 2: {
            return "#FDB6BA";
        }
        case 3: {
            return "#F1AE00";
        }
    }
    return "";
};
const getNodeLineDash = (depth) => {
    switch (depth) {
        case 1: {
            return [10, 2];
        }
        case 2: {
            return [8, 4];
        }
        case 3: {
            return [6, 6];
        }
    }
    return "";
};

export function registerEdge() {
    G6.registerEdge(
        'edge-pipe',
        {
            afterDraw(cfg, group) {
                const shape = group.get('children')[0]
                const length = shape.getTotalLength()
                const pathList = shape.attrs.path
                const shapeW = 30
                const shapeGap = 15
                const shapeH = cfg.style.lineWidth - 2 || 6
                const cornerPoints = pathList.length >= 3 ? pathList.slice(1, pathList.length - 1) : null
                const shapeCount = Math.floor(length / (shapeW + shapeGap))

                function getCorner(start, end) {
                    if (!cornerPoints) {
                        return null
                    }
                    for (const i in cornerPoints) {
                        const item = cornerPoints[i]
                        const isXCenter = Math.round(item[1]) >= Math.round(Math.min(start.x, end.x)) && Math.round(item[1]) <= Math.round(Math.max(start.x, end.x))
                        const isYCenter = Math.round(item[2]) >= Math.round(Math.min(start.y, end.y)) && Math.round(item[2]) <= Math.round(Math.max(start.y, end.y))
                        if (isXCenter && isYCenter) {
                            return item
                        }
                    }
                    return null
                }

                function getPath(start, end) {
                    if (!start || !end) {
                        return
                    }
                    const pathData = [['M', start.x, start.y]]
                    const corner = getCorner(start, end)
                    if (corner) {
                        pathData.push(['L', corner[1], corner[2]])
                    }
                    pathData.push(['L', end.x, end.y])
                    return pathData
                }

                const _loop = function _loop(i) {
                    const startPoint = shape.getPoint((i / shapeCount))
                    const endPoint = shape.getPoint(i / shapeCount + shapeW / length)
                    const path = group.addShape('path', {
                        attrs: {
                            path: getPath(startPoint, endPoint),
                            stroke: cfg.style.stroke,
                            lineWidth: shapeH,
                            lineAppendWidth: 0,
                            cursor: 'pointer',
                            miterLimit: 0,
                            lineCap: 'round'
                        },
                        name: 'path-shape'
                    })

                    path.animate(
                        ratio => {
                            ratio += i / shapeCount
                            const boundVal = shapeCount > 1 ? (shapeCount - 1) / shapeCount : 1
                            if (ratio > boundVal) {
                                ratio %= boundVal
                            }
                            const tmpPoint = shape.getPoint(ratio)
                            const tmpPoint2 = ratio + shapeW / length < 1 ? shape.getPoint(ratio + shapeW / length) : shape.getPoint(1)
                            return { path: getPath(tmpPoint, tmpPoint2) }
                        },
                        {
                            repeat: true,
                            duration: 20 * length,
                            easing: 'easeLinear'
                        }
                    )

                }

                for (let i = 0; i < shapeCount; i++) {
                    _loop(i)
                }
            }
        },
        'line'
    )
}

const legendData = {
    nodes: [
        { id: "正常", label: "正常" },
        { id: "错误", label: "错误" },
        { id: "告警", label: "告警" }
    ], edges: [
        { id: "正常", label: "正常" },
        { id: "错误", label: "错误" },
        { id: "告警", label: "告警" }
    ]
};

const createLegend = () => {
    const legend = new G6.Legend({
        data: legendData,
        align: "center",
        layout: "horizontal", // vertical
        position: "top-left",

        filter: {
            enable: true,
            multiple: true,
            trigger: "click",
            graphActiveState: "activeByLegend",
            graphInactiveState: "inactiveByLegend",
            filterFunctions: {
                "正常": (d) => {
                    if (d.state === 1) return true;
                    return false
                },
                "错误": (d) => {
                    if (d.state === 2) return true;
                    return false
                },
                "告警": (d) => {
                    if (d.state === 3) return true;
                    return false
                },
            }
        }
    });

    return legend;
}

const createGraph = (domRef, data, legend) => {
    const graph = new G6.Graph({
        container: domRef,
        width: 1440,
        height: 765,
        linkCenter: true,
        plugins: [tooltip, legend],
        modes: {
            default: ['drag-node'],
        },
        defaultNode: {
            size: 80,
            labelCfg: {
                position: 'bottom'
            },
            style: defaultNodeStyle
        },
        defaultEdge: {
            type: 'edge-pipe',
            style: {
                lineJoin: 'round',
                stroke: colorList[0],
                shapeColor: colorList[1],
                lineWidth: 10,
                opacity: 0.5
            },
            modes: { default: ['drag-canvas', 'zoom-canvas'] }
        },
        layout: {
            type: 'force', // 设置布局算法为 force
            linkDistance: 200, // 设置边长为 100
            preventOverlap: true, // 设置防止重叠
        },
        nodeStateStyles: {
            activeByLegend: {
                lineWidth: 5,
                strokeOpacity: 0.5,
                stroke: '#f00'
            },
            inactiveByLegend: {
                opacity: 0.5
            }
        },
    });

    return graph;
}

export function init(domRef, data) {
    const legend = createLegend();
    const graph = createGraph(domRef, data, legend);
    domRef.graph = graph;

    const nodes = data.nodes;
    const edges = data.edges;
    nodes.forEach((node) => {
        node.style = { ...defaultNodeStyle };
        node.style.stroke = getNodeStroke(node.state);
        node.style.lineDash = getNodeLineDash(node.depth);
    });
    graph.data(data);
    graph.render();

    graph.on('node:mouseenter', (e) => {
        graph.setItemState(e.item, 'active', true);
    });
    graph.on('node:mouseleave', (e) => {
        graph.setItemState(e.item, 'active', false);
    });
    graph.on('edge:mouseenter', (e) => {
        graph.setItemState(e.item, 'active', true);
    });
    graph.on('edge:mouseleave', (e) => {
        graph.setItemState(e.item, 'active', false);
    });
}

export function destroy(domRef) {
    let graph = domRef.graph;
    graph.destroy();
}

export function update(domRef, data) {
    let graph = domRef.graph;
    const nodes = data.nodes;
    const edges = data.edges;
    nodes.forEach((node) => {
        node.style = { ...defaultNodeStyle };
        node.style.stroke = getNodeStroke(node.state);
        node.style.lineDash = getNodeLineDash(node.depth);
    });
    graph.data(data);
    graph.render();
}