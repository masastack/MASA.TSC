function setPosition(index, total) {
    let scroll = document.getElementsByClassName("m-virtual-scroll__container")[0], parent = scroll.parentNode, children = scroll.childNodes;
    let scrollheight = scroll.scrollHeight, clientHeight = parent.offsetHeight;
    if (clientHeight - scrollheight >= 0)
        return;
    let height = children[2].offsetHeight + 4;
    let topHeight = (index - 1) * height;

    let isTop = topHeight == 0;
    let isBottom = index - total == 0;
    let preDiv = children[1], nextDiv = children[children.length - 1];

    if (isTop) {
        preDiv.style.height = '0px'
        parent.scrollTo({ left: 0, top: 0, behavior: 'smooth' })
    }
    else if (isBottom) {
        parent.scrollTo({ left: 0, top: scrollheight, behavior: 'smooth' })
        nextDiv.style.height = '0px';
    }
    else {
        let top = preDiv.offsetHeight, bottom = nextDiv.offsetHeight;
        if (scrollheight - topHeight - clientHeight < 0)
            topHeight = scrollheight - clientHeight;
        if (topHeight - top > 0) {
            parent.scrollTo({ left: 0, top: topHeight, behavior: 'smooth' });
            nextDiv.style.height = (top + bottom - topHeight) + 'px';
        } else {
            preDiv.style.height = (top + bottom - topHeight) + 'px';
            parent.scrollTo({ left: 0, top: topHeight, behavior: 'smooth' })
        }
    }
}

export { setPosition }