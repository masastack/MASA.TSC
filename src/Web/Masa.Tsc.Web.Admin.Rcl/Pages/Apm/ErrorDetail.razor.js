let id = false;

function autoHeight() {
    debugger
    if (!id) return;
    let table = document.getElementById(id);
    if (!table) return;
    table = table.getElementsByTagName("table")[0];
    if (!table) return;
    let parent = table?.parentNode;
    if (!table || !parent) return;
    parent.style.height = (table.offsetHeight + 10) + "px"
}

function setTableId(i) {
    id = i;
    setTimeout(autoHeight,200);
}

window.onload += autoHeight;

export { setTableId }