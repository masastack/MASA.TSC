export function scrollBottom(scrollElementId) {
    var scrollElementRef = document.getElementById(scrollElementId);
    scrollElementRef.scrollTop = scrollElementRef.scrollTopMax;
    window.setTimeout(() =>
    {
        scrollElementRef.scrollTop = scrollElementRef.scrollTopMax;
    },100);
}