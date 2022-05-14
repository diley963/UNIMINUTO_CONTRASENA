var btnLogin = document.getElementById("btnLogin");

btnLogin.onclick = function (event) {
    event.preventDefault();
    var frm = new FormData();
    frm.append("userName", document.getElementById("userName").value);
    frm.append("password", document.getElementById("password").value);

    $.ajax({

        type: 'POST',
        url: 'ValidateLogin',
        data: frm,
        contentType: false,
        processData: false,
        statusCode: function(data) {
            data.rpt.statusCode
        }
       /* success: function (data) {
            if (data.rpt.StatusCode == 200) {
                console.log("Logeado => rpt " + data.rpt)
            } else {
                console.log("NO se Logeo => rpt " + data.rpt)
               StatusCode:400
            }
        },*/
        

    });

    return;
}