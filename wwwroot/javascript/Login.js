function checkLogin() {
    var para1 = $("#Usercde").val();
    var para2 = $("#Pwd").val();
    $.ajax({
        type: "Post",
        url: "/MsUsers/AjaxLogin",
        data: { uc: para1, p: para2 },
        success: function (response) {
            if (response.includes('#Error.')) {
                $("#Cmpy").val('-');
                response = response.replace('#Error.', '');
                $("#loginconfirmmsg").text(response);
                $('#btnGo').hide();
            }
            else {
                $("#Cmpy").val(response);
                $("#loginconfirmmsg").text("You are logged in as a member of " + response + ".");
                $('#btnGo').show();
            }
            var stepper = new Stepper(document.querySelector('.bs-stepper'))
            stepper.next()
        },
        error: function (response) {
            alert("Authorization Error...");
            console.log(response);
        }
    });
}

function checkEnter(event) {
    if (event.key == "Enter") {
        checkLogin();
    }
}


