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

    //Mostramos el enlace de actualizar o el texto segun el tipo de cuenta
    //if (document.getElementById("tipoCuenta").value == 1) {
    //    $("#showEnlace").css("display", "block");
    //} else {
    //    $("#showTexto").css("display", "block");
    //}

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

        var SendMethod;

        if (rbtEmail.checked == true && rbtMobile.checked == false /*&& rbtTengoCodigo.checked == false*/) {
            SendMethod = rbtEmail.value;
        } else if (rbtEmail.checked == false && rbtMobile.checked == true /*&& rbtTengoCodigo.checked == false*/) {
            SendMethod = rbtMobile.value;
        } /*else if (rbtTengoCodigo.checked == true && rbtEmail.checked == false && rbtMobile.checked == false) {
        SendMethod = rbtTengoCodigo.value;
        //document.getElementById("idForm").submit();
    }*/

        //the value of the selected radiobuton is captured and sent by the selected method
        if (rbtEmail.checked == false && rbtMobile.checked == false /*&& rbtTengoCodigo.checked == false*/) {
            ShowMessageFail("Debe seleccionar un método de envió");
            event.preventDefault();
        } else {
            //oculto spiner
            $("#spinerBtnSend").show();

            //bloqueo elementos del formulario
            btnSendCode.disabled = true;
            //var frm = new FormData();
            //frm.append("SendMethod", SendMethod);
            //frm.append("mobile", document.getElementById("mobile").value)
            //frm.append("alternativeEmail", document.getElementById("alternativeEmail").value)
            //frm.append("nDocument", document.getElementById("nDocument").value)
            //frm.append("email", document.getElementById("email").value)
            //frm.append("nombre", document.getElementById("nombre").value)
            //frm.append("apellido", document.getElementById("apellido").value)
            //frm.append("descripcion", document.getElementById("descripcion").value)
            //we send the data to the controller's SendCode method 

            $.ajax({
                type: 'POST',
                url: 'SendCode',
                dataType: 'json',
                data: { metodo: SendMethod },
                success: function (data) {
                    if (data.isSend == true) {

                        if (data.messageReply == "tengoCodigo") {
                            // document.getElementById("idForm").submit()
                            formulario.submit();
                        } else {
                            //Display message and redirect to a new window in a time frame
                            ShowMessageOk(data.messageReply + " Sera redirigido en 5 segundos");
                            // setTimeout(function () { window.location.href = "/Home/CodeConfirmationForm/"; }, 5000
                            setTimeout(function () { /*document.getElementById("idForm").submit();*/  formulario.submit(); }, 5000);
                        }

                    } else {
                        //Display failure message 
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


    //The functionality of the send code button is programmed.
    btnTengoCode.onclick = function (event) {

        var SendMethod;

        SendMethod = btnTengoCode.value;

        //var frm = new FormData();
        //frm.append("SendMethod", SendMethod);
        //frm.append("mobile", document.getElementById("mobile").value)
        //frm.append("alternativeEmail", document.getElementById("alternativeEmail").value)
        //frm.append("nDocument", document.getElementById("nDocument").value)
        //frm.append("email", document.getElementById("email").value)
        //frm.append("nombre", document.getElementById("nombre").value)
        //frm.append("apellido", document.getElementById("apellido").value)
        //frm.append("descripcion", document.getElementById("descripcion").value)
        //we send the data to the controller's SendCode method 

        $.ajax({
            type: 'POST',
            url: 'SendCode',
            dataType: 'json',
            data: { metodo: SendMethod },
            success: function (data) {
                if (data.isSend == true) {

                    if (data.messageReply == "tengoCodigo") {
                        // document.getElementById("idForm").submit()
                        formulario.submit();
                    } else {
                        //Display message and redirect to a new window in a time frame
                        ShowMessageOk(data.messageReply + " Sera redirigido en 5 segundos");
                        // setTimeout(function () { window.location.href = "/Home/CodeConfirmationForm/"; }, 5000
                        setTimeout(function () { /*document.getElementById("idForm").submit();*/  formulario.submit(); }, 5000);
                    }

                } else {
                    //Display failure message 
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
});