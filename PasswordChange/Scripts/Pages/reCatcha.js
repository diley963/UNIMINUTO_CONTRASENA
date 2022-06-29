var onloadCallback = function () {
    grecaptcha.render('dvCaptcha', {
        'sitekey': '6Lf4uqogAAAAAPIcrA26_u5XjzdrgR4r0qlbKK11',
        'callback': function (response) {
            $.ajax({
                type: "POST",
                url: "/prueba/AjaxMethod",
                data: "{response: '" + response + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    var captchaResponse = jQuery.parseJSON(r.Response);
                    if (captchaResponse.success) {
                        document.getElementById("btnOptions").style.display = "block";
                    } else {
                        $("#hfCaptcha").val("");
                        $("#rfvCaptcha").show();
                        var error = captchaResponse["error-codes"][0];
                        $("#rfvCaptcha").html("RECaptcha error. " + error);
                    }
                }
            });
        }
    });
};


