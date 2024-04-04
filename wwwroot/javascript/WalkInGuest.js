/*Quick Check-In Js functions*/

//Common 

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


$('#quickCheckInForm').submit(function (event) {
    event.preventDefault();
    const shouldProceed = window.confirm("Are you sure of saving record?");
    if (shouldProceed) {

        const table = document.getElementById('guestDetailsTable');
        const tableData = [];

        for (let i = 1; i < table.rows.length; i++) {
            const row = table.rows[i];
            const rowData = [];
            for (let j = 0; j < row.cells.length; j++) {
                rowData.push(getCellContent(row.cells[j]));
            }
            tableData.push(rowData);
        }

        var inputData = {
            guests: tableData
        }

        var form = this; // Store a reference to the form element.

        $.ajax({
            type: 'POST',
            url: '/WalkInGuest/SaveGuests',
            data: inputData,
            success: function () {
                form.submit();
            },
            error: function (error) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';
            }
        });
    }

});


// Billing
function changeAmount() {
    var selectedPrice = $('#selectRmRate').val();
    if (selectedPrice == '-' || selectedPrice == '') {
        return;
    }
    var value = parseFloat(selectedPrice).toFixed(2);
    $('#inputRmprice').val(value);
    calculateAmount();
}

function calculateAmount() {
    var extraBedQty = parseFloat($('#inputExtrabedqty').val()) || 0;
    var extraBedPrice = parseFloat($('#inputExtrabedprice').val()) || 0;
    var rmPrice = parseFloat($('#inputRmprice').val()) || 0;
    var discAmt = parseFloat($('#inputDiscountamt').val()) || 0;

    const tExbedPrice = extraBedQty * extraBedPrice;

    const tAmt = rmPrice + tExbedPrice - discAmt;

    $('#inputAmt').val(tAmt.toFixed(2));
};



// Room Selection

