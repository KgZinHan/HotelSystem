/*RoomEnquiry Js functions*/

function checkRoom(roomNo) {

    $.ajax({
        type: 'GET',
        url: "/RoomEnquiry/CheckRoomIsAvailable",
        data: { roomNo: roomNo },
        success: function (response) {
            if (response) {
                chooseRoom(roomNo);
            }
            else {
                alert(roomNo + ' is already occupied.')
            }
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function chooseRoom(roomNo) {
    $.ajax({
        type: 'POST',
        url: "/RoomEnquiry/GetRoomDetails",
        data: { roomNo: roomNo },
        success: function (response) {
            window.location.href = response.redirectTo;
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

