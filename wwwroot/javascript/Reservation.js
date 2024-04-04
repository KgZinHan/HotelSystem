// Reservation Javascript functions

function calculateCheckOutDate() {
    const chkInDte = $('#inputCheckInDateId').val();
    const noOfDays = parseInt($('#inputNoOfDaysId').val());
    const chkInDate = new Date(chkInDte); // Convert check-in date to a Date object
    const selectedValue = $('input[name="paxType"]:checked').val();

    if (!isNaN(chkInDate.getTime()) && !isNaN(noOfDays)) {
        const chkOutDate = new Date(chkInDate);
        chkOutDate.setDate(chkOutDate.getDate() + noOfDays);
        const formattedChkOutDate = chkOutDate.toISOString().slice(0, 10); // Format the date as YYYY-MM-DD
        $('#inputCheckOutDateId').val(formattedChkOutDate);
    }
}

function searchRooms() {

    const arrivalDate = $('#inputCheckInDateId').val();
    const checkoutDate = $('#inputCheckOutDateId').val();

    if (arrivalDate.trim() === "" || checkoutDate.trim() === "") {
        alert("Please fill in all required fields.");
        return; // Prevent form submission
    }

    const allData = {
        arrivalDate: arrivalDate,
        checkoutDate: checkoutDate
    }

    $.ajax({
        type: 'POST',
        url: "/Reservation/SearchRooms",
        data: allData,
        success: function (data) {
            $('#defaultContainer').html(data);
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

function checkOut() {
    const table = document.getElementById('tblHotelRoomList');
    const rsvQtyIndex = 4;
    var chooseRmFlg = false;

    for (let i = 0; i < table.rows.length; i++) {
        const row = table.rows[i];
        if (row.cells[rsvQtyIndex].querySelector("input").value > 0) {
            chooseRmFlg = true;
        }
    }

    //search info
    const arrivalDate = $('#inputCheckInDateId').val();
    const checkoutDate = $('#inputCheckOutDateId').val();

    //room info
    const tableData = [];
    const rmTypIndex = 1;
    const rmAmtIndex = 2;
    const extraBedIndex = 5;
    const rmRateIndex = 6;

    for (let i = 0; i < table.rows.length; i++) {
        const row = table.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            if (j === rmTypIndex || j === rmAmtIndex || j === rsvQtyIndex || j === extraBedIndex || j === rmRateIndex) {
                const cellContent = getCellContent(row.cells[j]);
                rowData.push(cellContent);
            }
        }
        if (rowData[2] > 0) { // check user chose room or not
            tableData.push(rowData);
        }
    }

    const data = {
        arrivalDate: arrivalDate,
        checkoutDate: checkoutDate,
        hotelRoomData: tableData
    }

    if (chooseRmFlg) {
        $.ajax({
            type: 'POST',
            url: '/Reservation/ShowCheckOut',
            data: data,
            success: function (data) {
                const userInfoModal = $('#userInfoModal');
                userInfoModal.show();
                $('#userInfoModelBodyId').html(data)
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
    else {
        alert('Please choose at least one room.');
    }

}

function saveCheckOut() {

    //search info
    const arrivalDate = $('#inputCheckInDateId').val();
    const nightQty = $('#inputNoOfDaysId').val();

    // user info
    const contactName = $('#inputContactNme').val();
    const contactNo = $('#inputContactNo').val();
    const totalAdult = $('#inputTotalAdult').val();
    const totalChild = $('#inputTotalChild').val();
    const specialRemark = $('#inputSpecialRemark').val();
    var vipStatus = $('#checkBoxVipStatus:checked').length > 0;

    if (contactName.trim() === "" || contactNo.trim() === "") {
        alert("Please fill in all required fields.");
        return; // Prevent form submission
    }

    var formData = {
        arrivalDate: arrivalDate,
        nightQty: nightQty,
        contactName: contactName,
        contactNo: contactNo,
        totalAdult: totalAdult,
        totalChild: totalChild,
        specialRemark: specialRemark,
        vipStatus: vipStatus.toString()
    };

    //room info
    const tableData = [];
    const table = document.getElementById('tblHotelRoomList');
    const rmTypIndex = 1;
    const rmAmtIndex = 2;
    const rsvQtyIndex = 4;
    const extraBedIndex = 5;
    const rmRateIndex = 6;

    for (let i = 0; i < table.rows.length; i++) {
        const row = table.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            if (j === rmTypIndex || j === rmAmtIndex || j === rsvQtyIndex || j === extraBedIndex || j === rmRateIndex) {
                const cellContent = getCellContent(row.cells[j]);
                rowData.push(cellContent);
            }
        }
        if (rowData[2] > 0) { // check user chose room or not
            tableData.push(rowData);
        }
    }

    const allData = {
        HotelRoomInfos: tableData,
        UserInfos: formData

    };

    $.ajax({
        type: 'POST',
        url: '/Reservation/SaveCheckOut',
        data: JSON.stringify(allData),
        contentType: 'application/json',
        success: function (response) {
            window.location.href = '/Reservation/Index';
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

function getCellContent(cell) {
    const select = cell.querySelector("select");
    if (select) {
        return select.value;
    }
    const input = cell.querySelector("input");
    if (input) {
        return input.value;
    }
    return cell.textContent;
}

function showImages(rmtypid) {

    cleanImageList();
    const imagesModal = $('#imagesModal');
    const loadingText = $('#loadingTextId');

    var inputData = {
        rmTypId: rmtypid
    };

    const paddingStyle = {
        'padding': '0px',
        'margin': '0px'
    }

    $.ajax({
        url: "/Reservation/ShowImages",
        type: "GET",
        data: inputData,
        success: function (list) {
            loadingText.hide();
            var mainDiv = $('<div>').addClass('row col-12').css(paddingStyle);
            list.forEach(function (image) {
                var imgSrc = 'data:image/jpeg;base64,' + image;
                var imgElement = $('<div>').addClass('col-6').css(paddingStyle).append(
                    $('<img>').attr('src', imgSrc).attr('alt', 'Image').attr('width', '100%').attr('height', '180')
                        .css('border', '1px solid black'));
                mainDiv.append(imgElement);
            });
            $('#imagesBodyListId').append(mainDiv);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('Error.');
        }
    });
    imagesModal.show();
}

function changeExtraBedMax(button) {
    const row = button.closest('tr');
    const reservedRoomInput = row.querySelector('input[name="reservedRoom"]');
    const extraBedInput = row.querySelector('input[name="extraBed"]');
    const maxReservedRoom = parseInt(reservedRoomInput.value, 10);
    if (maxReservedRoom > 0) {
        extraBedInput.setAttribute('max', '10');
    }
    else {
        extraBedInput.setAttribute('max', '0');
    }
}

function cleanHotelList() {
    $('#secondCardBodyId').empty();
}

function cleanImageList() {
    $('#imagesBodyListId').empty();
}

function closeImagesModal() {
    const imagesModal = $('#imagesModal');
    imagesModal.hide();
}

function closeUserInfoModal() {
    const userInfoModal = $('#userInfoModal');
    userInfoModal.hide();
}
