 window.addEventListener("popstate", function (e) {
    let url = window.location.pathname;
    const route = "/dashboard/configuration", target = "/dashboard";
    if (url.includes(route)) {
        history.pushState({}, '', target);
    }
});