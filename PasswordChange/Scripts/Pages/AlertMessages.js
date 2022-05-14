// To hide the messages 
function FullHiddeMessages() {
    $("#alertSuccess").hide("swing");
    $("#alertFails").hide("swing");
    $(".alert-danger").alert('close');
    $(".alert-primary").alert('close');
}


// Function to show the Succes Messages 
function ShowMessageOk(message, time) {
    var timeOut = time == null ? 5000 : time;
    FullHiddeMessages();
    var succesPanel = '<div class="alert alert-primary"> <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-check-circle"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg> <strong>Success!</strong> ' + message + '</div>';
    $("#alertSuccess").append(succesPanel);
    window.scrollTo(0, 0);
    $("#alertSuccess").show("swing");
    setTimeout(function () {

        FullHiddeMessages()

    }, timeOut);
}

// To show the Message error Details Section
function ShowMessageFail(message, time) {
    var timeOut = time == null ? 5000 : time;
    FullHiddeMessages();
    var ErrorPanel = '<div class="alert alert-danger"> <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x-circle"><circle cx="12" cy="12" r="10"></circle><line x1="15" y1="9" x2="9" y2="15"></line><line x1="9" y1="9" x2="15" y2="15"></line></svg> <strong>Error! </strong>' + message + '</div>';
    $("#alertFails").append(ErrorPanel);
    //window.scrollTo(0, 98.8888931274414);
    window.scrollTo(0, 0);
    $("#alertFails").show("swing");
    setTimeout(function () {

        FullHiddeMessages()

    }, timeOut);
}