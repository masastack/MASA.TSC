window.isEdit = 'False';
window.onbeforeunload = function (e) {
    if (window.isEdit != 'False' && (window.location.href.includes('configuration') || window.location.href.includes('teamDetail'))) return 'Leaving this page, unsaved data will be lost!';
};