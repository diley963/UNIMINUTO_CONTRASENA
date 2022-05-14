//Se bloquea la tecla enter
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('input[type=number]').forEach(node => node.addEventListener('keypress', e => {
        if (e.keyCode == 13) {
            e.preventDefault();
        }
    }))
});


//Capture the button validating the code
var btnValidateCode = document.getElementById("btnValidateCode");

var inputConfirmPassword = document.getElementById("ConfirmPassword");
var inputPassword = document.getElementById("password");
var code = document.getElementById("code");


//var missingAttempts = 0;
var attemps = document.getElementById("attemps").value;

//if the code is correct, I hide the initial form and display another form where the password change will be made.
btnValidateCode.onclick = function () { SendDataCode(); }


function SendDataCode() {
   
    //The value entered in the input Code is captured.
    if (code.value == '') {
        ShowMessageFail("No puede haber campos vacíos ingrese un código");
        return;
    } else {
        let codigo= code.value;
        //missingAttempts += 1;
       // var frm = new FormData();
       // frm.append("code", code.value);
       //// frm.append("missingAttempts", missingAttempts);
       // frm.append("cedula", document.getElementById("nDocument").value);
       // frm.append("email", document.getElementById("email").value);
       // frm.append("nombre", document.getElementById("nombre").value);
       // frm.append("apellido", document.getElementById("apellido").value);
        //Enviamos la Informacion al Controlador

        $.ajax({
            type: 'POST',
            url: 'CodeIsValid',
            dataType: 'json',
            data: { code: codigo },
            success: function (data) {
                if (data.response == 1) {

                    $("#divConfirmacionContrasena").css("display", "block");
                    $("#divConfirmacionContrasena").fadeIn('slow');
                    $("#divConfirmacionContrasena").fadeIn(3000);
                    //$('#defaultAccordionTwo').addClass('collapse').removeClass('show');
                    //$('#defaultAccordionOne').addClass('show');
                    $("#divConfirmCode").css("display", "none");

                    //ShowMessageOk("Éxito código validado");
                    ShowMessageOk(data.messageReply);

                } else
                    if (data.response == -1) {
                        //ShowMessageFail("Este código ha caducado por favor solicite un código nuevo");
                        ShowMessageFail(data.messageReply);
                        setTimeout(function () { window.location.href = "/Home" }, 5000);
                    }
                    else {
                        var statuscode;
                        var MensajeExitPage;
                        if (data.response == -2) {
                            statuscode = 600;
                            // MensajeExitPage = "Este código ya fue utilizado por favor solicite un código nuevo";
                            MensajeExitPage = data.messageReply;
                            window.location.href = "/Home/ExitPage/?code=" + statuscode + "&mensaje=" + MensajeExitPage
                        }

                        if (data.missingAttempts >= attemps) {
                            statuscode = 400;
                            MensajeExitPage = "No se pudo validar el código inténtelo nuevamente en  " +
                                document.getElementById("retry_change_pass_minutes").value + " minutos";

                            ShowMessageFail("Código erróneo supero el numero máximo de intentos permitidos");
                            setTimeout(function () { window.location.href = "/Home/ExitPage/?code=" + statuscode + "&mensaje=" + MensajeExitPage }, 5000);
                        } else {
                            //ShowMessageFail("Código erróneo inténtelo nuevamente tiene  " + (attemps - missingAttempts) + " intentos");
                            ShowMessageFail(data.messageReply + (attemps - data.missingAttempts) + " intentos");
                            code.value = "";
                        }

                    }
            },
            error: function (e) {
                alert(e.statusText);
                //Swal.fire("Señor(a) Usuario(a)", "Fallo al traer los registros", "error");
            },

        });


        

    }


}


//Functionality of the view password button for the second confirmation input
var btnShowPasswordConfirm = document.getElementById("toggle-password2");
var showPasswordConfirm = 0;
btnShowPasswordConfirm.onclick = function () {
    if (showPasswordConfirm == 0) {
        document.getElementById("ConfirmPassword").type = "text";
        showPasswordConfirm = 1;
    } else {
        document.getElementById("ConfirmPassword").type = "password";
        showPasswordConfirm = 0;
    }
}


