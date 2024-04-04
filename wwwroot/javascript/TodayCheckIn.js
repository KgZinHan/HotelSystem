/*TodayCheckIn Js functions*/

//Common 

function restrictDate() {
    const today = new Date().toISOString().split('T')[0];

    // Set the min attribute to today's date
    document.getElementById('inputCheckInDateId').setAttribute('min', today);
}

function calculateCheckOutDate() {
    const chkInDte = $('#inputCheckInDate').val();
    const noOfDays = parseInt($('#inputNightQty').val());
    const chkInDate = new Date(chkInDte); // Convert check-in date to a Date object
    const selectedValue = $('input[name="paxType"]:checked').val();

    if (!isNaN(chkInDate.getTime()) && !isNaN(noOfDays)) {
        const chkOutDate = new Date(chkInDate);
        chkOutDate.setDate(chkOutDate.getDate() + noOfDays);
        const formattedChkOutDate = chkOutDate.toISOString().slice(0, 10); // Format the date as YYYY-MM-DD
        $('#inputCheckOutDate').val(formattedChkOutDate);
    }
}


// choose guest functions
/*function chooseGuest() {
    var guestId = $('#hiddenGuestId').val();
    $.ajax({
        type: 'GET',
        url: "/TodayCheckIn/GetGuestList",
        data: { guestId: guestId },
        success: function (data) {
            $('#guestModalBodyId').html(data);
            const roomLgId = $('#inputRoomLgId').val();
            $('#hiddenRoomLgId').val(roomLgId);
            console.log(roomLgId);
            $('#guestModal').show();
        },
        error: function (data) {
            alert('error');
        }
    });
}*/


// Billing

function calculateAmount() {
    const tExbedPrice = parseFloat($('#inputExtrabedqty').val()) * parseFloat($('#inputExtrabedprice').val());
    const tAmt = parseFloat($('#inputRmprice').val()) + tExbedPrice - parseFloat($('#inputDiscountamt').val());
    $('#inputAmt').val(parseFloat(tAmt));
};


// Room Selection

function chooseRoom(rmLgId, rmtypId) {

    var allData = {
        roomTypeId: rmtypId,
        roomLgId: rmLgId
    }

    $.ajax({
        type: 'GET',
        url: "/TodayCheckIn/GetRoomsList",
        data: allData,
        success: function (data) {
            $('#roomModalBodyId').html(data);
            $('#roomModal').show();
        },
        error: function (data) {
            alert('error');
        }
    });
}

function assignRoom(roomNo) {
    $('#roomModal').hide();

    $('#inputRoomNo').val(roomNo);
}

function closeRoomModal() {
    $('#roomModal').hide();
}


// Guest Info

function addGuest() {

    if ($('#inputGuestfullnme').val() === '' || $('#inputGuestLastNme').val() === '' || $('#inputIdppno').val() === '') {
        alert('Guest fullname,lastname and id should not be empty!');
        return;
    }

    const newRow = $('<tr>').css({ 'font-size': '14px', 'text-align': 'left' });

    var centerCssStyle = {
        'padding': '5px',
        'text-align': 'center'
    };

    // auto NO.
    var tdNumber = lastNumber();
    newRow.append($('<td>').css(centerCssStyle).text(tdNumber));

    // saluation
    var hdnSaluteId = $('#selectSaluteId');
    newRow.append($('<td>').css('padding', '5px').text(hdnSaluteId.val()).hide());

    // fullName
    var tdFullName = $('#inputGuestfullnme');
    newRow.append($('<td>').css('padding', '5px').text(tdFullName.val()));

    // lastName
    var hdnLastName = $('#inputGuestLastNme');
    newRow.append($('<td>').css('padding', '5px').text(hdnLastName.val()).hide());

    // idppno
    var tdIdppno = $('#inputIdppno');
    newRow.append($('<td>').css('padding', '5px').text(tdIdppno.val()));

    // idIssueDate
    var hdnIssueDate = $('#inputIdissuedte');
    newRow.append($('<td>').css('padding', '5px').text(hdnIssueDate.val()).hide());

    // Date of Birth
    var hdnDob = $('#inputDob');
    newRow.append($('<td>').css('padding', '5px').text(hdnDob.val()).hide());

    // country
    var hdnCountryId = $('#selectCountryid');
    newRow.append($('<td>').css('padding', '5px').text(hdnCountryId.val()).hide());

    // state
    var hdnStateId = $('#selectStateid');
    newRow.append($('<td>').css('padding', '5px').text(hdnStateId.val()).hide());

    // nation
    var hdnNationId = $('#selectNationid');
    newRow.append($('<td>').css('padding', '5px').text(hdnNationId.val()).hide());

    // vipFlg
    var inputVipflg = $('#inputVipflg:checked').length > 0;
    newRow.append($('<td>').css('padding', '5px').text(inputVipflg.toString()).hide());

    // email Address
    var hdnEmailAddr = $('#inputEmailaddr');
    newRow.append($('<td>').css('padding', '5px').text(hdnEmailAddr.val()).hide());

    // phone1
    var tdPhone1 = $('#inputPhone1');
    newRow.append($('<td>').css('padding', '5px').text(tdPhone1.val()));

    // phone2
    var hdnPhone2 = $('#inputPhone2');
    newRow.append($('<td>').css('padding', '5px').text(hdnPhone2.val()).hide());

    // credit Limit
    var hdnCreditLimit = $('#inputCrlimitamt');
    newRow.append($('<td>').css('padding', '5px').text(hdnCreditLimit.val()).hide());

    // remark
    var hdnRemark = $('#inputRemark');
    newRow.append($('<td>').css('padding', '5px').text(hdnRemark.val()).hide());

    // gender
    var hdnGender = $("#selectGender");
    newRow.append($('<td>').css('padding', '5px').text(hdnGender.val()).hide());

    // guestId
    var hdnGuestId = $('#hiddenGuestId');
    newRow.append($('<td>').css('padding', '5px').text(hdnGuestId.val()).hide());
    console.log($('#hiddenGuestId').val());

    // delete btn
    var tdDel = $('<td>').css({ 'padding': '5px', 'color': 'red', 'textAlign': 'center', 'cursor': 'pointer' }).text("Delete");
    newRow.append(tdDel);
    tdDel.on('click', function (event) {
        newRow.remove();
        changeNumberColumn();
    })

    newRow.on('click', function () {
        var textArray = $(this).find('td').map(function () {
            return $(this).text();
        }).get();

        hdnSaluteId.val(textArray[1]);
        tdFullName.val(textArray[2]);
        hdnLastName.val(textArray[3]);
        tdIdppno.val(textArray[4]);
        hdnIssueDate.val(textArray[5]);
        hdnDob.val(textArray[6]);
        hdnCountryId.val(textArray[7]);
        hdnStateId.val(textArray[8]);
        hdnNationId.val(textArray[9]);
        $('#inputVipflg').prop('checked', textArray[10]);
        hdnEmailAddr.val(textArray[11]);
        tdPhone1.val(textArray[12]);
        hdnPhone2.val(textArray[13]);
        hdnCreditLimit.val(textArray[14]);
        hdnRemark.val(textArray[15]);
        hdnGender.val(textArray[16]);
        hdnGuestId.val(textArray[17]);
        $('#hiddenTempId').val(textArray[19]); // skip action

    })

    if ($('#hiddenTempId').val() != '') { // Edit
        // tempId
        var hdnTemp = $('#hiddenTempId');
        newRow.append($('<td>').css('padding', '5px').text(hdnTemp.val()).hide());
        newRow.attr('id', hdnTemp.val());

        var rowId = hdnTemp.val();
        $('#' + rowId).replaceWith(newRow);

        changeNumberColumn();
    }
    else { // Add
        // tempId
        var idTemp = "temp_" + new Date().getTime();
        newRow.append($('<td>').css('padding', '5px').text(idTemp).hide());
        newRow.attr('id', idTemp);

        var tBody = $('#guestDetailsTableBody');
        tBody.append(newRow);
    }

    clearGuestForm();
}

