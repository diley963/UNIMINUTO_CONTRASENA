var progreso = 0;
var barra = document.getElementById("barra");
var idIterval = setInterval(function () {
    // Aumento en 10 el progeso
    progreso += 10;
    ///  $('#barra').css('width', progreso + '%');
    barra.setAttribute('style', 'width:' + progreso + '%');
    barra.innerHTML = progreso + "%";
    //Si llegó a 100 elimino el interval
    if (progreso == 100) {
        clearInterval(idIterval);
        window.location.href = "/Home"
    }
}, 1000);

function DisableBackButtonAllBrowsers() {
    window.history.forward()
};
DisableBackButtonAllBrowsers();
window.onload = DisableBackButtonAllBrowsers;
window.onpageshow = function (evts) { if (evts.persisted) DisableBackButtonAllBrowsers(); };
window.onunload = function () { void (0) };