
function updateReservation(resvNo, selectElement) {
    var selectedText = $(selectElement).find('option:selected').text();
    var shouldProcess = window.confirm("Are you sure you want to " + selectedText + " this reservation ?");

    if (shouldProcess) {
        var data = {
            resvNo: resvNo,
            resvState: $(selectElement).val()
        }

        $.ajax({
            url: "/ReservationConfirm/UpdateReservation",
            type: "GET",
            data: data,
            success: function () {
                alert(resvNo + ' is updated.');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            }
        });
    }
    else {
        console.log($(selectElement).find('option:first').val());
        var firstOptionValue = $(selectElement).find('option:first').val();
        $(selectElement).val(firstOptionValue);
    }

}

function viewResvDetails(resvNo) {
    const detailsModal = $('#resvDetailsModal');

    var data = {
        resvNo: resvNo
    }

    $.ajax({
        url: "/ReservationConfirm/ViewResvDetails",
        type: "GET",
        data: data,
        success: function (data) {
            $('#resvDetailsModalBody').html(data);
            detailsModal.show();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error.');
            }
        }
    });
}

function clearScreen() {
    $("#filterGuestName").val('');
    $('#filterResvState').val('');
    $('#filterActive').val('');
    $('#filterFromDte').val('');
    $('#filterToDte').val('');
}

function closeDetailsModal() {
    $('#resvDetailsModal').hide();
}