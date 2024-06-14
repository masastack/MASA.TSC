function setPosition(isTop) {
    let parent = document.getElementsByClassName("m-virtual-scroll__container")[0];
    let height = parent.offsetHeight;

    let preDiv = parent.childNodes[1], nextDiv = parent.childNodes[parent.childElementCount - 1];
    if (isTop) {
        preDiv.style.height = 0 + "px";
        nextDiv.style.height = height + "px";
    }
    else {
        preDiv.style.height = height + "px";
        nextDiv.style.height = 0 + "px";
    }
}

export { setPosition }