//functionality of the password change button
var btnChangePassword = document.getElementById("btnChangePassword");
btnChangePassword.onclick = function () {

    var MensajeExitPage;
    var code;
    if (inputConfirmPassword.value == "" || inputPassword.value == "") {
        ShowMessageFail("No puede haber campos vacíos");
    }
    else if (inputPassword.value.length < 8 || inputConfirmPassword.value.length < 8) {
        ShowMessageFail("Como mínimo deben Haber 8 caracteres");
    }
    else
        if (inputConfirmPassword.value == inputPassword.value) {

            //if ( (/[A-Z]/.test(inputPassword.value)) && (/[0-9]/.test(inputPassword.value)) 
            //    && ( (/#/.test(inputPassword.value)) || (/@/.test(inputPassword.value)) || (/%/.test(inputPassword.value)) || (/&/.test(inputPassword.value)) || (inputPassword.value.indexOf("*") > 0) || (inputConfirmPassword.value.indexOf("$") > 0)
            //    || (/'/.test(inputPassword.value)) || (/./.test(inputPassword.value)) || (inputPassword.value.indexOf("?")>0) || (/!/.test(inputPassword.value)) )  
            //) 


            if ((/[A-Z]/.test(inputConfirmPassword.value)) && (/[0-9]/.test(inputConfirmPassword.value))
                && ((/#/.test(inputConfirmPassword.value)) || (/@/.test(inputConfirmPassword.value))
                || inputConfirmPassword.value.indexOf("%")>0 || inputConfirmPassword.value.indexOf("&")>0 
                || inputConfirmPassword.value.indexOf("*")>0  || inputConfirmPassword.value.indexOf("$")>0  
                || inputConfirmPassword.value.indexOf("'")>0 || inputConfirmPassword.value.indexOf(".")>0 
                || inputConfirmPassword.value.indexOf("?") > 0 || inputConfirmPassword.value.indexOf("!") > 0
                || inputConfirmPassword.value.indexOf("_") > 0
                )
            )
            {
                //MensajeExitPage = "Contraseña Cambiada con Exito";
                //code = 200;
                //ShowMessageOk("Clave Cambiada con exito");
                //setTimeout(function () { window.location.href = "/Home/ExitPage/?code=" + code + "&mensaje=" + MensajeExitPage }, 5000);
                //var frm = new FormData();
                //frm.append("password", inputPassword.value)
                //frm.append("email", document.getElementById("email").value)
                //frm.append("nDocument", document.getElementById("nDocument").value)

                let password = inputPassword.value;

                $.ajax({

                    type: 'POST',
                    url: 'IsUpdatePassword',
                    data: password,
                    contentType: false,
                    processData: false,
                    success: function (data) {

                        //if the process is successful we show the hidden form for password change
                        if (data.isUpdate == 1) {
                            ShowMessageOk(data.messageReply);
                            code = 200;
                            MensajeExitPage = data.messageReply;
                            setTimeout(function () { window.location.href =/* "/TestLogin/Index" */ "/" }, 5000);
                        } else {
                            ShowMessageFail(data.messageReply);
                        }
                    }
                });

            } else {
                ShowMessageFail("Clave invalida debe contener por lo menos 1 mayúscula un número y un carácter especial");
            }

        }
        else {
            ShowMessageFail("Las claves no coinciden por favor validar");
        }
}


//ASCII annotations are written to validate what type of characters are allowed.
inputPassword.onkeypress = function (event) {

    //we indicate that only numbers with a length of 10 characters can be entered
    return ((event.charCode >= 65 && event.charCode <= 90) || (event.charCode >= 97 && event.charCode <= 122) || (event.charCode >= 48 && event.charCode <= 57)
        || (event.charCode == [35]) || (event.charCode == [64]) || (event.charCode == [42]) || (event.charCode == [36]) || (event.charCode == [37])
        || (event.charCode == [38]) || (event.charCode == [39]) || (event.charCode == [46]) || (event.charCode == [63]) || (event.charCode == [33]) || (event.charCode == [95]));

    // No permitir ñ o Ñ  (event.charCode != [241] && event.charCode != [209])
    // Permitir @  (event.charCode == [64])
    // Permitir numero de 0-9 (event.charCode >= 48 && event.charCode <= 57)
    // Permitit A-Z Mayuscula (event.charCode >= 65 && event.charCode <= 90)
    // Permitir a-z minuscula (event.charCode >= 97 && event.charCode <= 122)
    // Permitir # (event.charCode == [35])
    // Permitir * (event.charCode == [42])
    // Permitir $ (event.charCode == [36])
    // Permitir % (event.charCode == [37])
    // Permitir & (event.charCode == [38])
    // Permitir ' (event.charCode == [39])
    // Permitir . (event.charCode == [46])
    // Permitir ? (event.charCode == [63])
    // Permitir ! (event.charCode == [33])
    // Permitir _ (event.charCode == [95])
}


inputPassword.onkeyup = function () {
    var nombre = this.value.toLowerCase();
    var contieneNombre = nombre.indexOf(document.getElementById("nombre").value.toLowerCase());
    var contieneApellido = nombre.indexOf(document.getElementById("apellido").value.toLowerCase());
    if (contieneNombre >= 0 || contieneApellido >= 0) {
        ShowMessageFail("Se recomienda no usar su nombre o apellidos en las credenciales");
    }
}


function DisableBackButtonAllBrowsers() {
    window.history.forward()
};
DisableBackButtonAllBrowsers();
window.onload = DisableBackButtonAllBrowsers;
window.onpageshow = function (evts) { if (evts.persisted) DisableBackButtonAllBrowsers(); };
window.onunload = function () { void (0) };

