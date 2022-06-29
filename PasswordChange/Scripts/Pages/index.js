//we capture the inputs
var btnSend = document.getElementById("btnSend");
//we capture the inputs text and email value
var email = document.getElementById("email");
var nDocument = document.getElementById("nDocument");
var alternativeEmail = document.getElementById("alternativeEmail");
var mobile = document.getElementById("mobile");
var nombre = document.getElementById("nombre");
var apellido = document.getElementById("apellido");
var descripcion = document.getElementById("descripcion");
//we hide the sniper from the send button
$("#spinerBtnSend").hide();
//Focus is set in the email input
email.focus();

// we block inputs until you enter a valid email address
//nDocument.disabled = true;
//btnSend.disabled = true;

btnSend.onclick = function () { SendData(); }

//envia la data al controller y valida si hay calpos vacios
function SendData() {

    var contiene = email.value.indexOf("@");
    //we show the send button sniper
    $("#spinerBtnSend").show();
    //We verify that the Inputs are not empty.
    if (email.value == "" || nDocument.value == "") {
        ShowMessageFail("No pueden Haber campos Vacios");
        //we hide the send button sniper after 5 seconds
        setTimeout(function () { $("#spinerBtnSend").hide(); }, 5000);

        // event.preventDefault();
    }
    else
        if (contiene < 0) {
            ShowMessageFail("Email no valido debe tener @ + dominio de Uniminuto");
            //we hide the send button sniper after 5 seconds
            setTimeout(function () { $("#spinerBtnSend").hide(); }, 5000);
        }
        else {

            var frm = new FormData();
            frm.append("cedula", nDocument.value)
            frm.append("email", email.value)

            $.ajax({

                type: 'POST',
                url: '/Home/ValidateUser',
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.existe > 0) {                        
                        document.getElementById("idFormIndex").submit();
                    } else {
                        ShowMessageFail(data.messageReply)
                        setTimeout(function () { $("#spinerBtnSend").hide(); }, 5000);
                    }
                }
            });
        }
}

//alphabetic fields are locked for nDocumnet input
nDocument.onkeypress = function (event) {

    //se Corta a la longitud establecida
    if (this.value.length >10)
        this.value = this.value.slice(0, 11 - 1);

    var codigo = event.which || event.keyCode;
    if (codigo === 13) {
       
        SendData();
    }

    //we indicate that only numbers with a length of 10 characters can be entered
    return ((event.charCode >= 48 && event.charCode <= 57));

    
}
//we validate that the when focus is lost in the email input it has the valid formatting 
//email.onblur = function () {
//    var caracter = this.value;
//    var contiene = caracter.indexOf("@")
//    if (contiene < 0) {
//        ShowMessageFail("Email no valido debe tener @ + dominio de Uniminuto");
//    } else {
//        nDocument.disabled = false;
//        btnSend.disabled = false;
//    }
//}

//CAMBIO 
function utf8_to_b64(str) {
    return window.btoa(unescape(encodeURIComponent(str)));
}
