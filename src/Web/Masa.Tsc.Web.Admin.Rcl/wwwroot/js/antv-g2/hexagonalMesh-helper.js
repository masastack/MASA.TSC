﻿const handleData = (data) => {
    let hexes = data.reduce((target, item, index) => {
        target[item.key] = item;
        return target;
    }, {})
    return {
        "layout": "odd-r",
        "hexes": hexes
    }
}

const getStroke = (state) => {
    switch (state) {
        case 1: {
            return "#299F00";
        }
        case 2: {
            return "#F80E1C";
        }
        case 3: {
            return "#F1AE00";
        }
    }
    return "";
};

export function addPolygon(domRef, d) {
    let newData = handleData(d);
    let chart = domRef.chart;
    let dv = new DataSet.View().source(newData, {
        type: 'hex',
        width: 80,
        height: 80,
    });

    let bgView = chart.view();
    bgView.source(dv);
    bgView.polygon().position('x*y').color('#FFFFFF').opacity(0.5).style('state', {
        stroke: (state) => {
            return getStroke(state)
        },
        lineWidth: 3,
    }).label('key', {
        htmlTemplate: function formatter(text, item, index) {
            let d = item._origin;
            let html = `<p class="h5 default--text text-center">${d.name}</p>`;
            let items = d.items.slice(0,3);
            items.forEach(e => {
                html += `<p class="pa-1 body fill--text regular3 rounded-sm" style='width: max-content;background:${getStroke(e.status)}'>${e.name}</p>`
            });
            return html;
        }
    })
        .tooltip('name');
}


export function init(domRef, data) {    
    let chart = new G2.Chart({
        container: domRef,
        forceFit: true,
        height: window.innerHeight,
        padding: window.innerHeight / 24
    });
    domRef.chart = chart;
    chart.scale({
        x: {
            nice: false,
            sync: true
        },
        y: {
            nice: false,
            sync: true
        }
    });
    chart.coord().reflect(); // 视数据而定要不要翻转 Y 轴。

    chart.axis(false);

    chart.tooltip({
        showTitle: false,
        itemTpl: `<div>{value}<br/>{key}<br/>应用：{findByName(value)}<br/>members to your team to start collaborating. Click here to edit this<div>`
    });

    function findByName(name) {
        for (let item in data) {
            if (item.name === name)
                return item;
        }
        return null;
    }
}

export function render(domRef) {
    let chart = domRef.chart;
    chart.render();
}