export function historyToList(dotNetHelper) {
    const _dotNetHelper = dotNetHelper;
    window.addEventListener("popstate", function () {
        _dotNetHelper.invokeMethodAsync("GoDashbordList");
    })
}