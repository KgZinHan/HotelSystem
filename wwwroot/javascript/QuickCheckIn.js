/* QuickCheckIn Js functions*/


// Billing
function changeAmount() {
    var rmRateId = $('#selectRmRate').val();
    if (rmRateId == '-' || rmRateId == '') {
        return;
    }

    $.ajax({
        type: 'GET',
        url: "/WalkInGuest/GetRoomPrice",
        data: { rmrateId: rmRateId },
        success: function (price) {
            var value = parseFloat(price).toFixed(2);
            $('#inputRmprice').val(value);
            calculateAmount();
        },
        error: function (data) {
            alert('Error occured in getting room price.');
        }
    });
}

function calculateAmount() {
    const tExbedPrice = parseFloat($('#inputExtrabedqty').val()) * parseFloat($('#inputExtrabedprice').val());
    const tAmt = parseFloat($('#inputRmprice').val()) + tExbedPrice - parseFloat($('#inputDiscountamt').val());
    $('#inputAmt').val(parseFloat(tAmt));
};


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
        error: function (data) {
            alert('error');
        }
    });
}

function searchGuest() {

    const keyword = $('#inputKeyword').val();
    const methods = $('#selectSearchMethod').val();

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
        },
        error: function (data) {
            alert('error');
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
            var idissuedte = guest.idissuedte.split('T')[0];
            $('#inputIdissuedte').val(idissuedte);
            var dob = guest.dob.split('T')[0];
            $('#inputDob').val(dob);
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
            var lastvisitdte = guest.lastvisitdte.split('T')[0];
            $('#inputLastVisitDate').val(lastvisitdte);
            $('#inputVisitCount').val(guest.visitcount);
        },
        error: function (error) {
            alert('Error occured in retrieving guest.');
        }
    });
}

function addGuest() {

    if ($('#inputGuestfullnme').val() === '' || $('#inputIdppno').val() === '') {
        alert('Guest fullname and id should not be empty!');
        return;
    }
    var guestFullName = $('#inputGuestfullnme').val();
    $('#inputRaisedBy').val(guestFullName);

    var guestId = $('#hiddenGuestId').val();
    $('#inputGuestId').val(guestId);

    clearGuestForm();
    closeGuestModal();
}

function clearGuestForm() { // fix the default settings
    var form = $('#guestForm');
    form.find('input, textarea, select').val('');
    $('#selectSaluteId').val(4);
    $('#selectCountryid').val(1);
    changeStateSelectList(1, 3);
    $('#selectNationid').val(1);
    $('#hiddenTempId').val('');
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
            url: '/CheckIn/SaveGuests',
            data: inputData,
            success: function () {
                form.submit();
            },
            error: function (error) {
                alert('Error occured in saving guests.');
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

function removeRow(element) {
    var row = element.parentNode;
    row.parentNode.removeChild(row);
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
            alert('error in creating country.');
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
            alert('error in changing state select list.');
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
            alert('error in creating state.');
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
            alert('error in creating nationality.');
        }
    });
}

function openCreateNationalityModal() {
    $('#createNationalityModal').show();
}

function closeCreateNationalityModal() {
    $('#createNationalityModal').hide();
}


