/* ExpectedCheckIn Js functions */

function search(keyword) {

    $('#loader-wrapper').show();

    $.ajax({
        type: 'GET',
        url: "/Home/Search",
        data: { keyword: keyword },
        success: function (data) {
            $('#homeBody').html(data);
            $('#loader-wrapper').hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error in searching keyword.');
                $('#spanSearchingText').hide();
            }
        }
    });
}

// Action Modal
function openActions(roomlgId) {
    var allInputData = {
        roomlgId: roomlgId
    }
    $.ajax({
        type: 'GET',
        url: "/Home/OpenActions",
        data: allInputData,
        success: function (data) {
            $('#actionsModal').show();
            $('#actionsModalBody').html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error');
            }
        }
    });
}

function closeActionsModal() {
    $('#actionsModal').hide();
}

// Cancel Reservation Modal

function confirmCancelResv(roomLgId) {
    var allInputData = {
        roomLgId: roomLgId
    }
    $.ajax({
        type: 'GET',
        url: "/Home/OpenCancelResv",
        data: allInputData,
        success: function (data) {
            $('#cancelResvModal').show();
            $('#cancelResvModalBody').html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error');
            }
        }
    });
}

function cancelResv() {

    const resvNo = $('#hiddenResvNo').val();

    var inputData = {
        resvNo: resvNo
    }

    $.ajax({
        type: 'POST',
        url: "/Home/CancelResv",
        data: inputData,
        success: function () {
            alert('Successfully updated.');
            location.reload();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error');
            }
        }
    });
}

function closeCancelResvModal() {
    $('#cancelResvModal').hide();
}

// Check-out Modal

function openCheckOutModal(roomLgId) {
    var allInputData = {
        roomLgId: roomLgId
    }
    $.ajax({
        type: 'GET',
        url: "/Home/OpenCheckOut",
        data: allInputData,
        success: function (data) {
            $('#checkOutModal').show();
            $('#checkOutModalBody').html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error');
            }
        }
    });
}

function confirmForceCheckOut() {
    var userConfirm = confirm('Are you sure you want to force check-out?');
    return userConfirm;
}

function closeCheckOutModal() {
    $('#checkOutModal').hide();
}

