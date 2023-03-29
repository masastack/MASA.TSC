export function scrollBottom(scrollElementId) {
    var scrollElementRef = document.getElementById(scrollElementId);
    window.setTimeout(() =>
    {
        scrollElementRef.scrollTop = scrollElementRef.scrollHeight;
    },100);
}