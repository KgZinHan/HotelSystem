/*Transfer/Upgrade Js functions*/

$(document).ready(function () {
    const roomlgId = $('#hiddenRoomLgId').val();
    if (roomlgId != '' && roomlgId != 0 && roomlgId != null) {
        chooseInHouseRoom(roomlgId);
    }
});

// InHouseRoomModal

function openInHouseRoomModal() {
    $('#inHouseRoomModal').show();
}

function closeInHouseRoomModal() {
    $('#inHouseRoomModal').hide();
}

function chooseInHouseRoom(roomLgId) {
    var inputData = {
        roomLgId: roomLgId
    };

    $.ajax({
        type: 'GET',
        url: "/TransferUpgrade/GetInHouseRoomInfo",
        data: inputData,
        success: function (room) {
            $('#inputRoomNo').val(room.roomno);
            $('#inputOldRoomPrice').val(room.rmprice.toLocaleString());
            $('#inputExBedQty').val(room.extrabedqty);
            $('#inputExBedPrice').val(room.extrabedprice.toLocaleString());
            $('#inputDiscountAmt').val(room.discountamt.toLocaleString());
            $('#inputGuestName').val(room.guestName);
            var occudte = room.occudte.split('T')[0];
            $('#inputArriveDate').val(occudte);
            var departdte = room.departdte.split('T')[0];
            $('#inputDepartureDate').val(departdte);
            $('#hiddenRoomLgId').val(room.roomLgId);
            $('#inputCheckInId').val(room.checkInId);
            closeInHouseRoomModal();
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });


}

// AvailableRoomModal

function openAvailableRoomModal() {

    var roomLgId = $('#hiddenRoomLgId').val();

    if (roomLgId == '' || roomLgId == 0 || roomLgId == null) {
        alert('Please choose room first!');
        return;
    }

    $.ajax({
        type: 'GET',
        url: "/TransferUpgrade/GetAvailableRooms",
        data: { roomLgId: roomLgId },
        success: function (data) {
            $("#availableRoomModalBodyId").html(data);
            $('#availableRoomModal').show();
        },
        error: function () {
            alert('Error in retrieving availableRooms.');
            return;
        }
    })

}

function closeAvailableRoomModal() {
    $('#availableRoomModal').hide();
}

function chooseAvailableRoom(roomId) {
    var inputData = {
        roomId: roomId
    };

    $.ajax({
        type: 'GET',
        url: "/TransferUpgrade/GetAvailableRoomInfo",
        data: inputData,
        success: function (room) {
            $('#hiddenRmTypId').val(room.rmtypId);
            $('#hiddenRmRateid').val(room.rmrateid);
            $('#inputNewRoomNo').val(room.roomno);
            $('#inputNewExBedQty').val(0);
            $('#inputNewRoomPrice').val(room.rmprice);
            $('#inputNewExBedPrice').val(room.extrabedprice);
            $('#inputNewDiscountAmt').val(0);
            closeAvailableRoomModal();
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });


}
