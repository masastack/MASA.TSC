export function scrollBottom(scrollElementId, contentElementId) {
    var scrollElementRef = document.getElementById(scrollElementId);
    var contentElementRef = document.getElementById(contentElementId);
    window.setTimeout(() =>
    {
        scrollElementRef.scrollTop = contentElementRef.offsetHeight;
    },100);
}