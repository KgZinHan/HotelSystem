/*ExtendStay Js functions*/

function checkAvailability(roomLgId) {

    var dayCount = $('#' + roomLgId).val();

    if (dayCount == 0 || dayCount == '') {
        alert('Please input extend day first');
        return;
    }

    var inputData = {
        roomLgId: roomLgId,
        dayCount: dayCount
    }

    $.ajax({
        type: 'GET',
        url: '/ExtendStay/CheckAvailability',
        data: inputData,
        success: function (result) {
            if (result) {
                var userConfirm = confirm('Room is available for ' + dayCount + ' day(s). Are you sure you want to extend?')
                if (userConfirm) {
                    $.ajax({
                        type: 'GET',
                        url: '/ExtendStay/ExtendStay',
                        data: inputData,
                        success: function () {
                            alert('Successfully extend ' + dayCount + ' day(s).');
                            location.reload();
                        },
                        error: function () {

                        }

                    })
                }
            }
            else {
                alert('Room is not avaiable to extend ' + dayCount + ' day(s).');
            }


        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }

    })


}

function openExtendStayModal() {
    $("#extendStayModal").show();
}

function closeExtendStayModal() {
    $("#extendStayModal").hide();
}

