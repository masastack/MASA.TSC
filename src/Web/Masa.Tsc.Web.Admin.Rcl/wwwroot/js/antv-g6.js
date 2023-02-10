G6.registerEdge(
    'circle-running',
    {
        afterDraw(cfg, group) {
            // ��õ�ǰ�ߵĵ�һ��ͼ�Σ������Ǳ߱���� path
            const shape = group.get('children')[0];
            // �� path �����λ��
            const startPoint = shape.getPoint(0);

            // ��Ӻ�ɫ circle ͼ��
            const circle = group.addShape('circle', {
                attrs: {
                    x: startPoint.x,
                    y: startPoint.y,
                    fill: 'blue',
                    r: 3,                 
                },
                // �� G6 3.3 ��֮��İ汾�У�����ָ�� name�������������ַ���������Ҫ��ͬһ���Զ���Ԫ�������б���Ψһ��
                name: 'circle-shape',
            });

            // �Ժ�ɫԲ����Ӷ���
            circle.animate(
                (ratio) => {
                    // ÿһ֡�Ĳ�������� ratio����һ֡�ı���ֵ��Number��������ֵ����һ֡��Ҫ�仯�Ĳ�������Object����
                    // ���ݱ���ֵ������ڱ� path �϶�Ӧ������λ�á�
                    const tmpPoint = shape.getPoint(ratio);
                    // ������Ҫ�仯�Ĳ����������ﷵ����λ�� x �� y
                    return {
                        x: tmpPoint.x,
                        y: tmpPoint.y,
                    };
                },
                {
                    repeat: true, // �����ظ�
                    duration: 2000,
                },
            ); // һ�ζ�����ʱ�䳤��
        },
    },
    'cubic',
); // ���Զ���߼̳��������ױ��������� cubic
// lineDash �Ĳ�ֵ�������ں����ṩ util �����Զ�����
const lineDash = [4, 2, 1, 2];
G6.registerEdge(
    'line-dash',
    {
        afterDraw(cfg, group) {
            // ��øñߵĵ�һ��ͼ�Σ������Ǳߵ� path
            const shape = group.get('children')[0];
            shape.attrs.lineAppendWidth = 40;
            debugger;
            let index = 0;
            // �� path ͼ�εĶ���
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
                    // ������Ҫ�޸ĵĲ������������޸��� lineDash,lineDashOffset
                    return res;
                },
                {
                    repeat: true, // �����ظ�
                    duration: 3000, // һ�ζ�����ʱ��Ϊ 3000
                },
            );           
        },
    },
    'quadratic',
); // ���Զ���߼̳����������ױ��������߱� cubic

export function init(elementRef, option, data, pluginOption) {
    if (pluginOption.useNodeTooltip) {
        option.modes.default.push({
            type: 'tooltip', // ��ʾ��
            formatText(model) {
                return eval(`(${pluginOption.nodeTooltipFormatText})(model)`);
            },
        });
    }
    if (pluginOption.useEdgeTooltip) {
        option.modes.default.push({
            type: 'edge-tooltip', // ����ʾ��
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
    graph.data(data); // ��������
    graph.render(); // ��Ⱦ
};

export function render(elementRef, data) {
    const graph = elementRef.graph;
    graph.data(data); // ��������
    graph.render(); // ��Ⱦ
};