function chooseRoom() {

    var arriveDte = $('#inputCheckInDate').val();
    var nightQty = $('#inputNightQty').val();

    var inputData = {
        arriveDte: arriveDte,
        nightQty: nightQty
    }

    $.ajax({
        type: 'GET',
        url: "/WalkInGuest/GetAvailableRooms",
        data: inputData,
        success: function (data) {
            $('#roomModalBodyId').html(data);
            $('#roomModal').show();
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

function assignRoom(roomNo) {
    $('#roomModal').hide();

    $('#inputRoomNo').val(roomNo);
}

function closeRoomModal() {
    $('#roomModal').hide();
}


// Guest Info

function chooseGuest() {
    $.ajax({
        type: 'GET',
        url: "/CheckIn/GuestInfo",
        success: function (data) {
            $('#guestModalBodyId').html(data);
            $('#guestModal').show();
            $('#inputKeyword').focus();
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

function searchGuest() {

    const keyword = $('#inputKeyword').val();

    if (keyword == null || keyword == '') {
        alert('Please Enter keyword first');
        return;
    }

    const methods = $('#selectSearchMethod').val();

    const loadingText = $('#hdnLoading');
    loadingText.show();

    var allInputData = {
        keyword: keyword,
        methods: methods
    }

    $.ajax({
        type: 'GET',
        url: "/CheckIn/SearchGuest",
        traditional: true,
        data: allInputData,
        success: function (data) {
            $('#guestInfoTableBody').html(data);
            loadingText.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error in loading guests.');
                loadingText.hide();
            }
        }
    });
}

function fillForm(guestId) {

    clearGuestForm();

    $.ajax({
        type: 'GET',
        url: '/CheckIn/GetGuestById',
        data: { guestId: guestId },
        success: function (guest) {
            $('#selectSaluteId').val(guest.saluteid);
            $('#inputGuestfullnme').val(guest.guestfullnme);
            $('#inputGuestLastNme').val(guest.guestlastnme);
            $('#inputIdppno').val(guest.idppno);
            $('#inputChrgAccCde').val(guest.chrgacccde);
            if (guest.idissuedte != null) {
                var idissuedte = guest.idissuedte.split('T')[0];
                $('#inputIdissuedte').val(idissuedte);
            }

            if (guest.dob != null) {
                var dob = guest.dob.split('T')[0];
                $('#inputDob').val(dob);
            }

            $('#selectCountryid').val(guest.countryid);
            changeStateSelectList(guest.countryid, guest.stateid);
            $('#selectNationid').val(guest.nationid);
            $('#inputVipflg').prop('checked', guest.vipflg);
            $('#inputEmailaddr').val(guest.emailaddr);
            $('#inputPhone1').val(guest.phone1);
            $('#inputPhone2').val(guest.phone2);
            $('#inputCrlimitamt').val(guest.crlimitamt);
            $('#inputRemark').val(guest.remark);
            $("#selectGender").val(guest.gender);
            $('#hiddenGuestId').val(guest.guestid);
            if (guest.lastvisitdte != null) {
                var lastvisitDte = guest.lastvisitdte.split('T')[0];
                $('#inputLastVisitDate').val(lastvisitDte);
            }
            $('#inputVisitCount').val(guest.visitcount);
        },
        error: function (error) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function addGuest() {

    if ($('#inputGuestfullnme').val() === '' || $('#inputIdppno').val() === '' || $('#inputChrgAccCde').val() === '') {
        alert('Guest fullname,id and acc code should not be empty!');
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
    newRow.append($('<td>').text(hdnSaluteId.val()).hide());

    // fullName
    var tdFullName = $('#inputGuestfullnme');
    newRow.append($('<td>').css('padding', '5px').text(tdFullName.val()));

    // lastName
    var hdnLastName = $('#inputGuestLastNme');
    newRow.append($('<td>').text(hdnLastName.val()).hide());

    // idppno
    var tdIdppno = $('#inputIdppno');
    newRow.append($('<td>').text(tdIdppno.val()).hide());

    // idIssueDate
    var hdnIssueDate = $('#inputIdissuedte');
    newRow.append($('<td>').text(hdnIssueDate.val()).hide());

    // Date of Birth
    var hdnDob = $('#inputDob');
    newRow.append($('<td>').text(hdnDob.val()).hide());

    // country
    var hdnCountryId = $('#selectCountryid');
    newRow.append($('<td>').text(hdnCountryId.val()).hide());

    // state
    var hdnStateId = $('#selectStateid');
    newRow.append($('<td>').text(hdnStateId.val()).hide());

    // nation
    var hdnNationId = $('#selectNationid');
    newRow.append($('<td>').text(hdnNationId.val()).hide());

    // vipFlg
    var inputVipflg = $('#inputVipflg:checked').length > 0;
    newRow.append($('<td>').text(inputVipflg.toString()).hide());

    // email Address
    var hdnEmailAddr = $('#inputEmailaddr');
    newRow.append($('<td>').text(hdnEmailAddr.val()).hide());

    // phone1
    var tdPhone1 = $('#inputPhone1');
    newRow.append($('<td>').text(tdPhone1.val()).hide());

    // phone2
    var hdnPhone2 = $('#inputPhone2');
    newRow.append($('<td>').text(hdnPhone2.val()).hide());

    // credit Limit
    var hdnCreditLimit = $('#inputCrlimitamt');
    newRow.append($('<td>').text(hdnCreditLimit.val()).hide());

    // remark
    var hdnRemark = $('#inputRemark');
    newRow.append($('<td>').text(hdnRemark.val()).hide());

    // gender
    var hdnGender = $("#selectGender");
    newRow.append($('<td>').text(hdnGender.val()).hide());

    // guestId
    var hdnGuestId = $('#hiddenGuestId');
    newRow.append($('<td>').text(hdnGuestId.val()).hide());

    // principle flg
    var rowCount = $('#guestDetailsTableBody tr').length;
    if (rowCount > 0) {
        var inputPrincipleFlg = $('<input>').val('').attr('type', 'checkbox');
        newRow.append($('<td>').css({ 'padding': '5px', 'textAlign': 'center' }).append(inputPrincipleFlg));
    }
    else {
        var inputPrincipleFlg = $('<input>').val('').attr('type', 'checkbox').prop('checked', true);
        newRow.append($('<td>').css({ 'padding': '5px', 'textAlign': 'center' }).append(inputPrincipleFlg));
    }

    // last visited date
    var inputLastVisitDte = $('#inputLastVisitDate');
    newRow.append($('<td>').text(inputLastVisitDte.val()).hide());

    // visit count
    var inputVisitCount = $('#inputVisitCount');
    newRow.append($('<td>').text(inputVisitCount.val()).hide());

    // charge acc code
    var inputChrgAccCde = $('#inputChrgAccCde');
    newRow.append($('<td>').text(inputChrgAccCde.val()).hide());

    // delete btn
    var tdDel = $('<td>').css({ 'padding': '5px', 'color': 'red', 'textAlign': 'center', 'cursor': 'pointer' }).text("Remove");
    newRow.append(tdDel);
    tdDel.on('click', function (event) {
        newRow.remove();
        changeNumberColumn();
    })

    var tBody = $('#guestDetailsTableBody');
    tBody.append(newRow);

    clearGuestForm();
}

function clearGuestForm() { // fix the default settings
    var form = $('#guestForm');
    form.find('input, textarea, select').val('');
    $('#inputChrgAccCde').val('1401/H01');
    $('#selectSaluteId').val(1);
    $('#selectCountryid').val(13);
    changeStateSelectList(13, 1);
    $('#selectNationid').val(11);
    $('#hiddenTempId').val('');
}

function removeRow(element) {
    var row = element.parentNode;
    row.parentNode.removeChild(row);
}


$('#expectedCheckInForm').submit(function (event) {
    event.preventDefault();
    const shouldProceed = window.confirm("Are you sure of saving record?");
    if (shouldProceed) {

        const table = document.getElementById('guestDetailsTable');
        const tableData = [];

        for (let i = 1; i < table.rows.length; i++) {
            const row = table.rows[i];
            const rowData = [];
            for (let j = 0; j < row.cells.length; j++) {
                rowData.push(getCellContent(row.cells[j]));
            }
            tableData.push(rowData);
        }

        var inputData = {
            guests: tableData
        }

        var form = this; // Store a reference to the form element.

        $.ajax({
            type: 'POST',
            url: '/CheckIn/SaveGuests',
            data: inputData,
            success: function () {
                form.submit();
            },
            error: function (error) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            }
        });
    }

});

function getCellContent(cell) {
    const select = cell.querySelector("select");
    if (select) {
        return select.value;
    }
    const input = cell.querySelector("input");
    if (input) {
        if (input.type === "checkbox") {
            return input.checked; // Return true or false for checkbox
            console.log(input.checked);
        } else {
            return input.value;
        }
    }
    return cell.textContent;
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

function closeGuestModal() {
    $('#guestModal').hide();
}


// Country Creation

function createCountry() {
    const countryDesc = $('#inputCountryDesc').val();

    $.ajax({
        type: 'GET',
        url: "/CheckIn/CreateCountry",
        data: { countryDesc: countryDesc },
        success: function (countryId) {
            var selectCountry = $('#selectCountryid');
            $("<option>").val(countryId).text(countryDesc).appendTo(selectCountry);
            selectCountry.val(countryId);
            changeStateSelectList(countryId, '');
            closeCreateCountryModal();
        },
        error: function (data) {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function openCreateCountryModal() {
    $('#createCountryModal').show();
}

function closeCreateCountryModal() {
    $('#createCountryModal').hide();
}

function changeStateSelectList(countryId, guestId) {
    const selectState = $('#selectStateid');
    $.ajax({
        type: 'GET',
        url: "/CheckIn/ChangeStateSelectList",
        data: { countryId: countryId },
        success: function (states) {
            selectState.find("option").remove(); // Remove previous options first
            $("<option>").val("").text("Select One").appendTo(selectState);
            states.forEach(state => {
                $("<option>").val(state.gstateid).text(state.gstatedesc).appendTo(selectState);
            });
            selectState.val(guestId);
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}


// State Creation

function createState() {
    const stateDesc = $('#inputStateDesc').val();
    const countryId = $('#inputCountryId').val();

    var inputData = {
        stateDesc: stateDesc,
        countryId: countryId
    }

    $.ajax({
        type: 'GET',
        url: "/CheckIn/CreateState",
        data: inputData,
        success: function (stateId) {
            var selectState = $('#selectStateid');
            $("<option>").val(stateId).text(stateDesc).appendTo(selectState);
            selectState.val(stateId);
            closeCreateStateModal();
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function openCreateStateModal() {
    const countryId = $('#selectCountryid').val();
    const countryDesc = $('#selectCountryid option:selected').text();

    $('#inputCountryId').val(countryId);
    $('#inputCountry').val(countryDesc);
    $('#createStateModal').show();
}

function closeCreateStateModal() {
    $('#createStateModal').hide();
}


// Nationality Creation

function createNationality() {
    const nationDesc = $('#inputNationDesc').val();

    $.ajax({
        type: 'GET',
        url: "/CheckIn/CreateNationality",
        data: { nationDesc: nationDesc },
        success: function (nationId) {
            var selectNation = $('#selectNationid');
            $("<option>").val(nationId).text(nationDesc).appendTo(selectNation);
            selectNation.val(nationId);
            closeCreateNationalityModal();
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function openCreateNationalityModal() {
    $('#createNationalityModal').show();
}

function closeCreateNationalityModal() {
    $('#createNationalityModal').hide();
}


