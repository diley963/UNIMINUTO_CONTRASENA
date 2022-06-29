document.addEventListener("DOMContentLoaded", () => {
    //we capture the inputs
    var rbtEmail = document.getElementById("rbtEmail");
    var rbtMobile = document.getElementById("rbtMobile");
    //var rbtTengoCodigo = document.getElementById("rbtTengoCodigo");
    var btnSendCode = document.getElementById("btnSendCode");
    //var descripcion = document.getElementById("descripcion");
    var formulario = document.getElementById("idForm");
    var btnTengoCode = document.getElementById("btnTengoCode");
    $("#spinerBtnSend").hide();
    var viewbagMobileReplace = document.getElementById("viewbagMobileReplace");
    var viewbagEmailReplace = document.getElementById("viewbagEmailReplace");

    //Validamos que los radio butons tengan ek texto Indefinido si es asi los deshabilitamos
    if (viewbagEmailReplace.innerHTML === "Indefinido" && viewbagMobileReplace.innerHTML === "Indefinido") {
        rbtEmail.disabled = true;
        viewbagEmailReplace.style.color = "red";
        rbtMobile.disabled = true;
        viewbagMobileReplace.style.color = "red";
        btnSendCode.disabled = true;
        btnSendCode.style.color = "red";
    } else if (viewbagEmailReplace.innerHTML === "Indefinido") {
        rbtEmail.disabled = true;
        viewbagEmailReplace.style.color = "red";
    } else if (viewbagMobileReplace.innerHTML === "Indefinido") {
        rbtMobile.disabled = true;
        viewbagMobileReplace.style.color = "red";
    }  

    if (descripcion == "ADMINISTRATIVO" || descripcion == "DOCENTE" || descripcion == "BANACADEMICO") {
        $("#showTexto").css("display", "block");
    }

    if (descripcion == "ESTUDIANTE" || descripcion == "EGRESADO" || descripcion == "GRADUADO") {
        $("#showEnlace").css("display", "block");
    }


    document.getElementById("showEnlace").onclick = function () {
        abreventana();
        window.location.href = "/Home";
    }

    //Funcion de Redigirse al  index si se  le da click al boton back del navegador
    function DisableBackButtonAllBrowsers() {
        window.history.forward()
    };
    DisableBackButtonAllBrowsers();
    window.onload = DisableBackButtonAllBrowsers;
    window.onpageshow = function (evts) { if (evts.persisted) DisableBackButtonAllBrowsers(); };
    window.onunload = function () { void (0) };
    //

    function abreventana() {
        window.open("https://webchat.uniminuto.edu/index.html", "venta", "")
    }

    //The functionality of the send code button is programmed.
    btnSendCode.onclick = function (event) {
        document.getElementById("btnOptions").style.display = "none";

        var SendMethod;

        if (rbtEmail.checked == true && rbtMobile.checked == false /*&& rbtTengoCodigo.checked == false*/) {
            SendMethod = rbtEmail.value;
        } else if (rbtEmail.checked == false && rbtMobile.checked == true /*&& rbtTengoCodigo.checked == false*/) {
            SendMethod = rbtMobile.value;
        } 

        if (rbtEmail.checked == false && rbtMobile.checked == false ) {
            ShowMessageFail("Debe seleccionar un método de envió");
            event.preventDefault();
        } else {
            //oculto spiner
            $("#spinerBtnSend").show();

            //bloqueo elementos del formulario
            btnSendCode.disabled = true;
            
            $.ajax({
                type: 'POST',
                url: 'SendCode',
                dataType: 'json',
                data: { metodo: SendMethod },
                success: function (data) {
                    if (data.isSend == true) {

                        if (data.messageReply == "tengoCodigo") {
                            formulario.submit();
                        } else {
                            ShowMessageOk(data.messageReply + " Sera redirigido en 5 segundos");
                            setTimeout(function () {  formulario.submit(); }, 5000);
                        }

                    } else {
                        ShowMessageFail(data.messageReply);
                        $("#spinerBtnSend").hide();
                        btnSendCode.disabled = false;
                    }
                },
                error: function (e) {
                    alert(e.statusText);
                    //Swal.fire("Señor(a) Usuario(a)", "Fallo al traer los registros", "error");
                },

            });

        }

    }


    btnTengoCode.onclick = function (event) {

        var SendMethod;

        SendMethod = btnTengoCode.value;      

        $.ajax({
            type: 'POST',
            url: 'SendCode',
            dataType: 'json',
            data: { metodo: SendMethod },
            success: function (data) {
                if (data.isSend == true) {

                    if (data.messageReply == "tengoCodigo") {
                        formulario.submit();
                    } else {
                        ShowMessageOk(data.messageReply + " Sera redirigido en 5 segundos");
                        setTimeout(function () {formulario.submit(); }, 5000);
                    }

                } else {
                    ShowMessageFail(data.messageReply);
                    $("#spinerBtnSend").hide();
                    btnSendCode.disabled = false;
                }
            },
            error: function (e) {
                alert(e.statusText);
            },

        });

    }
});