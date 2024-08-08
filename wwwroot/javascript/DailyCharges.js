/*Daily Charges Js functions*/

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
        url: "/DailyCharges/GetInHouseRoomInfo",
        data: inputData,
        success: function (room) {
            $('#hiddenRoomLgId').val(room.roomLgId);
            $('#inputRoomNo').val(room.roomno);
            $('#inputGuestName').val(room.guestName);
            $('#inputArriveDate').val(room.stringArriveDte);
            $('#inputNightQty').val(room.nightqty);

            loadDailyCharges(roomLgId);
            closeInHouseRoomModal();
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });

}

// Daily Charges

function generateDailyCharge() {

    var roomLgId = $('#hiddenRoomLgId').val();
    if (roomLgId == null || roomLgId == '') {
        alert('Please choose Room No first!');
        return;
    }

    clearDailyChargeTable();
    // srvc Table
    const srvcTable = document.getElementById('srvcTable');
    const srvcTableData = [];

    for (let i = 1; i < srvcTable.rows.length; i++) {
        const row = srvcTable.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            rowData.push(getCellContent(row.cells[j]));
        }
        srvcTableData.push(rowData);
    }

    var inputData = {
        srvcTableData: srvcTableData,
        roomLgId: roomLgId
    }

    $.ajax({
        type: 'POST',
        url: '/DailyCharges/GenerateDailyCharges',
        data: inputData,
        success: function (generatedData) {
            generatedData.forEach(function (data) {
                const newRow = $('<tr>').css({ 'font-size': '14px', 'padding': '12px', 'text-align': 'center' });

                const columnStyles = {
                    padding: '12px',
                    width: '100%',
                    border: 'none',
                    color: '#31849B',
                    textAlign: 'center'
                };

                const leftColumnStyles = {
                    ...columnStyles,
                    textAlign: 'left'
                };


                //td Date
                var tdDate = $('<td>').css('textAlign','left').text(data.date);
                newRow.append(tdDate);

                //td SrvCde
                var tdSrvCde = $('<td>').text(data.serviceCode).css({ 'textAlign': 'left' });
                newRow.append(tdSrvCde);

                //input Amount
                var inputAmount = $('<input>').attr('type', 'number').css(leftColumnStyles).val(data.amount);
                newRow.append($('<td>').css('padding', '0px').append(inputAmount));

                //input Qty
                var inputQty = $('<input>').attr('type', 'number').css(columnStyles).val(data.qty);
                newRow.append($('<td>').css('padding', '0px').append(inputQty));

                // select Folio
                var selectFolio = $('<select>').css(leftColumnStyles);
                $.ajax({
                    url: "/DailyCharges/GetFolios",
                    data: { roomLgId: roomLgId },
                    success: function (folios) {
                        $("<option>").val('MF').text('MF').appendTo(selectFolio);
                        folios.forEach(folio => {
                            $("<option>").val(folio.folioCde).text(folio.folioCde).appendTo(selectFolio);
                        });
                        selectFolio.val(data.folio);
                    },
                    error: function () {
                        alert('Session Expired!');
                        window.location.href = '/MsUsers/Login';  // Redirect to login
                    }
                });
                newRow.append($('<td>').css('padding', '0px').append(selectFolio));

                //td Delete
                var tdDelete = $('<td>').text('remove').css({ 'color': 'red', 'cursor': 'pointer' });
                tdDelete.on('click', function () {
                    newRow.remove();
                })
                newRow.append(tdDelete);

                $('#dailyChargesTableBody').append(newRow);
            });
        },
        error: function (error) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function saveDailyCharges() {
    var roomLgId = $('#hiddenRoomLgId').val();

    // daily charge Table
    const dailyChargesTable = document.getElementById('dailyChargesTable');
    const dailyChargesTableData = [];

    for (let i = 1; i < dailyChargesTable.rows.length; i++) {
        const row = dailyChargesTable.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            rowData.push(getCellContent(row.cells[j]));
        }
        dailyChargesTableData.push(rowData);
    }

    var inputData = {
        roomLgId: roomLgId,
        dailyChargesTableData: dailyChargesTableData
    };

    $.ajax({
        type: 'POST',
        url: "/DailyCharges/SaveDailyCharges",
        data: inputData,
        success: function () {
            alert('Successfully Saved.');
            location.reload();
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function loadDailyCharges(roomLgId) {

    clearDailyChargeTable();

    $.ajax({
        type: 'POST',
        url: '/DailyCharges/LoadDailyCharges',
        data: { roomLgId: roomLgId },
        success: function (loadData) {
            loadData.forEach(function (data) {
                const newRow = $('<tr>').css({ 'font-size': '14px', 'padding': '12px', 'text-align': 'center' });

                const columnStyles = {
                    padding: '12px',
                    width: '100%',
                    border: 'none',
                    color: '#31849B',
                    textAlign: 'center'
                };

                const leftColumnStyles = {
                    ...columnStyles,
                    textAlign: 'left'
                };


                //td Date
                var tdDate = $('<td>').text(data.date);
                newRow.append(tdDate);

                //td SrvCde
                var tdSrvCde = $('<td>').text(data.serviceCode).css({ 'textAlign': 'left' });
                newRow.append(tdSrvCde);

                //input Amount
                var inputAmount = $('<input>').attr('type', 'number').css(leftColumnStyles).val(data.amount);
                newRow.append($('<td>').css('padding', '0px').append(inputAmount));

                //input Qty
                var inputQty = $('<input>').attr('type', 'number').css(columnStyles).val(data.qty);
                newRow.append($('<td>').css('padding', '0px').append(inputQty));

                // select Folio
                var selectFolio = $('<select>').css(leftColumnStyles);
                $.ajax({
                    url: "/DailyCharges/GetFolios",
                    data: { roomLgId: roomLgId },
                    success: function (folios) {
                        $("<option>").val('MF').text('MF').appendTo(selectFolio);
                        folios.forEach(folio => {
                            $("<option>").val(folio.folioCde).text(folio.folioCde).appendTo(selectFolio);
                        });
                        selectFolio.val(data.folio);
                    },
                    error: function () {
                        alert('Session Expired!');
                        window.location.href = '/MsUsers/Login';  // Redirect to login
                    }
                });
                newRow.append($('<td>').css('padding', '0px').append(selectFolio));

                //td Delete
                var tdDelete = $('<td>').text('Delete').css({ 'color': 'red', 'cursor': 'pointer' });
                tdDelete.on('click', function () {
                    newRow.remove();
                })
                newRow.append(tdDelete);

                $('#dailyChargesTableBody').append(newRow);
            });
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function getCellContent(cell) {
    const select = cell.querySelector("select");
    if (select) {
        return select.value;
    }
    const input = cell.querySelector("input");
    if (input) {
        if (input.type === "checkbox") {
            return input.checked; // Return true or false for checkbox
        } else {
            return input.value;
        }
    }
    return cell.textContent;
}

function clearDailyChargeTable() {
    $('#dailyChargesTableBody').empty();
}

