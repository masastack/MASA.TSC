(() => {
    // 重试次数
    const maximumRetryCount = 10000;

    // 重试间隔
    const retryIntervalMilliseconds = 1000;

    const startReconnectionProcess = () => {

        let isCanceled = false;

        (async () => {
            for (let i = 0; i < maximumRetryCount; i++) {
                console.log(`试图重新连接: ${i + 1} of ${maximumRetryCount}`)
                await new Promise(resolve => setTimeout(resolve, retryIntervalMilliseconds));

                if (isCanceled) {
                    return;
                }

                try {
                    const result = await Blazor.reconnect();
                    if (!result) {
                        // 已到达服务器，但连接被拒绝;重新加载页面。
                        location.reload();
                        return;
                    }

                    // 成功重新连接到服务器。
                    return;
                } catch {
                    //没有到达服务器;再试一次。
                }
            }

            // 重试次数太多;重新加载页面。
            location.reload();
        })();

        return {
            cancel: () => {
                isCanceled = true;
            },
        };
    };

    let currentReconnectionProcess = null;

    Blazor.start({
        configureSignalR: function (builder) {
            let c = builder.build();
            c.serverTimeoutInMilliseconds = 30000;
            c.keepAliveIntervalInMilliseconds = 15000;
            builder.build = () => {
                return c;
            };
        },
        reconnectionHandler: {
            onConnectionDown: () => currentReconnectionProcess ??= startReconnectionProcess(),
            onConnectionUp: () => {
                currentReconnectionProcess?.cancel();
                currentReconnectionProcess = null;
            },
        },
    });
})();