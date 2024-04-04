/*Folio Management Js functions*/

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
        url: "/FolioManagement/GetInHouseRoomInfo",
        data: inputData,
        success: function (room) {
            $('#hiddenRoomLgId').val(room.roomLgId);
            $('#inputRoomNo').val(room.roomno);
            $('#inputGuestName').val(room.guestName);
            $('#inputCheckInId').val(room.checkInId);
            var occudte = room.occudte.split('T')[0];
            $('#inputArriveDate').val(occudte);
            $('#inputNightQty').val(room.nightqty);
            closeInHouseRoomModal();
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });

    loadFolioManagement(roomLgId);


}

// Folio

function addNewFolio() {

    const columnStyles = {
        padding: '12px',
        width: '100%',
        border: 'none',
        color: '#31849B'
    };

    const newRow = $('<tr>').css({ 'font-size': '14px', 'text-align': 'left' });

    var newNo = lastNumber();

    newRow.append($('<td>').css('textAlign', 'center').append(newNo));

    var tdFolioCde = $('<td>').css('textAlign', 'center').text('OF' + (newNo - 1));
    newRow.append(tdFolioCde);

    var inputFolioName = $('<input>').css(columnStyles).val('Other Folio ' + (newNo - 1));
    newRow.append($('<td>').css('padding', '0px').append(inputFolioName));

    var tdDel = $('<td>').css({ 'color': 'red', 'cursor': 'pointer', 'textAlign': 'center' }).text('Remove');
    newRow.append(tdDel);

    tdDel.on('click', function () {
        newRow.remove();
        changeNumberColumn();
        var selects = $('#SrvcFolioTableBody select');
        selects.each(function () {
            $(this).find('option[value="' + tdFolioCde.text() + '"]').remove();
        });
    });

    newRow.on('keypress', function (event) {
        if (event.keyCode === 13) {
            addNewFolio();
        }
    })

    $('#FolioTableBody').append(newRow);

    // add new <option> based on Folio table
    var selects = $('#SrvcFolioTableBody select');
    selects.each(function () {
        $(this).append($("<option>").val(tdFolioCde.text()).text(tdFolioCde.text()));
    });

}

function loadFolioManagement(roomLgId) {

    var inputData = {
        roomLgId: roomLgId
    };

    const columnStyles = {
        padding: '12px',
        width: '100%',
        border: 'none',
        color: '#31849B'
    };

    $.ajax({
        type: 'GET',
        url: '/FolioManagement/GetSrvcFolioList',
        data: inputData,
        success: function (list) {
            $('#SrvcFolioTableBody').empty();
            list.forEach(function (folio) {
                const newRow = $('<tr>').css({ 'font-size': '14px', 'text-align': 'left' });

                newRow.append($('<td>').css('textAlign', 'left').text(folio.srvcDesc));
                newRow.append($('<td>').css('textAlign', 'left').text(folio.srvcCde));

                var tdFolioCde = $('<td>').css('padding', '0px');
                var selectFolioCde = $('<select>').css({ 'padding': '12px', 'width': '100%', 'border': 'none' });
                var optionFolioCde = $('<option>').text('MF');
                selectFolioCde.append(optionFolioCde);
                $.ajax({
                    url: "/FolioManagement/GetFolioOptions",
                    data: inputData,
                    success: function (folios) {
                        var fragment = document.createDocumentFragment();
                        folios.forEach(folio => {
                            $("<option>").val(folio.folioCde).text(folio.folioCde).appendTo(selectFolioCde);
                        });
                        selectFolioCde.val(folio.folioCde);
                    },
                    error: function () {
                        alert('Session Expired!');
                        window.location.href = '/MsUsers/Login';  // Redirect to login
                    }
                });
                selectFolioCde.append(optionFolioCde);
                tdFolioCde.append(selectFolioCde);
                newRow.append(tdFolioCde);

                $('#SrvcFolioTableBody').append(newRow);
            });

            $.ajax({
                type: 'GET',
                url: '/FolioManagement/GetFolioList',
                data: inputData,
                success: function (list) {

                    $('#FolioTableBody').empty();

                    const newRow = $('<tr>').css({ 'font-size': '14px', 'text-align': 'left' });

                    var newNo = lastNumber();
                    newRow.append($('<td>').css('textAlign', 'center').append(newNo));

                    var tdFolioCde = $('<td>').css('textAlign', 'center').text('MF');
                    newRow.append(tdFolioCde);

                    var tdFolioName = $('<td>').text('Main Folio');
                    newRow.append(tdFolioName);

                    var tdDel = $('<td>').css({ 'textAlign': 'center' }).text('Default');
                    newRow.append(tdDel);

                    $('#FolioTableBody').append(newRow);
                    list.forEach(function (folio) {
                        const newRow = $('<tr>').css({ 'font-size': '14px', 'text-align': 'left' });

                        var newNo = lastNumber();
                        newRow.append($('<td>').css('textAlign', 'center').append(newNo));

                        var tdFolioCde = $('<td>').css('textAlign', 'center').text(folio.foliocde);
                        newRow.append(tdFolioCde);

                        var inputFolioName = $('<input>').css(columnStyles).val(folio.foliodesc);
                        newRow.append($('<td>').css('padding', '0px').append(inputFolioName));

                        var tdDel = $('<td>').css({ 'color': 'red', 'cursor': 'pointer', 'textAlign': 'center' }).text('Remove');
                        newRow.append(tdDel);

                        tdDel.on('click', function () {
                            newRow.remove();
                            changeNumberColumn();
                            var selects = $('#SrvcFolioTableBody select');
                            selects.each(function () {
                                $(this).find('option[value="' + tdFolioCde.text() + '"]').remove();
                            });
                        });

                        $('#FolioTableBody').append(newRow);


                    })
                },
                error: function () {
                    alert('Session Expired!');
                    window.location.href = '/MsUsers/Login';  // Redirect to login
                }
            });
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function saveFolioManagement() {

    var checkInId = $('#inputCheckInId').val();
    if (checkInId == '' || checkInId == null) {
        alert('Please choose Room No first!');
        return;
    }

    const folioTable = document.getElementById('folioTable');
    const folioTableData = [];

    for (let i = 1; i < folioTable.rows.length; i++) { // skip Main Folio
        const row = folioTable.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            rowData.push(getCellContent(row.cells[j]));
        }
        folioTableData.push(rowData);
    }

    const srvcFolioTable = document.getElementById('srvcFolioTable');
    const srvcFolioTableData = [];

    for (let i = 1; i < srvcFolioTable.rows.length; i++) { // skip Column Head
        const row = srvcFolioTable.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            rowData.push(getCellContent(row.cells[j]));
        }
        srvcFolioTableData.push(rowData);
    }



    var inputData = {
        checkInId: checkInId,
        folioTable: folioTableData,
        srvcFolioTable: srvcFolioTableData
    }

    $.ajax({
        type: 'POST',
        url: '/FolioManagement/SaveFolio',
        data: inputData,
        success: function () {
            alert('Successfully saved.');
            location.reload();
        },
        error: function (error) {
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


// Common

function lastNumber() {
    var lastRow = $('#FolioTableBody tr:last td:first-child');
    if (lastRow.length <= 0) {
        return 1;
    }
    else {
        if (lastRow.text() == '') {
            return 1;
        }
        return parseFloat(lastRow.text()) + 1;
    }

}

function changeNumberColumn() {
    var numberColumnCells = $('#FolioTableBody tr td:first-child');
    numberColumnCells.each(function (index) {
        $(this).text(index + 1);
    });
}

