window.onbeforeunload = function (e) {
    if (window.location.href.includes('configuration')) return 'Leaving this page, unsaved data will be lost!';
};