function saveGuests() {
    const shouldProceed = window.confirm("Are you sure of saving record?");
    if (shouldProceed) {

        const roomLgId = $('#hiddenRoomLgId').val();
        const table = document.getElementById('guestDetailsTable');
        const tableData = [];

        for (let i = 1; i < table.rows.length; i++) {
            const row = table.rows[i];
            const rowData = [];
            for (let j = 0; j < row.cells.length; j++) {
                rowData.push(row.cells[j].textContent);
            }
            tableData.push(rowData);
        }

        var inputData = {
            guests: tableData,
            roomLgId: roomLgId
        }

        $.ajax({
            type: 'POST',
            url: '/TodayCheckIn/SaveGuests',
            data: inputData,
            success: function () {
                location.reload();
            },
            error: function (error) {
                alert('Error occured in saving guests.');
            }
        });
    }
}

function fillForm(guestId) {

    clearGuestForm();

    $.ajax({
        type: 'GET',
        url: '/TodayCheckIn/GetGuestById',
        data: { guestId: guestId },
        success: function (guest) {
            $('#selectSaluteId').val(guest.saluteid);
            $('#inputGuestfullnme').val(guest.guestfullnme);
            $('#inputGuestLastNme').val(guest.guestlastnme);
            $('#inputIdppno').val(guest.idppno);
            var idissuedte = guest.idissuedte.split('T')[0];
            $('#inputIdissuedte').val(idissuedte);
            var dob = guest.dob.split('T')[0];
            $('#inputDob').val(dob);
            $('#selectCountryid').val(guest.countryid);
            $('#selectStateid').val(guest.stateid);
            $('#selectNationid').val(guest.nationid);
            $('#inputVipflg').prop('checked', guest.vipflg);
            $('#inputEmailaddr').val(guest.emailaddr);
            $('#inputPhone1').val(guest.phone1);
            $('#inputPhone2').val(guest.phone2);
            $('#inputCrlimitamt').val(guest.crlimitamt);
            $('#inputRemark').val(guest.remark);
            $("#selectGender").val(guest.gender);
            $('#hiddenGuestId').val(guest.guestid);
            console.log($('#hiddenGuestId').val());
        },
        error: function (error) {
            alert('Error occured in retrieving guest.');
        }
    });
}

function clearGuestForm() {
    var form = $('#guestForm');
    form.find('input, textarea, select').val('');

    $('#hiddenTempId').val('');
}

function lastNumber() {
    var lastRow = $('#guestDetailsTableBody tr:last td:first-child');
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
    var numberColumnCells = $('#guestDetailsTableBody tr td:first-child');
    numberColumnCells.each(function (index) {
        $(this).text(index + 1);
    });

}
