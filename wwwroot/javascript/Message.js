/* Message Js functions*/

// Message

function editMsg(msgTypId) {

    var inputData = {
        msgTypId: msgTypId
    }

    $.ajax({
        type: 'GET',
        url: "/Messages/GetMsg",
        data: inputData,
        success: function (msg) {
            $('#hdnMsgTypId').val(msg.msgtypid);
            $('#inputCheckInId').val(msg.checkinid);
            $('#selectMsgTypCde').val(msg.msgtypcde);
            $('#inputGuestId').val(msg.guestid);
            $('#inputRaisedBy').val(msg.raisebynme);
            $('#selectMsgToDept').val(msg.msgtodept);
            $('#selectMsgToPerson').val(msg.msgtoperson);
            $('#selectPriority').val(msg.priority);
            $('#chkboxResolved').prop('checked', msg.resolveflg);
            $('#txtAreaResolvedetail').val(msg.resolvedetail);
            $('#inputMsgdetails').val(msg.msgdetail);
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
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
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
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
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
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
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
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

function closeGuestModal() {
    $('#guestModal').hide();
}
