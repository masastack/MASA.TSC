function setPosition(isTop) {
    //debugger
    let scroll = document.getElementsByClassName("m-virtual-scroll__container")[0], parent = scroll.parentNode;
    let scrollheight = scroll.offsetHeight, clientHeight = parent.offsetHeight;
    let preDiv = scroll.childNodes[1], nextDiv = scroll.childNodes[parent.childElementCount - 1];
    try {
        if (isTop) {
            preDiv.style.height = 0 + "px";
            nextDiv.style.height = (scrollheight - clientHeight) + "px";
        }
        else {
            preDiv.style.height = (scrollheight - clientHeight) + "px";
            nextDiv.style.height = 0 + "px";
        }
    }
    catch {
    }
}

export { setPosition }